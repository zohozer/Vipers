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

namespace Vipers///TangChi 2015.6.8
{
    public class superFibonacci : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent13 class.
        /// </summary>
        public superFibonacci()
            : base("列表前n项之和", "superFibonacci",
                "循环列表，每次记录前n项之和",
                "Vipers", "Viper.math")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            double[] li = {1,2,3,4,5,6,7,8,9};
            List<double> li2 = li.ToList();
            pManager.AddNumberParameter("数字列表", "L", "数字列表", GH_ParamAccess.list,li2);
            Message = "Super Fibonacci";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("输出结果", "F", "输出结果", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<double> listOfNumbers = new List<double>();
            if(!DA.GetDataList(0,listOfNumbers)) return;
            List<double> all = new List<double>();
            for (int i = 0; i < listOfNumbers.Count; i++)
            {
                List<double> list = listOfNumbers.Take(i + 1).ToList();
                all.Add(list.Sum());
            }
            DA.SetDataList(0,all);
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
                //return Resource1.super_Fibonacci;
                return Resource1.math_斐波拉契;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{9e3f6913-57c4-4271-ba17-96a3043fc8f4}"); }
        }
    }
}