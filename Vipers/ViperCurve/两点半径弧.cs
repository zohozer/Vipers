using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers///////////TangChi 2016.7.14
{
    public class 两点半径弧 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the 两点半径弧 class.
        /// </summary>
        public 两点半径弧()
            : base("两点半径弧", "PPR-Arc",
                "通过两个端点和半径创建弧线",
                "Vipers", "Viper.curve")
        {
        }
        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.obscure|GH_Exposure.primary ;
            }
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("平面","P","弧线所在平面",GH_ParamAccess.item,Plane.WorldXY);
            pManager.AddPointParameter("点","S","弧线的一个端点",GH_ParamAccess.item);
            pManager.AddPointParameter("点", "E", "弧线的另一个端点", GH_ParamAccess.item);
            pManager.AddNumberParameter("半径","R","弧线的半径",GH_ParamAccess.item);
            pManager.AddBooleanParameter("切换","B","可能满足条件的弧线不止一个，通过布尔值来切换",GH_ParamAccess.item,false);
            Message = "平面+两点+半径=弧";
            pManager.HideParameter(0);
            pManager.HideParameter(1);
            pManager.HideParameter(2);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddArcParameter("弧线1","A1","满足指定条件的弧线",GH_ParamAccess.item);
            pManager.AddArcParameter("弧线2", "A2", "A1的互补弧线(隐藏)", GH_ParamAccess.item);
            pManager.HideParameter(1);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane pln = new Plane();
            Point3d s = new Point3d();
            Point3d e = new Point3d();
            double r = 0;
            bool b = false;
            if (!DA.GetData(0, ref pln)||!DA.GetData(1,ref s)||!DA.GetData(2,ref e)||!DA.GetData(3,ref r)||!DA.GetData(4,ref b)) return;
            double distance = s.DistanceTo(e);
            if (distance > 2 * r)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "直径不能小于两点距离");
                return;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////////////
            Arc arc1 = Arc.Unset;
            Arc arc2 = Arc.Unset;
            Point3d center = (s + e) / 2;
            double move = (r - distance / 2) * (r + distance / 2);
            move = Math.Abs(Math.Sqrt(move));
            move = r + move;
            Line line = new Line(s, e);
            Vector3d vc = line.Direction;
            vc.Transform(Transform.Rotation(Math.PI * 0.5, pln.ZAxis, line.ToNurbsCurve().PointAtNormalizedLength(0.5)));
            vc.Unitize();
            vc = -1 * vc * (move);
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            if (distance == 2 * r)
            {
                if (b)
                {
                    vc = -1 * vc;
                }
                center.Transform(Transform.Translation(vc));
                arc1 = new Arc(s, center, e);
                arc2 = hubu(arc1);
                DA.SetData(0, arc1);
                DA.SetData(1, arc2);
            }
            else
            {
                if (b)
                {
                    vc = -1 * vc;
                }
                center.Transform(Transform.Translation(vc));
                arc1 = new Arc(s, center, e);
                arc2 = hubu(arc1);
                DA.SetData(0, arc1);
                DA.SetData(1, arc2);
            }
        }
        public Arc hubu(Arc arc) ///互补圆弧
        {
            Plane pln = arc.Plane;///参考平面
            Point3d center = arc.Center;////中点
            Vector3d vc = Point3d.Subtract(center, arc.MidPoint);
            Line line = new Line(center, vc);
            Point3d ptc = line.To;
            Arc another = new Arc(arc.EndPoint, ptc, arc.StartPoint);
            return another;
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
                return Resource1.curve_两点半径弧;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{3c700af1-6225-4746-90d7-6aa1eacc77bd}"); }
        }
    }
}