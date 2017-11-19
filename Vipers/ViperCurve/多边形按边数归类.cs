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

namespace Vipers////TangChi 2015.6.11(2015.9.6改)
{
    public class ViperPolylineClass : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent2 class.
        /// </summary>
        public ViperPolylineClass()
            : base("将相等边数曲线归类", "CurveClassify",
                "默认状态下将输入的曲线按边数分类，如果输入边数（edges），则找出满足该边数的曲线",
                "Vipers", "Viper.curve")
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
            int[] start = { 0 };
            pManager.AddCurveParameter("曲线", "C", "待分类的曲线（多段线或线段与曲线组合的曲线）", GH_ParamAccess.list);
            pManager.AddIntegerParameter("边数", "E", "待分类曲线的边数", GH_ParamAccess.list,start.ToList());
            pManager.HideParameter(0);
            Message = "曲线按边数分类";
        }
        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("分类后的曲线", "C", "分类后的曲线", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("索引值", "I", "分类的曲线在原来曲线列表的位置", GH_ParamAccess.tree);
            pManager.AddTextParameter("表述","E","阐述曲线按边数分类的情况",GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> curves=new List<Curve>();
            List<int> edges=new List<int>();
            if(!DA.GetDataList(0,curves))return;
            if(!DA.GetDataList(1,edges)) return;
            List<Curve> x = curves;
            List<int> y = edges;
            DataTree<Curve> curs = new DataTree<Curve>();
            DataTree<int> indexs = new DataTree<int>();
            List<string> says = new List<string>();
            int num = 0;
            int branch = DA.Iteration;
            if (y.Count == 1 && y[0] == 0)////当输入端为0时(默认状态下)
            {
                List<int> segnums = new List<int>();///记录所有边数
                for (int i = 0; i < x.Count; i++)
                {
                    Curve cc = x[i];
                    Curve[] cs = cc.DuplicateSegments();
                    segnums.Add(cs.Length);
                }
                List<int> nums = segnums.Distinct().ToList();////删除重合的元素
                nums.Sort();
                says.Add("输入的曲线中包括:");
                for (int i = 0; i < nums.Count; i++)
                {
                    for (int j = 0; j < x.Count; j++)
                    {
                        Curve[] cs = x[j].DuplicateSegments();
                        if (cs.Length == nums[i])
                        {
                            curs.Add(x[j], new GH_Path(0,branch ,num));
                            indexs.Add(j, new GH_Path(0,branch, num));
                        }
                    }
                    string str = +curs.Branch(num).Count + "个" + nums[i] + "边形";
                    says.Add(str);
                    num++;
                }
            }

            else
            {
                for (int k = 0; k < y.Count; k++)
                {
                    for (int i = 0; i < x.Count; i++)
                    {
                        Curve cc = x[i];
                        Curve[] cs = cc.DuplicateSegments();
                        if (cs.Length == y[k])
                        {
                            curs.Add(cc, new GH_Path(0,branch ,num));
                            indexs.Add(i, new GH_Path(0,branch ,num));
                        }
                    }
                    num++;
                }
            }
            DA.SetDataTree(0, curs);
            DA.SetDataTree(1, indexs);
            DA.SetDataList(2, says);
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
               // return Resource1.v3;
                return Resource1.curve_多边形按边数归类;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{6e0e715f-6905-4740-98a5-2b6d5b47003e}"); }
        }
    }
}