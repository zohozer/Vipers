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
    public class Bricks : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Bricks()
            : base("Brick Panels", "Bricks",
                "Based on Lunchbox Staggered Quads",
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
            Message = "Brick Panels";
            pManager.AddSurfaceParameter("Surface", "S", "Surface to divide", GH_ParamAccess.item);
            pManager.AddIntegerParameter("U Count", "U", "Number of divisions in U direction", GH_ParamAccess.item, 10);
            pManager.AddIntegerParameter("V Count", "V", "Number of divisions in V direction", GH_ParamAccess.item, 10);
            pManager.HideParameter(0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Bricks", "B", "Edge lines", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            /////TangChi 2016.3.11
            Surface surface = null;
            int uCount = 10;
            int vCount = 10;
            if (!DA.GetData(0, ref surface)) return;
            if (!DA.GetData(1, ref uCount)) return;
            if (!DA.GetData(2, ref vCount)) return;
            Surface x = surface;
            int u = uCount;
            int v = vCount;
            if (x.IsClosed(0))///////如果u方向闭合，将u值强行转换为偶数
            {
                u = Convert.ToInt32(Math.Round(u / 2.0)) * 2;
            }
            x.SetDomain(0, new Interval(0, 1));
            x.SetDomain(1, new Interval(0, 1));
            List<double> us = new List<double>();
            List<double> vs = new List<double>();
            double start = 0;
            double segU = 1.0 / u;
            double segV = 1.0 / v;
            for (int i = 0; i < u + 1; i++)
            {
                us.Add(start);
                start += segU;
            }
            start = 0;
            for (int i = 0; i < v + 1; i++)
            {
                vs.Add(start);
                start += segV;
            }
            /////////////////////////////////////////////////////////////////////
            DataTree<Point3d> pts1 = new DataTree<Point3d>();////等分部分网格
            int num = 0;
            for (int i = 0; i < us.Count; i++)
            {
                for (int j = 0; j < vs.Count; j++)
                {
                    pts1.Add(x.PointAt(us[i], vs[j]), new GH_Path(0, DA.Iteration, num));
                }
                num++;
            }
            //////////////////////////////////////////////////////////////////////
            DataTree<Polyline> ply1 = new DataTree<Polyline>();////等分部分网格
            num = 0;
            for (int i = 0; pts1.BranchCount - i >= 2; i += 2)
            {
                List<Point3d> group1 = pts1.Branch(i);
                List<Point3d> group2 = pts1.Branch(i + 1);
                ply1.AddRange(getPly1(group1, group2), new GH_Path(0, DA.Iteration, num));
                num++;
            }
            ////////////////////////////////////////////////////////////////////////不等分部分网格，重新设置点组
            double half = segV / 2;
            List<double> vs2 = new List<double>();
            start = 0;
            vs2.Add(start);
            for (int i = 1; i < v + 2; i++)
            {
                if (i == 1)////首项
                {
                    start += half;
                    vs2.Add(start);
                    continue;
                }
                else if (i == v + 1)/////末项
                {
                    start += half;
                    vs2.Add(start);
                    continue;
                }
                else
                {
                    start += segV;
                    vs2.Add(start);
                }
            }
            //////////////////////////////////////////////////////////////////////////////////
            DataTree<Point3d> pts2 = new DataTree<Point3d>();////不等分部分网格点
            num = 0;
            for (int i = 1; i < us.Count; i++)
            {
                for (int j = 0; j < vs2.Count; j++)
                {
                    pts2.Add(x.PointAt(us[i], vs2[j]), new GH_Path(0, DA.Iteration, num));
                }
                num++;
            }
            //////////////////////////////////////////////////////////////////////////////////
            DataTree<Polyline> ply2 = new DataTree<Polyline>();////不等分部分网格
            num = 0;
            if (x.IsClosed(1))//////如果v方向闭合
            {
                for (int i = 0; pts2.BranchCount - i >= 2; i += 2)
                {
                    List<Point3d> group1 = pts2.Branch(i);
                    List<Point3d> group2 = pts2.Branch(i + 1);
                    group1.RemoveAt(0);
                    group1.RemoveAt(group1.Count - 1);
                    group2.RemoveAt(0);
                    group2.RemoveAt(group2.Count - 1);
                    ply2.AddRange(getPly2(group1, group2), new GH_Path(0, DA.Iteration, num));
                    num++;
                }
            }
            else/////不闭合
            {
                for (int i = 0; pts2.BranchCount - i >= 2; i += 2)
                {
                    List<Point3d> group1 = pts2.Branch(i);
                    List<Point3d> group2 = pts2.Branch(i + 1);
                    ply2.AddRange(getPly1(group1, group2), new GH_Path(0, DA.Iteration, num));
                    num++;
                }
            }
            ///////////////////////////////////////////////////////////////////////////////////////
            DataTree<Polyline> lastPly = new DataTree<Polyline>();//////整合后的网格
            num = 0;
            int index = 0;
            while (true)
            {
                if (ply1.BranchCount <= num && ply2.BranchCount <= num)///ply1和ply2遍历完
                {
                    break;
                }
                if (ply1.BranchCount > num)///ply1没有遍历完
                {
                    lastPly.AddRange(ply1.Branch(num), new GH_Path(0, DA.Iteration, index));
                    index++;
                }
                if (ply2.BranchCount > num)///ply1没有遍历完
                {
                    lastPly.AddRange(ply2.Branch(num), new GH_Path(0, DA.Iteration, index));
                    index++;
                }
                num++;
            }
            DA.SetDataTree(0, lastPly);
        }
        public List<Polyline> getPly1(List<Point3d> x, List<Point3d> y) ///////不闭合两组点的网格
        {
            List<Polyline> ply = new List<Polyline>();
            List<Point3d> pts = new List<Point3d>();
            for (int i = 0; i < x.Count - 1; i++)
            {
                pts.Add(x[i]);
                pts.Add(x[i + 1]);
                pts.Add(y[i + 1]);
                pts.Add(y[i]);
                pts.Add(x[i]);
                Polyline pl = new Polyline(pts);
                ply.Add(pl);
                pts.Clear();
            }
            return ply;
        }
        public List<Polyline> getPly2(List<Point3d> x, List<Point3d> y) ///////闭合两组点的网格
        {
            x.Add(x[0]);
            y.Add(y[0]);
            List<Polyline> ply = new List<Polyline>();
            List<Point3d> pts = new List<Point3d>();
            for (int i = 0; i < x.Count - 1; i++)
            {
                pts.Add(x[i]);
                pts.Add(x[i + 1]);
                pts.Add(y[i + 1]);
                pts.Add(y[i]);
                pts.Add(x[i]);
                Polyline pl = new Polyline(pts);
                ply.Add(pl);
                pts.Clear();
            }
            return ply;
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
                return Resource1.surface_网格表皮F;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{aad90339-7d1d-49f9-b536-e347642ca558}"); }
        }
    }
}