using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Drawing;
using System.Windows.Forms;

namespace Vipers
{
    public class ChinaSimplifiedTraditional : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent53 class.
        /// </summary>
        public ChinaSimplifiedTraditional()
            : base("繁简转换", "SimplifiedTraditional",
                "通过设置布尔值切换繁体或简体，右键选择转换方式",
                "Vipers", "Viper.math")
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
            pManager.AddTextParameter("繁体或简体","T","待转换的字体",GH_ParamAccess.item);
            Message = "繁→简";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("简体或繁体", "T", "转换后的字体", GH_ParamAccess.item);
        }
        bool fanjian = true;

        public bool Fanjian
        {
            get { return fanjian; }
            set
            { 
                fanjian = value;
                if(fanjian==true)
                Message = "繁→简";
            }
        }
        bool jianfan = false;

        public bool Jianfan
        {
            get { return jianfan; }
            set
            { 
                jianfan = value;
                if(jianfan==true)
                Message = "简→繁";
            }
        }
        Color cor1 = Color.FromArgb(61, 200, 44);
        Color cor2 = Color.FromArgb(61, 150, 44);
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            // Append the item to the menu, making sure it's always enabled and checked if Absolute is True.
            ToolStripMenuItem item = Menu_AppendItem(menu, "繁→简", Menu_AbsoluteClicked, true, Fanjian);
            ToolStripMenuItem item2 = Menu_AppendItem(menu, "简→繁", Menu_AbsoluteClicked2, true, Jianfan);
            // Specifically assign a tooltip text to the menu item.
            item.BackColor = cor1;
            item2.BackColor = cor2;
        }
        private void Menu_AbsoluteClicked(object sender, EventArgs e)
        {
            RecordUndoEvent("Absolute");
            Fanjian = true;
            Jianfan = false;
            ExpireSolution(true);
        }
        private void Menu_AbsoluteClicked2(object sender, EventArgs e)
        {
            RecordUndoEvent("Absolute");
            Jianfan = true;
            Fanjian = false;
            ExpireSolution(true);
        }
        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string st = null;
            if(!DA.GetData(0,ref st))return;
            if (Jianfan)
                DA.SetData(0, SimplifiedTraditional.ToTraditional(st));
            else
                DA.SetData(0, SimplifiedTraditional.ToSimplified(st));
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
                //return Resource1.繁简转换;
                return Resource1.math_繁简转换;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{9d43d62b-83bb-40ca-9123-f4e5ee002fc6}"); }
        }
    }
}