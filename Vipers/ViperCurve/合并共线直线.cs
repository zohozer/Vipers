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

namespace Vipers ////TangChi 2015.5.28（改 2016.4.12）（改2017.10.16）
{
    public class ViperConnectLines : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public ViperConnectLines()
            : base("共线直线", "MergeCollineation",
                "找出所给直线列表中的共线直线并将其合并为一根直线",
                "Vipers", "Viper.curve")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddLineParameter("直线", "L", "待合并的直线", GH_ParamAccess.list);
            pManager.HideParameter(0);
            Message = "合并共线直线";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("直线", "L", "合并后的直线", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Line> lines = new List<Line>();
            if(!DA.GetDataList(0,lines))return;
            List<Line> last = new List<Line>();
            while (lines.Count > 0)
            {
                Line l1 = lines[0];
                lines.RemoveAt(0);
                bool flag = true;
                while (flag)
                {
                    flag = false;
                    for (int q = 0; q < lines.Count; q++)
                    {
                        Line l2 = lines[q];
                        Vector3d vc1 = Point3d.Subtract(l1.From, l1.To);
                        Vector3d vc2 = Point3d.Subtract(l2.From, l2.To);
                        if (Vector3d.VectorAngle(vc1, vc2) == Math.PI || Vector3d.VectorAngle(vc1, vc2) == 0)
                        {
                            if (OverlapLine(l1, l2))
                            {
                                l1 = UnionLine(l1, l2);
                                flag = true;
                                lines.RemoveAt(q);
                                q--;
                            }
                        }
                    }
                }
                last.Add(l1);
            }
           DA.SetDataList(0, last);
        }
        public static Line UnionLine(Line line1, Line line2) ///////合并两条线段
        {
            Point3d pa1 = line1.From;
            Point3d pa2 = line1.To;
            Point3d pb1 = line2.From;
            Point3d pb2 = line2.To;
            double l1 = line1.Length;
            double l2 = line2.Length;
            double l3 = pa1.DistanceTo(pb1);
            double l4 = pa1.DistanceTo(pb2);
            double l5 = pa2.DistanceTo(pb1);
            double l6 = pa2.DistanceTo(pb2);
            double[] values = new double[] { l1, l2, l3, l4, l5, l6 };
            double max = values.ToList().Max();
            Line last = new Line();
            if (max == l1)
                last = line1;
            else if (max == l2)
                last = line2;
            else if (max == l3)
                last = new Line(pa1, pb1);
            else if (max == l4)
                last = new Line(pa1, pb2);
            else if (max == l5)
                last = new Line(pa2, pb1);
            else
                last = new Line(pa2, pb2);
            return last;
        }
        public static bool OverlapLine(Line l1, Line l2) ////////判断两条平行的线段是否有相交点
        {
            Point3d pa1 = l1.From;
            Point3d pa2 = l1.To;
            Point3d pb1 = l2.From;
            Point3d pb2 = l2.To;
            Point3d test1 = l1.ClosestPoint(pb1, true);
            Point3d test2 = l1.ClosestPoint(pb2, true);
            Point3d test3 = l2.ClosestPoint(pa1, true);
            Point3d test4 = l2.ClosestPoint(pa2, true);
            if (test1.DistanceTo(pb1) <= 0.0001)
                return true;
            else if (test2.DistanceTo(pb2) <= 0.0001)
                return true;
            else if (test3.DistanceTo(pa1) <= 0.0001)
                return true;
            else if (test4.DistanceTo(pa2) <= 0.0001)
                return true;
            else
                return false;
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
                //return Resource1.v11;
                return Resource1.curve_共线线段合并;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{908e20e2-6894-4a5f-b428-bc40d85e2896}"); }
        }
    }
}