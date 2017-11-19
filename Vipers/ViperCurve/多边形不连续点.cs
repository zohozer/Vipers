using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Parameters;

namespace Vipers//////TangChi 2016.3.16
{
    public class Discontinuity : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the 多边形不连续点 class.
        /// </summary>
        public Discontinuity()
            : base("多边形不连续点", "PlyDiscontinuity",
                "如果多边形中(闭合或不闭合)任意相邻两边的夹角大于angle(弧度数)，夹角点则被视为不连续点",
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
            pManager.AddCurveParameter("多段线","P","待检测的多段线",GH_ParamAccess.item);
            AddNumberInput();
            pManager.HideParameter(0);
            Message = "多段线不连续点";
        }
        Param_Number p = new Param_Number();
        public void AddNumberInput()////创建number输出端 该输出端可以添加角度选项
        {
            p.AngleParameter = true;
            p.Name = "角度";
            p.NickName = "A";
            p.Description = "如果多边形中任意相邻两边的夹角大于该角度(弧度数)，夹角点则被视为不连续点";
            p.Access = GH_ParamAccess.item;
            p.SetPersistentData( Math.PI * 0.1);
            Params.RegisterInputParam(p);
            p.Optional = true;
        }
        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("结果","R","如果有不连续点，则输出该点，如果输入的曲线不是多段线，则输出false",GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve ply = null;
            double angle=0;
            if(!DA.GetData(0,ref ply))return;
            if (!DA.GetData(1, ref angle)) return;
            if (p.UseDegrees)
                angle = Rhino.RhinoMath.ToRadians(angle);
            Polyline polyline = null;
            if (ply.TryGetPolyline(out polyline))
            {

            }
            else
            {
                List<bool> last = new List<bool>();
                last.Add(false);
                DA.SetDataList(0,last);
                return;
            }
            Polyline x = polyline;
            double y = angle;
            List<Point3d> pts = new List<Point3d>();
            Line[] ls = x.GetSegments();
            if (x.IsClosed)/////////闭合情况
            {
                for (int i = 0; i < ls.Length; i++)
                {
                    if (i == ls.Length - 1)///末项
                    {
                        Line l1 = ls[i];
                        Line l2 = ls[0];
                        Vector3d v1 = l1.Direction;
                        Vector3d v2 = l2.Direction;
                        double angle2 = Vector3d.VectorAngle(v1, v2);
                        if (angle2 > Math.PI*0.5)
                        {
                            angle2 = Math.PI - angle;
                        }
                        if (angle2 >= y)
                        {
                            pts.Add(l1.To);
                        }
                        break;
                    }
                    else
                    {
                        Line l1 = ls[i];
                        Line l2 = ls[i + 1];
                        Vector3d v1 = l1.Direction;
                        Vector3d v2 = l2.Direction;
                        double angle2 = Vector3d.VectorAngle(v1, v2);
                        if (angle2 > Math.PI*0.5)
                        {
                            angle2 = Math.PI - angle;
                        }
                        if (angle2 >= y)
                        {
                            pts.Add(l1.To);
                        }
                    }
                }
            }
            else///////////////不闭合情况
            {
                for (int i = 0; i < ls.Length - 1; i++)
                {
                    Line l1 = ls[i];
                    Line l2 = ls[i + 1];
                    Vector3d v1 = l1.Direction;
                    Vector3d v2 = l2.Direction;
                    double angle2 = Vector3d.VectorAngle(v1, v2);
                    if (angle2 > Math.PI*0.5)
                    {
                        angle2 = Math.PI - angle;
                    }
                    if (angle2 >= y)
                    {
                        pts.Add(l1.To);
                    }
                }
            }
            DA.SetDataList(0, pts);
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
                return Resource1.curve_多段线不连续点;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{ad8cc10c-2292-4497-957a-009cd2b13805}"); }
        }
    }
}