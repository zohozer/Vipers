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

namespace Vipers////TangChi 2015.6.19
{
    public class ViperDivideInterval : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent2 class.
        /// </summary>
        public ViperDivideInterval()
            : base("列表划分区间", "ListDivideInterval",
                "根据列表中的数值占列表数总和的比例划分区间",
                "Vipers", "Viper.math")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            List<double> test = new List<double>();
            test.Add(1);
            test.Add(2);
            test.Add(3);
            pManager.AddIntervalParameter("待划分区间", "I", "待划分区间", GH_ParamAccess.item,new Interval(0,1));
            pManager.AddNumberParameter("数字列表", "L", "区间将根据列表中的数占列表所有数总和的比例划分区间", GH_ParamAccess.list,test);
            Message = "列表划分区间";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntervalParameter("划分后的区间", "I", "划分后的区间", GH_ParamAccess.list);     
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Interval singleInterval = new Interval();
            List<double> listNumbers = new List<double>();
            if(!DA.GetData(0,ref singleInterval)) return;
            if(!DA.GetDataList(1,listNumbers)) return;
            double t1 = singleInterval.T0;
            double t2 = singleInterval.T1;
            double t = t2 - t1;
            double total = listNumbers.Sum();
            List<double> mems = new List<double>();
            for (int i = 0; i < listNumbers.Count; i++)
            {
                mems.Add(listNumbers[i] / total * t);
            }
            List<Interval> interval = new List<Interval>();
            for (int i = 0; i < listNumbers.Count; i++)
            {
                Interval v = new Interval(t1, t1 + mems[i]);
                t1 += mems[i];
                interval.Add(v);
            }
            DA.SetDataList(0, interval);
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
                //return Resource1.v23;
                return Resource1.math_列表转换区间;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{ada67d67-5526-4f03-bad8-2ad4cff4014d}"); }
        }
    }
}