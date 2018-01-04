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
    public class PolyE : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public PolyE()
            : base("Isosceles Triangle Polyhedron", "PolygonE",
                "Create isosceles triangle polyhedron (RMB to change shape)",
                "Vipers", "Viper.surface")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quinary; }
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "Base plane", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddNumberParameter("Radius", "R", "Shape radius", GH_ParamAccess.item, 1);
            pManager.AddBooleanParameter("Surface", "S", "Output surfaces", GH_ParamAccess.item, false);
            pManager.HideParameter(0);
            Message = "11 Sides";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Polyline", "P", "Output polylines", GH_ParamAccess.list);
            pManager.AddBrepParameter("Polygon", "P", "Output faces", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        #region////////类型1
        static string str1 = @"{57.959572, -57.959572, -57.959572}
{-57.959572, 57.959572, -57.959572}
{-34.775743, -34.775743, -34.775743}
{-57.959572, 57.959572, -57.959572}
{-57.959572, -57.959572, 57.959572}
{-34.775743, -34.775743, -34.775743}
{57.959572, -57.959572, -57.959572}
{-57.959572, -57.959572, 57.959572}
{-34.775743, -34.775743, -34.775743}
{-34.775743, 34.775743, 34.775743}
{57.959572, 57.959572, 57.959572}
{-57.959572, -57.959572, 57.959572}
{-57.959572, -57.959572, 57.959572}
{-34.775743, 34.775743, 34.775743}
{-57.959572, 57.959572, -57.959572}
{57.959572, 57.959572, 57.959572}
{-57.959572, 57.959572, -57.959572}
{-34.775743, 34.775743, 34.775743}
{-57.959572, 57.959572, -57.959572}
{34.775743, 34.775743, -34.775743}
{57.959572, 57.959572, 57.959572}
{-57.959572, 57.959572, -57.959572}
{34.775743, 34.775743, -34.775743}
{57.959572, -57.959572, -57.959572}
{57.959572, 57.959572, 57.959572}
{57.959572, -57.959572, -57.959572}
{34.775743, 34.775743, -34.775743}
{57.959572, -57.959572, -57.959572}
{34.775743, -34.775743, 34.775743}
{57.959572, 57.959572, 57.959572}
{57.959572, -57.959572, -57.959572}
{34.775743, -34.775743, 34.775743}
{-57.959572, -57.959572, 57.959572}
{-57.959572, -57.959572, 57.959572}
{34.775743, -34.775743, 34.775743}
{57.959572, 57.959572, 57.959572}
";
        static string length1 = "333333333333";
        #endregion
        #region////////类型2
        static string str2 = @"{0.0, 0.0, 241.421356}
    {241.421356, 0.0, 0.0}
    {100.0, 100.0, 100.0}
    {241.421356, 0.0, 0.0}
    {0.0, 241.421356, 0.0}
    {100.0, 100.0, 100.0}
    {0.0, 0.0, 241.421356}
    {0.0, 241.421356, 0.0}
    {100.0, 100.0, 100.0}
    {0.0, 0.0, -241.421356}
    {0.0, 241.421356, 0.0}
    {100.0, 100.0, -100.0}
    {0.0, 241.421356, 0.0}
    {100.0, 100.0, -100.0}
    {241.421356, 0.0, 0.0}
    {0.0, 0.0, -241.421356}
    {241.421356, 0.0, 0.0}
    {100.0, 100.0, -100.0}
    {0.0, 0.0, 241.421356}
    {0.0, -241.421356, 0.0}
    {100.0, -100.0, 100.0}
    {0.0, -241.421356, 0.0}
    {241.421356, 0.0, 0.0}
    {100.0, -100.0, 100.0}
    {241.421356, 0.0, 0.0}
    {100.0, -100.0, 100.0}
    {0.0, 0.0, 241.421356}
    {241.421356, 0.0, 0.0}
    {100.0, -100.0, -100.0}
    {0.0, 0.0, -241.421356}
    {241.421356, 0.0, 0.0}
    {100.0, -100.0, -100.0}
    {0.0, -241.421356, 0.0}
    {0.0, 0.0, -241.421356}
    {0.0, -241.421356, 0.0}
    {100.0, -100.0, -100.0}
    {0.0, 241.421356, 0.0}
    {-100.0, 100.0, 100.0}
    {0.0, 0.0, 241.421356}
    {0.0, 241.421356, 0.0}
    {-241.421356, 0.0, 0.0}
    {-100.0, 100.0, 100.0}
    {0.0, 0.0, 241.421356}
    {-241.421356, 0.0, 0.0}
    {-100.0, 100.0, 100.0}
    {0.0, 0.0, -241.421356}
    {-241.421356, 0.0, 0.0}
    {-100.0, 100.0, -100.0}
    {-241.421356, 0.0, 0.0}
    {-100.0, 100.0, -100.0}
    {0.0, 241.421356, 0.0}
    {0.0, 241.421356, 0.0}
    {-100.0, 100.0, -100.0}
    {0.0, 0.0, -241.421356}
    {-241.421356, 0.0, 0.0}
    {-100.0, -100.0, 100.0}
    {0.0, 0.0, 241.421356}
    {-241.421356, 0.0, 0.0}
    {0.0, -241.421356, 0.0}
    {-100.0, -100.0, 100.0}
    {0.0, -241.421356, 0.0}
    {-100.0, -100.0, 100.0}
    {0.0, 0.0, 241.421356}
    {0.0, -241.421356, 0.0}
    {-100.0, -100.0, -100.0}
    {0.0, 0.0, -241.421356}
    {0.0, -241.421356, 0.0}
    {-100.0, -100.0, -100.0}
    {-241.421356, 0.0, 0.0}
    {-241.421356, 0.0, 0.0}
    {-100.0, -100.0, -100.0}
    {0.0, 0.0, -241.421356}
    ";
        static string length2 = "333333333333333333333333";
        #endregion
        #region////////类型3
        static string str3 = @"{180.901699, 0.0, 292.705098}
    {0.0, 292.705098, 180.901699}
    {0.0, 104.955318, 274.77659}
    {0.0, 292.705098, 180.901699}
    {-180.901699, 0.0, 292.705098}
    {0.0, 104.955318, 274.77659}
    {180.901699, 0.0, 292.705098}
    {-180.901699, 0.0, 292.705098}
    {0.0, 104.955318, 274.77659}
    {180.901699, 0.0, -292.705098}
    {-180.901699, 0.0, -292.705098}
    {0.0, 104.955318, -274.77659}
    {-180.901699, 0.0, -292.705098}
    {0.0, 292.705098, -180.901699}
    {0.0, 104.955318, -274.77659}
    {180.901699, 0.0, -292.705098}
    {0.0, 292.705098, -180.901699}
    {0.0, 104.955318, -274.77659}
    {-180.901699, 0.0, 292.705098}
    {0.0, -104.955318, 274.77659}
    {180.901699, 0.0, 292.705098}
    {-180.901699, 0.0, 292.705098}
    {0.0, -292.705098, 180.901699}
    {0.0, -104.955318, 274.77659}
    {180.901699, 0.0, 292.705098}
    {0.0, -292.705098, 180.901699}
    {0.0, -104.955318, 274.77659}
    {180.901699, 0.0, -292.705098}
    {0.0, -292.705098, -180.901699}
    {0.0, -104.955318, -274.77659}
    {0.0, -292.705098, -180.901699}
    {-180.901699, 0.0, -292.705098}
    {0.0, -104.955318, -274.77659}
    {-180.901699, 0.0, -292.705098}
    {0.0, -104.955318, -274.77659}
    {180.901699, 0.0, -292.705098}
    {180.901699, 0.0, 292.705098}
    {292.705098, -180.901699, 0.0}
    {274.77659, 0.0, 104.955318}
    {292.705098, -180.901699, 0.0}
    {292.705098, 180.901699, 0.0}
    {274.77659, 0.0, 104.955318}
    {180.901699, 0.0, 292.705098}
    {292.705098, 180.901699, 0.0}
    {274.77659, 0.0, 104.955318}
    {180.901699, 0.0, -292.705098}
    {292.705098, 180.901699, 0.0}
    {274.77659, 0.0, -104.955318}
    {292.705098, 180.901699, 0.0}
    {274.77659, 0.0, -104.955318}
    {292.705098, -180.901699, 0.0}
    {180.901699, 0.0, -292.705098}
    {292.705098, -180.901699, 0.0}
    {274.77659, 0.0, -104.955318}
    {-180.901699, 0.0, 292.705098}
    {-292.705098, 180.901699, 0.0}
    {-274.77659, 0.0, 104.955318}
    {-292.705098, 180.901699, 0.0}
    {-292.705098, -180.901699, 0.0}
    {-274.77659, 0.0, 104.955318}
    {-180.901699, 0.0, 292.705098}
    {-292.705098, -180.901699, 0.0}
    {-274.77659, 0.0, 104.955318}
    {-180.901699, 0.0, -292.705098}
    {-292.705098, -180.901699, 0.0}
    {-274.77659, 0.0, -104.955318}
    {-292.705098, -180.901699, 0.0}
    {-274.77659, 0.0, -104.955318}
    {-292.705098, 180.901699, 0.0}
    {-180.901699, 0.0, -292.705098}
    {-292.705098, 180.901699, 0.0}
    {-274.77659, 0.0, -104.955318}
    {292.705098, 180.901699, 0.0}
    {0.0, 292.705098, -180.901699}
    {104.955318, 274.77659, 0.0}
    {0.0, 292.705098, -180.901699}
    {0.0, 292.705098, 180.901699}
    {104.955318, 274.77659, 0.0}
    {292.705098, 180.901699, 0.0}
    {0.0, 292.705098, 180.901699}
    {104.955318, 274.77659, 0.0}
    {292.705098, -180.901699, 0.0}
    {0.0, -292.705098, 180.901699}
    {104.955318, -274.77659, 0.0}
    {0.0, -292.705098, 180.901699}
    {0.0, -292.705098, -180.901699}
    {104.955318, -274.77659, 0.0}
    {292.705098, -180.901699, 0.0}
    {0.0, -292.705098, -180.901699}
    {104.955318, -274.77659, 0.0}
    {-292.705098, 180.901699, 0.0}
    {0.0, 292.705098, 180.901699}
    {-104.955318, 274.77659, 0.0}
    {0.0, 292.705098, 180.901699}
    {-104.955318, 274.77659, 0.0}
    {0.0, 292.705098, -180.901699}
    {-292.705098, 180.901699, 0.0}
    {0.0, 292.705098, -180.901699}
    {-104.955318, 274.77659, 0.0}
    {-292.705098, -180.901699, 0.0}
    {0.0, -292.705098, -180.901699}
    {-104.955318, -274.77659, 0.0}
    {0.0, -292.705098, -180.901699}
    {-104.955318, -274.77659, 0.0}
    {0.0, -292.705098, 180.901699}
    {-292.705098, -180.901699, 0.0}
    {0.0, -292.705098, 180.901699}
    {-104.955318, -274.77659, 0.0}
    {292.705098, 180.901699, 0.0}
    {169.821272, 169.821272, 169.821272}
    {180.901699, 0.0, 292.705098}
    {292.705098, 180.901699, 0.0}
    {169.821272, 169.821272, 169.821272}
    {0.0, 292.705098, 180.901699}
    {0.0, 292.705098, 180.901699}
    {169.821272, 169.821272, 169.821272}
    {180.901699, 0.0, 292.705098}
    {0.0, 292.705098, -180.901699}
    {169.821272, 169.821272, -169.821272}
    {180.901699, 0.0, -292.705098}
    {0.0, 292.705098, -180.901699}
    {169.821272, 169.821272, -169.821272}
    {292.705098, 180.901699, 0.0}
    {292.705098, 180.901699, 0.0}
    {169.821272, 169.821272, -169.821272}
    {180.901699, 0.0, -292.705098}
    {0.0, -292.705098, 180.901699}
    {169.821272, -169.821272, 169.821272}
    {180.901699, 0.0, 292.705098}
    {0.0, -292.705098, 180.901699}
    {169.821272, -169.821272, 169.821272}
    {292.705098, -180.901699, 0.0}
    {292.705098, -180.901699, 0.0}
    {169.821272, -169.821272, 169.821272}
    {180.901699, 0.0, 292.705098}
    {292.705098, -180.901699, 0.0}
    {169.821272, -169.821272, -169.821272}
    {180.901699, 0.0, -292.705098}
    {292.705098, -180.901699, 0.0}
    {169.821272, -169.821272, -169.821272}
    {0.0, -292.705098, -180.901699}
    {0.0, -292.705098, -180.901699}
    {169.821272, -169.821272, -169.821272}
    {180.901699, 0.0, -292.705098}
    {0.0, 292.705098, 180.901699}
    {-169.821272, 169.821272, 169.821272}
    {-180.901699, 0.0, 292.705098}
    {0.0, 292.705098, 180.901699}
    {-169.821272, 169.821272, 169.821272}
    {-292.705098, 180.901699, 0.0}
    {-292.705098, 180.901699, 0.0}
    {-169.821272, 169.821272, 169.821272}
    {-180.901699, 0.0, 292.705098}
    {-292.705098, 180.901699, 0.0}
    {-169.821272, 169.821272, -169.821272}
    {-180.901699, 0.0, -292.705098}
    {-292.705098, 180.901699, 0.0}
    {-169.821272, 169.821272, -169.821272}
    {0.0, 292.705098, -180.901699}
    {0.0, 292.705098, -180.901699}
    {-169.821272, 169.821272, -169.821272}
    {-180.901699, 0.0, -292.705098}
    {-292.705098, -180.901699, 0.0}
    {-169.821272, -169.821272, 169.821272}
    {-180.901699, 0.0, 292.705098}
    {-292.705098, -180.901699, 0.0}
    {-169.821272, -169.821272, 169.821272}
    {0.0, -292.705098, 180.901699}
    {0.0, -292.705098, 180.901699}
    {-169.821272, -169.821272, 169.821272}
    {-180.901699, 0.0, 292.705098}
    {0.0, -292.705098, -180.901699}
    {-169.821272, -169.821272, -169.821272}
    {-180.901699, 0.0, -292.705098}
    {0.0, -292.705098, -180.901699}
    {-169.821272, -169.821272, -169.821272}
    {-292.705098, -180.901699, 0.0}
    {-292.705098, -180.901699, 0.0}
    {-169.821272, -169.821272, -169.821272}
    {-180.901699, 0.0, -292.705098}
    ";
        static string length3 = "333333333333333333333333333333333333333333333333333333333333";
        #endregion
        static double len1 = 100;
        static double len2 = 241;
        static double len3 = 344;
        double curvelength = len1;/////////////提取模型的实际长度
        public double Curvelength
        {
            get { return curvelength; }
            set { curvelength = value; }
        }
        string str = str1;///////////////////////////////实际用于计算的点数据
        public string Str
        {
            get { return str; }
            set { str = value; }
        }
        string length = length1;//////////////////////////实际用于计算的组
        public string Length
        {
            get { return length; }
            set { length = value; }
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        private bool polygonA = true;
        private bool polygonB = false;
        private bool polygonC = false;
        public bool PolygonA/////////////////稀疏
        {
            get { return polygonA; }
            set
            {
                polygonA = value;
                if ((polygonA))
                {
                    Message = "11 Sides";
                }
            }
        }
        public bool PolygonB///////////////普通
        {
            get { return polygonB; }
            set
            {
                polygonB = value;
                if ((polygonB))
                {
                    Message = "24 Sides";
                }
            }
        }
        public bool PolygonC///////////////致密
        {
            get { return polygonC; }
            set
            {
                polygonC = value;
                if ((polygonC))
                {
                    Message = "60 Sides";
                }
            }
        }
        Color cor1 = Color.FromArgb(61, 200, 44);
        Color cor2 = Color.FromArgb(61, 150, 44);
        Color cor3 = Color.FromArgb(61, 100, 44);
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            // Append the item to the menu, making sure it's always enabled and checked if Absolute is True.
            ToolStripMenuItem item = Menu_AppendItem(menu, "11 Sides", Menu_AbsoluteClicked, true, polygonA);
            ToolStripMenuItem item2 = Menu_AppendItem(menu, "24 Sides", Menu_AbsoluteClicked2, true, polygonB);
            ToolStripMenuItem item3 = Menu_AppendItem(menu, "60 Sides", Menu_AbsoluteClicked3, true, polygonC);
            // Specifically assign a tooltip text to the menu item.
            item.BackColor = cor1;
            item2.BackColor = cor2;
            item3.BackColor = cor3;
        }
        private void Menu_AbsoluteClicked(object sender, EventArgs e)
        {
            RecordUndoEvent("Absolute");
            PolygonA = true;
            PolygonB = false;
            PolygonC = false;
            Str = str1;
            Length = length1;
            Curvelength = len1;
            ExpireSolution(true);
        }
        private void Menu_AbsoluteClicked2(object sender, EventArgs e)
        {
            RecordUndoEvent("Absolute");
            PolygonA = false;
            PolygonB = true;
            PolygonC = false;
            Str = str2;
            Length = length2;
            Curvelength = len2;
            ExpireSolution(true);
        }
        private void Menu_AbsoluteClicked3(object sender, EventArgs e)
        {
            RecordUndoEvent("Absolute");
            PolygonA = false;
            PolygonB = false;
            PolygonC = true;
            Str = str3;
            Length = length3;
            Curvelength = len3;
            ExpireSolution(true);
        }

        public override bool Read(GH_IReader reader)
        {
            PolygonA = reader.GetBoolean("PolyE1");
            PolygonB = reader.GetBoolean("PolyE2");
            PolygonC = reader.GetBoolean("PolyE3");
            if (PolygonA)
            {
                Str = str1;
                Length = length1;
            }
            if (PolygonB)
            {
                Str = str2;
                Length = length2;
            }
            if (PolygonC)
            {
                Str = str3;
                Length = length3;
            }
            return base.Read(reader);
        }
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("PolyE1", PolygonA);
            writer.SetBoolean("PolyE2", PolygonB);
            writer.SetBoolean("PolyE3", PolygonC);
            return base.Write(writer);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane plane = Plane.WorldXY;
            double radius = 0;
            bool seal = false;
            if (!DA.GetData(0, ref plane)) return;
            if (!DA.GetData(1, ref radius)) return;
            if (!DA.GetData(2, ref seal)) return;
            double xx = plane.OriginX;
            double yy = plane.OriginY;
            double zz = plane.OriginZ;
            ////////////////////////////相对平面坐标
            string[] mm = str.Split(new string[] { "{", "}" }, StringSplitOptions.RemoveEmptyEntries);
            List<string> nn = mm.ToList();
            for (int i = 0; i < nn.Count; i++)//移除空项
            {
                if (nn[i].Length < 8)
                {
                    nn.RemoveAt(i);
                    i--;
                }
            }
            List<int> count = new List<int>();
            for (int i = 0; i < length.Length; i++)//点分组列表
            {
                count.Add(Convert.ToInt32(length[i].ToString()));
            }
            ///////////////////////////////////////////////////////////////////
            double rate = radius / Curvelength;//与默认半径的比例
            List<Point3d> pts = new List<Point3d>();
            for (int i = 0; i < nn.Count; i++)
            {
                string[] pt = nn[i].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                double ptx = Convert.ToDouble(pt[0]) * rate + xx;
                double pty = Convert.ToDouble(pt[1]) * rate + yy;
                double ptz = Convert.ToDouble(pt[2]) * rate + zz;
                Point3d point = new Point3d(ptx, pty, ptz);
                pts.Add(point);
            }
            ////////////////////////////////////////////////////////////////////获取点
            List<Polyline> ply = new List<Polyline>();
            List<Brep> bps = new List<Brep>();
            for (int i = 0; i < count.Count; i++)
            {
                List<Point3d> pts2 = new List<Point3d>();
                for (int j = 0; j < count[i]; j++)
                {
                    pts2.Add(pts[0]);
                    pts.RemoveAt(0);
                }
                pts2.Add(pts2[0]);
                Polyline pl = new Polyline(pts2);
                ply.Add(pl);
                if (seal)
                {
                    bps.Add(Brep.CreatePlanarBreps(pl.ToNurbsCurve())[0]);
                }
            }
            if (seal)
            {
                DA.SetDataList(1, bps);
            }
            DA.SetDataList(0, ply);
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
                return Resource1.surface_多面体E;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{9ffd3959-e20e-4540-86f3-8141119581be}"); }
        }
    }
}