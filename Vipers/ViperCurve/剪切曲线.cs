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

namespace Vipers////TangChi 2015.5.12
{
    public class ViperCutCurve2 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent2 class.
        /// </summary>
        public ViperCutCurve2()
            : base("剪切曲线", "CurveTailor",
                "通过曲线B剪切曲线A",
                 "Vipers", "Viper.curve")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("被剪切曲线", "CA", "该曲线将被曲线B剪切", GH_ParamAccess.item);
            pManager.AddCurveParameter("剪切曲线", "CB", "用于剪切曲线A", GH_ParamAccess.list);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
            Message = "剪切曲线";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("剪切结果","C","剪切后的曲线A",GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////声明变量
            Curve curveA = null;
            List<Curve> curveB = new List<Curve>();
            ///////////////////////////////////////////////////////////////////////////////////////////////////检测输入端是否合理
            if (!DA.GetData(0, ref curveA)) return;
            if (!DA.GetDataList(1, curveB)) return;
            List<Point3d> pts = new List<Point3d>();
            Curve[] crs;
            for (int i = 0; i < curveB.Count; i++)
            {
                Rhino.Geometry.Intersect.CurveIntersections cin = Rhino.Geometry.Intersect.Intersection.CurveCurve(curveA, curveB[i], 0, 0);
                int mm = cin.Count;
                if (mm >= 1)
                {
                    Rhino.Geometry.Intersect.IntersectionEvent[] evt = new Rhino.Geometry.Intersect.IntersectionEvent[mm];
                    for (int k = 0; k < mm; k++)
                    {
                        evt[k] = cin.ElementAt(k);
                        pts.Add(evt[k].PointA);
                    }
                }
            }
            List<double> ts = new List<double>();
            if (pts.Count >= 1)
            {
                for (int j = 0; j < pts.Count; j++)////点在curve上的t值
                {
                    double t = 0;
                    curveA.ClosestPoint(pts[j], out t);
                    ts.Add(t);
                }
            }
            crs = curveA.Split(ts);
            DA.SetDataList(0,crs);
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
                //return Resource1.v8;
                return Resource1.curve_曲线A剪切曲线B;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{4e0402c0-7bbc-4e0d-8fc0-21a7145273f4}"); }
        }
    }
}