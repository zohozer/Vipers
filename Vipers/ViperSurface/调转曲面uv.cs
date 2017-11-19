using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers////TangChi 2015.4.8
{
    public class ExchangeUV : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent21 class.
        /// </summary>
        public ExchangeUV()
            : base("对调曲面uv", "ExchangeUV",
                "将曲面uv对调",
                "Vipers", "Viper.surface")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure .hidden; }
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("待对调uv的曲面", "S", "待对调uv的曲面", GH_ParamAccess.item);
            pManager.HideParameter(0);
            Message = "对调曲面uv";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter("对调后的曲面", "S", "对调后的曲面", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Surface surface = null;
            if (!DA.GetData(0, ref surface)) return;
            Surface mm = surface.Transpose();
            DA.SetData(0, mm);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                //return Resource1.反转曲面uv;
                return Resource1.surface_对调曲面uv;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{e4ab5f27-0d99-4159-9403-12af25f3964c}"); }
        }
    }
}