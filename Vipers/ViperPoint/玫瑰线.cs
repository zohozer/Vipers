using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers///TangChi 2015.5.18
{
    public class RoseCurve : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent24 class.
        /// </summary>
        public RoseCurve()
            : base("玫瑰线", "RoseCurve",
                "根据半径，瓣数绘制玫瑰线",
                "Vipers", "Viper.point")
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
            pManager.AddNumberParameter("半径", "R", "玫瑰线半径", GH_ParamAccess.item,20);
            pManager.AddIntegerParameter("数量", "N", "叶子数量", GH_ParamAccess.item,5);
            pManager.AddNumberParameter("精细程度","D","数值越大越不精细",GH_ParamAccess.item,1);
            Message = "玫瑰线";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("生成点","P","生成的玫瑰线点",GH_ParamAccess.list);
        }
        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double radius = 0;
            int number = 0;
            double degree = 0;
            if(!DA.GetData(0,ref radius)) return;
            if (!DA.GetData(1, ref number)) return;
            if (!DA.GetData(2, ref degree)) return;
            List<Point3d> pts = new List<Point3d>();
            for (double i = 0; i <= 360; i += degree)
            {
                double nn = i / (180 / 3.1415926);
                double xx = radius * Math.Sin(number * nn) * Math.Cos(nn);
                double yy = radius * Math.Sin(number * nn) * Math.Sin(nn);
                Point3d pt = new Point3d(xx, yy, 0);
                pts.Add(pt);
            }
            DA.SetDataList(0, pts);
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
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{9e66af57-c113-47da-a6a8-a9f47c3f1acf}"); }
        }
    }
}