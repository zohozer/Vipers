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

namespace SuperVipers///////TangChi 2015.11.2
{
    public class Hexagons : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent61 class.
        /// </summary>
        public Hexagons()
            : base("Hexagonal Panels", "Hexagons",
                "Based on Lunchbox Hexagon Cells",
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
            Message = "Hexagonal Panels";
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
            pManager.AddCurveParameter("Hexagons", "H", "Edge lines", GH_ParamAccess.tree);
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
            ///////////////////////////////////////
            Surface x = surface;
            int Un = uCount;
            int Vn = vCount;
            /////////////////////////////////////////////////////////////////
            x.SetDomain(0, new Interval(0, 1));
            x.SetDomain(1, new Interval(0, 1));
            double u = 1.0 / (3 * Un);
            double v = 1.0 / (2 * Vn);
            double ustart = u;
            double vstart = 0;
            List<double> us1 = new List<double>();//u方向的第一种列表类型
            List<double> us2 = new List<double>();//u方向的第二种列表类型
            List<double> vs = new List<double>();/// v方向列表类型
            #region/////////点坐标设置
            us1.Add(0);
            us1.Add(u);
            for (int i = 0; i < 2 * Un - 1; i++)
            {
                if ((i + 1) / 2.0 == Convert.ToInt32((i + 1) / 2.0))
                {
                    ustart += 2 * u;
                }
                else
                {
                    ustart += u;
                }
                us1.Add(ustart);
            }
            us1.Add(1);
            ///////////////////
            ustart = 0.5 * u;
            us2.Add(0);
            us2.Add(u * 0.5);
            for (int i = 0; i < 2 * Un - 1; i++)
            {
                if ((i + 1) / 2.0 == Convert.ToInt32((i + 1) / 2.0))
                {
                    ustart += u;
                }
                else
                {
                    ustart += 2 * u;
                }
                us2.Add(ustart);
            }
            us2.Add(1);
            ///////////////////////////////////////////////////////////////////////////////////
            for (int i = 0; i < 2 * Vn + 1; i++)
            {

                vs.Add(vstart);
                vstart += v;
            }
            #endregion;
            DataTree<Point3d> pts = new DataTree<Point3d>();
            int index = 0;
            for (int i = 0; i < 2 * Vn + 1; i++)
            {
                for (int j = 0; j < 2 + 2 * Un; j++)
                {
                    if ((i + 1) / 2.0 == Convert.ToInt32((i + 1) / 2.0))
                    {
                        Point3d pp = x.PointAt(us2[j], vs[i]);
                        pts.Add(pp, new GH_Path(0, DA.Iteration, index));
                    }
                    else
                    {
                        Point3d pp = x.PointAt(us1[j], vs[i]);
                        pts.Add(pp, new GH_Path(0, DA.Iteration, index));
                    }
                }
                index++;
            }
            //////////////////////////////////////////////////////////////////////////////
            DataTree<Point3d> ptsnew = new DataTree<Point3d>();
            int index2 = 0;
            for (int i = 0; i < 2 * Un + 2; i++)
            {
                for (int j = 0; j < pts.BranchCount; j++)
                {
                    ptsnew.Add(pts.Branch(j)[i], new GH_Path(0, DA.Iteration, index2));
                }
                index2++;
            }
            ///////////////////////////////////////////////////////////////////////////////filp列表
            DataTree<Polyline> polys = new DataTree<Polyline>();
            int index3 = 0;
            for (int i = 0; i < ptsnew.BranchCount - 1; i++)
            {
                if ((i + 1) / 2.0 == Convert.ToInt32((i + 1) / 2.0))
                {
                    List<Polyline> ply = hexagon2(ptsnew.Branch(i), ptsnew.Branch(i + 1));
                    polys.AddRange(ply, new GH_Path(0, DA.Iteration, index3));
                }
                else
                {
                    List<Polyline> ply = hexagon1(ptsnew.Branch(i), ptsnew.Branch(i + 1));
                    polys.AddRange(ply, new GH_Path(0, DA.Iteration, index3));
                }

                index3++;
            }
            if (x.IsClosed(1))////v方向闭合
            {
                polys.Clear();
                index3 = 0;
                for (int i = 0; i < ptsnew.BranchCount - 1; i++)
                {
                    if ((i + 1) / 2.0 == Convert.ToInt32((i + 1) / 2.0))
                    {
                        List<Polyline> ply = hexagon2(ptsnew.Branch(i), ptsnew.Branch(i + 1));
                        polys.AddRange(ply, new GH_Path(0, DA.Iteration, index3));
                    }
                    else
                    {
                        List<Polyline> ply = hexagon3(ptsnew.Branch(i), ptsnew.Branch(i + 1));
                        polys.AddRange(ply, new GH_Path(0, DA.Iteration, index3));
                    }
                    index3++;
                }

            }
            //////////////////////////////////////////////////////////////////////////////////////////////////
            DataTree<Point3d> ptsnew2 = new DataTree<Point3d>();
            int index4 = 1;
            if (x.IsClosed(0))////u方向闭合
            {
                polys.Clear();
                ptsnew2.AddRange(ptsnew.Branch(ptsnew.BranchCount - 2), new GH_Path(0, DA.Iteration, 0));
                for (int i = 1; i < ptsnew.BranchCount - 1; i++)
                {
                    ptsnew2.AddRange(ptsnew.Branch(i), new GH_Path(0, DA.Iteration, index4));
                    index4++;
                }
                index4 = 0;
                for (int i = 0; i < ptsnew2.BranchCount - 1; i++)
                {
                    if ((i + 1) / 2.0 == Convert.ToInt32((i + 1) / 2.0))
                    {
                        List<Polyline> ply = hexagon2(ptsnew2.Branch(i), ptsnew2.Branch(i + 1));
                        polys.AddRange(ply, new GH_Path(0, DA.Iteration, index4));
                    }
                    else
                    {
                        List<Polyline> ply = hexagon1(ptsnew2.Branch(i), ptsnew2.Branch(i + 1));
                        polys.AddRange(ply, new GH_Path(0, DA.Iteration, index4));
                    }
                    index4++;
                }
                ptsnew = ptsnew2;
            }

            DA.SetDataTree(0, polys);
            DA.SetDataTree(1, ptsnew);
        }



        public static List<Polyline> hexagon1(List<Point3d> x, List<Point3d> y) ////////第一种六边形组合方式
        {
            List<Polyline> pls = new List<Polyline>();
            List<Point3d> pts = new List<Point3d>();
            pts.Add(x[0]);
            pts.Add(x[1]);
            pts.Add(y[1]);
            pts.Add(y[0]);
            pts.Add(x[0]);
            pls.Add(new Polyline(pts));
            pts.Clear();
            for (int i = 1; i < x.Count - 2; i += 2)
            {
                pts.Add(x[i]);
                pts.Add(x[i + 1]);
                pts.Add(x[i + 2]);
                pts.Add(y[i + 2]);
                pts.Add(y[i + 1]);
                pts.Add(y[i]);
                pts.Add(x[i]);
                pls.Add(new Polyline(pts));
                pts.Clear();
            }
            pts.Clear();
            pts.Add(x[x.Count - 2]);
            pts.Add(x[x.Count - 1]);
            pts.Add(y[y.Count - 1]);
            pts.Add(y[y.Count - 2]);
            pts.Add(x[x.Count - 2]);
            pls.Add(new Polyline(pts));
            return pls;
        }
        public static List<Polyline> hexagon2(List<Point3d> x, List<Point3d> y) ////////第二种六边形组合方式
        {
            List<Polyline> pls = new List<Polyline>();
            List<Point3d> pts = new List<Point3d>();
            for (int i = 0; i < x.Count - 2; i += 2)
            {
                pts.Add(x[i]);
                pts.Add(x[i + 1]);
                pts.Add(x[i + 2]);
                pts.Add(y[i + 2]);
                pts.Add(y[i + 1]);
                pts.Add(y[i]);
                pts.Add(x[i]);
                pls.Add(new Polyline(pts));
                pts.Clear();
            }
            return pls;
        }
        public static List<Polyline> hexagon3(List<Point3d> x, List<Point3d> y) ////////第三种六边形组合方式(v方向闭合)
        {
            List<Polyline> pls = new List<Polyline>();
            List<Point3d> pts = new List<Point3d>();
            for (int i = 1; i < x.Count - 2; i += 2)
            {
                pts.Add(x[i]);
                pts.Add(x[i + 1]);
                pts.Add(x[i + 2]);
                pts.Add(y[i + 2]);
                pts.Add(y[i + 1]);
                pts.Add(y[i]);
                pts.Add(x[i]);
                pls.Add(new Polyline(pts));
                pts.Clear();
            }
            pts.Clear();
            pts.Add(x[x.Count - 2]);
            pts.Add(x[x.Count - 1]);
            pts.Add(x[1]);
            pts.Add(y[1]);
            pts.Add(y[y.Count - 1]);
            pts.Add(y[y.Count - 2]);
            pts.Add(x[x.Count - 2]);
            pls.Add(new Polyline(pts));
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
                return Resource1.surface_网格表皮A;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{7f6d5796-5fe4-4e09-2f7e-d5d8a8a41faf}"); }
        }
    }
}