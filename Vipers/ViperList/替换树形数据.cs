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

namespace Vipers//////TangChi 2015.3.17
{
    public class ReplaceTree : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent2 class.
        /// </summary>
        public ReplaceTree()
            : base("替换树形数据", "ReplaceTree",
                "Description",
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
            pManager.AddGenericParameter("源树形数据", "T1", "该树形数据将被子树形数据替换", GH_ParamAccess.tree);
            pManager.AddGenericParameter("子树形数据", "T2", "该树形数据将插入源树形数据替换", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("索引值", "I", "子树形数据的每个列表将按索引值分别替换源树形数据", GH_ParamAccess.list);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
            Message = "替换树形数据";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("源树形数据", "T1", "插入后的树形数据插入", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //////origin_Tree:原始数据（树形数据）
            //////insert_Tree:插入数据
            //////index：插入的位置（在原始数据中的位置）
            GH_Structure<IGH_Goo> dad = new GH_Structure<IGH_Goo>();
            GH_Structure<IGH_Goo> son = new GH_Structure<IGH_Goo>();
            List<int> index = new List<int>();
            if (!DA.GetDataTree(0, out dad)) return;
            if (!DA.GetDataTree(1, out son)) return;
            if (!DA.GetDataList(2, index)) return;
            ///////////////////////////////////////////////////////////////
            List<GH_Structure<IGH_Goo>> trees = new List<GH_Structure<IGH_Goo>>();
            //////////////////////////////////////////////////////////////
            trees.Add(dad);
            for (int k = 0; k < index.Count; k++)//遍历列表中的索引
            {
                int mem = -1;
                GH_Structure<IGH_Goo> last = new GH_Structure<IGH_Goo>();
                for (int i = 0; i < trees[trees.Count - 1].PathCount; i++)
                {
                    mem++;
                    //last.Clear();
                    if (index[k] == i && son.PathCount > k)
                    {
                        last.AppendRange(son.Branches[k], new GH_Path(0, mem));
                        index[k] = int.MaxValue;
                        continue;
                    }
                    last.AppendRange(trees[trees.Count - 1].Branches[i], new GH_Path(0, mem));
                }
                trees.Add(last);
            }
            DA.SetDataTree(0, trees[trees.Count - 1]);
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
                return Resource1.data_替换树形数据;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{8eb61d56-ac16-4738-9397-07ed6e7452ec}"); }
        }
    }
}