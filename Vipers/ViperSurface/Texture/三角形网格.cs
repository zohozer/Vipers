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

namespace SuperVipers///////TangChi 2015.10.29
{
    public class Triangle : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent12 class.
        /// </summary>
        public Triangle()
            : base("Triangular Panels", "Triangle",
                "Based on Lunchbox Triangular Panels",
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
            Message = "Triangle Panels";
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
            pManager.AddCurveParameter("Triangles", "T", "Edge lines", GH_ParamAccess.tree);
            pManager.AddPointParameter("Points", "P", "Panel vertices", GH_ParamAccess.tree);
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
            //////////////////////////////////////////////
            Surface x = surface;
            int un = uCount;
            int vn = vCount;
            x.SetDomain(0, new Interval(0, 1));
            x.SetDomain(1, new Interval(0, 1));
            double uu = 1.0 / un;
            double vv = 1.0 / vn;
            double ustart = 0;
            double vstart = 0;
            List<double> us1 = new List<double>();
            List<double> us2 = new List<double>();
            List<double> vs = new List<double>();
            for (int i = 0; i < un + 1; i++)
            {
                us1.Add(ustart);
                ustart += uu;
            }
            ///////////////////////////////////////
            us2.Add(0);
            us2.Add(uu / 2);
            ustart = uu / 2;
            for (int i = 0; i < un - 1; i++)
            {
                ustart += uu;
                us2.Add(ustart);
            }
            us2.Add(1);
            ///////////////////////////////////////
            for (int i = 0; i < vn + 1; i++)
            {
                vs.Add(vstart);
                vstart += vv;
            }
            //////////////////////////////////////
            DataTree<Point3d> pts = new DataTree<Point3d>();
            int index = 0;
            for (int i = 0; i < vn + 1; i++)
            {
                if ((i + 1) / 2.0 == Convert.ToInt32((i + 1) / 2.0))//奇数
                {
                    for (int j = 0; j < us2.Count; j++)
                    {
                        Point3d pt = x.PointAt(us2[j], vs[i]);
                        pts.Add(pt, new GH_Path(0, DA.Iteration, index));
                    }
                }
                else//偶数
                {
                    for (int j = 0; j < us1.Count; j++)
                    {
                        Point3d pt = x.PointAt(us1[j], vs[i]);
                        pts.Add(pt, new GH_Path(0, DA.Iteration, index));
                    }
                }
                index++;
            }
            //////////////////////////////////////////////////////////////
            DataTree<Polyline> pls = new DataTree<Polyline>();
            index = 0;
            for (int i = 0; i < pts.BranchCount - 1; i++)
            {
                if ((i + 1) / 2.0 == Convert.ToInt32((i + 1) / 2.0))//奇数
                {
                    List<Polyline> pln = PL2(pts.Branch(i), pts.Branch(i + 1));
                    pls.AddRange(pln, new GH_Path(0, DA.Iteration, index));
                }
                else
                {
                    List<Polyline> pln = PL1(pts.Branch(i), pts.Branch(i + 1));
                    pls.AddRange(pln, new GH_Path(0, DA.Iteration, index));
                }
                index++;
            }
            ////////////////////////////////////////////////////////////////如果u方向闭合
            DataTree<Point3d> pts2 = new DataTree<Point3d>();
            index = 0;
            if (x.IsClosed(0))
            {
                for (int i = 0; i < pts.BranchCount; i++)
                {
                    Point3d[] ptsnew = Point3d.CullDuplicates(pts.Branch(i), 0.000001);
                    List<Point3d> ptend = ptsnew.ToList();
                    if ((i + 1) / 2.0 == Convert.ToInt32((i + 1) / 2.0))//奇数
                    {
                        ptend.RemoveAt(0);
                    }
                    pts2.AddRange(ptend, new GH_Path(0, DA.Iteration, index));
                    index++;
                }
                pls.Clear();
                index = 0;
                for (int i = 0; i < pts2.BranchCount - 1; i++)
                {
                    if ((i + 1) / 2.0 == Convert.ToInt32((i + 1) / 2.0))//奇数
                    {
                        List<Polyline> plyss = PL4(pts2.Branch(i), pts2.Branch(i + 1));
                        pls.AddRange(plyss, new GH_Path(0, DA.Iteration, index));
                    }
                    else
                    {
                        List<Polyline> plyss = PL3(pts2.Branch(i), pts2.Branch(i + 1));
                        pls.AddRange(plyss, new GH_Path(0, DA.Iteration, index));
                    }
                    index++;
                }
                DA.SetDataTree(1, pts2);
            }
            else 
            {
                DA.SetDataTree(1, pts);
            }
            DA.SetDataTree(0, pls);
        }
        public static List<Polyline> PL1(List<Point3d> x, List<Point3d> y) ///第一组点数大于第二组
        {
            List<Polyline> pls = new List<Polyline>();////x+1=y;
            for (int i = 0; i < x.Count; i++)
            {
                List<Point3d> pts = new List<Point3d>();
                Point3d p1 = x[i];
                Point3d p2 = y[i];
                Point3d p3 = y[i + 1];
                pts.Add(p1);

                pts.Add(p2);
                pts.Add(p3);
                pts.Add(p1);
                Polyline pl = new Polyline(pts);
                pls.Add(pl);
                if (i == x.Count - 1)
                {
                    break;
                }
                p2 = y[i + 1];
                p3 = x[i + 1];
                pts.Clear();
                pts.Add(p1);
                pts.Add(p2);
                pts.Add(p3);
                pts.Add(p1);
                Polyline pl2 = new Polyline(pts);
                pls.Add(pl2);
            }
            return pls;
        }
        /////////////////////////////////////////////////////////////////////
        public static List<Polyline> PL2(List<Point3d> x, List<Point3d> y) ///第二组点数大于第一组
        {
            List<Polyline> pls = new List<Polyline>();////x=y+1;
            for (int i = 0; i < y.Count; i++)
            {
                List<Point3d> pts = new List<Point3d>();
                Point3d p1 = y[i];
                Point3d p2 = x[i + 1];
                Point3d p3 = x[i];
                pts.Add(p1);
                pts.Add(p2);
                pts.Add(p3);
                pts.Add(p1);
                Polyline pl = new Polyline(pts);
                pls.Add(pl);
                if (i == y.Count - 1)
                {
                    break;
                }
                p2 = y[i + 1];
                p3 = x[i + 1];
                pts.Clear();
                pts.Add(p1);
                pts.Add(p2);
                pts.Add(p3);
                pts.Add(p1);
                Polyline pl2 = new Polyline(pts);
                pls.Add(pl2);
            }
            return pls;
        }
        /////////////////////////////////////////////////////////////////////
        public static List<Polyline> PL3(List<Point3d> x, List<Point3d> y)
        {
            List<Polyline> pls = new List<Polyline>();
            for (int i = 0; i < x.Count; i++)
            {
                List<Point3d> pts = new List<Point3d>();
                Polyline pl = new Polyline();
                if (i == x.Count - 1)
                {
                    pts.Add(x[i]);
                    pts.Add(y[i - 1]);
                    pts.Add(y[i]);
                    pts.Add(x[i]);
                    pl = new Polyline(pts);
                    pls.Add(pl);
                    pts.Clear();
                    pts.Add(x[i]);
                    pts.Add(y[i]);
                    pts.Add(x[0]);
                    pts.Add(x[i]);
                    pl = new Polyline(pts);
                    pls.Add(pl);
                    break;
                }
                if (i == 0)
                {
                    pts.Add(x[i]);
                    pts.Add(y[y.Count - 1]);
                    pts.Add(y[i]);
                    pts.Add(x[i]);
                    pl = new Polyline(pts);
                    pls.Add(pl);
                    pts.Clear();
                    pts.Add(x[i]);
                    pts.Add(y[i]);
                    pts.Add(x[i + 1]);
                    pts.Add(x[i]);
                    pl = new Polyline(pts);
                    pls.Add(pl);
                }
                else
                {
                    pts.Add(x[i]);
                    pts.Add(y[i - 1]);
                    pts.Add(y[i]);
                    pts.Add(x[i]);
                    pl = new Polyline(pts);
                    pls.Add(pl);
                    pts.Clear();
                    pts.Add(x[i]);
                    pts.Add(y[i]);
                    pts.Add(x[i + 1]);
                    pts.Add(x[i]);
                    pl = new Polyline(pts);
                    pls.Add(pl);
                }
            }
            return pls;
        }
        ///////////////////////////////////////////////
        public static List<Polyline> PL4(List<Point3d> x, List<Point3d> y)
        {
            List<Polyline> pls = new List<Polyline>();
            for (int i = 0; i < x.Count; i++)
            {
                List<Point3d> pts = new List<Point3d>();
                Polyline pl = new Polyline();
                if (i == x.Count - 1)
                {
                    pts.Add(x[i]);
                    pts.Add(y[i]);
                    pts.Add(y[0]);
                    pts.Add(x[i]);
                    pl = new Polyline(pts);
                    pls.Add(pl);
                    pts.Clear();
                    pts.Add(x[i]);
                    pts.Add(y[0]);
                    pts.Add(x[0]);
                    pts.Add(x[i]);
                    pl = new Polyline(pts);
                    pls.Add(pl);
                    break;
                }
                pts.Add(x[i]);
                pts.Add(y[i]);
                pts.Add(y[i + 1]);
                pts.Add(x[i]);
                pl = new Polyline(pts);
                pls.Add(pl);
                pts.Clear();
                pts.Add(x[i]);
                pts.Add(y[i + 1]);
                pts.Add(x[i + 1]);
                pts.Add(x[i]);
                pl = new Polyline(pts);
                pls.Add(pl);
            }
            return pls;
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
                return Resource1.surface_网格表皮C;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{b41bd253-c090-4b72-8946-e65e1252011c}"); }
        }
    }
}