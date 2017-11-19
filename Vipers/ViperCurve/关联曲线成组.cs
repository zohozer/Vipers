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

namespace Vipers ///TangChi 2015.6.2(2016.4.6改)
{
    public class VipersGroupCurves : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public VipersGroupCurves()
            : base("关联曲线成组", "CurveGroup",
                "将有相交部分的曲线分组",
                "Vipers", "Viper.curve")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("曲线", "C", "一组相关联的曲线或线段", GH_ParamAccess.list);
            pManager.HideParameter(0);
            Message = "关联曲线成组";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("成组的曲线", "C", "相关联的曲线各自一组", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("索引","I","分组后的曲线在原曲线中的位置",GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////声明变量
            List<Curve> curves = new List<Curve>();
            ///////////////////////////////////////////////////////////////////////////////////////////////////检测输入端是否合理
            if (!DA.GetDataList(0, curves)) return;
            DataTree<Curve> last = new DataTree<Curve>();
            DataTree<int> index = new DataTree<int>();
            int mem = 0;
            int branch = DA.Iteration;
            int count = 0;/////如果count等于curves长度，则表示都遍历完了
            for (int i = 0; i < curves.Count; i++)
            {
                if (count == curves.Count)
                {
                    break;
                }
                if (curves[i] == null)
                {
                    continue;
                }
                Curve c1 = curves[i];
                List<Curve> cs = new List<Curve>();
                for (int k = 0; k < curves.Count; k++)
                {
                    if (curves[k] == null || k == i)
                    {
                        continue;
                    }
                    Curve c2 = curves[k];
                    if (Rhino.Geometry.Intersect.Intersection.CurveCurve(c1, c2, 0, 0).Count > 0)
                    {
                        cs.Add(c2);
                        index.Add(k, new GH_Path(0,branch ,mem));
                        curves[k] = null;
                        count++;
                    }
                }
                //////////////////////////////////////////找出与第一根曲线有连接的所有线
                //////////////////////////////////////////
                if (cs.Count == 0)
                {
                    last.Add(c1, new GH_Path(0, branch,mem));
                    index.Add(i, new GH_Path(0, branch,mem));
                    curves[i] = null;
                    mem++;
                    continue;
                }
                ////////////////////////////////////////没有有连接的曲线，返回
                ////////////////////////////////////////
                List<Curve> cs2 = new List<Curve>();
                last.AddRange(cs, new GH_Path(0, branch,mem));
                while (true)
                {
                    for (int q = 0; q < cs.Count; q++)
                    {
                        for (int j = 0; j < curves.Count; j++)
                        {
                            if (curves[j] == null)
                            {
                                continue;
                            }
                            if (Rhino.Geometry.Intersect.Intersection.CurveCurve(cs[q], curves[j], 0, 0).Count > 0)
                            {
                                cs2.Add(curves[j]);
                                index.Add(j, new GH_Path(0, branch,mem));
                                curves[j] = null;
                                count++;
                            }
                        }
                    }
                    if (cs2.Count == 0)
                    {
                        curves[i] = null;
                        mem++;
                        cs.Clear();
                        break;
                    }
                    else
                    {
                        Curve[] copy = cs2.ToArray();
                        cs = copy.ToList();
                        last.AddRange(cs2, new GH_Path(0, branch,mem));
                        cs2.Clear();
                    }
                }
                count++;////加上自身curves[i]
            }
            DA.SetDataTree(0, last);
            DA.SetDataTree(1, index);
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
                //return Resource1.v6;
                return Resource1.curve_关联曲线成组;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{35fbdbbf-b0e6-42d7-89ad-e7004a244ee2}"); }
        }
    }
}