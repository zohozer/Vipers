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
using Vipers;

namespace SuperVipers///////TangChi 2015.10.29(2015.12.4改)
{
    public class Rhombus : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent60 class.
        /// </summary>
        public Rhombus()
            : base("菱形网格", "Rhombus",
                "基于Lunchbox的菱形网格，修复了闭合曲面接缝bug",
                "Vipers", "Viper.surface")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quarternary; }
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            Message = "TC-1-02\n菱形网格";
            pManager.AddSurfaceParameter("曲面", "S", "待划分的曲面", GH_ParamAccess.item);
            pManager.AddIntegerParameter("u方向数量", "U", "u方向的网格数量", GH_ParamAccess.item, 50);
            pManager.AddIntegerParameter("v方向数量", "V", "v方向的网格数量", GH_ParamAccess.item, 50);
            pManager.HideParameter(0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("菱形网格", "C", "生成的菱形网格", GH_ParamAccess.tree);
            pManager.AddCurveParameter("三角形网格", "C", "生成的三角形网格", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Surface surface = null;
            int uCount = 10;
            int vCount = 10;
            if (!DA.GetData(0, ref surface)) return;
            if (!DA.GetData(1, ref uCount)) return;
            if (!DA.GetData(2, ref vCount)) return;
            /////////////////////////////////

            Surface x = surface;
            int un = uCount;
            int vn = vCount;
            /////////////////////////////////////////////////////////////
            x.SetDomain(0, new Interval(0, 1));
            x.SetDomain(1, new Interval(0, 1));
            double uu = 1.0 / (2 * un);
            double vv = 1.0 / (2 * vn);
            List<double> us = new List<double>();
            List<double> vs = new List<double>();
            DataTree<Point3d> pts1 = new DataTree<Point3d>();
            DataTree<Point3d> pts2 = new DataTree<Point3d>();
            //DataTree<Brep> bps = new DataTree<Brep>();/////四边形面
            //DataTree<Brep> bps2 = new DataTree<Brep>();/////三角形面
            DataTree<Polyline> Four = new DataTree<Polyline>();/////四边形面
            DataTree<Polyline> Three = new DataTree<Polyline>();/////三角形面
            double startU = 0;
            double startV = 0;
            int index1 = 0;
            int index2 = 0;
            int indexBrep = 0;
            #region///////////////////////////////////////////////////得到网格角点值
            for (int i = 0; i < 2 * un + 1; i++)
            {
                us.Add(startU);
                startU += uu;
            }
            for (int i = 0; i < 2 * vn + 1; i++)
            {
                vs.Add(startV);
                startV += vv;
            }
            ////////////////////////////////////////////////////////////
            for (int i = 1; i < 2 * un + 1; i += 2)
            {
                for (int j = 0; j < 2 * vn + 1; j += 2)
                {
                    pts1.Add(x.PointAt(us[i], vs[j]), new GH_Path(0, DA.Iteration, index1));
                }
                index1++;
            }
            for (int i = 0; i < 2 * un + 1; i += 2)
            {
                for (int j = 1; j < 2 * vn + 1; j += 2)
                {
                    pts2.Add(x.PointAt(us[i], vs[j]), new GH_Path(0, DA.Iteration, index2));
                }
                index2++;
            }
            #endregion;





            #region////////////////////////////////////////////四边形曲面
            for (int i = 1; i < 2 * un; i++)
            {
                if (i / 2.0 != Convert.ToInt32(i / 2.0))//奇数
                {
                    List<Point3d> ptuu = pts1.Branch((i - 1) / 2);
                    List<Point3d> ptvv1 = pts2.Branch((i - 1) / 2);
                    List<Point3d> ptvv2 = pts2.Branch((i - 1) / 2 + 1);
                    for (int k = 0; k < ptvv1.Count; k++)
                    {
                        Point3d p1 = ptuu[k];
                        Point3d p2 = ptuu[k + 1];
                        Point3d p3 = ptvv1[k];
                        Point3d p4 = ptvv2[k];
                        //Brep bb = Brep.CreateFromCornerPoints(p1, p3, p2, p4, 0);
                        //bps.Add(bb, new GH_Path(0, indexBrep));
                        Point3d[] points = { p1, p3, p2, p4, p1 };
                        Polyline pln = new Polyline(points);
                        Four.Add(pln, new GH_Path(0, DA.Iteration, indexBrep));//////////////////////////
                    }
                }
                else///偶数
                {
                    List<Point3d> ptxx = pts2.Branch(i / 2);
                    List<Point3d> ptyy = pts1.Branch(i / 2 - 1);
                    List<Point3d> ptzz = pts1.Branch(i / 2);
                    for (int q = 0; q < ptxx.Count - 1; q++)
                    {
                        Point3d pa = ptxx[q];
                        Point3d pb = ptxx[q + 1];
                        Point3d pc = ptyy[q + 1];
                        Point3d pd = ptzz[q + 1];
                        //Brep bb = Brep.CreateFromCornerPoints(pa, pc, pb, pd, 0);
                        //bps.Add(bb, new GH_Path(0, indexBrep));
                        Point3d[] points = { pa, pc, pb, pd, pa };
                        Polyline pln = new Polyline(points);
                        Four.Add(pln, new GH_Path(0, DA.Iteration, indexBrep));//////////////////////////
                    }
                }
                indexBrep++;
            }
            if (x.IsClosed(1))//////////////////////////////////如果v方向闭合
            {
                int indexnew = -1;
                for (int k = 0; k < pts2.BranchCount - 2; k++)
                {
                    Point3d pa = pts2.Branch(k + 1)[pts2.Branch(k).Count - 1];
                    Point3d pb = pts2.Branch(k + 1)[0];
                    Point3d pc = pts1.Branch(k)[0];
                    Point3d pd = pts1.Branch(k + 1)[0];
                    //Brep bb = Brep.CreateFromCornerPoints(pa, pc, pb, pd, 0);
                    //bps.Add(bb, new GH_Path(0, indexnew += 2));
                    Point3d[] points = { pa, pc, pb, pd, pa };
                    Polyline pln = new Polyline(points);
                    Four.Add(pln, new GH_Path(0, DA.Iteration, indexnew += 2));//////////////////////////
                }
            }
            if (x.IsClosed(0))//////////////////////////////////如果u方向闭合
            {
                int indexnew = -1;
                for (int k = 0; k < pts2.Branch(0).Count - 1; k++)
                {
                    Point3d pa = pts2.Branch(0)[k];
                    Point3d pb = pts2.Branch(0)[k + 1];
                    Point3d pc = pts1.Branch(0)[k + 1];
                    Point3d pd = pts1.Branch(pts1.BranchCount - 1)[k + 1];
                    //Brep bb = Brep.CreateFromCornerPoints(pa, pc, pb, pd, 0);
                    //bps.Add(bb, new GH_Path(0, indexnew += 2));
                    Point3d[] points = { pa, pc, pb, pd, pa };
                    Polyline pln = new Polyline(points);
                    Four.Add(pln, new GH_Path(0, DA.Iteration, indexnew += 2));//////////////////////////
                }
            }
            #endregion;
            #region////////////////////////////////////////////////////////////////////三角形曲面
            List<Point3d> ptsa1 = new List<Point3d>();
            ptsa1.Add(x.PointAt(0, 0));
            ptsa1.AddRange(pts2.Branch(0));
            ptsa1.Add(x.PointAt(0, 1));
            for (int i = 0; i < ptsa1.Count - 1; i++)
            {
                //Brep bb = Brep.CreateFromCornerPoints(ptsa1[i], ptsa1[i + 1], pts1.Branch(0)[i], 0);
                //bps2.Add(bb, new GH_Path(0, 0));
                Point3d[] points = { ptsa1[i], ptsa1[i + 1], pts1.Branch(0)[i], ptsa1[i] };
                Polyline pln = new Polyline(points);
                Three.Add(pln, new GH_Path(0, DA.Iteration, 0));//////////////////////////
            }
            ////////////////////////////////////////////////////////////////////////////第一组
            List<Point3d> ptsa2 = new List<Point3d>();
            ptsa2.Add(x.PointAt(1, 0));
            ptsa2.AddRange(pts2.Branch(pts2.BranchCount - 1));
            ptsa2.Add(x.PointAt(1, 1));
            for (int i = 0; i < ptsa2.Count - 1; i++)
            {
                //Brep bb = Brep.CreateFromCornerPoints(ptsa2[i], ptsa2[i + 1], pts1.Branch(pts1.BranchCount - 1)[i], 0);
                //bps2.Add(bb, new GH_Path(0, 1));
                Point3d[] points = { ptsa2[i], ptsa2[i + 1], pts1.Branch(pts1.BranchCount - 1)[i], ptsa2[i] };
                Polyline pln = new Polyline(points);
                Three.Add(pln, new GH_Path(0, DA.Iteration, 1));//////////////////////////
            }
            ////////////////////////////////////////////////////////////////////////////第二组
            for (int i = 0; i < pts1.BranchCount - 1; i++)
            {
                //Brep bb = Brep.CreateFromCornerPoints(pts1.Branch(i)[0], pts1.Branch(i + 1)[0], pts2.Branch(i + 1)[0], 0);
                //bps2.Add(bb, new GH_Path(0, 2));
                Point3d[] points = { pts1.Branch(i)[0], pts1.Branch(i + 1)[0], pts2.Branch(i + 1)[0], pts1.Branch(i)[0] };
                Polyline pln = new Polyline(points);
                Three.Add(pln, new GH_Path(0, DA.Iteration, 2));//////////////////////////
            }
            ////////////////////////////////////////////////////////////////////////////第三组
            for (int i = 0; i < pts1.BranchCount - 1; i++)
            {
                //Brep bb = Brep.CreateFromCornerPoints(pts1.Branch(i)[pts1.Branch(i).Count - 1], pts1.Branch(i + 1)[pts1.Branch(i).Count - 1], pts2.Branch(i + 1)[pts2.Branch(i + 1).Count - 1], 0);
                //bps2.Add(bb, new GH_Path(0, 3));
                Point3d[] points = { pts1.Branch(i)[pts1.Branch(i).Count - 1], pts1.Branch(i + 1)[pts1.Branch(i).Count - 1], pts2.Branch(i + 1)[pts2.Branch(i + 1).Count - 1], pts1.Branch(i)[pts1.Branch(i).Count - 1] };
                Polyline pln = new Polyline(points);
                Three.Add(pln, new GH_Path(0, DA.Iteration, 3));//////////////////////////
            }
            ////////////////////////////////////////////////////////////////////////////第四组
            if (x.IsClosed(1))///////////////////如果v方向闭合
            {
                DataTree<Polyline> Three2 = new DataTree<Polyline>();/////三角形边
                List<Polyline> poly1 = Three.Branch(0);
                List<Polyline> poly2 = Three.Branch(1);
                poly1.RemoveAt(0);
                poly1.RemoveAt(poly1.Count - 1);
                poly2.RemoveAt(0);
                poly2.RemoveAt(poly2.Count - 1);
                Point3d[] points = { pts2.Branch(0)[0], pts2.Branch(0)[pts2.Branch(0).Count - 1], pts1.Branch(0)[0], pts2.Branch(0)[0] };
                Polyline pln = new Polyline(points);
                poly1.Add(pln);
                Point3d[] points2 = { pts2.Branch(pts2.BranchCount - 1)[0], pts2.Branch(pts2.BranchCount - 1)[pts2.Branch(pts2.BranchCount - 1).Count - 1], pts1.Branch(pts1.BranchCount - 1)[0], pts2.Branch(pts2.BranchCount - 1)[0] };
                Polyline pln2 = new Polyline(points2);
                poly2.Add(pln2);
                Three2.AddRange(poly1, new GH_Path(0, DA.Iteration, 0));
                Three2.AddRange(poly2, new GH_Path(0, DA.Iteration, 1));
                Three.Clear();
                Three = Three2;
            }
            if (x.IsClosed(0))///////////////////如果u方向闭合
            {
                //DataTree<Brep> bps3 = new DataTree<Brep>();/////三角形面
                DataTree<Polyline> Three2 = new DataTree<Polyline>();/////三角形边
                //List<Brep> newpb1 = bps2.Branch(2);
                //List<Brep> newpb2 = bps2.Branch(3);
                List<Polyline> poly1 = Three.Branch(2);
                List<Polyline> poly2 = Three.Branch(3);
                //Brep bb1 = Brep.CreateFromCornerPoints(pts1.Branch(0)[0], pts1.Branch(pts1.BranchCount - 1)[0], pts2.Branch(0)[0], 0);
                //newpb1.Add(bb1);
                Point3d[] points = { pts1.Branch(0)[0], pts1.Branch(pts1.BranchCount - 1)[0], pts2.Branch(0)[0], pts1.Branch(0)[0] };
                Polyline pln = new Polyline(points);
                poly1.Add(pln);//////////////////////////
                //Brep bb2 = Brep.CreateFromCornerPoints(pts1.Branch(0)[pts1.Branch(0).Count - 1], pts1.Branch(pts1.BranchCount - 1)[pts1.Branch(pts1.BranchCount - 1).Count - 1], pts2.Branch(pts2.BranchCount - 1)[pts2.Branch(pts2.BranchCount - 1).Count - 1], 0);
                //newpb2.Add(bb2);
                Point3d[] points2 = { pts1.Branch(0)[pts1.Branch(0).Count - 1], pts1.Branch(pts1.BranchCount - 1)[pts1.Branch(pts1.BranchCount - 1).Count - 1], pts2.Branch(pts2.BranchCount - 1)[pts2.Branch(pts2.BranchCount - 1).Count - 1], pts1.Branch(0)[pts1.Branch(0).Count - 1] };
                Polyline pln2 = new Polyline(points2);
                poly2.Add(pln2);//////////////////////////
                ////
                //bps3.AddRange(newpb1, new GH_Path(0, 0));
                //bps3.AddRange(newpb2, new GH_Path(0, 1));
                //bps2 = bps3;
                Three2.AddRange(poly1, new GH_Path(0, DA.Iteration, 0));
                Three2.AddRange(poly2, new GH_Path(0, DA.Iteration, 1));
                Three = Three2;
            }
            #endregion;
            DA.SetDataTree(0, Four);
            DA.SetDataTree(1, Three);
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
                return Resource1.surface_网格表皮B;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{aac70368-508a-4782-8dd1-6996872c61a9}"); }
        }
    }
}