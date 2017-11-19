using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers
{
    public class BrepKnots : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent17 class.
        /// </summary>
        public BrepKnots()
            : base("找出Brep的角点", "BrepKnots",
                "找出Brep的角点",
                "Vipers", "Viper.surface")
        {
        }
        public override GH_Exposure Exposure
        { 
         get{return GH_Exposure.obscure;}
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("brep物体","B","brep物体",GH_ParamAccess.item);
            pManager.HideParameter(0);
            Message = "Brep角点";
            
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("角点", "P", "brep的角点", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Brep brep = new Brep();
            if(!DA.GetData(0,ref brep)) return;
            DA.SetDataList(0, brep.DuplicateVertices());
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
               // return Resource1.Brep角点;
                return Resource1.surface_brep角点;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{1c998838-5350-4bbc-8dc6-1c1552a0ff16}"); }
        }
    }
}