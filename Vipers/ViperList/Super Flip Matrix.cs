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

namespace Vipers///////TangChi 2014.2.8
{
    public class SuperFlipMatrix : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SuperFlipMatrix class.
        /// </summary>
        public SuperFlipMatrix()
            : base("SuperFlipMatrix", "SuperFlipMatrix",
                "以牺牲路径为代价，强制Filp矩阵（GH自带的FlipMatrix运算对复杂路径的树形数据无法进行操作）",
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
            pManager.AddGenericParameter("树形数据", "T", "Filp前的树形数据", GH_ParamAccess.tree);
            pManager.HideParameter(0);
            Message = "SuperFlipMatrix";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("生成结果", "T", "Filp后的树形数据", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Structure<IGH_Goo> objects = new GH_Structure<IGH_Goo>();
            if (!DA.GetDataTree(0, out objects)) return;
            /////////////////////////////////////////////////////////////////////////////
            DataTree<object> newobj = new DataTree<object>();
            int num = objects.PathCount;
            List<int> index = new List<int>();//////每个分支的长度
            for (int i = 0; i < num; i++)
            {
                index.Add(objects.Branches[i].Count);
            }
            index.Sort();/////排序长度，找出最大值
            int maxnum = index[index.Count - 1];/////找出数据最长分支的长度
            for (int i = 0; i < maxnum; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    List<object> obb = new List<object>(objects.Branches[j]);
                    int num2 = obb.Count;
                    if (num2 >= i + 1)/////////////////////如果分支的数据长度大于序号，则进入
                    {
                        newobj.Add(obb[i], new GH_Path(0, i));
                    }
                }
            }
            DA.SetDataTree(0, newobj);
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
                return Resource1.data_filpmatrix;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{a4657ad4-3eb0-490a-b365-15e1207aa664}"); }
        }
    }
}