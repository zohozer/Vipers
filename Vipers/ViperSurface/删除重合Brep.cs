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

namespace Vipers////TangChi 2015.6.10(20151215修改)
{
    public class CullDuplicateBrep : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CullDuplicateBrep class.
        /// </summary>
        public CullDuplicateBrep()
            : base("Cull Duplicate Brep", "CullDupBrep",
                "Cull duplicate Breps",
                "Vipers", "Viper.surface")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep","B","Breps to operate on",GH_ParamAccess.list);
            pManager.AddNumberParameter("Tolerance","T","Delete duplicate brep within this tolerance",GH_ParamAccess.item,0);
            pManager.HideParameter(0);
            Message = "Cull Duplicate Brep";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep","B","Culled Breps",GH_ParamAccess.list);
            pManager.AddIntegerParameter("Indices", "I", "Index map of culled Breps", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Brep> breps = new List<Brep>();
            double tolerance = 0;
            if(!DA.GetDataList(0,breps))return;
            if(!DA.GetData(1,ref tolerance))return;
            DataTree<int> index = new DataTree<int>();
            List<Brep> bps = new List<Brep>();
            List<int> test = new List<int>();
            int num = 0;
            for (int i = 0; i < breps.Count; i++)
            {
                if (test.Contains(i))
                {
                    continue;
                }
                Brep bb = breps[i];
                bps.Add(bb);
                test.Add(i);
                index.Add(i, new GH_Path(0, DA.Iteration,num));
                for (int j = 0; j < breps.Count; j++)
                {
                    if (test.Contains(j))
                    {
                        continue;
                    }
                    if (bb.IsDuplicate(breps[j], tolerance))
                    {
                        test.Add(j);
                        index.Add(j, new GH_Path(0,DA.Iteration, num));
                    }
                }
                num++;
            }
            DA.SetDataList(0, bps);
            DA.SetDataTree(1, index);
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
                return Resource1.surface_删除重合brep;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{8ce3d975-06c6-4c12-8aeb-c54c0b94776b}"); }
        }
    }
}