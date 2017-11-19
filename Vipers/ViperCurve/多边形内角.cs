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

namespace Vipers////TangChi 2015.6.5
{
    public class ViperPolylineAngle : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public ViperPolylineAngle()
            : base("测量多边形内角", "PolygonAngle",
                "找出指定多边形的内角（封闭或不封闭）",
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
            pManager.AddCurveParameter("多边形", "P", "待测量的多边形", GH_ParamAccess.item);
            Message = "多边形内角(角度&弧度)";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("角度数", "A", "多边形的各个内角角度", GH_ParamAccess.list);
            pManager.AddNumberParameter("弧度数", "R", "多边形的各个内角弧度", GH_ParamAccess.list);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////声明变量
            Curve polyline1 = null;
            ///////////////////////////////////////////////////////////////////////////////////////////////////检测输入端是否合理
            if (!DA.GetData(0, ref polyline1)) return;
            Polyline polyline = new Polyline();
            polyline1.TryGetPolyline(out polyline);
            if (polyline.IsClosed)
            {
                Line[] lines = polyline.GetSegments();
                List<Point3d> pts = new List<Point3d>();
                for (int i = 0; i < lines.Length; i++)
                {
                    Line l = lines[i];
                    pts.Add(l.From);
                }
                List<Point3d> ptA = new List<Point3d>();
                List<Point3d> ptB = new List<Point3d>();
                for (int i = 0; i < lines.Length; i++)////角点的左边点
                {
                    if (i == lines.Length - 1)
                    {
                        ptA.Add(pts[0]);
                        break;
                    }
                    ptA.Add(pts[i + 1]);
                }
                for (int i = 0; i < lines.Length; i++)////角点的右边点
                {
                    if (i == 0)
                    {
                        ptB.Add(pts[lines.Length - 1]);
                        continue;
                    }
                    ptB.Add(pts[i - 1]);
                }
                List<double> agl = new List<double>();
                List<double> rad = new List<double>();
                for (int i = 0; i < lines.Length; i++)
                {
                    Vector3d v1 = Point3d.Subtract(pts[i], ptA[i]);
                    Vector3d v2 = Point3d.Subtract(pts[i], ptB[i]);
                    rad.Add(Vector3d.VectorAngle(v1, v2));
                    agl.Add(Vector3d.VectorAngle(v1, v2) * 180 / Math.PI);
                }
                DA.SetDataList(0, agl);
                DA.SetDataList(1, rad);
            }
            else
            {
                Line[] lines = polyline.GetSegments();
                List<Point3d> pts = new List<Point3d>();
                for (int i = 0; i < lines.Length; i++)
                {
                    Line l = lines[i];
                    pts.Add(l.From);
                }
                pts.Add(lines[lines.Length - 1].To);/////加入最后一个点
                List<Point3d> ptCenter = new List<Point3d>();
                List<Point3d> ptA = new List<Point3d>();
                List<Point3d> ptB = new List<Point3d>();
                for (int i = 0; i < pts.Count - 2; i++)
                {
                    ptA.Add(pts[i]);
                    ptCenter.Add(pts[i + 1]);
                    ptB.Add(pts[i + 2]);
                }
                List<double> agl = new List<double>();
                List<double> rad = new List<double>();
                for (int i = 0; i < ptA.Count; i++)
                {
                    Vector3d v1 = Point3d.Subtract(ptCenter[i], ptA[i]);
                    Vector3d v2 = Point3d.Subtract(ptCenter[i], ptB[i]);
                    rad.Add(Vector3d.VectorAngle(v1, v2));
                    agl.Add(Rhino.RhinoMath.ToDegrees(Vector3d.VectorAngle(v1, v2)));

                }
                DA.SetDataList(0, agl);
                DA.SetDataList(1, rad);
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
                //return Resource1.v5;
                return Resource1.curve_多段线内角;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{e7b72297-8ea4-447c-afe2-c816048997a2}"); }
        }
    }
}