using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers
{
    public class ListComparison : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the 比较两组数据 class.
        /// </summary>
        public ListComparison()
            : base("比较两组数据", "ListComparison",
                "比较两组数据是否相等，true：判断是否完全相等，即列表的值一一对应；false：只判断两组的值是否相等，顺序可以打乱",
               "Vipers", "Viper.data")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("列表","L1","待与列表2比较的数字列表或点列表",GH_ParamAccess.list);
            pManager.AddNumberParameter("列表", "L2","待与列表1比较的数字列表或点列表", GH_ParamAccess.list);
            pManager.AddBooleanParameter("比较模式","B","true：判断是否完全相等，即列表的值一一对应；false：只判断两组的值是否相等，顺序可以打乱",GH_ParamAccess.item,true);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
            Message = "比较两组数据是否全等";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("结果","R","判断结果",GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<double> list1 = new List<double>();
            List<double> list2 = new List<double>();
            bool b = true;
            if(!DA.GetDataList(0,list1))return;
            if (!DA.GetDataList(1, list2)) return;
            if(!DA.GetData(2,ref b))return;
            bool last = true;
            if (list1.Count != list2.Count)
            {
                DA.SetData(0, false);
                return;
            }
            if (b)////完全相同
            {
                for (int i = 0; i < list1.Count; i++)
                {
                    if (list1[i] != list2[i])
                    {
                        DA.SetData(0, false);
                        return;
                    }
                }
            }
            else
            {
                list1.Sort();
                list2.Sort();
                for (int i = 0; i < list1.Count; i++)
                {
                    if (list1[i] != list2[i])
                    {
                        DA.SetData(0, false);
                        return;
                    }
                }
            }
            DA.SetData(0, last);
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
                return Resource1.data_比较两组列表;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{73ced2c5-9a43-466f-b510-a617b53b9902}"); }
        }
    }
}