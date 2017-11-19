using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers/////TangChi 2015.9.10
{
    public class ReverseAxis : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent42 class.
        /// </summary>
        public ReverseAxis()
            : base("反转平面XYZ轴方向", "ReverseAxis",
                "通过设置布尔值反转平面XYZ轴",
                "Vipers", "Viper.point")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("平面","P","待切换的平面",GH_ParamAccess.item,Plane.WorldXY);
            pManager.AddBooleanParameter("反转X轴","X","反转平面X轴的方向",GH_ParamAccess.item,false);
            pManager.AddBooleanParameter("反转Y轴", "Y", "反转平面Y轴的方向", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("反转Z轴", "Z", "反转平面Z轴的方向", GH_ParamAccess.item, false);
            pManager.HideParameter(0);
            Message = "反转平面XYZ轴";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPlaneParameter("平面","P","切换后的平面",GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            Plane plane=new Plane();
            bool reverseX=false;
            bool reverseY=false;
            bool reverseZ=false;
            if(!DA.GetData(0,ref plane))return;
            if (!DA.GetData(1, ref reverseX)) return;
            if (!DA.GetData(2, ref reverseY)) return;
            if (!DA.GetData(3, ref reverseZ)) return;
            Plane p = plane;
            bool x = reverseX;
            bool y = reverseY;
            bool z = reverseZ;
            Vector3d vx = p.XAxis;
            Vector3d vy = p.YAxis;
            Point3d pt = p.Origin;
            Plane p2 = p;
            if (x)
            {
                vx.Reverse();
                p2 = new Plane(pt, vx, vy);
                p2.Flip();
            }
            if (y)
            {
                vy.Reverse();
                p2 = new Plane(pt, vx, vy);
                p2.Flip();
            }
            if (x && y)
            {
                p2.Flip();
            }
            if (z)
            {
                p2.Flip();
            }
            DA.SetData(0, p2);
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
                //return Resource1.反转平面XYZ轴;
                return Resource1.point_反转平面xyz轴;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{bf6c6cf2-266b-47e6-b375-524e92a12dd2}"); }
        }
    }
}