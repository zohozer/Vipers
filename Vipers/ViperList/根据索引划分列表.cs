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

namespace Vipers///////TangChi 2015.8.15
{
    public class SplitList : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent37 class.
        /// </summary>
        public SplitList()
            : base("根据索引划分列表", "SplitList",
                "根据列表中的索引值，在列表的索引位置划分列表",
                "Vipers", "Viper.data")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("列表","L","待划分的数据列表",GH_ParamAccess.list);
            pManager.AddIntegerParameter("索引", "I", "索引列表，列表中的值将根据索引列表的值划分", GH_ParamAccess.list);
            pManager.AddBooleanParameter("索引位置","C","是否将索引位置向下移动一个单位",GH_ParamAccess.item,false);
            pManager.AddBooleanParameter("去掉尾项","E","是否去掉划分后的数据段最后一组",GH_ParamAccess.item, false);
            pManager.HideParameter(0);
            Message = "根据索引划分列表";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("新列表","T","划分后的新列表（树形数据）",GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            List<System.Object> list=new List<object>();
            List<int> indexs=new List<int>();
            bool addOne=false;
            bool cutEnd=false;
            if(!DA.GetDataList(0,list)) return;
            if (!DA.GetDataList(1, indexs)) return;
            if (!DA.GetData(2, ref addOne)) return;
            if (!DA.GetData(3, ref cutEnd)) return;
            List<System.Object> l = list;
            List<int> i = indexs;
            bool b = addOne;
            bool b2 = cutEnd;
            i.Sort();
            List<int> i2 = i.Distinct().ToList();
            if (b == true)
            {
                for (int f = 0; f < i2.Count; f++)
                {
                    i2[f]++;
                }
            }
            DataTree<Object> ob = new DataTree<Object>();
            int index = 0;
            int index2 = 0;
            for (int q = 0; q <= i2.Count; q++)
            {
                if (q == i2.Count)////对最后所有的分在一起
                {
                    if (b2 == true)
                    {
                        break;
                    }
                    for (int j = index2; j < l.Count; j++)
                    {
                        ob.Add(l[j], new GH_Path(0,DA.Iteration,index));
                    }
                    break;
                }
                for (int k = index2; k < i2[q]; k++)
                {
                    ob.Add(l[k], new GH_Path(0,DA.Iteration,index));
                    if (k == i2[q] - 1)
                    {
                        index2 = k + 1;
                    }
                }
                index++;
            }
            DA.SetDataTree(0, ob);
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
                //return Resource1.分割列表;
                return Resource1.data_根据索引划分列表;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{3f16ce4b-7066-4a82-8a0d-0d6bce053237}"); }
        }
    }
}