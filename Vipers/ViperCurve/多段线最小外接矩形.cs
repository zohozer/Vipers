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

namespace Vipers//////TangChi 2016.01.19(2016.09.28改)(2016.10.4改)
{
    public class MinimumBoundingRectangle : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the 多段线最小外接矩形 class.
        /// </summary>
        public MinimumBoundingRectangle()
            : base("多段线最小外接矩形", "MBR",
                "求任意平面多段线（可不闭合，可为凹多边形）的最小外接矩形",
                "Vipers", "viper.curve")
        {
            Message = "多段线最小外接矩形";
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
            pManager.AddCurveParameter("多段线", "P", "待求最小外接矩形的多段线", GH_ParamAccess.item);
            pManager.HideParameter(0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddRectangleParameter("矩形", "R", "多段线的最小外接矩形", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve x = null;
            if(!DA.GetData(0,ref x))return;
            Polyline ply = new Polyline();
            if(!x.TryGetPolyline(out ply))return;
            List<Point3d> corners = new List<Point3d>();
            List<int> index = new List<int>();
            for (int i = 0; i < ply.SegmentCount; i++)
            {
                corners.Add(ply.SegmentAt(i).From);
                corners.Add(ply.SegmentAt(i).To);
            }
            corners = corners.Distinct().ToList();
            Plane pln;
            Plane.FitPlaneToPoints(corners, out pln);
            Polyline Tply = HullPolyline(corners, pln, out index);
            DA.SetData(0, MinRec(Tply));
        }
        public Rectangle3d MinRec(Polyline ply) ////////////////////空间任意平面凸多边形的最小外接矩形
        {
            Line[] lines = ply.GetSegments();
            List<Point3d> pts = new List<Point3d>();/////多边形角点
            List<Point3d> collect = new List<Point3d>();
            Vector3d newY = Vector3d.Unset;//////新的Y向量
            Rectangle3d last = new Rectangle3d();
            double area = double.MaxValue;
            for (int i = 0; i < lines.Length; i++)
            {
                pts.Add(lines[i].From);
            }
            Plane pln;
            Plane.FitPlaneToPoints(pts, out pln);///多边形的平面
            for (int i = 0; i < lines.Length; i++)
            {
                Line li = lines[i];
                for (int q = 0; q < pts.Count; q++)
                {
                    collect.Add(li.ClosestPoint(pts[q], false));
                    if (li.ClosestPoint(pts[q], false).DistanceTo(pts[q]) > 0.0000001)
                        newY = Point3d.Subtract(pts[q], li.ClosestPoint(pts[q], false));
                }
                collect.Sort();
                Point3d start = collect[0];
                Point3d end = collect[collect.Count - 1];
                Line li3 = new Line(start, newY, 100);///与li2的垂直线
                pln = new Plane(start, li.Direction, newY);
                collect.Clear();
                for (int q = 0; q < pts.Count; q++)
                {
                    collect.Add(li3.ClosestPoint(pts[q], false));
                }
                collect.Sort();
                Point3d start2 = collect[0];
                Point3d end2 = collect[collect.Count - 1];
                Point3d corner1 = end;
                Point3d corner2 = start2;
                if (start.DistanceTo(start2) < 0.000000001)///两点重合，排除
                    corner2 = end2;
                Rectangle3d rec = new Rectangle3d(pln, corner1, corner2);
                if (rec.Area < area)
                {
                    last = rec;
                    area = rec.Area;
                }
                collect.Clear();
            }
            return last;
        }
        public List<Point3d> newpts(List<Point3d> x, Plane pln, out List<int> index) //////////////////////////////////////////沿x轴排序
        {
            List<Point3d> ptsnew = new List<Point3d>();///重新排序的点
            List<Point3d> pts = new List<Point3d>();
            index = new List<int>();
            List<double> ptsX = new List<double>();
            for (int i = 0; i < x.Count; i++)/////remap
            {
                Point3d pt;
                pln.RemapToPlaneSpace(x[i], out pt);
                pts.Add(pt);
            }
            for (int i = 0; i < pts.Count; i++)
            {
                ptsX.Add(pts[i].X);
            }
            double[] ptsX2 = ptsX.ToArray();//原来点的x坐标值
            ptsX.Sort();///x坐标值排序
            for (int j = 0; j < ptsX.Count; j++)
            {
                for (int k = 0; k < ptsX2.Length; k++)
                {
                    if (ptsX[j] == ptsX2[k])
                    {
                        ptsX2[k] += double.PositiveInfinity;
                        index.Add(k);
                        ptsnew.Add(x[k]);
                        break;
                    }
                }
            }
            return ptsnew;
        }
        public Polyline HullPolyline(List<Point3d> pts, Plane pln, out List<int> indexLast) ///////////////////////////////根据（共面）点与平面创建包裹线
        {
            List<int> indexs = new List<int>();
            indexLast = new List<int>();
            pts = newpts(pts, pln, out indexs);
            Point3d start = pts[0];///包裹线起始点
            pln.Origin = start;
            Vector3d vc = pln.YAxis; ///判断起始轴与该轴夹角越小及包裹点
            List<Point3d> last = new List<Point3d>();/////包裹点
            last.Add(start);
            pts[0] = Point3d.Unset;
            int index = 0;
            indexLast.Add(indexs[0]);
            while (true)
            {
                double angle = double.MaxValue;///判断角度
                Point3d collect = Point3d.Unset;
                for (int i = 0; i < pts.Count; i++)
                {
                    if (pts[i] == Point3d.Unset) continue;
                    Vector3d vc2 = Point3d.Subtract(pts[i], last[last.Count - 1]);
                    if (Vector3d.VectorAngle(vc, vc2) < angle)
                    {
                        collect = pts[i];
                        angle = Vector3d.VectorAngle(vc, vc2);
                        index = i;
                    }
                }
                Vector3d vc3 = Point3d.Subtract(start, last[last.Count - 1]);///最后与起始点做比较
                if (angle > Vector3d.VectorAngle(vc3, vc) && last.Count != 1)
                {
                    last.Add(start);
                    break;
                }
                pts[index] = Point3d.Unset;
                vc = Point3d.Subtract(collect, last[last.Count - 1]);
                last.Add(collect);
                indexLast.Add(indexs[index]);
            }
            return new Polyline(last);
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
                return Resource1.curve_多边形最小外接矩形;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{b0504f3b-feac-485c-8ba2-7690d0d3a3cd}"); }
        }
    }
}