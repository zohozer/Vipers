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

namespace Vipers/////////TangChi 2015.4.5
{
    public class RandomLoop : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent8 class.
        /// </summary>
        public RandomLoop()
            : base("循环列表之和小于给定总数", "RandomLoop",
                "循环给定列表，直到之和小于给定总数",
                "Vipers", "Viper.math")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            double[] numsa = { 1, 2, 3, 4, 5, 6 };
            List<double> numsb = numsa.ToList();
            pManager.AddNumberParameter("指定列表","L","指定列表",GH_ParamAccess.list,numsb);
            pManager.AddNumberParameter("总数", "C", "列表数之和小于该数", GH_ParamAccess.item, 50);
            Message = "循环列表之和小于给定总数";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("循环列表", "L", "从numberList选择的数组成的列表", GH_ParamAccess.list);
            pManager.AddNumberParameter("余数", "R", "randomList中数字之和与总数的差值", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<double> members = new List<double>();
            double total = 0;
            if (!DA.GetDataList(0, members)) return;
            if (!DA.GetData(1, ref total)) return;
            List<double> lis = new List<double>();
            double tot = 0;
            if (members.Sum() != 0)
            {
                for (int q = 0; q <= 0; q += 0)
                {
                    for (int i = 0; i < members.Count; i++)
                    {
                        tot += members[i];
                        if (tot > total)
                        {
                            q = 100;
                            tot -= members[i];
                            break;
                        }
                        lis.Add(members[i]);
                    }
                }
            }

            DA.SetDataList(0,lis);
           DA.SetData(1, total - tot);
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
               // return Resource1.循环列表之和小于给定总数;
                return Resource1.math_循环列表;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{297857de-dbc1-4c7f-af0f-4fcedffd9669}"); }
        }
    }
}