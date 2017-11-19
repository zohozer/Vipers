using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers///////TangChi 2015.2.28
{
    public class SurfaceReverse : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent21 class.
        /// </summary>
        public SurfaceReverse()
            : base("反转曲面", "SurfaceReverse",
                "将曲面前后反转",
                "Vipers", "Viper.surface")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.hidden; }
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("待反转曲面","S","待反转的曲面",GH_ParamAccess.item);
            pManager.HideParameter(0);
            Message = "翻转曲面";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter("反转后的曲面", "S", "反转后的曲面", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            Surface surface = null;
            if(!DA.GetData(0,ref surface)) return;
            Surface mm = surface.Reverse(0);
            DA.SetData(0, mm);
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
               // return Resource1.反转曲面;
                return Resource1.surface_反转曲面;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{5bfbb070-371a-40ba-962d-9c05a9dc19bc}"); }
        }
    }
}