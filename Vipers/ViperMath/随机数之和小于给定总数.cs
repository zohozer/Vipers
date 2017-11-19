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

namespace Vipers /////////TangChi 2015.2.10
{
    public class RandomPickOut : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent7 class.
        /// </summary>
        public RandomPickOut()
            : base("随机取出列表中的数", "RandomPickOut",
                "随机取出列表中的数，直到总和小于给定总和",
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
            pManager.AddNumberParameter("数字列表", "L", "给定一个列表的数字，将从中随机选出数字", GH_ParamAccess.list, numsb);
            pManager.AddNumberParameter("总数", "C", "随机取出的数之和小于该数", GH_ParamAccess.item,50);
            pManager.AddIntegerParameter("随机种子", "S", "切换数字改变随机列表结果", GH_ParamAccess.item, 5);
            Message = "随机列表之和小于给定总数";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("随机列表", "L", "从numberList随机选择的数组成的列表", GH_ParamAccess.list);
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
            int seed = 0;
            if (!DA.GetDataList(0, members)) return;
            if (!DA.GetData(1, ref total)) return;
            if (!DA.GetData(2, ref seed)) return;
            int nums = members.Count;
            double tot = 0;
            List<double> mems = new List<double>();
            Random rand = new Random(seed);
            if (members.Sum() > 0)
            {
                for (int i = 0; tot < total; i++)
                {
                    int index = rand.Next() % nums;
                    double mem = members[index];
                    tot += mem;
                    if (tot >= total)
                    {
                        tot -= mem;
                        break;
                    }
                    mems.Add(mem);
                }
                double tot2 = total - tot;
                if (members.Contains(tot2))
                {
                    mems.Add(tot2);
                    DA.SetDataList(0, mems);
                }
                else
                {
                    DA.SetDataList(0, mems);
                    DA.SetData(1, tot2);
                }
            }
        }
            //        Random r = new Random();
            //List<int> list = new List<int>();
            //for (int i = 0; i<20; i++)
            //{
            //    int mm = r.Next() % 20;
            //    Console.WriteLine(mm);
            //}
        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                //return Resource1.随机数列表;
                return Resource1.math_随机列表;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{9bd85529-2119-4c79-8856-2e00e32c62b7}"); }
        }
    }
}