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

namespace Vipers
{
    public class UnifyCurve: GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent3 class.
        /// </summary>
        public UnifyCurve()
            : base("顺时针方向统一曲线", "UnifyCurve",
                "将曲线（直线除外）按照指定平面的顺时针统一曲线方向",
                "Vipers", "Viper.curve")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("曲线", "C", "待更改的曲线", GH_ParamAccess.item);
            pManager.AddPlaneParameter("平面", "P", "参照平面，将按该平面的顺时针方向为参考方向", GH_ParamAccess.item,Plane.WorldXY);
            pManager.AddBooleanParameter("反转", "R", "如果为false，则为逆时针方向统一曲线", GH_ParamAccess.item,true);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
            Message = "顺时针方向统一曲线";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddCurveParameter("统一结果","C","顺时针统一方向的曲线",GH_ParamAccess.item);
            pManager.AddBooleanParameter("判断", "J", "判断输入的曲线是否为顺时针方向曲线", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////声明变量
            Curve curve = null;
            Plane plane = new Plane();
            bool reverse = true;
            ///////////////////////////////////////////////////////////////////////////////////////////////////检测输入端是否合理
            if (!DA.GetData(0, ref curve)) return;
            if (!DA.GetData(1, ref plane)) return;
            if (!DA.GetData(2, ref reverse)) return;
            Plane pln = Plane.WorldXY;
            int aaa = (int)curve.ClosedCurveOrientation(plane);
            if (aaa == -1)
            {
                curve.Reverse();
                DA.SetData(1, false);
            }
            else
            {
                DA.SetData(1,true);
            }
            if (reverse == false)
            {
                curve.Reverse();
            }
            DA.SetData(0,curve);
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
                //return Resource1.顺时针方向统一曲线;
                return Resource1.curve_时针方向统一曲线;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{ba240804-38d3-4fc8-9c0f-518a69d3f8cc}"); }
        }
    }
}