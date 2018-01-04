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

namespace Vipers////TangChi 2015 8 11
{
    public class PlanerSurface : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent35 class.
        /// </summary>
        public PlanerSurface()
            : base("Planar Surface", "PlanarSurface",
                "Create a planar surface by its length and width",
                 "Vipers", "Viper.surface")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.obscure;}
        }
            
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane","P","Base plane",GH_ParamAccess.item,Plane.WorldXY);
            pManager.AddNumberParameter("Length","L","Surface length",GH_ParamAccess.item,100);
            pManager.AddNumberParameter("Width", "W", "Surface width", GH_ParamAccess.item,100);
            pManager.HideParameter(0);
            Message = "Planar Surface";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface","S","Resulting surface",GH_ParamAccess.list);
            pManager.AddPointParameter("Vertices","V","Surface vertices",GH_ParamAccess.list);
            pManager.AddCurveParameter("Edges","E","Surface edges",GH_ParamAccess.list);
            pManager.HideParameter(1);
            pManager.HideParameter(2);
        }
        
        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane plane = new Plane();
            double length = 0;
            double width = 0;
            if(!DA.GetData(0,ref plane)) return;
            if (!DA.GetData(1, ref length)) return;
            if (!DA.GetData(2, ref width)) return;
            Interval i1 = new Interval(0, length);
            Interval i2 = new Interval(0, width);
            if (length < 0)
            {
                i1 = new Interval(length, 0);
            }
            if (width < 0)
            {
                i2 = new Interval(width, 0);
            }
            PlaneSurface ps = new PlaneSurface(plane, i1, i2);
            Surface s = (Surface)ps;
            Point3d[] pts = s.ToBrep().DuplicateVertices();
            Curve[] css = s.ToBrep().DuplicateEdgeCurves();
            DA.SetData(0, s);
            DA.SetDataList(1, pts.ToList());
            DA.SetDataList(2, css.ToList());
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
                //return Resource1.平面曲面;
                return Resource1.surface_平面曲面;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{7c004214-64a3-4f6c-9651-8bc4ae90c480}"); }
        }
    }
}