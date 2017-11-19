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
namespace Vipers//////TangChi 2015.9.7
{
    public class DomainList : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent40 class.
        /// </summary>
        public DomainList()
            : base("通过区间得到列表", "DomainList",
                "通过给定的区间，找出列表中的数值，同时返回未被找出的数值",
                "Vipers", "Viper.data")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("列表","L","待查找的列表",GH_ParamAccess.list);
            pManager.AddIntervalParameter("区间","D","指定的查找的范围",GH_ParamAccess.list);
            pManager.HideParameter(0);
            Message = "通过区间得到列表";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("列表","L","通过区间找出的对应的列表值",GH_ParamAccess.tree);
            pManager.AddGenericParameter("列表", "L", "未被找出的列表值", GH_ParamAccess.list);
            pManager.HideParameter(0);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            List<object> list=new List<object>();
            List<Interval> domain=new List<Interval>();
            if(!DA.GetDataList(0,list))return;
            if (!DA.GetDataList(1,domain)) return;
            List<object> x = list;
            List<Interval> y = domain;
            DataTree<object> objs = new DataTree<object>();
            List<object> rest = new List<object>();
            List<int> used = new List<int>();
            int index = 0;
            for (int i = 0; i < y.Count; i++)
            {
                bool flag = false;
                Interval inter = y[i];
                if (inter.IsIncreasing == false)
                {
                    flag = true;
                }
                inter.MakeIncreasing();
                int mem1 = Convert.ToInt32(inter.T0);
                int mem2 = Convert.ToInt32(inter.T1);
                List<object> obj = new List<object>();
                for (int j = mem1; j <= mem2; j++)
                {
                    obj.Add(x[j]);
                    used.Add(j);
                }
                if (flag)////如果给定区间是降序排列，那么反转列表
                {
                    obj.Reverse();
                }
                objs.AddRange(obj, new GH_Path(0, DA.Iteration,index));
                index++;
            }
            List<int> used2 = used.Distinct().ToList();
            used2.Sort();
            for (int i = 0; i < x.Count; i++)
            {
                if (used2.Contains(i))
                {
                    continue;
                }
                rest.Add(x[i]);
            }
            DA.SetDataTree(0, objs);
            DA.SetDataList(1, rest);
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
                //return Resource1.通过区间得到列表;
                return Resource1.data_区间得到列表;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{72f1963a-a1a7-41bc-a943-1de8ff5eab58}"); }
        }
    }
}