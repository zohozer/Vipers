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

namespace Vipers/////// TangChi 2015.3.10（2016.8.15 改）
{
    public class ViperCutCurve : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public ViperCutCurve()
            : base("相交位置打断曲线", "IntersectionBreak",
                "通过曲线相交位置打断曲线",
                "Vipers", "Viper.curve")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("一组相交曲线", "C", "一组有交点的曲线或线段", GH_ParamAccess.list);
            pManager.AddNumberParameter("公差1","T1","公差范围内两曲线创建交点",GH_ParamAccess.item,0);
            pManager.AddNumberParameter("公差2", "T2", "The tolerance with which the curves are tested（我也不知道啥意思）", GH_ParamAccess.item, 0);
            pManager.HideParameter(0);
            Message = "相交位置打断曲线";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("打断后的曲线", "C", "在交点位置打断的曲线", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////声明变量
            List<Curve> c = new List<Curve>();
            double t1 = 0;
            double t2 = 0;
            ///////////////////////////////////////////////////////////////////////////////////////////////////检测输入端是否合理
            if (!DA.GetDataList(0, c)) return;
            if (!DA.GetData(1,ref t1)) return;
            if (!DA.GetData(2,ref t2)) return;
            DataTree<Curve> last = new DataTree<Curve>();
            DataTree<Point3d> collects = ViperClass.EmptyTree(c.Count);
            int index = 0;
            int branch = DA.Iteration;
            for (int i = 0; i < c.Count; i++)
            {
                for (int q = i + 1; q < c.Count; q++)
                {
                    Rhino.Geometry.Intersect.CurveIntersections result = Rhino.Geometry.Intersect.Intersection.CurveCurve(c[i], c[q], t1, t2);
                    if (result.Count > 0)////有交集
                    {
                        for (int k = 0; k < result.Count; k++)////记录每次相交的t值并分别赋值给两曲线
                        {
                            collects.Branch(i).Add((result[k].PointA));
                            collects.Branch(q).Add((result[k].PointB));
                        }
                    }
                }
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////
            for (int i = 0; i < c.Count; i++)
            {
                if (collects.Branch(i).Count == 0)
                {
                    last.Add(c[i], new GH_Path(0,branch,index));
                    index++;
                    continue;
                }
                last.AddRange(ViperClass.SplitByPts(c[i], collects.Branch(i).ToList()), new GH_Path(0, branch, index));
                index++;
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
                //return Resource1.打断曲线;
                return Resource1.curve_相交位置打断曲线;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{89cf2108-9d77-427f-b758-24cf7b576bb7}"); }
        }
    }
}