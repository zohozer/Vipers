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

namespace Vipers///TangChi 2015.2.12
{
    public class ViperPointsOnCurve : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent2 class.
        /// </summary>
        public ViperPointsOnCurve()
            : base("与曲线距离在指定范围的点", "PointToCurve",
                "公差内点在曲线上，设置公差（maxdistance），在此公差内的点认为在曲线上",
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
            pManager.AddCurveParameter("曲线", "C", "与点产生关系的曲线", GH_ParamAccess.list);
            pManager.AddPointParameter("待选点", "P", "待选点", GH_ParamAccess.list);
            pManager.AddNumberParameter("距离公差", "D", "在公差范围内的点均被选出", GH_ParamAccess.item,0);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
            Message = "与曲线距离在指定范围的点";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("选出点", "P1", "满足要求的点", GH_ParamAccess.tree);
            pManager.AddPointParameter("剩余点", "P2", "不满足要求的点", GH_ParamAccess.list);
            pManager.AddIntegerParameter("序号", "I1", "满足要求的点所在序号", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("序号", "I2", "不满足要求的点所在序号", GH_ParamAccess.list);
            pManager.HideParameter(1);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> curves = new List<Curve>();
            List<Point3d> points = new List<Point3d>();
            double distance = 0;
            if (!DA.GetDataList(0, curves)) return;
            if (!DA.GetDataList(1, points)) return;
            if (!DA.GetData(2, ref distance)) return;
            double kkk = 0;
            double u = distance + 0.0000001;//ClosestPoint这种方法中的u是指不能大于（不包括等于）
            DataTree<Point3d> ptss = new DataTree<Point3d>();/////接受点
            DataTree<int> indexall = new DataTree<int>();///接受点的序号
            List<int> ptss2 = new List<int>();///接受点的序号(列表)
            List<int> indexmem = new List<int>();///所有点的序号
            for (int i = 0; i < points.Count; i++)
            {
                indexmem.Add(i);
            }
            Point3d[] ptarray = points.ToArray();
            Point3d ptchange = new Point3d(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
            int index = 0;////数列序号
            for (int i = 0; i < curves.Count; i++)
            {
                Curve cv = curves[i];
                for (int j = 0; j < ptarray.Length; j++)
                {
                    bool judge = cv.ClosestPoint(ptarray[j], out kkk, u);
                    if (judge == true)///如果有，则进入，重新循环该列表
                    {
                        for (int q = j; q < ptarray.Length; q++)////从第j项开始
                        {
                            bool judge2 = cv.ClosestPoint(ptarray[q], out kkk, u);
                            if (judge2 == true && ptarray[q] != new Point3d(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity))
                            {
                                ptss.Add(ptarray[q], new GH_Path(0, DA.Iteration,index));
                                indexall.Add(q, new GH_Path(0,DA.Iteration, index));
                                ptss2.Add(q);
                                ptarray[q] = ptchange;
                            }
                        }
                        index++;
                        break;
                    }
                }
            }
            ptss2.Sort();
            List<int> restmem = new List<int>();/////未被找出的点的序号
            List<Point3d> ptrest = new List<Point3d>();/////未被找出的点
            foreach (int i in indexmem)//////通过ptss2和indexmem找出未被选出的点的序号
            {
                if (!ptss2.Contains(i))
                {
                    restmem.Add(i);
                    ptrest.Add(points[i]);
                }
            }
            DA.SetDataTree(0, ptss);
            DA.SetDataList(1, ptrest);
            DA.SetDataTree(2, indexall);
            DA.SetDataList(3, restmem);
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
                //return Resource1.满足与曲线位置关系的点;
                return Resource1.point_与曲线距离在指定范围的点;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{7cbaf34f-4c49-4e26-8925-3490f349b35e}"); }
        }
    }
}