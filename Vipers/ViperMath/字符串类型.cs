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

namespace Vipers//////////TangChi 2016.4.7
{
    public class CharacterTypes : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the 字符串类型 class.
        /// </summary>
        public CharacterTypes()
            : base("字符成分", "CharacterComponent",
                "根据字符的组成成分给予字符特定身份。如字符完全由“数字”组成的，身份为“1”；“字母”为“2”；“文字”为“3”；“符号”为“4”。混合类型则根据组成如“AK - 47”身份为“241”（字母+符号+数字）",
                "Vipers", "Viper.math")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("字符","S","待检测的字符",GH_ParamAccess.item);
            Message = "字符成分";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("构成","F","中文阐述输入字符串的构成成分",GH_ParamAccess.item);
            pManager.AddTextParameter("身份", "D", "根据字符的组成成分给予特定身份", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string x = null;
            if(!DA.GetData(0,ref x))return;
            string fuhao = @"[~!@#\$%\^&\*\(\)\+=\|\\\}\]\{\[:;<,>\?\/""~！￥……（）？，。、《》；：”“—_-.]";
            List<int> lastK = new List<int>();
            List<string> lastV = new List<string>();
            int index = 0;
            if (System.Text.RegularExpressions.Regex.Match(x, @"[-+]?\d+([.]\d+)?").Success)////数字
            {
                index = System.Text.RegularExpressions.Regex.Matches(x, @"[-+]?\d+([.]\d+)?")[0].Index;
                x = System.Text.RegularExpressions.Regex.Replace(x, @"[-+]?\d+([.]\d+)?", " ");
                lastK.Add(index);
                lastV.Add("数字");
            }
            if (System.Text.RegularExpressions.Regex.Match(x, @"[a-zA-Z]").Success)////字母
            {
                index = System.Text.RegularExpressions.Regex.Matches(x, @"[a-zA-Z]")[0].Index;
                lastK.Add(index);
                lastV.Add("字母");
            }
            if (System.Text.RegularExpressions.Regex.Match(x, "[\u4e00-\u9fa5]").Success)////汉字
            {
                index = System.Text.RegularExpressions.Regex.Matches(x, "[\u4e00-\u9fa5]")[0].Index;
                lastK.Add(index);
                lastV.Add("汉字");
            }
            if (System.Text.RegularExpressions.Regex.Match(x, fuhao).Success)///符号
            {
                index = System.Text.RegularExpressions.Regex.Matches(x, fuhao)[0].Index;
                lastK.Add(index);
                lastV.Add("符号");
            }
            //////////////////////////////////////////////////////
            List<int> lastK2 = lastK.ToArray().ToList();
            lastK.Sort();
            string last = null;/////字符身份
            string id = null;///数字身份
            for (int i = 0; i < lastK.Count; i++)
            {
                for (int q = 0; q < lastK2.Count; q++)
                {
                    if (lastK2[q] == lastK[i])
                    {
                        last += lastV[q] + "+";
                        switch (lastV[q])
                        {
                            case "数字":
                                id += "1";
                                break;
                            case "字母":
                                id += "2";
                                break;
                            case "汉字":
                                id += "3";
                                break;
                            case "符号":
                                id += "4";
                                break;
                        }
                        break;
                    }
                }
            }
            last = last.Remove(last.Length - 1, 1);
            ///////////////////////////////////////////////////////字符操作
            DA.SetData(0,  last);
            DA.SetData(1, id);
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
                return Resource1.math_字符串身份;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{5fdf7b5b-3951-4ce3-9f27-1180f98a7c91}"); }
        }
    }
}