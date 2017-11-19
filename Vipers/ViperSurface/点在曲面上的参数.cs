using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers///TangChi 2015.11.17
{
    public class PointInformation : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent51 class.
        /// </summary>
        public PointInformation()
            : base("点在曲面上的参数", "PointInformation",
                "通过u，v百分比得到曲面上的点，同时得到该点在曲面的信息",
                "Vipers", "Viper.surface")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("曲面", "S", "点所在的曲面", GH_ParamAccess.item);
            pManager.AddNumberParameter("U方向百分比", "PU", "点曲面u方向的百分比", GH_ParamAccess.item,0.5);
            pManager.AddNumberParameter("V方向百分比", "PP", "点曲面v方向的百分比", GH_ParamAccess.item, 0.5);
            pManager.HideParameter(0);
            Message = "点在曲面上的参数";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("点","P","对应uv百分比的点的位置",GH_ParamAccess.item);
            pManager.AddPointParameter("点", "P", "uv点的位置", GH_ParamAccess.item);
            pManager.AddNumberParameter("高斯曲率", "G", "点在曲面的高斯曲率", GH_ParamAccess.item);
            pManager.AddNumberParameter("平均曲率", "M", "点在曲面的平均曲率", GH_ParamAccess.item);
            pManager.AddVectorParameter("向量","N","点在曲面的法向量",GH_ParamAccess.item);
            pManager.HideParameter(1);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
             Surface surface=null;
            double uPercent=0;
            double vPercent=0;
            if (!DA.GetData(0,ref surface))return;
            if (!DA.GetData(1,ref uPercent))return;
            if (!DA.GetData(2, ref vPercent)) return;
           ////////////////////////////////////////////
            Interval uu = surface.Domain(0);
            Interval vv = surface.Domain(1);
            double num1 = (uu.T1 - uu.T0) * uPercent + uu.T0;
            double num2 = (vv.T1 - vv.T0) * vPercent + vv.T0;
            Point3d pt = surface.PointAt(num1, num2);
            SurfaceCurvature sc = surface.CurvatureAt(num1, num2);
            Point2d pt2 = sc.UVPoint;
            DA.SetData(0, pt);
            DA.SetData(1, pt2);
            DA.SetData(2, sc.Gaussian);
            DA.SetData(3, sc.Mean);
            DA.SetData(4, sc.Normal);
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
                //return Resource1.点在曲面上的参数;
                return Resource1.surface_点在曲面上参数;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{83b6f719-faba-42ae-ba33-1c727dcf41da}"); }
        }
    }
}