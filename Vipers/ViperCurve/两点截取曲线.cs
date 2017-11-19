using Rhino;
using Rhino.Geometry;
using Rhino.DocObjects;
using Rhino.Collections;

using GH_IO;
using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Reflection;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Vipers////TangChi 2015.3.1
{
    public class CurveCutOut : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent4 class.
        /// </summary>
        public CurveCutOut()
            : base("两点截取曲线", "CurveCutOut",
                "通过设置起始点和终止点截取曲线",
                "Vipers", "Viper.curve")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("待剪切曲线", "C", "待剪切曲线", GH_ParamAccess.item);
            pManager.AddPointParameter("起始点","P1","截取的起始点",GH_ParamAccess.item);
            pManager.AddPointParameter("终止点", "P2", "截取的终止点", GH_ParamAccess.item);
            pManager.AddBooleanParameter("反转","R","得到另一条曲线",GH_ParamAccess.item ,false);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
            pManager.HideParameter(2);
            Message = "两点截取曲线";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("截取后的曲线","C1","通过起始点和终止点截取后的曲线",GH_ParamAccess.list);
            pManager.AddCurveParameter("截取后的另一条曲线", "C2", "原曲线截取掉C1后剩余部分", GH_ParamAccess.list);
            pManager.HideParameter(1);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve curve = null;
            Point3d pt1 = Point3d.Unset;
            Point3d pt2 = Point3d.Unset;
            bool reverse = false;
           if(DA.GetData(0,ref curve)&&DA.GetData(1,ref pt1)&&DA.GetData(2,ref pt2)&&DA.GetData(3,ref reverse))
           {
               List<double> ts = new List<double>();
               double t1 = 0;
              curve.ClosestPoint(pt1,out t1);
              ts.Add(t1);
              double t2 = 0;
              curve.ClosestPoint(pt2, out t2);
              ts.Add(t2);
              List<Curve> curves= ShatterCurve(curve,ts);////剪切后的曲线群
              List<Curve> last1 = new List<Curve>();
              List<Curve> last2 = new List<Curve>();
              if (!curve.IsClosed)////不闭合曲线的情况
              {
                  for (int i = 0; i < curves.Count; i++)
                  {
                      Point3d pt = curves[i].PointAtNormalizedLength(0.5);
                      double t = 0;
                      curve.ClosestPoint(pt, out t);
                      if ((t > t1 && t < t2) || (t < t1 && t > t2))////点在两点之间
                      {
                          last1.Add(curves[i]);
                          curves.RemoveAt(i);
                          last2.AddRange(curves);
                          break;
                      }
                  }
              }
              else//////闭合曲线的情况
              {
                  last1.Add(curves[0]);
                  last2.Add(curves[1]);
              }
               if(reverse)/////如果选择反转
               {
                   List<Curve> last3 = last2.ToArray().ToList();
                   List<Curve> last4 = last1.ToArray().ToList();
                   last1 = last3;
                   last2 = last4;
               }
              DA.SetDataList(0, last1);
              DA.SetDataList(1, last2);
           }

        }
        public List<Curve> splitcurves(Curve destination,List<double> list)/////原电池中shattercurve的解决方案
        {
                double min = destination.Domain.Min;
                double max = destination.Domain.Max;
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if ((list[i] > max) || (list[i] < min))
                    {
                        list.RemoveAt(i);
                    }
                }
                List<Curve> data = ShatterCurve(destination, list);
                if ((data == null) || (data.Count == 0))
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Curve shattering failed");
                }
                return data;
        }
        public List<Curve> ShatterCurve(Curve crv, List<Double>ts)
        {
            List<Curve> list = new List<Curve>();
            if (crv != null)
            {
                if (ts == null)
                {
                    return list;
                }
                if (ts.Count == 0)
                {
                    list.Add(crv.DuplicateCurve());
                    return list;
                }
                bool isClosed = crv.IsClosed;
                if (isClosed)
                {
                    double num = crv.Domain.T0;
                    foreach (double num2 in ts)
                    {
                        if (Math.Abs((double)(num2 - num)) < 1E-16)
                        {
                            isClosed = false;
                            break;
                        }
                    }
                }
                list.Capacity = ts.Count + 1;
                for (int i = 0; i < (ts.Count - 1); i++)
                {
                    if (ts[i] > ts[i + 1])
                    {
                        ts.Sort();
                        break;
                    }
                }
                double min = crv.Domain.Min;
                double num5 = min;
                for (int j = 0; j < ts.Count; j++)
                {
                    num5 = ts[j];
                    if (Math.Abs((double)(num5 - min)) > 1E-12)
                    {
                        Curve item = crv.Trim(min, num5);
                        if (item.IsValid)
                        {
                            list.Add(item);
                        }
                        min = num5;
                    }
                }
                if (Math.Abs((double)(crv.Domain.Max - num5)) > 1E-10)
                {
                    Curve curve2 = crv.Trim(num5, crv.Domain.Max);
                    if (!curve2.IsValid)
                    {
                        return list;
                    }
                    if (isClosed)
                    {
                        Curve[] curveArray = Curve.JoinCurves(new Curve[] { list[0], curve2 });
                        if ((curveArray != null) && (curveArray.Length == 1))
                        {
                            Curve curve3 = curveArray[0].Simplify(CurveSimplifyOptions.All, 1E-06, 0.001);
                            if (curve3 == null)
                            {
                                list[0] = curveArray[0];
                                return list;
                            }
                            list[0] = curve3;
                            return list;
                        }
                        list.Add(curve2);
                        return list;
                    }
                    list.Add(curve2);
                }
            }
            return list;
        }
        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                //return Resource1.两点截取曲线;
                return Resource1.curve_两点截取曲线;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{00ec4dfd-94bc-4d11-baea-37bb7ea78c32}"); }
        }
    }
}