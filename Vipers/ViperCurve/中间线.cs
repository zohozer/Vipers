using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers
{
    public class TweenCurves : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the TweenCurves class.
        /// </summary>
        public TweenCurves()
            : base("中间线", "TweenCurves",
                "在指定的两条曲线之间创建多条中间线(直接调用Rhino.Geometry.Curve.CreateTweenCurves)",
                "Vipers", "Viper.curve")
        {
            Message = "创建中间线";
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("曲线1","C1","第一条曲线",GH_ParamAccess.item);
            pManager.AddCurveParameter("曲线2", "C2", "第二条曲线", GH_ParamAccess.item);
            pManager.AddIntegerParameter("数量","N","在指定两条曲线间创建的中间线的数量",GH_ParamAccess.item,10);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("曲线","C","在第一条和第二条曲线间建立的多条中间线",GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve c1=null;
            Curve c2=null;
            int n = 0;
            if(!DA.GetData(0,ref c1)||!DA.GetData(1,ref c2)||!DA.GetData(2,ref n))return;
            DA.SetDataList(0,Curve.CreateTweenCurves(c1,c2,n));
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
                return Resource1.curve_中间线;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{c591069a-fe44-4bd2-8288-fc3109178250}"); }
        }
    }
}