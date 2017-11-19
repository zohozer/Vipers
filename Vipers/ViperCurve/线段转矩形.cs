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

namespace Vipers/////TangChi 2016.11.30
{
    public class LineToRectangle : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the LineToRectangle class.
        /// </summary>
        public LineToRectangle()
            : base("多段线转线框", "PlyFromWidth",
                "将平面多段线偏移成指定宽度的闭合多段线框，右键选择对齐方式",
                 "Vipers", "Viper.curve")
        {
            Message = "中心对齐";
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.obscure; }
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        bool flag = true;
        public bool Flag
        {
            get { return flag; }
            set
            { 
                flag = value;
                if (flag)
                    Message = "中心对齐";
            }
        }
        bool flag2 = false;
        public bool Flag2
        {
            get { return flag2; }
            set
            { 
                flag2 = value;
                if (flag2)
                    Message = "左边对齐";
            }
        }
        bool flag3 = false;
        public bool Flag3
        {
            get { return flag3; }
            set 
            { 
                flag3 = value;
                if (flag3)
                    Message = "右边对齐";
            }
        }
        public override bool Read(GH_IReader reader)
        {
            Flag = reader.GetBoolean("PlyFromWidth1");
            Flag2 = reader.GetBoolean("PlyFromWidth2");
            Flag3 = reader.GetBoolean("PlyFromWidth3");
            return base.Read(reader);
        }
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("PlyFromWidth1", Flag);
            writer.SetBoolean("PlyFromWidth2", Flag2);
            writer.SetBoolean("PlyFromWidth3", Flag3);
            return base.Write(writer);
        }
        Color cor1 = Color.FromArgb(61, 200, 44);
        Color cor2 = Color.FromArgb(61, 150, 44);
        Color cor3 = Color.FromArgb(61, 100, 44);
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            ToolStripMenuItem item = Menu_AppendItem(menu, "中心对齐", new EventHandler(Menu_AbsoluteClicked), true, Flag);
            ToolStripMenuItem item2 = Menu_AppendItem(menu, "左边对齐", new EventHandler(Menu_AbsoluteClicked2), true, Flag2);
            ToolStripMenuItem item3 = Menu_AppendItem(menu, "右边对齐", new EventHandler(Menu_AbsoluteClicked3), true, Flag3);
            item.ToolTipText = "指定线段位于多边形线框中心位置";
            item.BackColor = cor1;
            item2.ToolTipText = "指定线段位于多边形线框左边位置";
            item2.BackColor = cor2;
            item3.ToolTipText = "指定线段位于多边形线框右边位置";
            item3.BackColor = cor3;
        }
        private void Menu_AbsoluteClicked(object sender, EventArgs e)
        {
            RecordUndoEvent("LTR");
            Flag = true;
            Flag2 = false;
            Flag3 = false;
            ExpireSolution(true);
        }
        private void Menu_AbsoluteClicked2(object sender, EventArgs e)
        {
            RecordUndoEvent("LTR");
            Flag = false;
            Flag2 = true;
            Flag3 = false;
            ExpireSolution(true);
        }
        private void Menu_AbsoluteClicked3(object sender, EventArgs e)
        {
            RecordUndoEvent("LTR");
            Flag = false;
            Flag2 = false;
            Flag3 = true;
            ExpireSolution(true);
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("多段线","P","需要生成线框的多段线",GH_ParamAccess.item);
            pManager.AddPlaneParameter("平面","P","参考平面",GH_ParamAccess.item,Plane.WorldXY);
            pManager.AddNumberParameter("宽度","W","多段线框的宽度",GH_ParamAccess.item,10);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("线框","R","生成的多边形线框",GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.DisableGapLogic();
            Curve cr = null;
            Plane plane = Plane.Unset;
            double width = 0;
            Curve last = null;
            if(!DA.GetData(0,ref cr)||!DA.GetData(1,ref plane)||!DA.GetData(2,ref width))return;
            if(width<0)
                width=Math.Abs(width);
            if (Flag)
            {
               Curve[] cs1= AfterOffset(cr,width*0.5,plane,1);
               Curve[] cs2 = AfterOffset(cr, width * 0.5*-1, plane, 1);
               Curve c1 = cs1[0];
               Curve c2 = cs2[0];
               Curve start = new Line(c1.PointAtStart,c2.PointAtStart).ToNurbsCurve();
                Curve end =new Line(c1.PointAtEnd,c2.PointAtEnd).ToNurbsCurve();
                Curve[] collects = new Curve[] {c1,c2,start,end};
                last = Curve.JoinCurves(collects,0.0001)[0];
            }
            else if(Flag2)
            {
                Curve[] cs1 = AfterOffset(cr, width, plane, 1);
                Curve c1 = cs1[0];
                Curve start = new Line(c1.PointAtStart, cr.PointAtStart).ToNurbsCurve();
                Curve end = new Line(c1.PointAtEnd, cr.PointAtEnd).ToNurbsCurve();
                Curve[] collects = new Curve[] { c1, cr, start, end };
                last = Curve.JoinCurves(collects, 0.0001)[0];
            }
            else if (Flag3)
            {
                Curve[] cs1 = AfterOffset(cr, width*-1, plane, 1);
                Curve c1 = cs1[0];
                Curve start = new Line(c1.PointAtStart, cr.PointAtStart).ToNurbsCurve();
                Curve end = new Line(c1.PointAtEnd, cr.PointAtEnd).ToNurbsCurve();
                Curve[] collects = new Curve[] { c1, cr, start, end };
                last = Curve.JoinCurves(collects, 0.0001)[0];
            }
            DA.SetData(0,last);
        }
        public Curve[] AfterOffset(Curve destination, double naN, Plane unset, int num2)
        {
            if (!RhinoMath.IsValidDouble(naN))
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "不是有效的数字");
                return new Curve[] { };
            }
            else if (!unset.IsValid)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "不是有效的平面");
                return new Curve[] { };
            }
            else
            {
                double num3 = Math.Abs(naN);
                double tolerance = Math.Min(GH_Component.DocumentTolerance(), 0.1 * num3);
                GH_Component.DocumentAngleTolerance();
                num2 = Math.Max(Math.Min(num2, 4), 0);
                if (naN == 0.0)
                {
                    return new Curve[] { destination };
                }
                else
                {
                    CurveOffsetCornerStyle none = CurveOffsetCornerStyle.None;
                    switch (num2)
                    {
                        case 0:
                            none = CurveOffsetCornerStyle.None;
                            break;

                        case 1:
                            none = CurveOffsetCornerStyle.Sharp;
                            break;

                        case 2:
                            none = CurveOffsetCornerStyle.Round;
                            break;

                        case 3:
                            none = CurveOffsetCornerStyle.Smooth;
                            break;

                        case 4:
                            none = CurveOffsetCornerStyle.Chamfer;
                            break;
                    }
                    Curve[] inputCurves = null;
                    inputCurves = destination.Offset(unset, naN, tolerance, none);
                    if (inputCurves == null)
                    {
                        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "不能偏移该曲线");
                        return new Curve[] { };
                    }
                    else
                    {
                        Curve[] data = Curve.JoinCurves(inputCurves);
                        if (data == null)
                            return inputCurves;
                        else
                            return data;
                    }
                }
            }
        }//////////////GH自带的偏移命令
        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Resource1.curve_直线转矩形;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{70f37867-6260-44ae-8e82-e7c360dbbdc7}"); }
        }
    }
}