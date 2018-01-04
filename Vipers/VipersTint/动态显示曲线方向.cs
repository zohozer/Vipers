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
    public class CurveDirection : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the 动态显示曲线方向 class.
        /// </summary>
        public CurveDirection()
            : base("Curve Direction", "CurveDirection",
                "Displays the direction of curves (use timer for dynamic display, RMB to change number of arrows)",
                "Vipers", "Viper.tint")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves","C","Curves to display",GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Size","S","Size of arrows",GH_ParamAccess.item,30);
            Message = "26 Arrows";
            pManager.HideParameter(0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Structure<GH_Curve> curves = new GH_Structure<GH_Curve>();
            int size = 0;
            if (!DA.GetDataTree(0,out curves)) return;
            if(!DA.GetData(1,ref size))return;
            cs = curves;
            cuxi = size;
            if (curves.PathCount== 0)
            {
                return;
            }
            ///////////////////////////////////////////////////////////
            List<Color> cols = new List<Color>();
            for (int i = 0; i < mem + 1; i++)
            {
                if (R < 0)
                {
                    R += fuyuan;
                }
                if (G < 0)
                {
                    G += fuyuan;
                }
                if (B < 0)
                {
                    B += fuyuan;
                }
                cols.Add(Color.FromArgb(R, G, B));
                R -= 10;
                G -= 10;
                B -= 10;
            }
            List<Color> Reverse = reverse(cols, yidong);
            yidong++;
            quanju = Reverse;
            ///////////////////////////////////////////////////渐变色
        }
        int R = 250;
        int G = 250;
        int B = 250;
        int fuyuan = 255;
        int mem = 25;
        int cuxi = 2;
        int yidong = 0;
        GH_Structure<GH_Curve> cs = new GH_Structure<GH_Curve>();
        DataTree<Vector3d> vs = new DataTree<Vector3d>();
        DataTree<Point3d> all = new DataTree<Point3d>();
        List<Color> quanju = new List<Color>();
        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            int num = 0;
            for (int i = 0; i < cs.PathCount; i++)
            {
                for (int k = 0; k < cs.Branches[i].Count; k++)
                {
                    Point3d[] pts;
                    cs.Branches[i][k].Value.DivideByCount(mem, true, out pts);
                    List<Vector3d> collect = new List<Vector3d>();
                    for (int q = 0; q < pts.Length; q++)
                    {
                        double t = 0;
                        cs.Branches[i][k].Value.ClosestPoint(pts[q], out t);
                        Vector3d vc = cs.Branches[i][k].Value.TangentAt(t);
                        if(this.Attributes.Selected)
                        {
                            args.Display.DrawArrowHead(pts[q], vc, args.WireColour_Selected, 0, cuxi);
                        }
                        else
                        {
                        args.Display.DrawArrowHead(pts[q], vc, quanju[q], 0, cuxi);
                        }
                    }
                }
                num++;
            }
        }
        ///////////////////////移动列表x中的数据y次。
        public List<Color> reverse(List<Color> x, int y) //////////移动列表x中的数据y次。
        {
            Color[] copy = x.ToArray();
            List<Color> mems1 = copy.ToList();
            List<Color> mems2 = copy.ToList();
            int num = x.Count;
            y = y % num;
            mems1.RemoveRange(num - y, y);
            mems2.RemoveRange(0, num - y);
            List<Color> mem = new List<Color>();
            mem.AddRange(mems2);
            mem.AddRange(mems1);
            return mem;
        }
        private bool m_absolute1 = true;
        private bool m_absolute2 = false;
        private bool m_absolute3 = false;
        public bool Absolute1/////////////////稀疏
        {
            get { return m_absolute1; }
            set
            {
                m_absolute1 = value;
                if ((m_absolute1))
                {
                    Message = "26 Arrows";
                }
            }
        }
        public bool Absolute2///////////////普通
        {
            get { return m_absolute2; }
            set
            {
                m_absolute2 = value;
                if ((m_absolute2))
                {
                    Message = "50 Arrows";
                }
            }
        }
        public bool Absolute3///////////////致密
        {
            get { return m_absolute3; }
            set
            {
                m_absolute3 = value;
                if ((m_absolute3))
                {
                    Message = "74 Arrows";
                }
            }
        }
        Color cor1 = Color.FromArgb(61, 200, 44);
        Color cor2 = Color.FromArgb(61, 150, 44);
        Color cor3 = Color.FromArgb(61, 100, 44);
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            // Append the item to the menu, making sure it's always enabled and checked if Absolute is True.
            ToolStripMenuItem item = Menu_AppendItem(menu, "26 Arrows", Menu_AbsoluteClicked, true, Absolute1);
            ToolStripMenuItem item2 = Menu_AppendItem(menu, "50 Arrows", Menu_AbsoluteClicked2, true, Absolute2);
            ToolStripMenuItem item3 = Menu_AppendItem(menu, "74 Arrows", Menu_AbsoluteClicked3, true, Absolute3);
            // Specifically assign a tooltip text to the menu item.
            item.BackColor = cor1;
            item2.BackColor = cor2;
            item3.BackColor = cor3;
        }
        private void Menu_AbsoluteClicked(object sender, EventArgs e)
        {
            RecordUndoEvent("Absolute");
            Absolute1 = true;
            Absolute2 = false;
            Absolute3 = false;
            mem = 25;
            ExpireSolution(true);
        }
        private void Menu_AbsoluteClicked2(object sender, EventArgs e)
        {
            RecordUndoEvent("Absolute");
            Absolute2 = true;
            Absolute1 = false;
            Absolute3 = false;
            mem = 49;
            ExpireSolution(true);
        }
        private void Menu_AbsoluteClicked3(object sender, EventArgs e)
        {
            RecordUndoEvent("Absolute");
            Absolute3 = true;
            Absolute1 = false;
            Absolute2 = false;
            mem = 73;
            ExpireSolution(true);
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
                return Resource1.tint_曲线方向;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{c0cb58f4-be42-461b-a49f-dd3bf29b6ee5}"); }
        }
    }
}