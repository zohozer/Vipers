using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers////TangChi 2016.01.04
{
    public class TriangleFromLength : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the TriangleFromLength class.
        /// </summary>
        public TriangleFromLength()
            : base("三边长度确定三角形", "TriangleFromLength",
                "通过用户输入的长度确定三角形",
                "Vipers", "Viper.curve")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary|GH_Exposure.obscure; }
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("平面","P","三角形参考平面",GH_ParamAccess.item,Plane.WorldXY);
            pManager.AddNumberParameter("边长A","LA","边长A的长度，该边将于指定平面的x轴平齐",GH_ParamAccess.item,150);
            pManager.AddNumberParameter("边长B", "LB", "边长B的长度", GH_ParamAccess.item, 200);
            pManager.AddNumberParameter("边长C", "LC", "边长C的长度", GH_ParamAccess.item, 250);
            pManager.AddBooleanParameter("翻转","R","翻转三角形方向",GH_ParamAccess.item,false);
            pManager.HideParameter(0);
            Message = "三边长度确定三角形";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("三角形","C","输出结果，如果正确，则生成三角形，否则输出错误原因",GH_ParamAccess.item);
            pManager.AddPointParameter("A点","PA","三角形的A点",GH_ParamAccess.item);
            pManager.AddPointParameter("B点", "PB", "三角形的B点", GH_ParamAccess.item);
            pManager.AddPointParameter("C点", "PC", "三角形的C点", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane plane=Plane.WorldXY;
            double x =0;
            double y = 0;
            double z = 0;
            bool reverse=false;
            if(!DA.GetData(0,ref plane))return;
            if (!DA.GetData(1, ref x)) return;
            if (!DA.GetData(2, ref y)) return;
            if (!DA.GetData(3, ref z)) return;
            if (!DA.GetData(4, ref reverse)) return;
            ///////////////////////////////////////////////
            if (x == 0 || y == 0 || z == 0)
            {
                DA.SetData(0, "边长不能0");
                return;
            }
            else
            {
                if (x + y <= z || x + z <= y || y + z <= x)
                {
                   DA.SetData(0, "输入的长度不满足三角形三边关系");
                    return;
                }
                Point3d pta = plane.Origin;
                Point3d ptb = plane.Origin;
                Vector3d vc = plane.XAxis;
                ptb.Transform(Transform.Translation(vc * x));
                Circle c1 = new Circle(plane, pta, z);
                Circle c2 = new Circle(plane, ptb, y);
                Point3d ptc1 = Rhino.Geometry.Intersect.Intersection.CurveCurve(c1.ToNurbsCurve(), c2.ToNurbsCurve(), 0, 0)[0].PointA;
                Point3d ptc2 = Rhino.Geometry.Intersect.Intersection.CurveCurve(c1.ToNurbsCurve(), c2.ToNurbsCurve(), 0, 0)[1].PointA;
                List<Point3d> pts = new List<Point3d>();
                Point3d ptc;
                pts.Add(pta);
                pts.Add(ptb);
                if (reverse)
                {
                    ptc = ptc2;
                }
                else
                {
                    ptc = ptc1;
                }
                pts.Add(ptc);
                pts.Add(pta);
                Polyline ply = new Polyline(pts);
                DA.SetData(0, ply);
                DA.SetData(1, pta);
                DA.SetData(2, ptb);
                DA.SetData(3, ptc);
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
                return Resource1.curve_三边确定三角形;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{715c33f4-6591-4ca8-85b7-d90be6c205bb}"); }
        }
    }
}