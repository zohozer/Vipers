using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers//2014.12.14 TangChi
{
    public class ViperPrime1 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public ViperPrime1()
            : base("求出第N个质数是多少", "PrimeN",
                "求出第N个质数是多少",
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
            pManager.AddIntegerParameter("第N个质数","N","求第N个质数是多少",GH_ParamAccess.item,1);
            Message = "第N个质数的值";

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("质数的值","P","所得结果",GH_ParamAccess.item);
            pManager.AddTextParameter("中文表达","E","中文阐述",GH_ParamAccess.item);
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
            if (Number < 1)
            {
            }
            else if (Number >= 1)
            {
                int m = 1;
                int b = 2;//被除数
                int c = 1;//从第一个数开始
                while (m <= Number)
                {
                    c++;
                    b = 2;
                    while (c % b != 0 && b < c)
                    {
                        b++;
                    }
                    if (c == b)
                    {
                        m++;
                    }
                    else if (c != b)
                    {
                    }
                }
                string str = string.Format("第{0}个质数是{1}", Number, c);
                DA.SetData(0,c);
                DA.SetData(1,str);
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
            get { return new Guid("{7f29a3b2-6c7d-41e9-a703-2b4c2068086c}"); }
        }
    }
}