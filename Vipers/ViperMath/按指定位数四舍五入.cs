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

namespace Vipers/////TangChi 2015.4.13(2016.12.25改)
{
    public class DigitRound : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent18 class.
        /// </summary>
        public DigitRound()
            : base("按指定位数四舍五入", "DigitRound",
                "小数点精确到指定位数",
                "Vipers", "Viper.math")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("小数", "D", "double类型的数字", GH_ParamAccess.item);
            pManager.AddIntegerParameter("位数", "N", "指定四舍五入的位数", GH_ParamAccess.item, 2);
            Message = "四舍五入+百分比+千分比";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("计算结果", "R", "将数字按指定位数四舍五入后的结果", GH_ParamAccess.item);
            pManager.AddTextParameter("百分比","%","指定位数的百分比",GH_ParamAccess.item);
            pManager.AddTextParameter("千分比", "‰", "指定位数的百分比", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double x = 0;
            int y = 0;
            if(!DA.GetData(0,ref x)) return;
            if (!DA.GetData(1, ref y)) return;
            string wuru = Round(x, y);
            /////////////百分号
            double number1 = x * 100;
            string bai = Round(number1, y);
            bai += "%";
            ///////////千分号
            double number2 = x * 1000;
            string qian = Round(number2, y);
            qian += "‰";
            DA.SetData(0,wuru);
            DA.SetData(1, bai);
            DA.SetData(2, qian);
        }
        string Round(double x, int y)
        {
            double result = Math.Round(x, y);
            if (y == 0)
                return result.ToString();
            //////////加位数
            string last = result.ToString();
            string dian = "[.]";
            if (!System.Text.RegularExpressions.Regex.Match(last, dian).Success)/////给数值加小数点
                last += ".";
            /////////从小数点开始将小数分为两部分
            String[] groups = System.Text.RegularExpressions.Regex.Split(last, "[.]");
            if (groups[1].Length < y)/////位数不满足指定位数
            {
                for (int i = 0; i < y - groups[1].Length; i++)
                {
                    last += "0";
                }
            }
            return last;
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
                //return Resource1.指定位数四舍五入;
                return Resource1.math_四舍五入百分比千分比;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{ae8bf6d4-b9c6-4202-a620-2db3580f1316}"); }
        }
    }
}