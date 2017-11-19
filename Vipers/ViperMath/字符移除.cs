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

namespace Vipers
{
    public class StringRemove : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the 字符整理 class.
        /// </summary>
        public StringRemove()
            : base("字符移除", "StringRemove",
                "将字符按照用户指定方法移除",
                "Vipers", "Viper.math")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("字符","S","待删除的字符",GH_ParamAccess.item);
            pManager.AddBooleanParameter("数字", "N", "如果为true，则将字符中的所有数字去除", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("字母", "W", "如果为true，则将字符中的所有字母去除", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("空格", "E", "如果为true，则将字符中的所有空格去除", GH_ParamAccess.item, false);
            pManager.AddTextParameter ("符号", "S", "将字符中的所有指定符号去除", GH_ParamAccess.item,string.Empty);
            Message = "字符移除";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("字符","S","删除后的字符",GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string x = null;
            bool zs = false;
            bool zm = false;
            bool kg = false;
            string fh = null;
            if(!DA.GetData(0,ref x))return;
            if (!DA.GetData(1, ref zs)) return;
            if (!DA.GetData(2, ref zm)) return;
            if (!DA.GetData(3, ref kg)) return;
            if (!DA.GetData(4, ref fh)) return;
            ////////////////////////////////////////////////////
            fuhao = fh;
            if (kg)
            {
                x = System.Text.RegularExpressions.Regex.Replace(x, kongge, "");
            }
            if (zs)////remove负整数与正整数
            {
                x = System.Text.RegularExpressions.Regex.Replace(x, shuzi, "");
            }
            if (zm)
            {
                x = System.Text.RegularExpressions.Regex.Replace(x, zimu, "");
            }
            if (fh.Length>0)
            {
                string real = null;
                for (int i = 0; i < fh.Length; i++)
                {
                    if (fh[i] == '\\' || fh[i] == '[' || fh[i] == ']' || fh[i] == '"')////几种特殊符号需要转意
                    {
                        real = real + "\\" + fh[i];
                        continue;
                    }
                    real += fh[i];
                }
                fuhao = "[" + real + "]" + "+";
                x = System.Text.RegularExpressions.Regex.Replace(x, fuhao, "");
            }
            DA.SetData(0, x);
        }
        string shuzi = @"[-+]?\d+([.]\d+)?";
        string zimu = @"[a-zA-Z]+";
        string fuhao = null;
        string kongge = "[ ]+";
        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Resource1.math_字符移除;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{5a944692-1b8a-4325-881b-3abed161aab0}"); }
        }
    }
}