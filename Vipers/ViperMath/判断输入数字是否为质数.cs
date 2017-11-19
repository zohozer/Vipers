using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers//2014.12.14 TangChi
{
    public class ViperPrime3 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public ViperPrime3()
            : base("判定输入数字是否为质数", "PrimeOrNot",
                "判断给定数字是否为质数，如果不是，则求出其因数",
                "Vipers", "Viper.math")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("输入正整数", "N", "用以判断是否为质数", GH_ParamAccess.item, 2);
            Message = "判断数字是否为质数";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("判断结果", "R", "判断结果", GH_ParamAccess.item);
            pManager.AddIntegerParameter("因数", "F", "如果不是质数则返回其因数", GH_ParamAccess.list);
            pManager.AddTextParameter("中文表达", "E", "中文阐述", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////声明变量
            int Number = 0;
            ///////////////////////////////////////////////////////////////////////////////////////////////////检测输入端是否合理
            if (!DA.GetData(0, ref Number)) return;
            List<int> ints = new List<int>();
            int b = 2;
            if (Number <= 1)
            {
                string str = string.Format("输入的数字应为大于2的正整数");
                DA.SetData(2,str);
            }
            else
            {
                while (Number % b != 0 && b < Number)
                {

                    b++;

                }
                if (b == Number)
                {
                    string str = string.Format("您输入的数字是质数");
                    DA.SetData(2,str);
                    DA.SetData(0, true);
                    ints.Add(1);
                    ints.Add(Number);
                    DA.SetDataList(1,ints);

                }
                else if (b != Number)
                {
                    for (b = 1; b <= Number; b++)
                    {
                        if (Number % b == 0)
                        {

                            ints.Add(b);
                        }

                    }
                    DA.SetDataList(1, ints);//A为最小因数
                    string str = string.Format("您输入的数字是合数");
                    DA.SetData(2, str);
                    DA.SetData(0, false );
                }
            }
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
                return Resource1.math_质数;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{0990eb85-31d2-47a5-ba67-c642e44fbc9c}"); }
        }
    }
}