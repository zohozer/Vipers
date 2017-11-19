using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers////TangChi 2015.7.27
{
    public class CurveOnSurface : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent31 class.
        /// </summary>
        public CurveOnSurface()
            : base("通过点创建曲面曲线", "CurveOnSurface",
                "通过曲面上的点，创建一条完全在曲面上的曲线",
                "Vipers", "Viper.curve")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.obscure; }
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("曲面","S","曲线位于该曲面",GH_ParamAccess.item);
            pManager.AddPointParameter("点", "P", "曲线通过的点", GH_ParamAccess.list);
            pManager.AddNumberParameter("公差", "T", "公差(没什么卵用)", GH_ParamAccess.item,0);
            pManager.AddBooleanParameter("闭合","B","设置曲线是否闭合",GH_ParamAccess.item,false);
            pManager.HideParameter(0);
            Message = "通过点创建曲面曲线";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("曲线","C","通过曲面上点的曲线",GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Surface surface=null;
            List<Point3d> points=new List<Point3d>();
            double tolerance=0;
            bool closed = false;
            if(!DA.GetData(0,ref surface)) return;
            if(!DA.GetDataList(1,points)) return;
            if (!DA.GetData(2, ref tolerance)) return;
            if (!DA.GetData(3, ref closed)) return;
            Surface x = surface;
            List<Point3d> y = points;
            double z = tolerance;
            List<Point2d> pts = new List<Point2d>();
            if (closed)
            {
                y.Add(y[0]);
            }
            foreach (Point3d pt in y)
            {
                double u;
                double v;
                x.ClosestPoint(pt, out u, out v);
                pts.Add(new Point2d(u, v));
            }
            NurbsCurve mm = x.InterpolatedCurveOnSurfaceUV(pts, z);
            DA.SetData(0, mm);
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
                //return Resource1.通过曲面上的点在曲面上画曲线;
                return Resource1.curve_通过点创建曲面线;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{b4c976c4-6760-4211-8987-fba008414822}"); }
        }
    }
}