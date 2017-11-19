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
using Vipers;


namespace SuperVipers/////TangChi 2015.8.14
{
    public class BulgeSkin2 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent57 class.
        /// </summary>
        public BulgeSkin2()
            : base("突出表皮2", "BulgeSkin2",
                "根据用户提供的曲面生成由平板构成的突出表皮及平板顶盖",
                "Vipers", "Viper.surface")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("曲面", "S", "待加盖的曲面", GH_ParamAccess.item);
            pManager.AddNumberParameter("公差1", "T1", "距离公差，用于简化曲面轮廓", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("公差2", "T2", "角度公差，用于简化曲面轮廓", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("高度", "H", "突出于平面的总高度", GH_ParamAccess.item, 100);
            pManager.AddNumberParameter("程度", "P", "突出部分占高度的比例", GH_ParamAccess.item, 0.5);
            pManager.AddBooleanParameter("方向", "R", "是否切换方向", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("加盖", "C", "是否加盖", GH_ParamAccess.item, false);
            pManager.HideParameter(0);
            Message = "TC-0-02\n凸板2";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("表皮", "B", "建筑表皮", GH_ParamAccess.list);
            pManager.AddCurveParameter("轮廓线", "C", "表皮的轮廓线", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Brep surface = new Brep();
            double tolerance = 0;
            double angleToler = 0;
            double height = 0;
            double percent = 0;
            bool isSwitch = false;
            bool isCap = false;
            if (!DA.GetData(0, ref surface)) return;
            if (!DA.GetData(1, ref tolerance)) return;
            if (!DA.GetData(2, ref angleToler)) return;
            if (!DA.GetData(3, ref height)) return;
            if (!DA.GetData(4, ref percent)) return;
            if (!DA.GetData(5, ref isSwitch)) return;
            if (!DA.GetData(6, ref isCap)) return;
            Brep x = surface;
            double t = tolerance;
            double a = angleToler;
            double h = height;
            double p = percent;
            if (isSwitch)
            {
                h = h * (-1);
            }
            if (p > 1 || p < 0)
            {
                p = 1;
            }
            Curve[] cs = Curve.JoinCurves(x.DuplicateEdgeCurves(true));
            Curve c = cs[0].Simplify(CurveSimplifyOptions.All, t, a);
            Curve[] cs2 = c.DuplicateSegments();
            double xx = 0;
            double yy = 0;
            double zz = 0;
            for (int i = 0; i < cs2.Length; i++)
            {
                xx += cs2[i].PointAtStart.X;
                yy += cs2[i].PointAtStart.Y;
                zz += cs2[i].PointAtStart.Z;
            }
            Point3d center = new Point3d(xx / cs2.Length, yy / cs2.Length, zz / cs2.Length);
            Point3d pt;
            ComponentIndex ci;
            double tt;
            double s;
            Vector3d vc;
            x.ClosestPoint(center, out pt, out ci, out s, out tt, 0, out vc);
            Plane pln = new Plane(pt, vc);//////切割平面
            pt.Transform(Transform.Translation(vc * h));
            pln.Transform(Transform.Translation(vc * h * p));
            List<Point3d> ptss = new List<Point3d>();///////平面与线段交点
            for (int i = 0; i < cs2.Length; i++)
            {
                double param;
                Line l = new Line(cs2[i].PointAtStart, pt);
                Rhino.Geometry.Intersect.Intersection.LinePlane(l, pln, out param);
                ptss.Add(l.PointAt(param));
            }
            List<Line> ls1 = new List<Line>();
            List<NurbsCurve> ls11 = new List<NurbsCurve>();///轮廓线2
            List<Line> ls2 = new List<Line>();
            List<NurbsCurve> ls22 = new List<NurbsCurve>();///轮廓线3
            List<Line> ls3 = new List<Line>();
            for (int i = 0; i < cs2.Length; i++)
            {
                if (i == cs2.Length - 1)
                {
                    ls1.Add(new Line(ptss[i], ptss[0]));
                    ls11.Add(new Line(ptss[i], ptss[0]).ToNurbsCurve());
                    ls2.Add(new Line(cs2[i].PointAtStart, ptss[i]));
                    ls22.Add(new Line(cs2[i].PointAtStart, ptss[i]).ToNurbsCurve());
                    ls3.Add(new Line(cs2[i].PointAtEnd, ptss[0]));
                    continue;
                }
                ls1.Add(new Line(ptss[i], ptss[i + 1]));
                ls11.Add(new Line(ptss[i], ptss[i + 1]).ToNurbsCurve());
                ls2.Add(new Line(cs2[i].PointAtStart, ptss[i]));
                ls22.Add(new Line(cs2[i].PointAtStart, ptss[i]).ToNurbsCurve());
                ls3.Add(new Line(cs2[i].PointAtEnd, ptss[i + 1]));
            }
            List<Brep> sfs = new List<Brep>();
            for (int i = 0; i < cs2.Length; i++)
            {
                Curve[] cc = { cs2[i], ls1[i].ToNurbsCurve(), ls2[i].ToNurbsCurve(), ls3[i].ToNurbsCurve() };
                sfs.Add(Brep.CreateEdgeSurface(cc));
            }
            if (isCap)
            {
                sfs.Add(Brep.CreatePlanarBreps(ls11)[0]);/////加顶盖
            }
            List<Curve> curves = new List<Curve>();
            curves.AddRange(cs2);
            curves.AddRange(ls11);
            curves.AddRange(ls22);
            DA.SetDataList(0, sfs);
            DA.SetDataList(1, curves);
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
                return Resource1.surface_突出表皮1;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{7348e874-467f-469e-96ad-06c8d8fcd411}"); }
        }
    }
}