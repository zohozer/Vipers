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
using Grasshopper.Kernel.Parameters;

namespace Vipers///////////////TangChi 2016.12.26
{
    public class ConnectedCurve : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the 与曲线关联物件 class.
        /// </summary>
        public ConnectedCurve()
            : base("与曲线关联物件", "ConnectedCurve",
                "找出与指定曲线关联的物件，右键多选菜单。完全重合：表示选出的物件中的边缘线中至少一条与指定曲线完全重合，部分重合：物件中的边缘线至少一条与指定曲线有部分重合，所有关联：物件的边缘线与指定曲线有相交部分（包括重合）",
                "Vipers", "viper.curve")
        {
            Message = "完全重合";
        }
        bool strict = true;
        public bool Strict
        {
            get { return strict; }
            set 
            { 
                strict = value;
                if (Strict)
                    Message = "完全重合";
            }
        }
        bool some = false;
        public bool Some
        {
            get { return some; }
            set 
            { 
                some = value;
                if (Some)
                    Message = "部分重合";
            }
        }
        bool all = false;
        public bool All
        {
            get { return all; }
            set 
            { 
                all = value;
                if(All)
                Message = "所有关联";
            }
        }
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("StrictChoice", Strict);
            writer.SetBoolean("SomeChoice", Some);
            writer.SetBoolean("AllChoice", All);
            return base.Write(writer);
        }
        public override bool Read(GH_IReader reader)
        {
            Strict = reader.GetBoolean("StrictChoice");
            Some = reader.GetBoolean("SomeChoice");
            All = reader.GetBoolean("AllChoice");
            return base.Read(reader);
        }
        Color cor1 = Color.FromArgb(61, 200, 44);
        Color cor2 = Color.FromArgb(61, 150, 44);
        Color cor3 = Color.FromArgb(61, 100, 44);
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            // Append the item to the menu, making sure it's always enabled and checked if Absolute is True.
            ToolStripMenuItem item1 = Menu_AppendItem(menu, "完全重合", Menu_AbsoluteClicked1, true, Strict);
            ToolStripMenuItem item2 = Menu_AppendItem(menu, "部分重合", Menu_AbsoluteClicked2, true, Some);
            ToolStripMenuItem item3 = Menu_AppendItem(menu, "所有关联", Menu_AbsoluteClicked3, true, All);
            // Specifically assign a tooltip text to the menu item.
            item1.ToolTipText = "物件的一边与指定曲线的一边完全重合";
            item1.BackColor = cor1;
            item2.ToolTipText = "物件的一边与指定曲线的一边部分重合";
            item2.BackColor = cor2;
            item3.ToolTipText = "物件的一边与指定曲线有任何相交部分";
            item3.BackColor = cor3;
        }
        private void Menu_AbsoluteClicked1(object sender, EventArgs e)
        {
            RecordUndoEvent("StrictChoice");
            Strict = true;
            Some = false;
            All = false;
            ExpireSolution(true);
        }
        private void Menu_AbsoluteClicked2(object sender, EventArgs e)
        {
            RecordUndoEvent("SomeChoice");
            Strict = false;
            Some = true;
            All = false;
            ExpireSolution(true);
        }
        private void Menu_AbsoluteClicked3(object sender, EventArgs e)
        {
            RecordUndoEvent("AllChoice");
            Strict = false ;
            Some = false;
            All = true ;
            ExpireSolution(true);
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("物件","G","几何物件",GH_ParamAccess.list);
            pManager.AddCurveParameter("曲线","C","指定曲线，找出与指定曲线有关联的物件",GH_ParamAccess.item);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
        }
        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGeometryParameter("物件","G","与指定曲线有关联的物件",GH_ParamAccess.list);
            pManager.AddIntegerParameter("索引","I","有关联物件在原物件中的索引位置",GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<IGH_GeometricGoo> geometry = new List<IGH_GeometricGoo>();
            Curve curve = null;
            if (!DA.GetDataList(0, geometry) || !DA.GetData(1, ref curve)) return;
            ////////////////将于指定曲线线有关系的物件选出（Brep,Curve,Mesh）strict 选出完全重合，some 局部重合 ，all 相关联所有
            List<IGH_GeometricGoo> collect = new List<IGH_GeometricGoo>();
            List<int> indexes = new List<int>();
            for (int i = 0; i < geometry.Count; i++)
            {
                IGH_GeometricGoo obj = geometry[i];
                if (obj is GH_Curve)
                {
                    GH_Curve ghcurve = (GH_Curve)obj;
                    if (CurveAndCurve(ghcurve.Value, curve, strict, some, all))
                    {
                        indexes.Add(i);
                        collect.Add(obj);
                    }
                }
                else if (obj is GH_Brep)
                {
                    GH_Brep ghbrep = (GH_Brep)obj;
                    if (CurveAndBrep(ghbrep.Value, curve, strict, some, all))
                    {
                        indexes.Add(i);
                        collect.Add(obj);
                    }
                }
                else if (obj is GH_Surface)
                {
                    GH_Surface ghsurface = (GH_Surface)obj;
                    if (CurveAndBrep(ghsurface.Value, curve, strict, some, all))
                    {
                        indexes.Add(i);
                        collect.Add(obj);
                    }
                }
                else if (obj is GH_Mesh)
                {
                    GH_Mesh ghmesh = (GH_Mesh)obj;
                    if (CurveAndMesh(ghmesh.Value, curve, strict, some, all))
                    {
                        indexes.Add(i);
                        collect.Add(obj);
                    }
                }
            }
            DA.SetDataList(0, collect);
            DA.SetDataList(1, indexes);
        }
        public bool CurveAndBrep(Brep brep, Curve curve, bool strict, bool some, bool all) ////曲线和物件
        {
            ///////////////////////////////////////////////////////
            List<Curve> cs = brep.GetWireframe(0).ToList();
            List<Curve> cs2 = curve.DuplicateSegments().ToList();
            if (curve.IsLinear())
            {
                cs2.Add(curve);
            }
            ////////////////////////////////////////////////////////
            bool flag = false;
            for (int q = 0; q < cs2.Count; q++)
            {
                for (int i = 0; i < cs.Count; i++)
                {
                    if (Overlap(cs2[q], cs[i]) && strict)//完全重合模式
                    {
                        return true;
                    }
                    if (HalfOverlap(cs2[q], cs[i]) && some)/////部分线重合模式
                    {
                        return true;
                    }
                    if (Rhino.Geometry.Intersect.Intersection.CurveCurve(cs2[q], cs[i], 0.01, 0.01).Count > 0 && all)//////所有模式
                    {
                        return true;
                    }
                }
            }
            return flag;
        }
        public bool CurveAndMesh(Mesh mesh, Curve curve, bool strict, bool some, bool all)
        {
            ///////////////////////////////////////////////////////
            List<Polyline> plys = mesh.GetNakedEdges().ToList();
            List<Curve> cs = new List<Curve>();
            for (int i = 0; i < plys.Count; i++)
            {
                for (int q = 0; q < plys[i].SegmentCount; q++)
                {
                    cs.Add(plys[i].SegmentAt(q).ToNurbsCurve());
                }
            }
            List<Curve> cs2 = curve.DuplicateSegments().ToList();
            if (curve.IsLinear())
            {
                cs2.Add(curve);
            }
            ////////////////////////////////////////////////////////
            bool flag = false;
            for (int q = 0; q < cs2.Count; q++)
            {
                for (int i = 0; i < cs.Count; i++)
                {
                    if (Overlap(cs2[q], cs[i]) && strict)//完全重合模式
                    {
                        return true;
                    }
                    if (HalfOverlap(cs2[q], cs[i]) && some)/////部分线重合模式
                    {
                        return true;
                    }
                    if (Rhino.Geometry.Intersect.Intersection.CurveCurve(cs2[q], cs[i], 0.01, 0.01).Count > 0 && all)//////所有模式
                    {
                        return true;
                    }
                }
            }
            return flag;
        }
        public bool CurveAndCurve(Curve curves, Curve curve, bool strict, bool some, bool all)
        {
            //////////////////////////////////////////////////因为单独的曲线不能使用duplicate方法故需要单独处理
            List<Curve> cs = curves.DuplicateSegments().ToList();
            if (curves.IsLinear())
            {
                cs.Add(curves);
            }
            List<Curve> cs2 = curve.DuplicateSegments().ToList();
            if (curve.IsLinear())
            {
                cs2.Add(curve);
            }
            //////////////////////////////////////////////////
            bool flag = false;
            for (int i = 0; i < cs.Count; i++)
            {
                for (int q = 0; q < cs2.Count; q++)
                {
                    if (Overlap(cs[i], cs2[q]) && strict)////完全重合模式
                    {
                        return true;
                    }
                    else if (HalfOverlap(cs[i], cs2[q]) && some)/////部分线重合模式
                    {
                        return true;
                    }
                    else if (Rhino.Geometry.Intersect.Intersection.CurveCurve(cs[i], cs2[q], 0.1, 0.1).Count > 0 && all)//////所有模式
                    {
                        return true;
                    }
                }
            }
            return flag;
        }
        public bool Overlap(Curve x, Curve y) /////判断两条曲线是否完全重合
        {
            bool flag = false;
            if (Rhino.Geometry.Intersect.Intersection.CurveCurve(x, y, 0.01, 0.01).Count > 0)///有交集
            {
                Interval aaa = Rhino.Geometry.Intersect.Intersection.CurveCurve(x, y, 0.01, 0.01)[0].OverlapA;
                Interval bbb = Rhino.Geometry.Intersect.Intersection.CurveCurve(x, y, 0.01, 0.01)[0].OverlapB;
                Interval AAA = x.Domain;
                Interval BBB = y.Domain;
                aaa.MakeIncreasing();
                bbb.MakeIncreasing();
                AAA.MakeIncreasing();
                BBB.MakeIncreasing();
                if (aaa == AAA && bbb == BBB)
                    flag = true;
            }
            return flag;
        }
        public bool HalfOverlap(Curve x, Curve y) /////判断两条曲线是否部分重合
        {
            bool flag = false;
            if (Rhino.Geometry.Intersect.Intersection.CurveCurve(x, y, 0.01, 0.01).Count > 0)///有交集
            {
                Interval aaa = Rhino.Geometry.Intersect.Intersection.CurveCurve(x, y, 0.01, 0.01)[0].OverlapA;
                Interval bbb = Rhino.Geometry.Intersect.Intersection.CurveCurve(x, y, 0.01, 0.01)[0].OverlapB;
                Interval AAA = x.Domain;
                Interval BBB = y.Domain;
                aaa.MakeIncreasing();
                bbb.MakeIncreasing();
                AAA.MakeIncreasing();
                BBB.MakeIncreasing();
                if ((AAA.IncludesInterval(aaa) || BBB.IncludesInterval(bbb)) && aaa.Length != 0 && bbb.Length != 0)
                    flag = true;
            }
            return flag;
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
                return Resource1.curve_与曲线关联物件;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{aef47dfd-cdbd-49cf-b687-dae323fbc7b6}"); }
        }
    }
}