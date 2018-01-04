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
    public class 混合网格 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the 混合网格 class.
        /// </summary>
        public 混合网格()
            : base("Complex Panels", "Complex",
                "A hybrid mesh composed of octagons, rhombuses and triangles",
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
            pManager.AddSurfaceParameter("Surface", "S", "Surface to divide", GH_ParamAccess.item);
            pManager.AddIntegerParameter("U Count", "U", "Number of divisions in U direction", GH_ParamAccess.item, 10);
            pManager.AddIntegerParameter("V Count", "V", "Number of divisions in V direction", GH_ParamAccess.item, 10);
            pManager.HideParameter(0);
            Message = "Complex Panels";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Octagons", "O", "Edge lines", GH_ParamAccess.tree);
            pManager.AddCurveParameter("Rhombuses", "R", "Edge lines", GH_ParamAccess.tree);
            pManager.AddCurveParameter("Triangles", "T", "Edge lines", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Surface s = null;
            int u= 10;
            int v = 10;
            if (!DA.GetData(0, ref s)) return;
            if (!DA.GetData(1, ref u)) return;
            if (!DA.GetData(2, ref v)) return;
            /////////////////////////////////
            s.SetDomain(0, new Interval(0, 1));
            s.SetDomain(1, new Interval(0, 1));
            double segU = 1.0 / ((1 + Math.Sqrt(2.0)) * u);
            double segV = 1.0 / ((1 + Math.Sqrt(2.0)) * v);
            List<Point3d> pts = new List<Point3d>();
            List<double> uuu = new List<double>();
            List<double> vvv = new List<double>();
            uuu.Add(segU * Math.Sqrt(2.0) / 2);
            uuu.Add(segU * Math.Sqrt(2.0) / 2 + segU);
            uuu.Add(segU * (1 + Math.Sqrt(2.0)));
            uuu.Add(segU * (1 + Math.Sqrt(2.0)));
            uuu.Add(segU * Math.Sqrt(2.0) / 2 + segU);
            uuu.Add(segU * Math.Sqrt(2.0) / 2);
            uuu.Add(0);
            uuu.Add(0);
            uuu.Add(segU * Math.Sqrt(2.0) / 2);
            vvv.Add(0);
            vvv.Add(0);
            vvv.Add(segV * Math.Sqrt(2.0) / 2);
            vvv.Add(segV * Math.Sqrt(2.0) / 2 + segV);
            vvv.Add(segV * (Math.Sqrt(2.0) + 1));
            vvv.Add(segV * (Math.Sqrt(2.0) + 1));
            vvv.Add(segV * Math.Sqrt(2.0) / 2 + segV);
            vvv.Add(segV * Math.Sqrt(2.0) / 2);
            vvv.Add(0);
            /////////////////////////////////////////////////////////////////////////////
            for (int q = 0; q < 9; q++)
            {
                pts.Add(s.PointAt(uuu[q], vvv[q]));
            }
            DataTree<double> us = new DataTree<double>();
            DataTree<double> vs = new DataTree<double>();
            DataTree<Point3d> eight = new DataTree<Point3d>();
            DataTree<Polyline> last = new DataTree<Polyline>();
            int count = 0;
            for (int i = 0; i < u; i++)
            {
                eight.AddRange(pts, new GH_Path(0, DA.Iteration, count));
                last.Add(new Polyline(pts), new GH_Path(0, DA.Iteration, count));
                us.AddRange(uuu, new GH_Path(0, DA.Iteration, count));
                vs.AddRange(vvv, new GH_Path(0, DA.Iteration, count));
                for (int q = 0; q < pts.Count; q++)//////pts中每个点移动
                {
                    uuu[q] += segU * (1 + Math.Sqrt(2.0));
                    pts[q] = s.PointAt(uuu[q], vvv[q]);
                }
                count++;
            }
            ///////////////////////////////////////////////////////////////////////////////////
            count = 0;
            DataTree<Polyline> finalPly = new DataTree<Polyline>();
            DataTree<Point3d> finalPt = new DataTree<Point3d>();
            for (int q = 0; q < last.BranchCount; q++)
            {
                for (int k = 0; k < v; k++)
                {
                    finalPly.Add(new Polyline(eight.Branch(q)), new GH_Path(0, DA.Iteration, count));
                    for (int w = 0; w < 9; w++)
                    {
                        vs.Branch(q)[w] += segV * (1 + Math.Sqrt(2.0));
                        eight.Branch(q)[w] = s.PointAt(us.Branch(q)[w], vs.Branch(q)[w]);
                    }
                }
                count++;
            }
            #region//四边形
            List<double> uuu2 = new List<double>();
            List<double> vvv2 = new List<double>();
            List<Point3d> pts2 = new List<Point3d>();
            uuu2.Add(segU * (1 + Math.Sqrt(2.0)));
            uuu2.Add(segU * (1 + Math.Sqrt(2.0) * 1.5));
            uuu2.Add(segU * (1 + Math.Sqrt(2.0)));
            uuu2.Add(segU * (1 + Math.Sqrt(2.0) * 0.5));
            uuu2.Add(segU * (1 + Math.Sqrt(2.0)));
            vvv2.Add(segV * (Math.Sqrt(2.0) * 0.5 + 1));
            vvv2.Add(segV * (Math.Sqrt(2.0) + 1));
            vvv2.Add(segV * (Math.Sqrt(2.0) * 1.5 + 1));
            vvv2.Add(segV * (Math.Sqrt(2.0) + 1));
            vvv2.Add(segV * (Math.Sqrt(2.0) * 0.5 + 1));
            /////////////////////////////////////////////////////////////////////////////
            for (int q = 0; q < 5; q++)
            {
                pts2.Add(s.PointAt(uuu2[q], vvv2[q]));
            }
            DataTree<double> us2 = new DataTree<double>();
            DataTree<double> vs2 = new DataTree<double>();
            DataTree<Point3d> eight2 = new DataTree<Point3d>();
            DataTree<Polyline> last2 = new DataTree<Polyline>();
            count = 0;
            for (int i = 0; i < u - 1; i++)
            {
                eight2.AddRange(pts2, new GH_Path(0, DA.Iteration, count));
                last2.Add(new Polyline(pts2), new GH_Path(0, DA.Iteration, count));
                us2.AddRange(uuu2, new GH_Path(0, DA.Iteration, count));
                vs2.AddRange(vvv2, new GH_Path(0, DA.Iteration, count));
                for (int q = 0; q < pts2.Count; q++)//////pts中每个点移动
                {
                    uuu2[q] += segU * (1 + Math.Sqrt(2.0));
                    pts2[q] = s.PointAt(uuu2[q], vvv2[q]);
                }
                count++;
            }
            ///////////////////////////////////////////////////////////////////////////////////
            count = 0;
            DataTree<Polyline> finalPly2 = new DataTree<Polyline>();
            DataTree<Point3d> finalPt2 = new DataTree<Point3d>();
            for (int q = 0; q < last2.BranchCount; q++)
            {
                for (int k = 0; k < v - 1; k++)
                {
                    finalPly2.Add(new Polyline(eight2.Branch(q)), new GH_Path(0, DA.Iteration, count));
                    for (int w = 0; w < 5; w++)
                    {
                        vs2.Branch(q)[w] += segV * (1 + Math.Sqrt(2.0));
                        eight2.Branch(q)[w] = s.PointAt(us2.Branch(q)[w], vs2.Branch(q)[w]);
                    }
                }
                count++;
            }
            #endregion

            #region////三角形网格
            List<Polyline> plys1 = new List<Polyline>();
            List<Polyline> plys2 = new List<Polyline>();
            List<Polyline> plys3 = new List<Polyline>();
            List<Polyline> plys4 = new List<Polyline>();////边缘四组三角形
            List<Polyline> plys5 = new List<Polyline>();////V方向闭合的情况下，网格组成
            List<Polyline> plys6 = new List<Polyline>();////U方向闭合的情况下，网格组成
            double su1 = 0;
            double su2 = Math.Sqrt(2.0) * segU / 2;
            double su3 = 0;
            double sv1 = 0;
            double sv2 = 0;
            double sv3 = Math.Sqrt(2.0) * segV / 2;
            double su4 = 1 - Math.Sqrt(2.0) * segU / 2;
            double su5 = 1;
            double su6 = 1;
            double sv4 = 0;
            double sv5 = 0;
            double sv6 = Math.Sqrt(2.0) * segV / 2;
            for (int i = 0; i < v + 1; i++)
            {
                List<Point3d> collect = new List<Point3d>();
                if (i == 0)////起始角三角形
                {
                    collect.Add(s.PointAt(su1, sv1));
                    collect.Add(s.PointAt(su2, sv2));
                    collect.Add(s.PointAt(su3, sv3));
                    collect.Add(s.PointAt(su1, sv1));
                    plys1.Add(new Polyline(collect));
                    collect.Clear();
                    collect.Add(s.PointAt(su4, sv4));
                    collect.Add(s.PointAt(su5, sv5));
                    collect.Add(s.PointAt(su6, sv6));
                    collect.Add(s.PointAt(su4, sv4));
                    plys2.Add(new Polyline(collect));
                    if (s.IsClosed(0))////////////////////////////////////////u方向闭合的情况
                    {
                        collect.Clear();
                        collect.Add(s.PointAt(su4, sv4));
                        collect.Add(s.PointAt(su2, sv2));
                        collect.Add(s.PointAt(su3, sv3));
                        collect.Add(s.PointAt(su4, sv4));
                        plys3.Add(new Polyline(collect));////闭合时将其加入
                    }
                    sv1 += segV * (1 + Math.Sqrt(2.0) * 0.5);
                    sv2 += segV * (1 + Math.Sqrt(2.0));
                    sv3 += segV * (1 + Math.Sqrt(2.0));
                    sv4 += segV * (1 + Math.Sqrt(2.0));
                    sv5 += segV * (1 + Math.Sqrt(2.0) * 0.5);
                    sv6 += segV * (1 + Math.Sqrt(2.0));
                    continue;
                }
                else if (i == v)
                {
                    collect.Add(s.PointAt(su1, sv1));
                    collect.Add(s.PointAt(su2, sv2));
                    collect.Add(s.PointAt(su3, 1));
                    collect.Add(s.PointAt(su1, sv1));
                    plys1.Add(new Polyline(collect));
                    collect.Clear();
                    collect.Add(s.PointAt(su4, sv4));
                    collect.Add(s.PointAt(su5, sv5));
                    collect.Add(s.PointAt(su6, 1));
                    collect.Add(s.PointAt(su4, sv4));
                    plys2.Add(new Polyline(collect));
                    if (s.IsClosed(0))////////////////////////////////////////u方向闭合的情况
                    {
                        collect.Clear();
                        collect.Add(s.PointAt(su4, sv4));
                        collect.Add(s.PointAt(su5, sv5));
                        collect.Add(s.PointAt(su2, sv2));
                        collect.Add(s.PointAt(su4, sv4));
                        plys4.Add(new Polyline(collect));////闭合时将其加入
                    }
                    sv4 += segV * (1 + Math.Sqrt(2.0) * 0.5);
                    sv5 += segV * (1 + Math.Sqrt(2.0));
                    sv6 += segV * (1 + Math.Sqrt(2.0));
                    continue;
                }
                collect.Add(s.PointAt(su1, sv1));
                collect.Add(s.PointAt(su2, sv2));
                collect.Add(s.PointAt(su3, sv3));
                collect.Add(s.PointAt(su1, sv1));
                plys1.Add(new Polyline(collect));
                collect.Clear();
                collect.Add(s.PointAt(su4, sv4));
                collect.Add(s.PointAt(su5, sv5));
                collect.Add(s.PointAt(su6, sv6));
                collect.Add(s.PointAt(su4, sv4));
                plys2.Add(new Polyline(collect));
                //////////////////////////////////u方向闭合的情况
                collect.Clear();
                collect.Add(s.PointAt(su4, sv4));
                collect.Add(s.PointAt(su5, sv5));
                collect.Add(s.PointAt(su2, sv2));
                collect.Add(s.PointAt(su3, sv3));
                collect.Add(s.PointAt(su4, sv4));
                plys6.Add(new Polyline(collect));
                sv1 += segV * (1 + Math.Sqrt(2.0));
                sv2 += segV * (1 + Math.Sqrt(2.0));
                sv3 += segV * (1 + Math.Sqrt(2.0));
                sv4 += segV * (1 + Math.Sqrt(2.0));
                sv5 += segV * (1 + Math.Sqrt(2.0));
                sv6 += segV * (1 + Math.Sqrt(2.0));
            }

            ///////////////////////////////////////////////////////
            //////////////////////第二部分
            ///////////////////////////////////////////////////////
            double SU1 = segU * (Math.Sqrt(2.0) / 2 + 1);
            double SU2 = segU * (Math.Sqrt(2.0) * 1.5 + 1);
            double SU3 = segU * (Math.Sqrt(2.0) + 1);
            double SV1 = 0;
            double SV2 = 0;
            double SV3 = Math.Sqrt(2.0) * segV / 2;
            double SU4 = segU * (Math.Sqrt(2.0) / 2 + 1);
            double SU5 = segU * (Math.Sqrt(2.0) + 1);
            double SU6 = segU * (Math.Sqrt(2.0) * 1.5 + 1);
            double SV4 = 1;
            double SV5 = 1 - Math.Sqrt(2.0) * segV / 2;
            double SV6 = 1;
            for (int i = 0; i < u - 1; i++)
            {
                List<Point3d> collect = new List<Point3d>();
                collect.Add(s.PointAt(SU1, SV1));
                collect.Add(s.PointAt(SU2, SV2));
                collect.Add(s.PointAt(SU3, SV3));
                collect.Add(s.PointAt(SU1, SV1));
                plys3.Add(new Polyline(collect));
                collect.Clear();
                collect.Add(s.PointAt(SU4, SV4));
                collect.Add(s.PointAt(SU5, SV5));
                collect.Add(s.PointAt(SU6, SV6));
                collect.Add(s.PointAt(SU4, SV4));
                plys4.Add(new Polyline(collect));
                ////////////////V方向闭合的情况
                collect.Clear();
                collect.Add(s.PointAt(SU5, SV5));
                collect.Add(s.PointAt(SU2, SV2));
                collect.Add(s.PointAt(SU3, SV3));
                collect.Add(s.PointAt(SU1, SV1));
                collect.Add(s.PointAt(SU5, SV5));
                plys5.Add(new Polyline(collect));
                ////////////////////////////////////////////
                SU1 += segU * (1 + Math.Sqrt(2.0));
                SU2 += segU * (1 + Math.Sqrt(2.0));
                SU3 += segU * (1 + Math.Sqrt(2.0));
                SU4 += segU * (1 + Math.Sqrt(2.0));
                SU5 += segU * (1 + Math.Sqrt(2.0));
                SU6 += segU * (1 + Math.Sqrt(2.0));

            }
            //////////////////////////////////////////////////////////////
            ////////////////////////////////////闭合情况下
            /////////////////////////////////////////////////////////////
            if (s.IsClosed(1))
            {
                List<Point3d> change = new List<Point3d>();
                change.Add(s.PointAt(0, 1 - Math.Sqrt(2.0) * segV / 2));
                change.Add(s.PointAt(Math.Sqrt(2.0) * segU / 2, 0));
                change.Add(s.PointAt(0, Math.Sqrt(2.0) * segV / 2));
                change.Add(s.PointAt(0, 1 - Math.Sqrt(2.0) * segV / 2));
                plys1[0] = new Polyline(change);
                plys1.RemoveAt(plys1.Count - 1);
                change.Clear();
                change.Add(s.PointAt(1, 1 - Math.Sqrt(2.0) * segV / 2));
                change.Add(s.PointAt(1 - Math.Sqrt(2.0) * segU / 2, 0));
                change.Add(s.PointAt(1, Math.Sqrt(2.0) * segV / 2));
                change.Add(s.PointAt(1, 1 - Math.Sqrt(2.0) * segV / 2));
                plys2[0] = new Polyline(change);
                plys2.RemoveAt(plys2.Count - 1);
            }
            #endregion

            DataTree<Polyline> finalPly3 = new DataTree<Polyline>();///三角形
            if (s.IsClosed(0))/////U方向闭合
            {
                finalPly2.AddRange(plys6, new GH_Path(0, DA.Iteration, v));
                finalPly3.AddRange(plys3, new GH_Path(0, DA.Iteration, 0));
                finalPly3.AddRange(plys4, new GH_Path(0, DA.Iteration, 1));
                DA.SetDataTree(0, finalPly);
                DA.SetDataTree(1, finalPly2);
                DA.SetDataTree(2, finalPly3);
            }
            else if (s.IsClosed(1))/////V方向闭合
            {
                for (int i = 0; i < plys5.Count; i++)
                {
                    finalPly2.Add(plys5[i], new GH_Path(0, DA.Iteration, i));
                }
                finalPly3.AddRange(plys1, new GH_Path(0, DA.Iteration,0));
                finalPly3.AddRange(plys2, new GH_Path(0, DA.Iteration,1));
                DA.SetDataTree(0, finalPly);
                DA.SetDataTree(1, finalPly2);
                DA.SetDataTree(2, finalPly3);
            }
            else
            {
                finalPly3.AddRange(plys1, new GH_Path(0, DA.Iteration, 0));
                finalPly3.AddRange(plys2, new GH_Path(0, DA.Iteration, 1));
                finalPly3.AddRange(plys3, new GH_Path(0, DA.Iteration, 2));
                finalPly3.AddRange(plys4, new GH_Path(0, DA.Iteration, 3));
                DA.SetDataTree(0, finalPly);
                DA.SetDataTree(1, finalPly2);
                DA.SetDataTree(2, finalPly3);
            }
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
                return Resource1.surface_网格表皮G;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{9974f148-b9cf-4167-9a62-24beb812eaa7}"); }
        }
    }
}