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

namespace Vipers/////TangChi 2015.10.23
{
    public class CullDuplicates : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent46 class.
        /// </summary>
        public CullDuplicates()
            : base("删除重合点", "CullDuplicates",
                "公差内删除重合点并将重合点分组",
                "Vipers", "Viper.point")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("点","P","待删除的点群",GH_ParamAccess.list);
            pManager.AddNumberParameter("公差","T","在此范围内被视为重合点",GH_ParamAccess.item,0);
            pManager.HideParameter(0);
            Message = "删除重合点";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("点","P","删除重合部分后的点",GH_ParamAccess.list);
            pManager.AddIntegerParameter("序号", "I", "重合的点的序号在一个分支", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> x = new List<Point3d>();
            double t = 0;
            if(!DA.GetDataList(0,x))return;
            if(!DA.GetData(1,ref t))return;
            //////////////////////////////////////////////////////////
            DataTree<int> index = new DataTree<int>();
            int mem = 0;
            int branch = DA.Iteration;
            List<Point3d> last = new List<Point3d>();
            for (int i = 0; i < x.Count; i++)
            {
                if (x[i] == Point3d.Unset) continue;
                last.Add(x[i]);
                index.Add(i, new GH_Path(0, branch, mem));
                for (int q = 0; q < x.Count; q++)
                {
                    if (x[q] == Point3d.Unset || i == q) continue;
                    if (x[i].DistanceTo(x[q]) <= t + 0.000000001)
                    {
                        x[q] = Point3d.Unset;
                        index.Add(q, new GH_Path(0, branch, mem));
                    }
                }
                mem++;
            }
            DA.SetDataList(0, last);
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
                //return Resource1.删除重合点;
                return Resource1.point_删除重合点;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{105ddf89-3dd0-4536-9d41-ddfe88c24c59}"); }
        }
    }
}