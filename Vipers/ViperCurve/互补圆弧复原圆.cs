using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers/////TangChi 2015.9.7
{
    public class ComplementaryCircle : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent40 class.
        /// </summary>
        public ComplementaryCircle()
            : base("互补圆弧与复原圆", "ComplementaryCircle",
                "通过输入的已知圆弧找出互补圆弧以及其对应的圆",
                "Vipers", "Viper.curve")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddArcParameter("圆弧","A","已知圆弧",GH_ParamAccess.item);
            pManager.HideParameter(0);
            Message = "互补圆弧&复原圆";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddArcParameter("互补圆弧","C","与已知圆弧能形成整圆的圆弧",GH_ParamAccess.item);
            pManager.AddCircleParameter("复原圆","C","已知圆弧对应的圆",GH_ParamAccess.item);
            pManager.AddPlaneParameter("平面", "P", "已知圆弧所在的平面", GH_ParamAccess.item);
            pManager.AddNumberParameter("半径", "R", "已知圆弧的半径", GH_ParamAccess.item);
            pManager.AddNumberParameter("弧度数", "R", "已知圆弧的弧度数", GH_ParamAccess.item);
            pManager.HideParameter(1);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Arc arc = new Arc();
            if(!DA.GetData(0,ref arc)) return;
            Plane pln = arc.Plane;///参考平面
            Point3d center = arc.Center;////中点
            double radius2 = arc.Radius;////半径
            double angle2 = arc.Angle;////给定圆弧的角度
            Circle ccc = new Circle(pln, center, radius2);////圆弧对应的圆
            Vector3d vc = Point3d.Subtract(center, arc.MidPoint);
            Line line = new Line(center, vc);
            Point3d ptc = line.To;
            Arc another = new Arc(arc.EndPoint, ptc, arc.StartPoint);
            DA.SetData(0, another);
            DA.SetData(1, ccc);
            DA.SetData(2, pln);
            DA.SetData(3, radius2);
            DA.SetData(4,angle2);
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
                //return Resource1.互补圆弧_复原圆;
                return Resource1.curve_创建互补圆;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{5918626c-c832-4496-acf9-34f633876360}"); }
        }
    }
}