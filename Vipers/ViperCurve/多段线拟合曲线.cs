using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers///////TangChi 2016.01.25
{
    public class PolylineMatch : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CurveConvert class.
        /// </summary>
        public PolylineMatch()
            : base("多段线拟合曲线", "Curve→Polyline",
                "用多段线拟合指定曲线",
                "Vipers", "Viper.curve")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("曲线","C","指定的曲线",GH_ParamAccess.item);
            pManager.AddNumberParameter("相似度","S","相似度是指拟合的多段线中每一段线段与对应的曲线的长度的比值，值越接近1，就越接近原曲线，但不可能等于1",GH_ParamAccess.item,0.999);
            Message = "Curve→Polyline";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("拟合结果", "P", "拟合曲线的多段线", GH_ParamAccess.item);
            pManager.AddPointParameter("节点","P","多段线节点",GH_ParamAccess.list);
            pManager.HideParameter(1);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve curve = null;
            double similarity = 0;
            if(!DA.GetData(0,ref curve))return;
            if (!DA.GetData(1, ref similarity)) return;
            if (similarity >= 1)
            {
                similarity = 0.9999;
            }
            ////////////////////////每段曲线两点距离与曲线长度之比不可能等于大于1
            if (curve.IsPolyline())
            {
                DA.SetData(0, curve);
                return;
            }
            ////////////////////////如果输入曲线本身为多段线，则不执行以下步骤
            List<Point3d> pts = new List<Point3d>();
            pts.Add(curve.PointAtStart);
            while (true)
            {
                Point3d start = curve.PointAtStart;
                Point3d end = curve.PointAtEnd;
                double length = curve.GetLength();
                if (end.DistanceTo(start) / length >= similarity)////先判断首尾两距离是否满足。
                {
                    pts.Add(end);
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
                    if (pt.DistanceTo(curve.PointAtStart) / cr1.GetLength() >= similarity)
                    {
                        pts.Add(pt);
                        curve = cr2;
                        flag = false;
                    }
                }
            }
            DA.SetData(0, new Polyline(pts));
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
                return Resource1.curve_多段线拟合曲线;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{a0aa9e3b-c6c9-47f1-b978-6383b15f2935}"); }
        }
    }
}