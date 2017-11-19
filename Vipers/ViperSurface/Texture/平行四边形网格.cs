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

namespace SuperVipers//////TangChi 2015.12.25
{
    public class Parallelograms : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Parallelograms class.
        /// </summary>
        public Parallelograms()
            : base("平行四边形网格", "Parallelograms",
                "基于Lunchbox的平行四边形网格，修复了闭合曲面接缝bug",
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
            
            pManager.AddSurfaceParameter("曲面", "S", "待划分的曲面", GH_ParamAccess.item);
            pManager.AddIntegerParameter("u方向数量", "U", "u方向的网格数量", GH_ParamAccess.item, 50);
            pManager.AddIntegerParameter("v方向数量", "V", "v方向的网格数量", GH_ParamAccess.item, 50);
            pManager.HideParameter(0);
            Message = "TC-1-04\n平行四边形网格";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("平行四边形网格", "C", "生成的平行四边形网格", GH_ParamAccess.tree);
            pManager.AddPointParameter("网格点", "P", "平行四边形网格对应的点", GH_ParamAccess.tree);
            pManager.HideParameter(1);
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
            surface.SetDomain(0, new Interval(0, 1));
            surface.SetDomain(1, new Interval(0, 1));
            DataTree<Polyline> plys = new DataTree<Polyline>();
            int num = 0;
            List<double> U = new List<double>();
            List<double> V = new List<double>();
            double ustart = 0;
            double vstart = 0;
            for (int i = 0; i < uCount + 1; i++)
            {
                U.Add(ustart);
                ustart += 1.0 / uCount;
            }
            for (int i = 0; i < vCount + 1; i++)
            {
                V.Add(vstart);
                vstart += 1.0 / vCount;
            }
            ///////////////////////////////////////////
            DataTree<Point3d> pts = new DataTree<Point3d>();
            for (int i = 0; i < U.Count; i++)
            {
                for (int j = 0; j < V.Count; j++)
                {
                    pts.Add(surface.PointAt(U[i], V[j]), new GH_Path(0, DA.Iteration, num));
                }
                num++;
            }
            num = 0;
            if (surface.IsClosed(1))
            {
                for (int i = 0; i < pts.BranchCount - 1; i++)
                {
                    plys.AddRange(PLS2(pts.Branch(i), pts.Branch(i + 1)), new GH_Path(0, DA.Iteration, num));
                    num++;
                }
            }
            else
            {
                for (int i = 0; i < pts.BranchCount - 1; i++)
                {
                    plys.AddRange(PLS(pts.Branch(i), pts.Branch(i + 1)), new GH_Path(0, DA.Iteration, num));
                    num++;
                }
            }
            DA.SetDataTree(0, plys);
            DA.SetDataTree(1, pts);
        }
        public List<Polyline> PLS(List<Point3d> x, List<Point3d> y) /////不封闭曲面的做法
        {
            List<Polyline> plys = new List<Polyline>();
            List<Point3d> pts = new List<Point3d>();
            pts.Add(x[0]);
            pts.Add(y[0]);
            pts.Add(y[1]);
            pts.Add(x[0]);
            plys.Add(new Polyline(pts));
            pts.Clear();
            for (int i = 0; i < x.Count - 2; i++)
            {
                pts.Add(x[i]);
                pts.Add(y[i + 1]);
                pts.Add(y[i + 2]);
                pts.Add(x[i + 1]);
                pts.Add(x[i]);
                plys.Add(new Polyline(pts));
                pts.Clear();
            }
            pts.Add(x[x.Count - 2]);
            pts.Add(y[x.Count - 1]);
            pts.Add(x[x.Count - 1]);
            pts.Add(x[x.Count - 2]);
            plys.Add(new Polyline(pts));
            return plys;
        }
        /////////////////////////////////////////////////////////
        public List<Polyline> PLS2(List<Point3d> x, List<Point3d> y) /////封闭曲面的做法(V)
        {
            List<Polyline> plys = new List<Polyline>();
            List<Point3d> pts = new List<Point3d>();
            pts.Add(x[x.Count - 2]);
            pts.Add(y[x.Count - 1]);
            pts.Add(y[1]);
            pts.Add(x[0]);
            pts.Add(x[x.Count - 2]);
            plys.Add(new Polyline(pts));
            pts.Clear();
            for (int i = 0; i < x.Count - 2; i++)
            {
                pts.Add(x[i]);
                pts.Add(y[i + 1]);
                pts.Add(y[i + 2]);
                pts.Add(x[i + 1]);
                pts.Add(x[i]);
                plys.Add(new Polyline(pts));
                pts.Clear();
            }
            return plys;
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
                return Resource1.surface_网格表皮D;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{3babc814-aaa9-4d5d-ac6b-000f7d28480b}"); }
        }
    }
}