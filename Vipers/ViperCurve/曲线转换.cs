using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Drawing;
using System.Windows.Forms;

namespace Vipers///TangChi 2015.12.21
{
    public class CurveExchange : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CurveExchange class.
        /// </summary>
        public CurveExchange()
            : base("曲线转换", "CurveExchange",
                "将曲线转换成圆弧，圆，椭圆或多段线，右键可选择转换类型(直接调用rhinocommon方法，不推荐)",
                "Vipers", "Viper.curve")
        {
        }
        public override GH_Exposure Exposure 
        {
            get { return GH_Exposure.hidden; }
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("曲线","C","待转换的曲线",GH_ParamAccess.item);
            pManager.HideParameter(0);
            Message = "曲线→弧线";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("转变","C","转变后的曲线",GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        /// 
        private bool arc = true;
        public bool Arc
        {
            get { return arc; }
            set { 
                arc = value;
            if(arc)
            {
                Message = "曲线→弧线";
            }
            }
        }

        private bool circle = false;
        public bool Circle
        {
            get { return circle; }
            set {
                circle = value;
                if (circle)
                {
                    Message = "曲线→圆";
                }
            }
        }

        private bool elli = false;
        public bool Elli
        {
            get { return elli; }
            set { 
                elli = value;
                if (elli)
                {
                    Message = "曲线→椭圆";
                }
            }
        }
        private bool poly = false;

        public bool Poly
        {
            get { return poly; }
            set { 
                poly = value;
                if (poly)
                {
                    Message = "曲线→多段线";
                }
            }
        }
        private int flag = 1;
        public int Flag
        {
            get { return flag; }
            set { flag = value; }
        }

        Color cor1 = Color.FromArgb(61, 200, 44);
        Color cor2 = Color.FromArgb(61, 170, 44);
        Color cor3 = Color.FromArgb(61, 140, 44);
        Color cor4 = Color.FromArgb(61, 110, 44);
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            // Append the item to the menu, making sure it's always enabled and checked if Absolute is True.
            ToolStripMenuItem item1 = Menu_AppendItem(menu, "曲线转换为弧线", Menu_AbsoluteClicked1, true, arc);
            ToolStripMenuItem item2 = Menu_AppendItem(menu, "曲线转换为圆", Menu_AbsoluteClicked2, true, circle);
            ToolStripMenuItem item3 = Menu_AppendItem(menu, "曲线转换为椭圆", Menu_AbsoluteClicked3, true, elli);
            ToolStripMenuItem item4 = Menu_AppendItem(menu, "曲线转换为多段线", Menu_AbsoluteClicked4, true,poly);
            item1.BackColor = cor1;
            item2.BackColor = cor2;
            item3.BackColor = cor3;
            item4.BackColor = cor4;
        }
        private void Menu_AbsoluteClicked1(object sender, EventArgs e)
        {
            RecordUndoEvent("Absolute");
           Arc = true;
            Circle = false;
            Elli = false;
            Poly = false;
            Flag = 1;
            ExpireSolution(true);
        }
        private void Menu_AbsoluteClicked2(object sender, EventArgs e)
        {
            RecordUndoEvent("Absolute");
            Arc = false;
            Circle = true;
            Elli = false;
            Poly = false;
            flag = 2;
            ExpireSolution(true);
        }
        private void Menu_AbsoluteClicked3(object sender, EventArgs e)
        {
            RecordUndoEvent("Absolute");
            Arc = false;
            Circle = false;
            Elli = true ;
            Poly = false;
           Flag = 3;
            ExpireSolution(true);
        }
        private void Menu_AbsoluteClicked4(object sender, EventArgs e)
        {
            RecordUndoEvent("Absolute");
            Arc =false;
            Circle = false;
            Elli = false;
            Poly = true;
            Flag = 4;
            ExpireSolution(true);
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve c = null;
            if(!DA.GetData(0,ref c))return;
            double t = c.GetLength() / 300;
            double check = t;
            bool fg = false;
            switch (flag)
            {
                case 1:
                    Rhino.Geometry.Arc arcs = new Arc();
                    for (int i = 0; i < 301; i++)//循环300次
                    {
                        fg = c.TryGetArc(out arcs, check);
                        if (fg)
                        {
                            DA.SetData(0, arcs);
                            return;
                        }
                        check += t;
                    } 
                    return;
                case 2:
                    Rhino.Geometry.Circle circles = new Rhino.Geometry.Circle();
                    for (int i = 0; i < 301; i++)//循环300次
                    {
                        fg = c.TryGetCircle(out circles, check);
                        if (fg)
                        {
                            DA.SetData(0, circles);
                            return;
                        }
                        check +=t;
                    } 
                    return;
                case 3:
                    Rhino.Geometry.Ellipse ellipse = new Ellipse();
                    for (int i = 0; i < 301; i++)//循环300次
                    {
                        fg = c.TryGetEllipse(out ellipse, check);
                        if (fg)
                        {
                            DA.SetData(0, ellipse);
                            return;
                        }
                        check +=t;
                    } 
                    return;
                case 4:
                    double minseg = c.GetLength() / 400;
                    double maxseg = c.GetLength() / 10;
                    PolylineCurve pl = c.ToPolyline(0, 0, Math.PI, 1, 1, 1, minseg, maxseg, true);
                    DA.SetData(0, pl);
                    return;
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
                return Resource1.curve_曲线转换;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{d29f681d-f195-411a-ab17-237251a121f7}"); }
        }
    }
}