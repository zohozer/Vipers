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

namespace Vipers//2015.1.3 TangChi
{
    public class PolygonEmbedPolygon : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent6 class.
        /// </summary>
        public PolygonEmbedPolygon()
            : base("多边形内嵌多边形", "PolygonEmbedPolygon",
                "在多边形内部嵌入多边形",
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
            pManager.AddCurveParameter("多边形","P","多边形",GH_ParamAccess.item);
            pManager.AddNumberParameter("偏移位置", "P", "输入0~1之间的数值", GH_ParamAccess.item,0.2);
            pManager.AddNumberParameter("偏移距离", "O", "相邻多段线之间偏移的距离", GH_ParamAccess.item, 0);
            pManager.AddIntegerParameter("数量", "N", "内嵌多边形的数量", GH_ParamAccess.item,10);
            pManager.AddBooleanParameter("首项","A","是否加入第一个多边形",GH_ParamAccess.item,true);
            pManager.HideParameter(0);
            Message = "多边形内嵌多边形";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("生成多边形", "P", "生成多边形", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve polyline1 = null;
            double Position = 0;
            int Num = 0;
            bool addFirst=true;
            double offset = 0;
            if (!DA.GetData(0, ref polyline1)) return;
            if (!DA.GetData(1, ref Position)) return;
            if (!DA.GetData(2, ref  offset)) return;
            if (!DA.GetData(3, ref Num)) return;
            if(!DA.GetData(4,ref  addFirst))return;
            Polyline polyline = new Polyline();
            polyline1.TryGetPolyline(out polyline);
            List<Polyline> last = new List<Polyline>();
            last.Add(polyline);
            for (int i = 0; i < Num; i++)
            {
                Polyline one = last[last.Count - 1];
                one = PolylineOffset(one, offset);
                if (one.IsClosed == false)
                {
                    break;
                }
                last.Add(Pl(one, Position));
            }
            if (addFirst == false)
            {
                last.RemoveAt(0);
            }
            DA.SetDataList(0, last);
        }
        public Polyline Pl(Polyline pli, double y) ///////多边形内嵌多边形方法
        {
            Line[] a = pli.GetSegments();
            int num = a.Length;
            Curve[] aa = new Curve[num];
            Point3d[] pts = new Point3d[num + 1];
            for (int i = 0; i < num; i++)
            {
                aa[i] = a[i].ToNurbsCurve();
                pts[i] = aa[i].PointAt(y);
            }
            pts[num] = pts[0];//////////////////////形成闭合polyline
            Polyline newpl = new Polyline(pts);
            return newpl;
        }
        public Polyline PolylineOffset(Polyline C, double D) /////////////////////偏移闭合多段线(精确算法)
        {
            List<Point3d> pts = new List<Point3d>();
            for (int i = 0; i < C.SegmentCount; i++)
            {
                pts.Add(C.SegmentAt(i).From);
            }
            Plane pln;
            Plane.FitPlaneToPoints(pts, out pln);
            pln.Origin = C.CenterPoint();
            List<Curve> cs = new List<Curve>();
            for (int i = 0; i < C.SegmentCount; i++)
            {
                Curve test = C.SegmentAt(i).ToNurbsCurve();
                double t = 0;
                test.ClosestPoint(C.CenterPoint(), out t);
                Point3d clost = test.PointAt(t);
                Vector3d vc = Point3d.Subtract(C.CenterPoint(), clost);
                vc.Unitize();
                test.Transform(Transform.Translation(vc * D));
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

        public void ClosedPt(Curve x, Curve y, out Point3d last1, out Point3d last2) /////两条线段最近点算法
        {
            last1 = Point3d.Unset;
            last2 = Point3d.Unset;
            Point3d p1 = x.PointAtStart;/////A(x1,y1,z1)
            Point3d p2 = x.PointAtEnd;//////B(x2,y2,z2)
            Point3d p3 = y.PointAtStart;//////C(x3,y3,z3)
            Point3d p4 = y.PointAtEnd;//////D(x4,y4,z4)
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
      
        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                //return Resource1.多边形内嵌多边形;
                return Resource1.curve_多边形内嵌多边形;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{7ab5b8c5-253e-4993-80f8-a1912f8231e3}"); }
        }
    }
}