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

namespace Vipers
{
    class ViperClass
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////首尾连接的两条直线求角度(精度自己调整)
        public static double getangle(Line a, Line b)
        {
            Vector3d vcx = new Vector3d();
            Vector3d vcy = new Vector3d();
            Point3d pt1 = a.From;
            Point3d pt2 = a.To;
            Point3d pt3 = b.From;
            Point3d pt4 = b.To;
            vcx = Point3d.Subtract(pt2, pt1);
            vcy = Point3d.Subtract(pt3, pt4);
            double angle = Vector3d.VectorAngle(vcx, vcy);
            return angle;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////给定曲线找出其控制点
        public static List<Point3d> Ptsss(Curve x)
        {
            NurbsCurve mm = x.ToNurbsCurve();
            ControlPoint list = new ControlPoint();
            List<Point3d> ptss = new List<Point3d>();
            Rhino.Geometry.Collections.NurbsCurvePointList listA = mm.Points;
            for (int i = 0; i < listA.Count; i++)
            {
                list = listA.ElementAt(i);
                Point3d pt = list.Location;
                ptss.Add(pt);
            }
            List<Point3d> ptssResult = ptss.Distinct().ToList();/////删除重合部分（多边形）
            return ptssResult;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////给定两点，判断其余空间点是否共线
        public static  bool gxline(Point3d x, Point3d y, Point3d z)
        {
            bool mm = false;
            double a = Math.Round((x.X - y.X) * (x.Y - z.Y), 2);//////因为小数精度越大越容易出现错误，所以四舍五入到第二位
            double b = Math.Round((x.Y - y.Y) * (x.X - z.X), 2);
            double c = Math.Round((x.X - y.X) * (x.Z - z.Z), 2);
            double d = Math.Round((x.X - z.X) * (x.Z - y.Z), 2);
            double e = Math.Round((z.Y - x.Y) * (x.Z - y.Z), 2);
            double f = Math.Round((x.Y - y.Y) * (z.Z - x.Z), 2);
            if (a == b && c == d && e == f)
            {
                mm = true;
            }
            return mm;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////点云连线
        public static List<Line> lineToLine(List<Point3d> Points)
        {
            int nums = Points.Count;
            List<Line> lis = new List<Line>();
            for (int i = 0; i < nums - 1; i++)
            {
                for (int j = i + 1; j < nums; j++)
                {
                    Line li = new Line(Points[i], Points[j]);
                    lis.Add(li);
                }
            }
            return lis;       
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////将一组curve转换为polyline
        public static List<Polyline> getPolyline(List<Curve> x)
        {
            List<Polyline> pls = new List<Polyline>();
            for (int i = 0; i < x.Count; i++)
            {
                Polyline p = new Polyline();
                Curve c = x[i];
                if (c.TryGetPolyline(out p))
                {
                    pls.Add(p);
                }
            }
            return pls;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////删除有重合的曲线（只要有重合部分，都删除）
        public static List<Curve> deleteSameCurve(List<Curve> x, double tolerance)
        {
            List<Curve> afterCr = new List<Curve>();
            for (int i = 0; i < x.Count; i += 0)
            {
                Curve a = x[i];
                int notOne = 0;
                afterCr.Add(a);
                x.RemoveAt(i);
                for (int j = 0; j < x.Count; j++)
                {
                    Curve b = x[j];
                    Rhino.Geometry.Intersect.CurveIntersections cin = Rhino.Geometry.Intersect.Intersection.CurveCurve(a, b, 0, 0);
                    int mm = cin.Count;
                    double toler = Math.Abs(a.GetLength() - b.GetLength());
                    Point3d pa = a.PointAtNormalizedLength(0.5);
                    Point3d pb = b.PointAtNormalizedLength(0.5);
                    double distance = Math.Abs(pa.DistanceTo(pb));
                    if (mm == 1 && toler <= tolerance && distance <= tolerance)
                    {
                        notOne += 1;
                        x.RemoveAt(j);
                        j--;
                    }
                }
                if (notOne > 0)
                {
                    afterCr.RemoveAt(afterCr.Count - 1);
                }
            }
            return afterCr;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////给定一个向量，一个已知点，判断另一点是否共线
        public static bool gxline(Vector3d vc, Point3d getpt, Point3d judgept)
        {
            bool mm = false;
            double a = Math.Round(vc.X * (getpt.Y - judgept.Y), 2);//////因为小数精度越大越容易出现错误，所以四舍五入到第二位
            double b = Math.Round(vc.Y * (getpt.X - judgept.X), 2);
            double c = Math.Round(vc.X * (getpt.Z - judgept.Z), 2);
            double d = Math.Round((getpt.X - judgept.X) * vc.Z, 2);
            double e = Math.Round((getpt.Y - judgept.Y) * vc.Z, 2);
            double f = Math.Round(vc.Y * (getpt.Z - judgept.Z), 2);
            if (a == b && c == d && e == f)
            {
                mm = true;
            }
            return mm;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////点在新坐标平面的位置
        public static Point3d newPt(Point3d point, Plane targetPlane)
        {
            Point3d o = targetPlane.Origin;
            Vector3d xx = targetPlane.XAxis;
            Vector3d yy = targetPlane.YAxis;
            Vector3d zz = targetPlane.ZAxis;
            Plane pxy = new Plane(o, zz);
            Plane pyz = new Plane(o, xx);
            Plane pxz = new Plane(o, yy);
            double ax = pyz.DistanceTo(point);
            Point3d ptx = pyz.ClosestPoint(point);
            Vector3d vx = Point3d.Subtract(ptx, point);
            double ay = pxz.DistanceTo(point);
            Point3d pty = pxz.ClosestPoint(point);
            Vector3d vy = Point3d.Subtract(pty, point);
            double az = pxy.DistanceTo(point);
            Point3d ptz = pxy.ClosestPoint(point);
            Vector3d vz = Point3d.Subtract(ptz, point);
            Point3d newpt = new Point3d(ax, ay, az);
            return newpt;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////得到节点的t值
        public static List<double> curveKnot(Curve x)
        {
            NurbsCurve nc = x.ToNurbsCurve();
            Rhino.Geometry.Collections.NurbsCurveKnotList knot = nc.Knots;
            List<double> knotT = new List<double>();
            for (int i = 0; i < knot.Count; i++)
            {
                knotT.Add(knot.ElementAt(i));
            }
            List<double> knots = knotT.Distinct().ToList();
            return knots;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////在指定平面创建指定边数多边形
        public static Polyline CreatePolyline(Plane x, double y, int n)
        {
            Circle c = new Circle(x, y);
            NurbsCurve cr = c.ToNurbsCurve();
            Point3d[] pts = new Point3d[n];
            cr.DivideByCount(n, true, out pts);
            List<Point3d> pts2 = pts.ToList();
            pts2.Add(cr.PointAtStart);
            Polyline pl = new Polyline(pts2);
            return pl;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////求一组点的中点
        public static Point3d center(List<Point3d> pts)
        {
            double xx = 0;
            double yy = 0;
            double zz = 0;
            for (int i = 0; i < pts.Count; i++)
            {
                xx += pts[i].X;
                yy += pts[i].Y;
                zz += pts[i].Z;
            }
            Point3d cc = new Point3d(xx / pts.Count, yy / pts.Count, zz / pts.Count);
            return cc;
        }

        /////////////////////////////////////判断点云是否位于线段同侧
        public static bool SameSide(Line x, List<Point3d> y)
        {
            Vector3d vc = Vector3d.Unset;
            for (int q = 0; q < y.Count; q++)
            {
                if (x.ClosestPoint(y[q], false).DistanceTo(y[q]) >= 0.00000001)
                {
                    vc = Point3d.Subtract(y[q], x.ClosestPoint(y[q], false));
                    break;
                }
            }
            bool flag = true;
            for (int i = 1; i < y.Count; i++)
            {
                if (vc * Point3d.Subtract(y[i], x.ClosestPoint(y[i], false)) < 0)/////如果点在线段上也视为同侧
                {
                    flag = false;
                    break;
                }
            }
            return flag;
        }
        public static void ClosedPt(Line x, Line y, out Point3d last1, out Point3d last2) /////两条线段最近点算法
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
        public static Point3d PtToLine(Point3d x, Line y) ////////////////点到直线最近点
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
        ///////////////////////////////////////////////////////////////////////////创建一个有N个分支的空树
        public static DataTree<Point3d> EmptyTree(int n) 
        {
            DataTree<Point3d> tree = new DataTree<Point3d>();
            List<Point3d> kk = new List<Point3d>();
            for (int k = 0; k < n; k++)
            {
                tree.AddRange(kk, new GH_Path(0, k));
            }
            return tree;
        }
        //////////////////////////////////////////////////////////////////////////////一组点切一条曲线
        public static Curve[] SplitByPts(Curve c, List<Point3d> pts) 
        {
            List<double> ts = new List<double>();
            double t = 0;
            if (c.IsClosed)
            {
                c.ClosestPoint(pts[0], out t);
                c.ChangeClosedCurveSeam(t);
            }
            for (int i = 0; i < pts.Count; i++)
            {
                c.ClosestPoint(pts[i], out t);
                ts.Add(t);
            }
            return c.Split(ts);
        }
        public  static int CurveSide(Curve destination,Point3d unset,Plane plane)//////////////判断点在指定平面内曲线的左侧还是右侧还是在曲线上（用的是gh的curveside同样的方法）
        {
            int last = 2;
                     if (plane.IsValid)
                    {
                        double num;
                        destination.ClosestPoint(unset, out num);
                        Point3d pointd2 = destination.PointAt(num);
                        if (pointd2.DistanceTo(unset) < 0.00001)
                        {
                            last = 0;
                        }
                        else
                        {
                            Vector3d b = destination.TangentAt(num);
                            b.Transform(Transform.PlanarProjection(plane));
                            if (b.IsTiny())
                            {
                                last = 0;
                            }
                            else
                            {
                                Vector3d a = (Vector3d)(pointd2 - unset);
                                Vector3d vectord3 = Vector3d.CrossProduct(a, b);
                                if (!vectord3.IsValid)
                                {
                                    last = 0;
                                }
                                else
                                {
                                    switch (vectord3.IsParallelTo(plane.ZAxis, 1.5707963267948966))
                                    {
                                        case 0:
                                            last = 0;
                                            break;

                                        case -1:
                                            last = -1;
                                            break;

                                        case 1:
                                            last = 1;
                                            break;
                                    }
                                }
                            }
                        }
                    }
                     return last;
        }
    }
    class SimplifiedTraditional/////繁简转换
    {

        /// <summary>
        /// 中文字符工具类
        /// </summary>
        private const int LOCALE_SYSTEM_DEFAULT = 0x0800;
        private const int LCMAP_SIMPLIFIED_CHINESE = 0x02000000;
        private const int LCMAP_TRADITIONAL_CHINESE = 0x04000000;
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LCMapString(int Locale, int dwMapFlags, string lpSrcStr, int cchSrc, [Out] string lpDestStr, int cchDest);

        /// <summary>
        /// 将字符转换成简体中文
        /// </summary>
        /// <param name="source">输入要转换的字符串</param>
        /// <returns>转换完成后的字符串</returns>
        public static string ToSimplified(string source)
        {
            String target = new String(' ', source.Length);
            int ret = LCMapString(LOCALE_SYSTEM_DEFAULT, LCMAP_SIMPLIFIED_CHINESE, source, source.Length, target, source.Length);
            return target;
        }

        /// <summary>
        /// 讲字符转换为繁体中文
        /// </summary>
        /// <param name="source">输入要转换的字符串</param>
        /// <returns>转换完成后的字符串</returns>
        public static string ToTraditional(string source)
        {
            String target = new String(' ', source.Length);
            int ret = LCMapString(LOCALE_SYSTEM_DEFAULT, LCMAP_TRADITIONAL_CHINESE, source, source.Length, target, source.Length);
            return target;
        }
    }
    class SuperVipersClass
    {
        public static List<Point3d> surfacePoint(Surface x, out int U, out int V) //////得到曲面控制点
        {
            U = 0;///u方向控制点数量
            V = 0;///v方向控制点数量
            NurbsSurface sf = x.ToNurbsSurface();
            Rhino.Geometry.Collections.NurbsSurfacePointList list = sf.Points;
            U = list.CountU;
            V = list.CountV;
            List<Point3d> pts = new List<Point3d>();
            for (int i = 0; i < list.CountU; i++)
            {
                for (int j = 0; j < list.CountV; j++)
                {
                    ControlPoint ps = list.GetControlPoint(i, j);
                    pts.Add(ps.Location);
                }
            }
            return pts;
        }
        public static Point3d centerPoint(List<Point3d> p) ////////得到一组点的中点
        {
            List<double> x = new List<double>();
            List<double> y = new List<double>();
            List<double> z = new List<double>();
            for (int i = 0; i < p.Count; i++)
            {
                x.Add(p[i].X);
                y.Add(p[i].Y);
                z.Add(p[i].Z);
            }
            double xa = x.Sum() / p.Count;
            double xb = y.Sum() / p.Count;
            double xc = z.Sum() / p.Count;
            Point3d ptnew = new Point3d(xa, xb, xc);
            return ptnew;
        }
        public static List<Point3d> sortPoints(List<Point3d> x, Curve y) //////点沿曲线排序
        {
            List<double> ts = new List<double>();
            List<Point3d> pts = new List<Point3d>();
            for (int i = 0; i < x.Count; i++)
            {
                double t;
                y.ClosestPoint(x[i], out t);
                ts.Add(t);
            }
            double[] ts2 = ts.ToArray();
            ts.Sort();
            for (int i = 0; i < ts.Count; i++)
            {
                for (int j = 0; j < ts.Count; j++)
                {
                    if (ts[i] == ts2[j])
                    {
                        ts2[j] = double.PositiveInfinity;
                        pts.Add(x[j]);
                        break;
                    }
                }
            }
            return pts;
        }
    }
}
