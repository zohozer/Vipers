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
    public class PointColour : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public PointColour()
            : base("Colour Ponts", "PointColour",
                "Displays custom colours for the points",
                "Vipers", "Viper.tint")
        {
            Message = "ControlPoint";
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "P", "Points to colour", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Radius", "R", "Radius of points", GH_ParamAccess.item, 5);
            pManager.AddColourParameter("Colour", "C", "Colours to display", GH_ParamAccess.item, Color.LightGreen);
            pManager.AddIntegerParameter("Seed", "S", "Seed of random colours for each branch", GH_ParamAccess.item, 3);
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
            GH_Structure<GH_Point> pts = new GH_Structure<GH_Point>();
            int thickness = 0;
            Color color = Color.Pink;
            int seed = 0;
            if (!DA.GetDataTree(0, out pts)) return;
            if (!DA.GetData(1, ref thickness)) return;
            if (!DA.GetData(2, ref color)) return;
            if (!DA.GetData(3, ref seed)) return;
            trees = pts;
            w = thickness;
            c = color;
            if (trees.PathCount >= 1)
            {
                List<Color> colors = new List<Color>();
                Random rand = new Random(seed);
                int aaa = 0;
                int bbb = 0;
                int ccc = 0;
                for (int i = 0; i < trees.PathCount; i++)
                {
                    aaa = rand.Next(0, 255);
                    bbb = rand.Next(0, 225);
                    ccc = rand.Next(0, 255);
                    colors.Add(Color.FromArgb(aaa, bbb, ccc));
                }
                cos = colors;
            }
        }
        List<GH_Point> all = new List<GH_Point>();
        List<Color> cos = new List<Color>();
        int w = 1;
        Color c = Color.Pink;
        GH_Structure<GH_Point> trees = new GH_Structure<GH_Point>();
        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            if (trees.PathCount == 0)
            {
                return;
            }
            if (this.Attributes.Selected)////////////////////////电池选中时的点颜色
            {
                for (int i = 0; i < trees.PathCount; i++)
                {
                    all = trees.Branches[i];
                    for (int k = 0; k < all.Count; k++)
                    {
                        args.Display.DrawPoint(all[k].Value, style, w,args.WireColour_Selected);
                    }
                }
                return;
            }
            if (trees.PathCount == 1)
            {
                all = trees.Branches[0];
                for (int i = 0; i < all.Count; i++)
                {
                    args.Display.DrawPoint(all[i].Value, style, w, c);
                }
            }
            else
            {
                for (int i = 0; i < trees.PathCount; i++)
                {
                    all = trees.Branches[i];
                    for (int k = 0; k < all.Count; k++)
                    {
                        args.Display.DrawPoint(all[k].Value, style, w, cos[i]);
                    }
                }
            }
        }

        bool abs1 = false;

        public bool Abs1
        {
            get { return abs1; }
            set
            { 
                abs1 = value;
                if ((abs1))
                {
                    Message = "SimplePoint";
                }
            }
        }
        bool abs2 =true;

        public bool Abs2
        {
            get { return abs2; }
            set 
            { 
                abs2 = value;
                if ((abs2))
                {
                    Message = "ControlPoint";
                }
            }
        }
        bool abs3 = false;

        public bool Abs3
        {
            get { return abs3; }
            set 
            {
                abs3 = value;
                if ((abs3))
                {
                    Message = "ActivePoint";
                }
            }
        }

        public override bool Read(GH_IReader reader)
        {
            Abs1 = reader.GetBoolean("Abs1");
            Abs2 = reader.GetBoolean("Abs2");
            Abs3 = reader.GetBoolean("Abs3");
            return base.Read(reader);
        }
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("Abs1", Abs1);
            writer.SetBoolean("Abs2", Abs2);
            writer.SetBoolean("Abs3", Abs3);
            return base.Write(writer);
        } 

        Rhino.Display.PointStyle style = Rhino.Display.PointStyle.ControlPoint;/////默认点样式

        Color cor1 = Color.FromArgb(61, 200, 44);
        Color cor2 = Color.FromArgb(61, 150, 44);
        Color cor3 = Color.FromArgb(61, 100, 44);
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            // Append the item to the menu, making sure it's always enabled and checked if Absolute is True.
            ToolStripMenuItem item = Menu_AppendItem(menu, "SimplePoint", Menu_AbsoluteClicked, true, Abs1);
            ToolStripMenuItem item2 = Menu_AppendItem(menu, "ControlPoint", Menu_AbsoluteClicked2, true, Abs2);
            ToolStripMenuItem item3 = Menu_AppendItem(menu, "ActivePoint", Menu_AbsoluteClicked3, true, Abs3);
            // Specifically assign a tooltip text to the menu item.
            item.ToolTipText = "Style: Simple";
            item.BackColor = cor1;
            item2.ToolTipText = "Style: ControlPoint";
            item2.BackColor = cor2;
            item3.ToolTipText = "Style: ActivePoint";
            item3.BackColor = cor3;
        }
        private void Menu_AbsoluteClicked(object sender, EventArgs e)
        {
            RecordUndoEvent("PTC");
            Abs1 = true;
            Abs2 = false;
            Abs3 = false;
            Message = "SimplePoint";
            style = Rhino.Display.PointStyle.Simple;
            ExpireSolution(true);
        }
        private void Menu_AbsoluteClicked2(object sender, EventArgs e)
        {
            RecordUndoEvent("PTC");
            Abs1 = false;
            Abs2 = true;
            Abs3 = false;
            Message = "ControlPoint";
            style = Rhino.Display.PointStyle.ControlPoint;
            ExpireSolution(true);
        }
        private void Menu_AbsoluteClicked3(object sender, EventArgs e)
        {
            RecordUndoEvent("PTC");
            Abs1 = false;
            Abs2 = false;
            Abs3= true ;
            Message = "ActivePoint";
            style=Rhino.Display.PointStyle.ActivePoint;
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
                return Resource1.tint_点云上色;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{745edefe-133e-4085-aa4c-49c8c7907d6b}"); }
        }
    }
}