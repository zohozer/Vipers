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


namespace Vipers//////TangChi 2015.5.6（2016.9.19改）
{
    public class AngleEqual : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AngleEqual class.
        /// </summary>
        public AngleEqual()
            : base("按指定数量平分角", "AngleEqual",
                "按用户输入的数量平分A，B线段组成的角，两线段可以不相交但必须共面，否则会报错",
                "Vipers", "Viper.curve")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddLineParameter("线段A","L","夹角边A",GH_ParamAccess.item);
            pManager.AddLineParameter("线段B", "L", "夹角边B", GH_ParamAccess.item);
            pManager.AddIntegerParameter("平分数量","N","将A，B两边形成的夹角平分为N个",GH_ParamAccess.item,10);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
            Message = "按数量平分角";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("平分后的线段", "L", "平分后的线段", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Line lineA = new Line();
            Line lineB = new Line();
            int number = 0;
            if(!DA.GetData(0,ref lineA)) return;
            if (!DA.GetData(1, ref lineB)) return;
            if (!DA.GetData(2, ref number)) return;
            double t1 = 0;
            double t2 = 0;
            Rhino.Geometry.Intersect.Intersection.LineLine(lineA, lineB, out t1, out t2);
            Point3d pt1 = lineA.PointAt(t1);
            Point3d pt2 = lineB.PointAt(t2);
            if(number<=1)return;
            if (pt1.DistanceTo(pt2) > 0.0000000001)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,"指定的两条直线不共面");
                ///////////////////////////////
                return;
            }
            Point3d start1 = lineA.From;
            Point3d end1 = lineA.To;
            if (lineA.To.DistanceTo(pt1) > lineA.From.DistanceTo(pt1))
            {
                end1 = lineA.From;
                start1 = lineA.To;
            }
            Point3d start2 = lineB.From;
            Point3d end2 = lineB.To;
            if (lineB.To.DistanceTo(pt1) > lineB.From.DistanceTo(pt1))
            {
                end2 = lineB.From;
                start2 = lineB.To;
            }
            Vector3d v1 = Point3d.Subtract(start1, pt1);
            Vector3d v2 = Point3d.Subtract(start2, pt2);
            double angle = Vector3d.VectorAngle(v1, v2);
            double sgement = angle / number;
            ////////////////////////////////////////////////////////////////////等分
            double radius = pt1.DistanceTo(start1);///圆弧半径
            if (radius < pt1.DistanceTo(start2))
                radius = pt1.DistanceTo(start2);
            Plane pln = new Plane(pt1, v1, v2);
            Arc arc = new Arc(pln, radius, angle);
            Point3d[] pts;
            arc.ToNurbsCurve().DivideByCount(number, false, out pts);///等分点
            ///////////////////////////////////////////////////////////////////////////
            Line line1 = new Line(start1, start2);
            Line line2 = new Line(end1, end2);
            List<Line> collect = new List<Line>();
            foreach (Point3d ppp in pts)
            {
                collect.Add(new Line(pt1, ppp));
            }
            ////////////////////////////////////////////////////////////////////
            List<Line> last = new List<Line>();
            for (int i = 0; i < collect.Count; i++)
            {
                Point3d start = pt1;
                if (Rhino.Geometry.Intersect.Intersection.CurveCurve(lineA.ToNurbsCurve(), lineB.ToNurbsCurve(), 0, 0).Count == 0)////输入的两条直线无交点
                {
                    if (Rhino.Geometry.Intersect.Intersection.CurveCurve(line2.ToNurbsCurve(), collect[i].ToNurbsCurve(), 0, 0).Count == 0)
                        start = pt1;
                    else
                        start = Rhino.Geometry.Intersect.Intersection.CurveCurve(line2.ToNurbsCurve(), collect[i].ToNurbsCurve(), 0, 0)[0].PointA;
                }
                Point3d end = Rhino.Geometry.Intersect.Intersection.CurveCurve(line1.ToNurbsCurve(), collect[i].ToNurbsCurve(), 0, 0)[0].PointA;
                last.Add(new Line(start, end));
            }
            DA.SetDataList(0, last);
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
                //return Resource1.平分角;
                return Resource1.curve_指定数量平分角;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{f61d51a0-02d2-4250-b0c7-5eff90f9e620}"); }
        }
    }
}