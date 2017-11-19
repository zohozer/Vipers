using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers//////TangChi 2016.01.07
{
    public class QuadrangleFromLength : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the QuadrangleFromLength class.
        /// </summary>
        public QuadrangleFromLength()
            : base("四边长度及对角线确定四边形", "QuadrangleFromLength",
                "通过用户输入的四边长度及对角线确定四边形",
                "Vipers", "Viper.curve")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary | GH_Exposure.obscure; }
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("平面", "P", "四边形参考平面", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddNumberParameter("边长A", "LA", "边长A的长度", GH_ParamAccess.item, 200);
            pManager.AddNumberParameter("边长B", "LB", "边长B的长度", GH_ParamAccess.item, 200);
            pManager.AddNumberParameter("边长C", "LC", "边长C的长度", GH_ParamAccess.item, 200);
            pManager.AddNumberParameter("边长D", "LD", "边长D的长度", GH_ParamAccess.item, 200);
            pManager.AddNumberParameter("对角E", "LE", "四边形对角线长度，该边将于指定平面的x轴平齐", GH_ParamAccess.item, 350);
            pManager.HideParameter(0);
            Message = "四边长度及对角线确定四边形";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("四边形", "C", "输出结果，如果正确，则生成四边形", GH_ParamAccess.item);
            pManager.AddPointParameter("A点", "PA", "四边形的A点", GH_ParamAccess.item);
            pManager.AddPointParameter("B点", "PB", "四边形的B点", GH_ParamAccess.item);
            pManager.AddPointParameter("C点", "PC", "四边形的C点", GH_ParamAccess.item);
            pManager.AddPointParameter("D点", "PD", "四边形的D点", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane plane = Plane.WorldXY;
            double lengthA = 0;
            double lengthB = 0;
            double lengthC = 0;
            double lengthD = 0;
            double lengthE = 0;
            if (!DA.GetData(0, ref plane)) return;
            if (!DA.GetData(1, ref lengthA)) return;
            if (!DA.GetData(2, ref lengthB)) return;
            if (!DA.GetData(3, ref lengthC)) return;
            if (!DA.GetData(4, ref lengthD)) return;
            if (!DA.GetData(5, ref lengthE)) return;
            ///////////////////////////////////////////////
            Point3d pta = plane.Origin;
            Point3d ptc = plane.Origin;
            ptc.Transform(Transform.Translation(plane.XAxis * lengthE));
            Circle c1 = new Circle(plane, pta, lengthA);
            Circle c2 = new Circle(plane, ptc, lengthB);
            Point3d ptb = Rhino.Geometry.Intersect.Intersection.CurveCurve(c1.ToNurbsCurve(), c2.ToNurbsCurve(), 0, 0)[0].PointA;
            Circle c3 = new Circle(plane, ptc, lengthC);
            Circle c4 = new Circle(plane, pta, lengthD);
            Point3d ptd = Rhino.Geometry.Intersect.Intersection.CurveCurve(c3.ToNurbsCurve(), c4.ToNurbsCurve(), 0, 0)[1].PointA;
            List<Point3d> pts = new List<Point3d>();
            pts.Add(pta);
            pts.Add(ptb);
            pts.Add(ptc);
            pts.Add(ptd);
            pts.Add(pta);
            Polyline ply = new Polyline(pts);
            DA.SetData(0,ply);
            DA.SetData(1, pta);
           DA.SetData(2, ptb);
            DA.SetData(3, ptc);
            DA.SetData(4, ptd);
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
                return Resource1.curve_四边及对角确定四边形;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{b07b3a6d-4df8-42f4-baec-0ce67fa7c50b}"); }
        }
    }
}