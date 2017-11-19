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

namespace Vipers////TangChi 2015.7.29
{
    public class CurveModulusDivide : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent32 class.
        /// </summary>
        public CurveModulusDivide()
            : base("根据模数划分曲线", "CurveModulusDivide",
                "将曲线等分后，相邻点之间的距离为模数的整数倍，可通过公差来控制是否满足条件",
                 "Vipers", "Viper.curve")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("曲线","C","待划分曲线",GH_ParamAccess.item);
            pManager.AddNumberParameter("最小尺寸", "S1", "每段曲线允许的最小值", GH_ParamAccess.item);
            pManager.AddNumberParameter("最大尺寸", "S2", "每段曲线允许的最大值", GH_ParamAccess.item);
            pManager.AddNumberParameter("模数", "M", "每段曲线为该模数的倍数", GH_ParamAccess.item,10);
            pManager.AddNumberParameter("公差", "T", "每段曲线除以模数后的余数不能大于该公差", GH_ParamAccess.item,0.1);
            pManager.HideParameter(0);
            Message = "按模数划分曲线";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("段数", "N", "满足条件情况下，等分段数", GH_ParamAccess.list);
            pManager.AddNumberParameter("长度", "L", "满足条件情况下，每段长度", GH_ParamAccess.list);
            pManager.AddTextParameter("最佳匹配", "M", "列举最接近公差的匹配对象", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("序号", "I", "最佳对象在列表中的序号", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve curve = null;
            double sizeMin = 0;
            double sizeMax = 0;
            double multiple = 0;
            double tolerance = 0;
            if (!DA.GetData(0, ref curve)) return;
            if (!DA.GetData(1, ref sizeMin)) return;
            if (!DA.GetData(2, ref sizeMax)) return;
            if (!DA.GetData(3, ref multiple)) return;
            if (!DA.GetData(4, ref tolerance)) return;
            double length = curve.GetLength();
            List<int> segments = new List<int>();
            List<double> lens = new List<double>();
            List<double> difference = new List<double>();
            DataTree<string> match = new DataTree<string>();////最佳匹配对象
            List<int> indexs = new List<int>();////最佳对象索引
            int n1 = Convert.ToInt32(length / sizeMin);/////最多能分的段数
            int n2 = Convert.ToInt32(length / sizeMax);/////最少能分的段数
            for (int i = n2; i < n1; i++)
            {
                Point3d[] pts = new Point3d[i];
                curve.DivideByCount(i, true, out pts);
                Point3d p1 = pts[0];
                Point3d p2 = pts[1];
                double dist = p1.DistanceTo(p2);
                if (dist % multiple <= tolerance)
                {
                    segments.Add(i);
                    lens.Add(dist);
                    difference.Add(dist % multiple);
                }
            }
            difference.Sort();
            int index = 0;
            int branch = DA.Iteration;
            for (int i = 0; i < difference.Count; i++)
            {
                if (lens[i] % multiple == difference[0])///////difference[0]为最接近公差的情况
                {
                    match.Add(segments[i].ToString() + "(段数)", new GH_Path(0,branch,index));
                    match.Add(lens[i].ToString() + "(长度)", new GH_Path(0,branch,index));
                    indexs.Add(i);
                    index++;
                }
            }
            DA.SetDataList(0, segments);
            DA.SetDataList(1,lens);
            DA.SetDataTree(2, match);
            DA.SetDataList(3, indexs);
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
                //return Resource1.根据模数划分曲线;
                return Resource1.curve_按模数划分曲线;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{805e527e-3e59-49bf-b31d-d8f8bfa0a5df}"); }
        }
    }
}