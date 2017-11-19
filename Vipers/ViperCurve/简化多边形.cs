using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers////////TangChi 2015.9.28
{
    public class PolylineSimplify : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent50 class.
        /// </summary>
        public PolylineSimplify()
            : base("简化多段线", "PolylineSimplify",
                "简化内角大于指定区间的多边形（闭合或非闭合）",
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
            pManager.AddCurveParameter("多段线","P","待简化的多边形",GH_ParamAccess.item);
            pManager.AddIntervalParameter("角度区间","D","在此角度区间的角将被简化为平角",GH_ParamAccess.item,new Interval(Math.PI*0.8,Math.PI));
            pManager.HideParameter(0);
            Message = "简化多边形";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("多段线","P","简化后的多段线",GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve polyline1 = null;
            Interval angleDomain = new Interval();
            if(!DA.GetData(0,ref polyline1))return;
            if (!DA.GetData(1, ref angleDomain)) return;
            if (angleDomain.Max == angleDomain.Min)/////////如果输入的区间格式不正确
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "输入端的区间的最大值和最小值不能相同");
                return;
            }
            ////string test = this.Params.Input[1].Sources[0].TypeName;
            //if (test == "Number" || test == "Integer")
            //{
            //    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "输入端的区间的最大值和最小值不能相同");
            //    return;
            //}
            Polyline x = new Polyline();
            polyline1.TryGetPolyline(out x);
            Interval y = angleDomain;
            if (x.IsClosed)
            {
                Line[] ls = x.GetSegments();
                List<Vector3d> vcs = new List<Vector3d>();
                List<Vector3d> vcs22 = new List<Vector3d>();
                List<Point3d> pts = new List<Point3d>();
                List<Point3d> pts22 = new List<Point3d>();
                List<double> angles = new List<double>();
                y.MakeIncreasing();
                double max = y.Max;
                double min = y.Min;
                foreach (Line l in ls)
                {
                    Vector3d vc = l.Direction;
                    vc.Unitize();
                    vcs.Add(vc);
                    vc.Reverse();
                    vcs22.Add(vc);
                    pts.Add(l.From);
                }
                /////////////////////////////////////
                for (int i = 0; i < vcs.Count; i++)
                {
                    if (i == vcs.Count - 1)
                    {
                        double nn = Vector3d.VectorAngle(vcs[i], vcs22[0]);
                        if (nn < min || nn > max)
                        {
                            pts22.Add(pts[0]);
                        }
                        break;
                    }
                    double mm = Vector3d.VectorAngle(vcs[i], vcs22[i + 1]);
                    if (mm < min || mm > max)
                    {
                        pts22.Add(pts[i + 1]);
                    }
                }
                pts22.Add(pts22[0]);
                Polyline pl = new Polyline(pts22);//最终生成的点
                if(!pl.IsValid)
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "无法根据条件简化多段线，简化后的多段线无效");
                    return ;
                }
                DA.SetData(0, pl);
            }
            //////////////////////////////////////////////////////////////////////////////////////
            else if (!x.IsClosed)
            {
                Line[] ls = x.GetSegments();
                List<Vector3d> vcs = new List<Vector3d>();
                List<Vector3d> vcs22 = new List<Vector3d>();
                List<Point3d> pts = new List<Point3d>();
                List<Point3d> pts22 = new List<Point3d>();
                List<double> angles = new List<double>();
                y.MakeIncreasing();
                double max = y.Max;
                double min = y.Min;
                foreach (Line l in ls)
                {
                    Vector3d vc = l.Direction;
                    vc.Unitize();
                    vcs.Add(vc);
                    vc.Reverse();
                    vcs22.Add(vc);
                    pts.Add(l.From);
                }
                pts.Add(ls[ls.Length - 1].To);///不闭合多段线的特殊情况
                //////////////////////////////////////
                pts22.Add(pts[0]);///不闭合多段线的特殊情况
                for (int i = 0; i < vcs.Count - 1; i++)
                {
                    double mm = Vector3d.VectorAngle(vcs[i], vcs22[i + 1]);
                    if (mm < min || mm > max)
                    {
                        pts22.Add(pts[i + 1]);
                    }
                }
                pts22.Add(pts[pts.Count - 1]);
                Polyline pl = new Polyline(pts22);//最终生成的点
                if (!pl.IsValid)
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "无法根据条件简化多段线，简化后的多段线无效");
                    return;
                }
                DA.SetData(0, pl);
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
                //return Resource1.简化内角大于指定区间的多边形;
                return Resource1.curve_简化多边形;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{026772bb-5ce5-4ecf-aa1c-d9176177438d}"); }
        }
    }
}