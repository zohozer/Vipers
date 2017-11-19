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
using Grasshopper.Kernel.Parameters;

namespace Vipers/////TangChi 2015.2.10
{
    public class ViperCurveExcursion : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent2 class.
        /// </summary>
        public ViperCurveExcursion()
            : base("曲线同向偏移", "CurveOffset",
                "输入正数为向外偏移，负数为向内偏移",
                "Vipers", "Viper.curve")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("曲线","C","待偏移的曲线",GH_ParamAccess.item);
            pManager.AddPlaneParameter("参考平面", "P", "偏移曲线的参考平面", GH_ParamAccess.item,Plane.WorldXY);
            pManager.AddNumberParameter("距离", "D", "偏移的距离", GH_ParamAccess.item,1);
            pManager.AddIntegerParameter("连接","C","连接类型",GH_ParamAccess.item,1);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
                Param_Integer integer = pManager[3] as Param_Integer;
    if (integer != null)
    {
        integer.AddNamedValue("无", 0);
        integer.AddNamedValue("尖角", 1);
        integer.AddNamedValue("圆角", 2);
        integer.AddNamedValue("平滑", 3);
        integer.AddNamedValue("斜角", 4);
    }
            Message = "曲线同向偏移";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("偏移后的曲线", "C", "偏移后的曲线", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve curve = null;
            Plane plane = Plane.WorldXY;
            double distance = 0;
            int type=0;
            if(!DA.GetData(0,ref curve)) return;
            if (!DA.GetData(1, ref plane)) return;
            if (!DA.GetData(2, ref distance)) return;
            if(!DA.GetData(3,ref type)) return;
            Curve[] last = AfterOffset(curve,distance,plane,type);
            //////////////////////////////////////////
            double lengths = 0;
            for (int i = 0;i<last.Length ;i++ )
                lengths += last[i].GetLength();
            if (lengths < curve.GetLength()&&distance>0)//////实际向内偏移了
                distance = distance * -1;
            else if(lengths>curve.GetLength()&&distance<0)
                distance = distance * -1;
            Curve[] last2 = AfterOffset(curve, distance, plane, type);
            DA.SetDataList(0,last2);
        }
        public Curve[] AfterOffset(Curve destination,double naN,Plane unset,int num2)
        {
                if (!RhinoMath.IsValidDouble(naN))
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "不是有效的数字");
                    return new Curve[]{};
                }
                else if (!unset.IsValid)
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "不是有效的平面");
                    return new Curve[] { };
                }
                else
                {
                    double num3 = Math.Abs(naN);
                    double tolerance = Math.Min(GH_Component.DocumentTolerance(), 0.1 * num3);
                    GH_Component.DocumentAngleTolerance();
                    num2 = Math.Max(Math.Min(num2, 4), 0);
                    if (naN == 0.0)
                    {
                        return new Curve[]{destination};
                    }
                    else
                    {
                        CurveOffsetCornerStyle none = CurveOffsetCornerStyle.None;
                        switch (num2)
                        {
                            case 0:
                                none = CurveOffsetCornerStyle.None;
                                break;

                            case 1:
                                none = CurveOffsetCornerStyle.Sharp;
                                break;

                            case 2:
                                none = CurveOffsetCornerStyle.Round;
                                break;

                            case 3:
                                none = CurveOffsetCornerStyle.Smooth;
                                break;

                            case 4:
                                none = CurveOffsetCornerStyle.Chamfer;
                                break;
                        }
                        Curve[] inputCurves = null;
                        inputCurves = destination.Offset(unset, naN, tolerance, none);
                        if (inputCurves == null)
                        {
                            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "不能偏移该曲线");
                            return new Curve[] { };
                        }
                        else
                        {
                            Curve[] data = Curve.JoinCurves(inputCurves);
                            if (data == null)
                                return inputCurves;
                            else
                                return data;
                        }
                    }
                }
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
                //return Resource1.曲线同向偏移;
                return Resource1.curve_曲线同向偏移2;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{25b78807-e917-4b3e-978e-50738bcf73f2}"); }
        }
    }
}