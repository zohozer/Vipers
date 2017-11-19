using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers////TangChi 2016.01.05
{
    public class NumbersContinuity : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the NumbersContinuity class.
        /// </summary>
        public NumbersContinuity()
            : base("检测数据连续性", "NumbersContinuity",
                "判断一组数是否连续（即判断该组数是否为等差数列），如果不是，输出不连续位置的索引值",
               "Vipers", "Viper.math")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            double[] test = {5,6,7,8,10,12,14,20,26,32,38};
            pManager.AddNumberParameter("列表", "L", "数字列表", GH_ParamAccess.list,test);
            Message = "检测数据连续性";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("判断结果","R","判断输入的数据是否是连续的（即等差数列）",GH_ParamAccess.item);
            pManager.AddIntegerParameter("索引","I","不连续位置的序号",GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<double> list = new List<double>();
            if(!DA.GetDataList(0,list))return;
            ////////////////////////////////////////////
            double gongcha = list[1] - list[0];////起始公差
            List<int> index = new List<int>();////不连续位置的序号
            bool flag = true;
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (list[i + 1] - list[i] != gongcha)
                {
                    index.Add(i + 1);
                    gongcha = list[i + 1] - list[i];
                    flag = false;
                }
            }
            DA.SetData(0, flag);
            DA.SetDataList(1, index);
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
                return Resource1.math_检测数据连续性;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{cf93d3b3-205f-43f3-b8ce-5fef24aa81c1}"); }
        }
    }
}