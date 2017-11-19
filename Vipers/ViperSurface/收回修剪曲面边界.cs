using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers////TangChi 2015.11.27
{
    public class ShrinkSurface : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent54 class.
        /// </summary>
        public ShrinkSurface()
            : base("收回曲面修剪边界", "ShrinkSurface",
                "将修剪曲面的边界收回至最小",
                "Vipers", "Viper.surface")
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
            pManager.AddBrepParameter("曲面","S","修剪的曲面",GH_ParamAccess.item);
            pManager.AddBooleanParameter("收回","B","true:匹配修剪曲面至边界,false:保持修剪曲面",GH_ParamAccess.item,false);
            pManager.HideParameter(0);
            Message = "收回修剪曲面边界";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("曲面", "S", "收回修剪边界的曲面", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Brep trimmed = null;
            bool shouhui = false;
            if(!DA.GetData(0,ref trimmed)||!DA.GetData(1,ref shouhui))return;
            //////////////////////////////////////////////////
            if (shouhui)
            {
                Rhino.Geometry.Collections.BrepFaceList bf = trimmed.Faces;
                bf.ShrinkFaces();
                List<Surface> sfs = new List<Surface>();
                for (int i = 0; i < bf.Count; i++)
                {
                    BrepFace bb = bf[i];
                    Surface sf = bb.DuplicateSurface();
                    sfs.Add(sf);
                }
                DA.SetDataList(0, sfs);
            }
            else
            {
                trimmed.Faces.ShrinkFaces();
                DA.SetData(0,trimmed);
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
                //return Resource1.收回曲面修剪边界;
                return Resource1.surface_收回边界;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{4a8571a7-eb42-4e50-87e9-bc386977e1e5}"); }
        }
    }
}