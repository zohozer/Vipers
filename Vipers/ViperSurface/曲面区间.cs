using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers
{
    public class SurfaceDomain : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SurfaceDomain class.
        /// </summary>
        public SurfaceDomain()
            : base("Surface Domain", "SrfDomain",
                "Change surface domain",
                "Vipers", "Viper.surface")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface","S","Surface to modify",GH_ParamAccess.item);
            pManager.AddIntervalParameter("U Domain", "U", "Input U domain", GH_ParamAccess.item, new Interval(0, 0));
            pManager.AddIntervalParameter("V Domain","V","Input V domain",GH_ParamAccess.item,new Interval(0,0));
            pManager.HideParameter(0);
            Message = "Surface Domain";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "S", "Modified surface", GH_ParamAccess.item);
            pManager.AddIntervalParameter("U Domain", "U", "Modified U domain", GH_ParamAccess.item);
            pManager.AddIntervalParameter("V Domain", "V", "Modified V domain", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Closed","C", "Determine direction of closed surface (0: U direction closed, 1: V direction closed)", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Surface srf = null;
            Interval u = new Interval();
            Interval v = new Interval();
            if (!DA.GetData(0,ref srf))return;
            if (!DA.GetData(1, ref u)) return;
            if (!DA.GetData(2, ref v)) return;
            if(u.ToString() !=new Interval(0,0).ToString())
            {
                srf.SetDomain(0,u);
            }
            else if (v.ToString() != new Interval(0, 0).ToString())
            {
                srf.SetDomain(0, v);
            }
            if(srf.IsClosed(0))
            {
                DA.SetData(3,0);
            }
             else if (srf.IsClosed(1))
            {
                DA.SetData(3, 1);
            }
            DA.SetData(0,srf);
            DA.SetData(1, srf.Domain(0));
            DA.SetData(2, srf.Domain(1));
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
                return Resource1.surface_曲面区间;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{bdec90ab-4122-47b1-8c1f-1a4a94df5bf7}"); }
        }
    }
}