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

namespace Vipers////////TangChi 2015.3.10
{
    public class ViperDividebyLength : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent2 class.
        /// </summary>
        public ViperDividebyLength()
            : base("根据不同长度划分曲线", "CurveDivideByLD",
                "根据用户长度列表中的数值依次划分曲线，如果长度列表数据类型为item，则用法同GH自带运算器“Divide Length”或“Divide Distance”该种情况只会返回划分点不会返回划分的曲线 。如果长度列表数据类型为list，则将曲线按照列表中的数值依次划分成小段。 可右键选择划分方式——“根据不同长度划分曲线”,“根据不同距离划分曲线”",
                "Vipers", "Viper.curve")
        {
            //flag1 = true;
            //flag2 = false;
            //this.UpdateMessage();
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("待划分的曲线", "C", "待划分的曲线", GH_ParamAccess.item);
            pManager.AddNumberParameter("长度列表", "L", "根据列表中的长度划分曲线", GH_ParamAccess.list);
            pManager.HideParameter(0);
            Message = "根据不同长度划分曲线";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("划分后的曲线", "C", "根据列表数据划分后的曲线", GH_ParamAccess.list);
            pManager.AddCurveParameter("剩余部分", "R", "划分后的曲线如果有剩余，则返回该部分曲线", GH_ParamAccess.item);
            pManager.AddPointParameter("划分点", "P", "点在曲线的对应位置", GH_ParamAccess.list);
            pManager.HideParameter(1);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>

        bool _lengthcutcurve = true;

        public bool _LengthCutCurve
        {
            get { return _lengthcutcurve; }
            set
            {
                _lengthcutcurve = value;
                if (_lengthcutcurve == true)
                    Message = "根据不同长度划分曲线";
                else
                    Message = "根据不同距离划分曲线";
            }
        }
        public override bool Read(GH_IReader reader)
        {
            //this._LengthCutCurve = true;
            _LengthCutCurve=reader.GetBoolean("_LengthCutCurve");
            return base.Read(reader);
        }
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("_LengthCutCurve", _LengthCutCurve);
            return base.Write(writer);
        }
       /// </summary>
        Color cor1 = Color.FromArgb(61, 200, 44);
        Color cor2 = Color.FromArgb(61, 150, 44);
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            // Append the item to the menu, making sure it's always enabled and checked if Absolute is True.
            ToolStripMenuItem item = Menu_AppendItem(menu, "根据不同长度划分曲线", new EventHandler(Menu_AbsoluteClicked), true, _LengthCutCurve);
            ToolStripMenuItem item2 = Menu_AppendItem(menu, "根据不同距离划分曲线", new EventHandler(Menu_AbsoluteClicked2), true, !_LengthCutCurve);
            // Specifically assign a tooltip text to the menu item.
            item.ToolTipText = "方法参考GH中的\"DivideLength\",依次按照列表中的数值划分曲线";
            item.BackColor = cor1;
            item2.ToolTipText = "方法参考GH中的\"DivideDistance\",依次按照列表中的数值划分曲线";
            item2.BackColor = cor2;
        }
        private void Menu_AbsoluteClicked(object sender, EventArgs e)
        {
            RecordUndoEvent("DivdeCurve");
            _LengthCutCurve = true;
            ExpireSolution(true);
        }
        private void Menu_AbsoluteClicked2(object sender, EventArgs e)
        {
            RecordUndoEvent("DivdeCurve");
            _LengthCutCurve = false;
            ExpireSolution(true);
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.DisableGapLogic();
            ///////////////////////////////////////////////////////////////////////////////////////////////////声明变量
            Curve curve = null;
            List<double> lengths = new List<double>();
            ///////////////////////////////////////////////////////////////////////////////////////////////////检测输入端是否合理
            if (!DA.GetData(0, ref curve)) return;
            if (!DA.GetDataList(1, lengths)) return;
            List<Curve> curs = new List<Curve>();
            List<Point3d> pts = new List<Point3d>();
            if (_LengthCutCurve)
            {
                if(lengths.Count==1)
                {
                    Point3d[] points;
                    curve.DivideByLength(lengths[0], true, out points);
                    if (points.Length > 1)
                    {
                        pts.AddRange(points);
                        DA.SetDataList(2, pts);
                    }
                }
                else
                {
                    pts.Add(curve.PointAtStart);
                    for (int i = 0; i < lengths.Count; i++)
                    {
                        if (lengths[i] > curve.GetLength())
                        {
                            break;
                        }
                        double[] ts = curve.DivideByLength(lengths[i], false);
                        Point3d pt = curve.PointAt(ts[0]);
                        pts.Add(pt);
                        Curve[] cs = curve.Split(ts[0]);
                        curs.Add(cs[0]);
                        curve = cs[1];
                    }
                    if (pts.Count <= 1)
                    {
                        pts.Clear();
                    }
                    DA.SetDataList(0, curs);
                    DA.SetData(1, curve);
                    DA.SetDataList(2, pts);
                }
            }
            else
            {
                if(lengths.Count==1)
                {
                    Point3d[] points=curve.DivideEquidistant(lengths[0]);
                    if (points.Length > 1)
                    {
                        pts.AddRange(points);
                        DA.SetDataList(2, pts);
                    }
                }
                else
                {
                    pts.Add(curve.PointAtStart);
                    for (int i = 0; i < lengths.Count; i++)
                    {
                        double length = curve.GetLength();
                        if (curve.DivideEquidistant(lengths[i]).Length == 1)
                        {
                            break;
                        }
                        Point3d pt = curve.DivideEquidistant(lengths[i])[1];
                        double t = 0;
                        curve.ClosestPoint(pt, out t);
                        curs.Add(curve.Split(t)[0]);
                        pts.Add(pt);
                        curve = curve.Split(t)[1];
                    }
                    if (pts.Count <= 1)
                    {
                        pts.Clear();
                    }
                    DA.SetDataList(0, curs);
                    DA.SetData(1, curve);
                    DA.SetDataList(2, pts);
                }
            }
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
                //return Resource1.根据不同长度划分曲线;
                return Resource1.curve_根据不同长度划分曲线;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{0322cc08-8e66-40fa-82fd-78da2a4c1315}"); }
        }
    }
}