using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers////TangChi 2018.8.6
{
    public class BoxInformation : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent34 class.
        /// </summary>
        public BoxInformation()
            : base("物体外包围盒信息", "BoxInformation",
                "提取物体外包围盒（boundingbox）及其长，宽，高，顶面中点，底面中点",
                 "Vipers", "Viper.surface")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("物件","B","被boundingbox包裹的物体",GH_ParamAccess.item);
            pManager.AddPlaneParameter("参考平面","P","设置参考平面",GH_ParamAccess.item,Plane.WorldXY);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
            Message = "物体外包围盒信息";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBoxParameter("包围盒","B","物体的外包围盒",GH_ParamAccess.item);
            pManager.AddNumberParameter("长度","L","包围盒的长度",GH_ParamAccess.item);
            pManager.AddNumberParameter("宽度", "W", "包围盒的宽度", GH_ParamAccess.item);
            pManager.AddNumberParameter("高度", "H", "包围盒的高度", GH_ParamAccess.item);
            pManager.AddPointParameter("顶面中点","P","包围盒的顶面中点",GH_ParamAccess.item);
            pManager.AddPointParameter("底面中点", "P", "包围盒的底面中点", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Brep brep=new Brep();
            Plane plane = new Plane();
            if(!DA.GetData(0,ref brep)) return;
            if (!DA.GetData(1, ref plane)) return;
            Box bb;
            brep.GetBoundingBox(plane, out bb);
            Point3d[] pts = bb.GetCorners();
            Point3d origin = pts[0];
            Point3d ptx = pts[1];
            Point3d pty = pts[3];
            Point3d ptz = pts[4];
            double x1 = pts[0].X + pts[1].X + pts[2].X + pts[3].X;
            double y1 = pts[0].Y + pts[1].Y + pts[2].Y + pts[3].Y;
            double z1 = pts[0].Z + pts[1].Z + pts[2].Z + pts[3].Z;
            double x2 = pts[4].X + pts[5].X + pts[6].X + pts[7].X;
            double y2 = pts[4].Y + pts[5].Y + pts[6].Y + pts[7].Y;
            double z2 = pts[4].Z + pts[5].Z + pts[6].Z + pts[7].Z;
            DA.SetData(0, bb);
            DA.SetData(1, origin.DistanceTo(ptx));
            DA.SetData(2, origin.DistanceTo(pty));
            DA.SetData(3, origin.DistanceTo(ptz));
            DA.SetData(4, new Point3d(x2 / 4, y2 / 4, z2 / 4));
            DA.SetData(5, new Point3d(x1 / 4, y1 / 4, z1 / 4));
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
                //return Resource1.物体外包围盒信息;
                return Resource1.surface_外包围盒信息;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{ffb8f8f6-60a2-40db-8f83-955808392b9a}"); }
        }
    }
}