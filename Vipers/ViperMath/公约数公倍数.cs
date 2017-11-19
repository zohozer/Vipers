using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers
{
    public class CommonDivisorMultiple : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent11 class.
        /// </summary>
        public CommonDivisorMultiple()
            : base("求列表数中公约数与最小公倍数", "CommonDivisorMultiple",
                "给定一组数字，找出他们的公约数及最小公倍数",
                "Vipers", "Viper.math")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("数字列表","L","找出列表数的公约数及最小公倍数",GH_ParamAccess.list);
            Message = "公约数与最小公倍数";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("公约数", "C1", "公约数", GH_ParamAccess.list);
            pManager.AddIntegerParameter("最小公倍数", "C2", "最小公倍数", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<int> numberList = new List<int>();
            if(!DA.GetDataList(0,numberList)) return;
            List<int> numbers = numberList;
            if (numbers.Count > 1)
            {
                List<double> minMems = new List<double>();
                numbers.Sort();
                for (double i = 1; i <= numbers[numbers.Count - 1]; i++)//公约数
                {
                    double flag = 0;
                    for (int j = 0; j < numbers.Count; j++)
                    {
                        if (numbers[j] % i != 0)
                        {
                            break;
                        }
                        flag++;
                    }
                    if (flag == numbers.Count)
                    {
                        minMems.Add(i);
                    }
                }
                double max = 0;
                for (int i = 0; ; i++)
                {
                    bool flag = false;
                    max += numbers[numbers.Count - 1];
                    for (int j = 0; j < numbers.Count - 1; j++)
                    {
                        if (max % numbers[j] != 0)
                        {
                            break;
                        }
                        else if (j == numbers.Count - 2)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
                DA.SetDataList(0 ,minMems);
                DA.SetData(1, max);
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
                //return Resource1.公约公倍数量;
                return Resource1.math_公约公倍;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{abb6b1f5-f07f-4d2b-a523-d195837cb0d3}"); }
        }
    }
}