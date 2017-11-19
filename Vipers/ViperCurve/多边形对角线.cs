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

namespace Vipers////TangChi 2015.6.11
{
    public class PolygonDiagonal : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent5 class.
        /// </summary>
        public PolygonDiagonal()
            : base("多边形对角线", "PolygonDiagonal",
                "连接多边形对角线",
                "Vipers", "Viper.curve")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary|GH_Exposure.hidden; }
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("多边形","P","多边形",GH_ParamAccess.item);
            pManager.HideParameter(0);
            Message = "多边形对角线";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("生成对角线","D","生成对角线",GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve polyline = null;
            if(!DA.GetData(0,ref polyline)) return;
            List<Point3d> pts = ViperClass.Ptsss(polyline);
            List<Line> lines = new List<Line>();
            if (pts.Count > 3)
            {
                for (int i = 0; i < pts.Count; i++)
                {
                    for (int j = i + 2; j < pts.Count; j++)
                    {
                        if (i == 0 && j == pts.Count - 1)
                        {
                            break;
                        }
                        Line l = new Line(pts[i], pts[j]);
                        lines.Add(l);
                    }
                }
            }
            DA.SetDataList(0, lines);
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
                //return Resource1.v4;
                return Resource1.curve_多边形对角线;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{aa0e8e5a-c2c6-4559-8994-6272dbd8da01}"); }
        }
    }
}