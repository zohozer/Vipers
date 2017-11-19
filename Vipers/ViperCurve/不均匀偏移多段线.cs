using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers///////TangChi 2016.5.11
{
    public class OptionalOffset : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the 不均匀偏移多段线 class.
        /// </summary>
        public OptionalOffset()
            : base("偏移闭合多段线", "OptionalOffset",
                "可通过设置布尔值(true或false)确定闭合多段线的偏移模式，如果为true，则多段线的每段线段按照列表中的数值统一偏移，如果为false，则每段对应列表的每个数值分别偏移",
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
            pManager.AddCurveParameter("多段线","P","待偏移的闭合多段线",GH_ParamAccess.item);
            pManager.AddNumberParameter("偏移列表","D","多段线中的每段线段按照列表中的数值统一或者分段偏移",GH_ParamAccess.list);
            pManager.AddBooleanParameter("切换","B","如果为true，则多段线的每段线段按照列表中的数值统一偏移，如果为false，则每段对应列表的每个数值分别偏移",GH_ParamAccess.item,true);
            pManager.HideParameter(0);
            Message = "偏移闭合多段线";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("多段线","P","偏移后的闭合多段线",GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve curve = null;
            List<double> D = new List<double>();
            bool B = true;
            if(!DA.GetData(0,ref curve))return;
            if(!DA.GetDataList(1,D))return;
            if(!DA.GetData(2,ref B))return;
            Polyline C = new Polyline();
            if (!curve.TryGetPolyline(out C))
            {
                return;
            }
            //////////////////////////////////////////////////////
            List<Polyline> last = new List<Polyline>();
            List<List<double>> collect = new List<List<double>>();
            if (B)////
            {
                for (int q = 0; q < D.Count; q++)
                {
                    List<double> one = new List<double>();
                    for (int i = 0; i < C.SegmentCount; i++)
                    {
                        one.Add(D[q]);
                    }
                    collect.Add(one);
                }
            }
            else
            {
                collect.Add(D);
            }

            ////////////////////////////////////////////////
            for (int k = 0; k < collect.Count; k++)
            {
                last.Add(OffsetPolyline(C, collect[k]));
            }
            DA.SetDataList(0, last);
        }
        public Polyline OffsetPolyline(Polyline C, List<double> D) //////不均匀偏移多边形
        {
            List<Point3d> pts = new List<Point3d>();
            List<Polyline> lastPly = new List<Polyline>();
            for (int i = 0; i < C.SegmentCount; i++)
            {
                pts.Add(C.SegmentAt(i).From);
            }
            Plane pln;
            Plane.FitPlaneToPoints(pts, out pln);
            pln.Origin = C.CenterPoint();
            List<Line> cs = new List<Line>();
            for (int i = 0; i < C.SegmentCount; i++)
            {
                Line test = C.SegmentAt(i);
                Point3d clost = PtToLine(C.CenterPoint(), test);
                Vector3d vc = Point3d.Subtract(C.CenterPoint(), clost);
                vc.Unitize();
                if (D.Count > i)/////长度列表的长度小于边数
                {
                    test.Transform(Transform.Translation(vc * D[i]));
                }
                cs.Add(test);
            }
            ////////////////////////////偏移每段线段
            List<Point3d> last = new List<Point3d>();
            for (int i = 0; i < cs.Count; i++)
            {
                Point3d pt1;
                Point3d pt2;
                if (i == cs.Count - 1)
                {
                    ClosedPt(cs[0], cs[cs.Count - 1], out pt1, out pt2);
                    last.Add((pt1 + pt2) / 2);
                    continue;
                }
                ClosedPt(cs[i], cs[i + 1], out pt1, out pt2);
                last.Add((pt1 + pt2) / 2);
            }
            last.Add(last[0]);
            return new Polyline(last);
        }


        public void ClosedPt(Line x, Line y, out Point3d last1, out Point3d last2) /////两条线段最近点算法
        {
            last1 = Point3d.Unset;
            last2 = Point3d.Unset;
            Point3d p1 = x.From;/////A(x1,y1,z1)
            Point3d p2 = x.To;//////B(x2,y2,z2)
            Point3d p3 = y.From;//////C(x3,y3,z3)
            Point3d p4 = y.To;//////D(x4,y4,z4)
            double s1 = Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2) + Math.Pow((p2.Z - p1.Z), 2);
            double t1 = (p2.X - p1.X) * (p4.X - p3.X) + (p2.Y - p1.Y) * (p4.Y - p3.Y) + (p2.Z - p1.Z) * (p4.Z - p3.Z);
            double r1 = (p1.X - p2.X) * (p1.X - p3.X) + (p1.Y - p2.Y) * (p1.Y - p3.Y) + (p1.Z - p2.Z) * (p1.Z - p3.Z);
            double s2 = -1 * t1;
            double t2 = Math.Pow((p4.X - p3.X), 2) + Math.Pow((p4.Y - p3.Y), 2) + Math.Pow((p4.Z - p3.Z), 2);
            double r2 = (p1.X - p3.X) * (p4.X - p3.X) + (p1.Y - p3.Y) * (p4.Y - p3.Y) + (p1.Z - p3.Z) * (p4.Z - p3.Z);
            ///////// s1*X-t1*Y=r1
            ///////// s2*X+t2*Y=r2
            if (s1 * t2 + (-1 * t1 * s2) != 0)/////向量法有解的情况
            {
                double ct1 = (t2 * r1 - r2 * (-1 * t1)) / (s1 * t2 - s2 * (-1 * t1));
                double ct2 = (s1 * r2 - r1 * s2) / (s1 * t2 - s2 * (-1 * t1));
                //List<Point3d> ptsss = new List<Point3d>();
                last1 = new Point3d(p1.X + (p2.X - p1.X) * ct1, p1.Y + (p2.Y - p1.Y) * ct1, p1.Z + (p2.Z - p1.Z) * ct1);
                last2 = new Point3d(p3.X + (p4.X - p3.X) * ct2, p3.Y + (p4.Y - p3.Y) * ct2, p3.Z + (p4.Z - p3.Z) * ct2);
            }
            return;
        }
        public Point3d PtToLine(Point3d x, Line y) ////////////////点到直线最近点
        {
            Plane pln = new Plane(x, Point3d.Subtract(y.From, y.To));
            Point3d last = y.To;
            if (Math.Abs(pln.DistanceTo(y.From)) < Math.Abs(pln.DistanceTo(y.To)))
            {
                last = y.From;
            }
            last = pln.ClosestPoint(last);
            return last;
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
                return Resource1.curve_不均匀偏移多段线;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{d2629bac-2099-4f70-a892-d410d872a6fb}"); }
        }
    }
}