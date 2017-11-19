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

namespace Vipers//TangChi 2015.2.4
{
    public class ReverseTree : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ReverseTree class.
        /// </summary>
        public ReverseTree()
            : base("移动树形数据顺序", "MoveTree",
                "将树形数据的每个列表视为一个单元，按用户输入的数量整体移动（正数为从上往下移动，反之相反）",
               "Vipers", "Viper.data")
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
            pManager.AddGenericParameter("树形数据", "T", "待移动的树形数据", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("移动步数", "S", "将树形数据的每个列表视为一个单元，整体移动（正数为从上往下移动，反之相反）", GH_ParamAccess.item, 1);
            pManager.HideParameter(0);
            Message = "移动树形数据";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("生成结果", "T", "移动后的树形数据", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Structure<IGH_Goo> objects = new GH_Structure<IGH_Goo>();
            int y = 0;
            if (!DA.GetDataTree(0, out objects)) return;
            if (!DA.GetData(1, ref y)) return;
            //////////////////////////////////////////////////////////
            DataTree<object> last = new DataTree<object>();
            int num = objects.PathCount;///树的数量
            List<int> mems1 = new List<int>();
            List<int> mems2 = new List<int>();
            for (int i = 0; i < num; i++)//原索引
            {
                mems1.Add(i);
                mems2.Add(i);
            }
            if (y < 0)////从下往上
            {
                y = Math.Abs(y);
                y = y % num;
                mems1.RemoveRange(y, num - y);
                mems2.RemoveRange(0, y);
            }
            else///从上往下
            {
                y = y % num;
                mems1.RemoveRange(num - y, y);
                mems2.RemoveRange(0, num - y);
            }
            List<int> mem = new List<int>();
            mem.AddRange(mems2);
            mem.AddRange(mems1);
            for (int i = 0; i < num; i++)
            {
                List<object> ob = new List<object>(objects.Branches[mem[i]]);
                last.AddRange(ob, objects.Paths[i]);
            }
            DA.SetDataTree(0,last);
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
                return Resource1.data_移动树形数据;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{1da2ae95-7934-4f59-b611-eb6896588cdc}"); }
        }
    }
}
