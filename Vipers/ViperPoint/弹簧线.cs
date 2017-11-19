using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers////TangChi 2015.10.20
{
    public class Spring : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent49 class.
        /// </summary>
        public Spring()
            : base("弹簧线", "Spring",
                "通过中点，起点，终点，高度，圈数，设置弹簧线",
                "Vipers", "Viper.point")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            Message = "弹簧线";
            pManager.AddPlaneParameter("平面","P","参考平面",GH_ParamAccess.item,Plane.WorldXY);
            pManager.AddPointParameter("中心点","P1","弹簧线的中点",GH_ParamAccess.item,new Point3d(0,0,0));
            pManager.AddPointParameter("起点", "P2", "弹簧线的起始点", GH_ParamAccess.item);
            pManager.AddPointParameter("终点", "P3", "弹簧线的终点", GH_ParamAccess.item);
            pManager.AddIntegerParameter("圈数","N","弹簧线的圈数",GH_ParamAccess.item,5);
            pManager.AddNumberParameter("高度","H","弹簧线的高度",GH_ParamAccess.item,1000);
            pManager.AddBooleanParameter("反转","R","反转方向",GH_ParamAccess.item,false);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
            pManager.HideParameter(2);
            pManager.HideParameter(3);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("弹簧线","C","根据条件得到的弹簧线",GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane plane = new Plane();
            Point3d centerPt=new Point3d();
            Point3d startPt=new Point3d();
            Point3d endPt=new Point3d();
            int cycles = 0;
            double height = 0;
            bool reverse = false;
            if(!DA.GetData(0,ref plane))return;
            if (!DA.GetData(1, ref centerPt)) return;
            if (!DA.GetData(2, ref startPt)) return;
            if (!DA.GetData(3, ref endPt)) return;
            if (!DA.GetData(4, ref cycles)) return;
            if (!DA.GetData(5, ref height)) return;
            if (!DA.GetData(6, ref reverse)) return;
            ////////////////////////////////////////////////////////////
            Plane p = plane;
            Point3d c = centerPt;
            Point3d s = startPt;
            Point3d e = endPt;
            Point3d c1 = p.ClosestPoint(c);
            Point3d s1 = p.ClosestPoint(s);
            Point3d e1 = p.ClosestPoint(e);
            Vector3d v1 = Point3d.Subtract(s1, c1);
            Vector3d v2 = Point3d.Subtract(e1, c1);
            double angle = Vector3d.VectorAngle(v1, v2);
            if (reverse == false)
            {
                double roal = angle + Math.PI * 2 * cycles;///总共旋转的度数
                int times = (1 + cycles) * 30;////旋转次数
                double dist = c.DistanceTo(s1);///半径
                Plane pp = new Plane(c1, v1, v2);///起始参考平面
                List<Point3d> pts = new List<Point3d>();
                for (int i = 0; i <= times; i++)
                {
                    pts.Add(s1);
                    s1.Transform(Transform.Rotation(roal / times, p.ZAxis, c1));
                    s1.Transform(Transform.Translation(p.ZAxis * (height / times)));
                }
                DA.SetData(0, Curve.CreateInterpolatedCurve(pts, 3));
            }
            else
            {
                angle = Math.PI * 2 - angle;
                double roal = angle + Math.PI * 2 * cycles;///总共旋转的度数
                int times = (1 + cycles) * 30;////旋转次数
                double dist = c1.DistanceTo(s1);///半径
                Plane pp = new Plane(c1, v1, v2);///起始参考平面
                List<Point3d> pts = new List<Point3d>();
                for (int i = 0; i <= times; i++)
                {
                    pts.Add(s1);
                    s1.Transform(Transform.Rotation(roal / times * (-1), p.ZAxis, c1));
                    s1.Transform(Transform.Translation(p.ZAxis * (height / times)));
                }
                DA.SetData(0, Curve.CreateInterpolatedCurve(pts, 3));
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
                //return Resource1.弹簧线;
                return Resource1.point_弹簧线;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{7fdafe63-5e5d-4074-a214-36a05cbbd765}"); }
        }
    }
}