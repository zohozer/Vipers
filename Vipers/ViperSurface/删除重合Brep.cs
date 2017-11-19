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
            : base("删除重合Brep", "CullDuplicateBrep",
                "将重合的Brep删除（直接调用Brep中的“isDuplicate”方法，其中tolerance这一选项似乎作用不大）",
                "Vipers", "Viper.surface")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("brep列表","B","待删除的brep",GH_ParamAccess.list);
            pManager.AddNumberParameter("公差","T","将重合的Brep删除（直接调用Brep中的“isDuplicate”方法，其中tolerance这一选项似乎作用不大）",GH_ParamAccess.item,0);
            pManager.HideParameter(0);
            Message = "删除重合Brep";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("brep列表","B","删除重合部分后的brep",GH_ParamAccess.list);
            pManager.AddIntegerParameter("序号", "I", "重合brep在原列表中对应的序号", GH_ParamAccess.tree);
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