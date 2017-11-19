using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers////TangChi 2016.7.8
{
    public class CancelFillet : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CancelFillet class.
        /// </summary>
        public CancelFillet()
            : base("取消圆角", "CancelFillet",
                "取消多段线的圆角，恢复多段线",
                 "Vipers", "Viper.curve")
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
            pManager.AddCurveParameter("多段线","P","有圆角的多段线",GH_ParamAccess.item);
            pManager.HideParameter(0);
            Message = "取消圆角";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("多段线", "P", "恢复圆角后的多段线", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve polylineCurve = null;
            if (!DA.GetData(0, ref polylineCurve)) return;
            Curve[] seg = polylineCurve.DuplicateSegments();
            List<Line> right = new List<Line>();//////分离出线段部分
            for (int i = 0; i < seg.Length; i++)
            {
                if (seg[i].IsLinear())
                {
                    Line test = new Line(seg[i].PointAtStart, seg[i].PointAtEnd);
                    right.Add(test);
                }
            }
            List<Point3d> last = new List<Point3d>();////重新计算交点
            Point3d pt1 = Point3d.Unset;
            Point3d pt2 = Point3d.Unset;
            if (polylineCurve.IsClosed)////闭合曲线
            {
                 ViperClass.ClosedPt(right[0], right[right.Count - 1], out pt1, out pt2);
                last.Add((pt1 + pt2) / 2);
                for (int i = 0; i < right.Count - 1; i++)
                {
                    ViperClass.ClosedPt(right[i], right[i + 1], out pt1, out pt2);
                    last.Add((pt1 + pt2) / 2);
                }
                last.Add(last[0]);
            }
            if (!polylineCurve.IsClosed)////不闭合曲线
            {
                last.Add(right[0].From);
                for (int i = 0; i < right.Count - 1; i++)
                {
                    ViperClass.ClosedPt(right[i], right[i + 1], out pt1, out pt2);
                    last.Add((pt1 + pt2) / 2);
                }
                last.Add(right[right.Count - 1].To);
            }
            DA.SetData(0, new Polyline(last));
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
                return Resource1.curve_取消圆角;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{9761651a-765d-46d1-907d-9a7eea49413f}"); }
        }
    }
}