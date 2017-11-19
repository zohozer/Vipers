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
    public class JoinLines : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the 自动闭合线段 class.
        /// </summary>
        public JoinLines()
            : base("闭合直线", "JoinLines",
                "通过设置布尔值选择闭合直线的方式，将指定的直线合并成多段线，注意，在“自动排序”模式下，只能生成凸多边形",
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
            pManager.AddLineParameter("直线","L","用于合并的一组直线",GH_ParamAccess.list);
            pManager.AddBooleanParameter("自动排序", "O", "true，自动将该组直线排序，然后生成多段线，该种情况下只能生成凸多边形(如果是空间线段无法自动排序)\nfalse，按照输入直线的本来顺序依次合并成多段线", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("闭合","C","是否生成闭合的多段线",GH_ParamAccess.item,false);
            pManager.HideParameter(0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("多段线","P","根据该组直线生成的多段线",GH_ParamAccess.item);
            Message = "默认顺序" + "+" +"不闭合";
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Line> x = new List<Line>();
            bool closed = false;
            bool order = false;
            if(!DA.GetDataList(0,x))return;
            if(!DA.GetData(2,ref closed))return;
            if (!DA.GetData(1, ref order)) return;
            string say1 = "默认顺序";
            string say2 = "不闭合";
            CPMethon dele;
            if (order)
            {
                say1 = "自动排序";
                dele = OrderClosetPly;
            }
            else
                dele = ClosetPly;
            if (closed)
                say2 = "闭合";
            Message = say1 + "+" + say2;
            DA.SetData(0, dele(x, closed));
        }
        delegate Polyline CPMethon(List<Line> lines, bool way);
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Polyline ClosetPly(List<Line> C, bool B) //////////////将线段组闭合成多段线
        {
            List<Point3d> last = new List<Point3d>();
            for (int i = 0; i < C.Count - 1; i++)
            {
                Point3d pt1;
                Point3d pt2;
                if (i == 0)
                {
                    ViperClass.ClosedPt(C[0], C[C.Count - 1], out pt1, out pt2);
                    last.Add((pt1 + pt2) / 2);
                }
                ViperClass.ClosedPt(C[i], C[i + 1], out pt1, out pt2);
                last.Add((pt1 + pt2) / 2);
            }
            last.Add(last[0]);

            if (B == false)//////不闭合的情况
            {
                Vector3d vc = Point3d.Subtract(C[C.Count - 1].From, C[C.Count - 1].To);//////该方向用于判断最后那个点的位置
                last[0] = C[0].From;
                if (last[1].DistanceTo(C[0].From) < last[1].DistanceTo(C[0].To))////最远
                {
                    last[0] = C[0].To;
                }
                last[last.Count - 1] = C[C.Count - 1].From;
                if (last[last.Count - 2].DistanceTo(C[C.Count - 1].From) < last[last.Count - 2].DistanceTo(C[C.Count - 1].To))///最远
                {
                    last[last.Count - 1] = C[C.Count - 1].To;
                }
            }
            return new Polyline(last);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public Polyline OrderClosetPly(List<Line> x, bool B) ////////////////////自动排序线段
        {
            List<GH_Point> gpt = new List<GH_Point>();
            for (int i = 0; i < x.Count; i++)
            {
                Point3d pt = x[i].From / 2 + x[i].To / 2;
                gpt.Add(new GH_Point(pt));
            }
            Polyline ply = Grasshopper.Kernel.Geometry.ConvexHull.Solver.ComputeHull(gpt);///得到外轮廓
            List<Line> order = new List<Line>();///通过外轮廓排序线段
            Line[] lines = ply.GetSegments();
            for (int i = 0; i < lines.Length; i++)
            {
                for (int k = 0; k < x.Count; k++)
                {
                    if (x[k] == Line.Unset) continue;
                    if ((x[k].From / 2 + x[k].To / 2) == lines[i].To)
                    {
                        order.Add(x[k]);
                        x[k] = Line.Unset;
                        break;
                    }
                }
            }
            return ClosetPly(order, B);
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
                return Resource1.curve线段最近点闭合;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{011ace69-b0f5-49b0-95ad-a15e03814828}"); }
        }
    }
}