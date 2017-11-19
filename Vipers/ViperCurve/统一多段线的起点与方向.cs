using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Collections;

namespace Vipers ///TangChi 2015.11.24
{
    public class PolygonDirection : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent52 class.
        /// </summary>
        public PolygonDirection()
            : base("统一多段线起点方向", "PolygonDirection",
                "指定平面中统一多段线起点方向（同时多段线统一为该平面的顺时针方向）",
                "Vipers", "Viper.curve")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("多段线","P","待更改的多段线",GH_ParamAccess.item);
            pManager.AddPlaneParameter("平面","P","参考平面",GH_ParamAccess.item,Plane.WorldXY);
            pManager.AddIntegerParameter("切换","C","切换起点方向:0为y轴正方向，1为x轴正方向，2为y轴负方向，3为x轴负方向",GH_ParamAccess.item,0);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
            Message = "统一多段线的起点与方向";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("多段线","P","更改方向和起始点的多段线",GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve polyline=null;
            Plane plane = new Plane();
            int change = 0;
            if (!DA.GetData(0, ref polyline)) return;
            if (!DA.GetData(1, ref plane)) return;
            if (!DA.GetData(2, ref change)) return;

            Polyline x=new Polyline();
            polyline.TryGetPolyline(out x);
            Plane y = plane;
            int z = change;
            //////////////////////////////////////////////
            Vector3d standar = y.YAxis;
            Vector3d vx = y.XAxis;
            Vector3d vy = y.YAxis;
            vx.Reverse();
            vy.Reverse();
            switch (z)
            {
                case 0: standar = y.YAxis;
                    break;
                case 1: standar = y.XAxis;
                    break;
                case 2: standar = vy;
                    break;
                default: standar = vx;
                    break;
            }

            List<Point3d> pts = new List<Point3d>();
            NurbsCurve nc = x.ToNurbsCurve();
            if (nc.ClosedCurveOrientation(y) == CurveOrientation.Clockwise)
            {
                nc.Reverse();
            }
            for (int i = 0; i < nc.Points.Count; i++)
            {
                pts.Add(nc.Points[i].Location);
            }
            double test = Math.PI;
            int index = 0;
            Point3d center = x.CenterPoint();
            for (int j = 0; j < pts.Count - 1; j++)
            {
                Vector3d vv = Point3d.Subtract(pts[j], center);
                double angle = Vector3d.VectorAngle(vv, standar);
                if (angle <= test)
                {
                    test = angle;
                    index = j;
                }
            }
            pts.RemoveAt(pts.Count - 1);
            List<Point3d> newlist = reverse(pts, index);
            newlist.Add(newlist[0]);
            DA.SetData(0, new Polyline(newlist));
        }
        public List<Point3d> reverse(List<Point3d> x, int y) //////////移动列表x中的数据y次。
        {

            int nnn = y % x.Count;//////使列表的项数循环
            List<Point3d> index = new List<Point3d>();///原来列表
            for (int i = 0; i < x.Count; i++)
            {
                index.Add(x[i]);
            }
            List<Point3d> index2 = new List<Point3d>();//现列表
            for (int i = nnn; i <= x.Count - 1; i++)
            {
                index2.Add(x[i]);
            }

            for (int i = 0; i < nnn; i++)
            {
                index2.Add(x[i]);
            }
            return index2;
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
                //return Resource1.统一闭合多段线方向;
                return Resource1.curve_统一多段线起点及方向;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{e4ac6d30-f511-4d4e-9138-ef8d04997636}"); }
        }
    }
}