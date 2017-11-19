using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers///////TangChi 2016.01.28
{
    public class ArcMatch : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the 曲线最优转换弧线 class.
        /// </summary>
        public ArcMatch()
            : base("弧线拟合曲线", "Curve→Arc",
                "用多个弧线拟合曲线",
                "Vipers", "Viper.curve")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("曲线", "C", "指定的曲线", GH_ParamAccess.item);
            pManager.AddNumberParameter("公差", "T", "设置0~1之间的公差", GH_ParamAccess.item, 0.1);
            Message = "Curve→Arc";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddArcParameter("拟合结果", "C", "拟合曲线的弧线", GH_ParamAccess.list);
            pManager.AddPointParameter("节点", "P", "每段弧线节点", GH_ParamAccess.list);
            pManager.HideParameter(1);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve curve = null;
            double tolerance = 0;
            if (!DA.GetData(0, ref curve)) return;
            if (!DA.GetData(1, ref tolerance)) return;
            if (curve.IsArc())
            {
                DA.SetData(0, curve);
                return;
            }
            if(tolerance==0)
            {
                return;
            }
            ////////////////////////如果输入曲线本身为弧线，则不执行以下步骤
            List<Point3d> pts = new List<Point3d>();
            List<Arc> arcss = new List<Arc>();
            pts.Add(curve.PointAtStart);
            while (true)
            {
                Point3d start = curve.PointAtStart;
                Point3d end = curve.PointAtEnd;
                double length = curve.GetLength();
                Arc aaa = Arc.Unset;
                if (curve.TryGetArc(out aaa, tolerance))////先判断首尾两距离是否满足。
                {
                    pts.Add(end);
                    arcss.Add(aaa);
                    break;
                }
                double norm = 1.0;
                bool flag = true;
                while (flag)
                {
                    norm = norm * 0.9;///每次从曲线该位置查找
                    Point3d pt = curve.PointAtNormalizedLength(norm);
                    double t = 0;
                    curve.ClosestPoint(pt, out t);
                    Curve cr1 = curve.Split(t)[0];/////用于判断的第一条曲线
                    Curve cr2 = curve.Split(t)[1];/////第二条曲线（如果第一条曲线满足条件则第二条为起始曲线）
                    if (cr1.TryGetArc(out aaa, tolerance))///是否成功转换弧线
                    {
                        pts.Add(pt);
                        arcss.Add(aaa);
                        curve = cr2;
                        flag = false;
                    }
                }
            }
            DA.SetDataList(0, arcss);
            DA.SetDataList(1, pts);
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
                return Resource1.curve_弧线拟合曲线;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{fd69020b-0d73-42a7-adbc-1d1f61ccc61c}"); }
        }
    }
}