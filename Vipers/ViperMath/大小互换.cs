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
namespace Vipers
{
    public class MaxMin : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the 大小互换 class.
        /// </summary>
        public MaxMin()
            : base("数值大小互换", "MaxMin",
                "将一组列表中的数值大小互换位置",
                "Vipers", "Viper.math")
        {
            Message = "数值大小互换";
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            double[] example={1,3,4,7,2,5,6,9,8};
            pManager.AddNumberParameter("列表","N","一组数值列表",GH_ParamAccess.list,example);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("列表","N","原列表中数值大小互换位置后的结果",GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<double> x = new List<double>();
            if(!DA.GetDataList(0,x))return;
            List<double> up = x.ToArray().ToList();
            List<double> down = x.ToArray().ToList();
            up.Sort();/////从小到大
            down.Sort();
            down.Reverse();////从大到小
            List<int> index = new List<int>();
            for (int q = 0; q < x.Count; q++)
            {
                for (int i = 0; i < up.Count; i++)
                {
                    if (x[q] == up[i])
                    {
                        index.Add(i);
                        break;
                    }
                }
            }
            List<double> last = new List<double>();
            for (int i = 0; i < index.Count; i++)
                last.Add(down[index[i]]);
            DA.SetDataList(0, last);
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
                return Resource1.math_数字大小互换;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{ee1c6a9f-6c2b-498b-a678-0f07361b02c9}"); }
        }
    }
}