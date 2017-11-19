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

namespace Vipers/////TangChi 2016.5.23
{
    public class ConcaveConvex : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the 凹凸多边形 class.
        /// </summary>
        public ConcaveConvex()
            : base("凹凸多边形", "ConcaveConvex",
                "判断输入的多边形是否为凸多边形，如果是凸多边形，则返回true，否则返回false",
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
            pManager.AddCurveParameter("多段线","P","待检测的闭合多段线",GH_ParamAccess.item);
            pManager.HideParameter(0);
            Message = "凹凸多边形";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("结果","R","检查结果，如果是凸多边形则返回true,否则返回false",GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve curve = null;
            if(!DA.GetData(0,ref curve))return;
            Polyline polyline = new Polyline();
            curve.TryGetPolyline(out polyline);
            List<Point3d> pts = new List<Point3d>();
            List<Line> lines = new List<Line>();
            for (int q = 0; q < polyline.SegmentCount; q++)
            {
                lines.Add(polyline.SegmentAt(q));
                pts.Add(polyline.SegmentAt(q).From);
                pts.Add(polyline .SegmentAt(q).To);
            }
            pts = pts.Distinct().ToList();
            bool test = true;
            for (int k = 0; k < lines.Count; k++)
            {
                if (!ViperClass.SameSide(lines[k], pts))
                {
                    test = false;
                    break;
                }
            }
            DA.SetData(0, test);
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
                return Resource1.curve_判断凹凸多边形;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{da6f7e00-d4fd-40e0-8ad2-cdeb0b100286}"); }
        }
    }
}