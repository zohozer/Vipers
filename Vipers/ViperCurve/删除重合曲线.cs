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

namespace Vipers ///////TangChi 2015.3.13(2015.9.22修改)(20151224修改)(TangChi 2016.3.25修改)
{
    public class ViperDeleteOverlappingCurve : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public ViperDeleteOverlappingCurve()
            : base("删除重合曲线", "RemoveOverlapCurve",
                "可删除重合的线段或曲线(调节公差,可将相似重合的曲线删除)",
                "Vipers", "Viper.curve")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("重合的曲线","C","待删除的重合曲线",GH_ParamAccess.list);
            pManager.AddNumberParameter("公差", "T", "在此范围内的曲线视为重合曲线", GH_ParamAccess.item,0);
            pManager.HideParameter(0);
            Message = "删除重合曲线";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("删除后的曲线","C","删除后的曲线",GH_ParamAccess.list);
            pManager.AddIntegerParameter("序号", "I1", "保留的曲线在原曲线中的序号", GH_ParamAccess.list);
            pManager.AddIntegerParameter("序号","I2","被删除的曲线在原曲线中的序号",GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////声明变量
            List<Curve> curves = new List<Curve>();
            double t = 0;
            ///////////////////////////////////////////////////////////////////////////////////////////////////检测输入端是否合理
            if (!DA.GetDataList(0, curves)) return;
            if (!DA.GetData(1, ref t)) return;
            
            List<Curve> last = new List<Curve>();//////最终保留的曲线
            List<int> index1 = new List<int>();//////最终保留的曲线的索引
            List<int> index2 = new List<int>();//////最终删除的曲线的索引
            Interval test = new Interval(0, 1);////用于检测是否重合
            for (int i = 0; i < curves.Count; i++)
            {
                if (curves[i] == null)
                {
                    continue;
                }
                curves[i].Domain = new Interval(0, 1);//////将曲线区间统一为0~1
                for (int q = 0; q < curves.Count; q++)
                {
                    if (curves[q] == null || i == q)
                    {
                        continue;
                    }
                    curves[q].Domain = new Interval(0, 1);
                    if (Rhino.Geometry.Intersect.Intersection.CurveCurve(curves[i], curves[q], t, t).Count == 1)////重合或只有一个交点的情况
                    {
                        Interval aa = Rhino.Geometry.Intersect.Intersection.CurveCurve(curves[i], curves[q], t, t)[0].OverlapA;
                        Interval bb = Rhino.Geometry.Intersect.Intersection.CurveCurve(curves[i], curves[q], t, t)[0].OverlapB;
                        aa.MakeIncreasing();
                        bb.MakeIncreasing();
                        if (test == aa)////如果完全重合则区间为（0~1）
                        {
                            curves[i] = null;
                            break;
                        }
                        else if (test == bb)
                        {
                            curves[q] = null;
                        }
                    }
                }
            }
            for (int i = 0; i < curves.Count; i++)
            {
                if (curves[i] == null)
                {
                    index2.Add(i);
                }
                else
                {
                    index1.Add(i);
                    last.Add(curves[i]);
                }
            }
           DA.SetDataList(0, last);
            DA.SetDataList(1, index1);
            DA.SetDataList(2, index2);
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
                //return Resource1.删除重合曲线_保留一个_;
                return Resource1.curve_删除重合曲线;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{2f26a31e-0ac6-42ad-88f0-89ea29aadd5f}"); }
        }
    }
}