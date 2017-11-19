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

namespace Vipers/////TangChi 2015.2.24(2016.7.11改)
{
    public class ViperPointsOnLine : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent2 class.
        /// </summary>
        public ViperPointsOnLine()
            : base("指定点是否共线", "CollineationPoint",
                "判断指定点是否共线，如果不共线，则找出所有共线点",
                "Vipers", "Viper.point")
        {
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
            pManager.AddPointParameter("待判断点", "P", "待判断点", GH_ParamAccess.list);
            pManager.HideParameter(0);
            Message = "指定点是否共线";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("检查结果","R","指定点是否共线",GH_ParamAccess.item);
            pManager.AddLineParameter("直线", "L", "共线点所在直线", GH_ParamAccess.list);
            pManager.AddIntegerParameter("序号","I","共线的点的序号在一个分支里面",GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> x = new List<Point3d>();
            int count = 0;
            if(!DA.GetDataList(0,x))return;
            DataTree<Point3d> last = new DataTree<Point3d>();
            List<Point3d> collect = new List<Point3d>();
            List<Line> ls = new List<Line>();
            DataTree<int> index = new DataTree<int>();
            int mem = 0;
            int branch = DA.Iteration;
            //x = Point3d.CullDuplicates(x, 0.000000001).ToList();
            bool flag = true;
            for (int i = 0; i < x.Count; i++)
            {
                Point3d test1 = x[i];
                for (int q = i + 1; q < x.Count; q++)
                {
                    Point3d test2 = x[q];
                    if (test1 == test2) continue;////两点完全重合
                    for (int w = 0; w < last.BranchCount; w++)///两点不能同时存在于一组
                    {
                        if (last.Branch(w).Contains(test1) && last.Branch(w).Contains(test2))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (!flag)////test1和test2已经是一组共线点了
                    {
                        flag = true;
                        continue;
                    }
                    List<int> collectIndex = new List<int>();
                    collect = Coplaner(test1, test2, x,out collectIndex);/////找出共线点
                    if (collect.Count >= 3)
                    {
                        last.AddRange(collect, new GH_Path(0, count));
                        Point3d[] sort = Point3d.SortAndCullPointList(collect, 0.0000001);
                        ls.Add(new Line(sort[0], sort[sort.Length - 1]));
                        index.Add(i, new GH_Path(0, branch, mem));
                        index.Add(q, new GH_Path(0, branch, mem));
                        index.AddRange(collectIndex, new GH_Path(0, branch, mem));
                        count++;
                        mem++;
                    }
                    collect.Clear();
                    collectIndex.Clear();
                }
            }
            if (last.BranchCount == 1 && last.Branch(0).Count == x.Count)////所有点是否共线
            {
                DA.SetData(0,true);
            }
            else 
            {
                DA.SetData(0, false);
            }
            DA.SetDataList(1, ls);
            DA.SetDataTree(2, index);
        }
        public List<Point3d> Coplaner(Point3d pt1, Point3d pt2, List<Point3d> pts,out List<int> index) /////通过pt1和pt2确定的直线判断pts上的点是否在该直线上,同时索引不能与index1和index2重合
        {
            List<Point3d> last = new List<Point3d>();
             index = new List<int>();
            Line test = new Line(pt1, pt2);
            for (int i = 0; i < pts.Count; i++)
            {
                Point3d pt3 = pts[i];
                if (test.DistanceTo(pt3, false) <= 0.00000001)
                {
                    last.Add(pt3);
                    index.Add(i);
                }
            }
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
                //return Resource1.找出共线点;
                return Resource1.point_指定点是否共线;
            }
        }
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{9b66828d-de2d-49c9-8b69-e551a99e5fda}"); }
        }
    }
}