using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers/////TangChi 2015.11.10
{
    public class 获取犀牛空间注释 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the 获取犀牛空间文字 class.
        /// </summary>
        public 获取犀牛空间注释()
            : base("获取犀牛空间文字", "RhinoAnnotation",
                "通过拾取ID，获取犀牛空间中的文字或注解点",
                "Vipers", "Viper.math")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("ID", "ID", "获取犀牛空间中文字或注解点的ID", GH_ParamAccess.item);
            Message = "获取犀牛空间文字";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("文字","T","提取文字",GH_ParamAccess.item);
            pManager.AddGenericParameter("基准平面", "O", "文字的基准平面或注解点的中点", GH_ParamAccess.item);
            pManager.AddNumberParameter("字高","H","文字高度",GH_ParamAccess.item);
            pManager.AddCurveParameter("文字曲线","C","文字外轮廓对应的曲线(当输入的是文字时)",GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string ID = null;
            if(!DA.GetData(0,ref ID))return;
            Guid gd = new Guid(ID);
            if (Rhino.RhinoDoc.ActiveDoc.Objects.Find(gd) is Rhino.DocObjects.TextObject)
            {
                Rhino.DocObjects.TextObject obj = Rhino.RhinoDoc.ActiveDoc.Objects.Find(gd) as Rhino.DocObjects.TextObject;
                TextEntity tx = obj.TextGeometry;
                Plane pln = tx.Plane;
                string mm = obj.DisplayText;
               DA.SetData(0, mm);
                DA.SetData(1, pln);
                DA.SetData(2, tx.TextHeight);
                DA.SetDataList(3, tx.Explode());
            }
            else if (Rhino.RhinoDoc.ActiveDoc.Objects.Find(gd) is Rhino.DocObjects.TextDotObject)
            {
                Rhino.DocObjects.TextDotObject obj = Rhino.RhinoDoc.ActiveDoc.Objects.Find(gd) as Rhino.DocObjects.TextDotObject;
                TextDot td = (TextDot)obj.Geometry;
               DA.SetData(0, td.Text);
                DA.SetData(1, td.Point);
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
                return Resource1.math_犀牛空间文字;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{bcbc81e1-d512-47b4-88ce-eab21b20d224}"); }
        }
    }
}