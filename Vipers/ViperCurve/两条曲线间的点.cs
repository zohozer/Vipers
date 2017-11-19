 using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using AnalysisComponents;
using System.Windows.Forms;
using System.Drawing;


namespace Vipers
{
    public class TwoCurveSide : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the 两条曲线间的点 class.
        /// </summary>
        public TwoCurveSide()
            : base("两条曲线间的点", "TwoCurveSide",
                "判断指定点是否位于两条曲线之间。true：在两条曲线间，false：不在(右键选择是否把在曲线上的点计入两曲线之间)",
                "Vipers", "viper.curve")
        {
            Message = "only between";
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("曲线1","C1","第一条曲线",GH_ParamAccess.item);
            pManager.AddCurveParameter("曲线2", "C2", "第二条曲线", GH_ParamAccess.item);
            pManager.AddPointParameter("点","Pt","判断这些点是否在两曲线之间",GH_ParamAccess.list);
            pManager.AddPlaneParameter("平面","Pl","参考平面",GH_ParamAccess.item,Plane.WorldXY);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
            pManager.HideParameter(2);
            pManager.HideParameter(3);
        }
        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("点","P","在两条曲线之间的点",GH_ParamAccess.list);
            pManager.AddBooleanParameter("结果","R","判断结果，如果在曲线之间为true，其余为false",GH_ParamAccess.list);
        }
        private bool between=true;//////不包括在曲线上的点
        public bool Between
        {
            get { return between; }
            set
            { 
                between = value; 
                if(Between)
                    Message="only between";
            }
        }
        private bool coincident=false;////////在曲线上的点
        public bool Coincident
        {
            get { return coincident; }
            set
            {
                coincident = value;
                if (Coincident)
                    Message = "coincident";
            }
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            Between = reader.GetBoolean("between");
            Coincident = reader.GetBoolean("coincident");
            return base.Read(reader);
        }
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            writer.SetBoolean("between",Between);
            writer.SetBoolean("coincident",Coincident);
            return base.Write(writer);
        }
        Color cor1 = Color.FromArgb(61, 200, 44);
        Color cor2 = Color.FromArgb(61, 150, 44);
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            ToolStripMenuItem item = Menu_AppendItem(menu, "only between", Menu_AbsoluteClicked, true, Between);
            ToolStripMenuItem item2 = Menu_AppendItem(menu, "coincident", Menu_AbsoluteClicked2, true, Coincident);
            item.ToolTipText = "两条曲线间的点，不包括在曲线上的点";
            item.BackColor = cor1;
            item2.ToolTipText = "两条曲线间的点，包括在曲线上的点";
            item2.BackColor = cor2;
        }
        private void Menu_AbsoluteClicked(object sender, EventArgs e)
        {
            RecordUndoEvent("between");
            Between = true;
            Coincident = false;
            ExpireSolution(true);
        }
        private void Menu_AbsoluteClicked2(object sender, EventArgs e)
        {
            RecordUndoEvent("coincident");
            Between = false;
            Coincident = true;
            ExpireSolution(true);
        }
        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
         protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve c1 = null;
            Curve c2 = null;
            List<Point3d> pts = new List<Point3d>();
            Plane pln = Plane.Unset;
            if(!DA.GetData<Curve>(0,ref c1)||!DA.GetData<Curve>(1,ref c2)||!DA.GetDataList<Point3d>(2,pts)||!DA.GetData<Plane>(3,ref pln))return;
            List<Point3d> last = new List<Point3d>();
            List<bool> last2 = new List<bool>();
             foreach(Point3d pt in pts )
             {
                 if (Between)
                 {
                     if ((ViperClass.CurveSide(c1, pt, pln) == -1 && ViperClass.CurveSide(c2, pt, pln) == 1) || (ViperClass.CurveSide(c1, pt, pln) == 1 && ViperClass.CurveSide(c2, pt, pln) == -1))
                     {
                         last.Add(pt);
                         last2.Add(true);
                     }
                     else
                         last2.Add(false);
                 }
                 else
                 {
                     if ((ViperClass.CurveSide(c1, pt, pln) == -1 && ViperClass.CurveSide(c2, pt, pln) == 1) || (ViperClass.CurveSide(c1, pt, pln) == 1 && ViperClass.CurveSide(c2, pt, pln) == -1) || ViperClass.CurveSide(c1, pt, pln) == 0 || ViperClass.CurveSide(c2, pt, pln) == 0)
                     {
                         last.Add(pt);
                         last2.Add(true);
                     }
                     else
                         last2.Add(false);
                 }
             }
             DA.SetDataList(0, last);
             DA.SetDataList(1, last2);
        }
         protected override void AfterSolveInstance()
         {
            
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
                return Resource1.curve_两条曲线间的点;
            }
        }
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{330fa5c2-08da-47b6-a03b-d2827487041b}"); }
        }
    }
}