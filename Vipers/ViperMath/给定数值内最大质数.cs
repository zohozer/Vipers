using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers//2014.12.14 TangChi
{
    public class ViperPrime2 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public ViperPrime2()
            : base("给定数字范围内最大质数", "PrimeMax",
                "找出输入数字内最大的质数",
                "Vipers", "Viper.math")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary | GH_Exposure.obscure; }
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("给定上限", "N", "该数值以内最大质数", GH_ParamAccess.item, 2);
            Message = "给定数值内最大质数";

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("质数的值", "P", "所得结果", GH_ParamAccess.item);
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
            int b = 2;
            int a = Number;
            bool c = true;
            if (Number <= 1)
            {
                string str = string.Format("您输入的数据不符合要求，请输入大于1的正整数");
                DA.SetData(1, str);
            }
            else
            {
                while (c == true)
                {
                    b = 2;
                    while (Number % b != 0 && b < Number)
                    {
                        b++;
                    }

                    if (b == Number)
                    {
                        c = false;
                    }
                    else if (b != Number)
                    {
                        Number--;
                        c = true;
                    }
                }
                string str = string.Format("在{0}以内最大的质数为{1}", a, Number);
                DA.SetData(1, str);
                DA.SetData(0, Number);
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
            get { return new Guid("{2dc5da0b-c7e6-4456-a5cd-3e11151cbe14}"); }
        }
    }
}