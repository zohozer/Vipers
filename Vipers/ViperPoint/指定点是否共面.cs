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

namespace Vipers /////TangChi 2015.9.10
{
    public class PointCoplane : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent9 class.
        /// </summary>
        public PointCoplane()
            : base("指定点是否共面", "PointCoplane",
                "判断指点是否共面",
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
            pManager.AddPointParameter("指定点","P","用以判断是否是共面点的点列表",GH_ParamAccess.list);
            pManager.AddNumberParameter("公差","T","在此范围内的点视为共面点（默认为0.0001）",GH_ParamAccess.item, 0.0001);
            pManager.HideParameter(0);
            Message = "指定点是否共面";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPlaneParameter("平面","P","公差范围内的共面平面",GH_ParamAccess.item);
            pManager.AddBooleanParameter("判断结果","R","判断指定点是否共面",GH_ParamAccess.item);
            
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> points = new List<Point3d>();
            double tolerance = 0;
            if(!DA.GetDataList(0,points)) return;
            if(!DA.GetData(1, ref tolerance)) return;
            Plane pln = new Plane();
            Plane.FitPlaneToPoints(points, out pln);
            bool flag = true;
            for (int i = 0; i < points.Count; i++)
            {
                double dis = pln.DistanceTo(points[i]);
                if (dis > tolerance)
                {
                    flag = false;
                    break;
                }
            }
            if (flag == true)
            {
                DA.SetData(0, pln);
                DA.SetData(1, flag);
            }
            else
            {
                DA.SetData(1, flag);
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
               // return Resource1.指定点是否共面;
                return Resource1.point_指定点是否共面;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{f2733dc0-5806-4e53-ac41-29ed207de915}"); }
        }
    }
}