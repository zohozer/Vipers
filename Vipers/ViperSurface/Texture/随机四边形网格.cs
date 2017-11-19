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
using Rhino.Geometry.Intersect;
using Heteroptera;

namespace Vipers
{
    public class RandomRectangle : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the 曲线围合区间 class.
        /// </summary>
        public RandomRectangle()
            : base("随机四边形网格", "RRectangle",
                "随机划分曲面的四边形网格，可支持开放曲面和单方向闭合曲面",
                "Vipers", "Viper.surface")
        {
            Message = "随机四边形网格";
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
            pManager.AddSurfaceParameter("曲面","S","待划分网格的曲面",GH_ParamAccess.item);
            pManager.AddIntegerParameter("数量","N1","曲面v方向划分的数量",GH_ParamAccess.item,30);
            pManager.AddIntegerParameter("数量", "N2", "曲面u方向划分的数量", GH_ParamAccess.item, 20);
            pManager.AddNumberParameter("振幅","A","输入0~1之间数值，数值越大随机效果越明显",GH_ParamAccess.item,0.6);
            pManager.AddIntegerParameter("随机","S","调整数值，改变随机效果",GH_ParamAccess.item,0);
            pManager.HideParameter(0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("网格","C","生成的网格",GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
          protected override void SolveInstance(IGH_DataAccess DA)
        {
            Surface surface = null;
            int count = 0;
            int divide = 0;
            double amplitude = 0;
            int seed = 0;
              if(!DA.GetData(0,ref surface)||!DA.GetData(1,ref count)||!DA.GetData(2,ref divide)||!DA.GetData(3,ref amplitude)||!DA.GetData(4,ref seed))return;
              if(amplitude<0)amplitude=0;
              if (amplitude > 1) amplitude = 1;
            /////////////////////////////////////////////////////////振幅范围确定
            int max = 1000;
            int start = Convert.ToInt32(max / 2 * (1 - amplitude));
            int end = max - start;
            /////////////////////////////////////////////////////////
            surface.SetDomain(0, new Interval(0, 1));
            surface.SetDomain(1, new Interval(0, 1));
            DataTree<Point3d> collect = new DataTree<Point3d>();
            DataTree<Point3d> collects = new DataTree<Point3d>();///随机点
            DataTree<Polyline> last = new DataTree<Polyline>();
            double u = 0;
            double v = 0;
            for (int i = 0; i < count + 1; i++)
            {
                for (int q = 0; q < divide + 1; q++)
                {
                    collect.Add(surface.PointAt(u, v), new GH_Path(0, i));
                    v += 1.0 / divide;
                }
                v = 0;
                u += 1.0 / count;
            }
            Random r = new Random(seed);
            if (surface.IsClosed(1))
            {
                for (int i = 0; i < collect.BranchCount; i++)/////v闭合曲面的随机点
                {
                    List<Point3d> pts1 = collect.Branch(i);
                    List<Point3d> collect1 = new List<Point3d>();
                    for (int q = 0; q < pts1.Count - 1; q++)
                    {
                        double mmm = r.Next(start, end) / 1000.0;
                        collect1.Add(RandomPt(pts1[q], pts1[q + 1], mmm));
                    }
                    collects.AddRange(collect1, new GH_Path(0, i));
                }
                for (int i = 0; i < collects.BranchCount - 1; i++)
                {
                    for (int q = 0; q < collects.Branch(i).Count; q++)
                    {
                        if (q == collects.Branch(i).Count - 1)////最后首尾相连
                        {
                            Point3d[] pts2 = new Point3d[] { collects.Branch(i)[q], collects.Branch(i)[0], collects.Branch(i + 1)[0], collects.Branch(i + 1)[q], collects.Branch(i)[q] };
                            last.Add(new Polyline(pts2), new GH_Path(0, i));
                            break;
                        }
                        Point3d[] pts = new Point3d[] { collects.Branch(i)[q], collects.Branch(i)[q + 1], collects.Branch(i + 1)[q + 1], collects.Branch(i + 1)[q], collects.Branch(i)[q] };
                        last.Add(new Polyline(pts), new GH_Path(0, i));
                    }
                }
            }
            else if (surface.IsClosed(0))////u方向闭合
            {
                for (int i = 0; i < collect.BranchCount; i++)/////u闭合曲面的随机点
                {
                    List<Point3d> pts1 = collect.Branch(i);
                    List<Point3d> collect1 = new List<Point3d>();
                    collect1.Add(pts1[0]);
                    for (int q = 0; q < pts1.Count - 1; q++)
                    {
                        double mmm = r.Next(start, end) / 1000.0;
                        collect1.Add(RandomPt(pts1[q], pts1[q + 1], mmm));
                    }
                    collect1.Add(pts1[pts1.Count - 1]);
                    collects.AddRange(collect1, new GH_Path(0, i));
                }
                for (int i = 0; i < collects.BranchCount - 1; i++)
                {
                    for (int q = 0; q < collects.Branch(i).Count - 1; q++)
                    {
                        Point3d[] pts = new Point3d[] { collects.Branch(i)[q], collects.Branch(i)[q + 1], collects.Branch(i + 1)[q + 1], collects.Branch(i + 1)[q], collects.Branch(i)[q] };
                        last.Add(new Polyline(pts), new GH_Path(0, i));
                    }
                }
            }
            else //////开放曲面
            {
                for (int i = 0; i < collect.BranchCount; i++)/////普通开放曲面的随机点
                {
                    List<Point3d> pts1 = collect.Branch(i);
                    List<Point3d> collect1 = new List<Point3d>();
                    collect1.Add(pts1[0]);
                    for (int q = 0; q < pts1.Count - 1; q++)
                    {
                        double mmm = r.Next(start, end) / 1000.0;
                        collect1.Add(RandomPt(pts1[q], pts1[q + 1], mmm));
                    }
                    collect1.Add(pts1[pts1.Count - 1]);
                    collects.AddRange(collect1, new GH_Path(0, i));
                }
                for (int i = 0; i < collects.BranchCount - 1; i++)
                {
                    for (int q = 0; q < collects.Branch(i).Count - 1; q++)
                    {
                        Point3d[] pts = new Point3d[] { collects.Branch(i)[q], collects.Branch(i)[q + 1], collects.Branch(i + 1)[q + 1], collects.Branch(i + 1)[q], collects.Branch(i)[q] };
                        last.Add(new Polyline(pts), new GH_Path(0, i));
                    }
                }
            }
            DA.SetDataTree(0,last);
        }
          public Point3d RandomPt(Point3d pt1, Point3d pt2, double mmm) ////////////创建两点之间的点
          {
              Curve cr = new Line(pt1, pt2).ToNurbsCurve();
              cr.Domain = new Interval(0, 1);
              Point3d last = cr.PointAt(mmm);
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
                return Resource1.surface_随机表皮;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{bb122355-a61b-4c3c-b581-d15661376f34}"); }
        }
    }
}