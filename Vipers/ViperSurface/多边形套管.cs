using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers/////TangChi 2015.7.27
{
    public class PolygonPipe : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent30 class.
        /// </summary>
        public PolygonPipe()
            : base("Polygonal Pipe", "PolygonPipe",
                "Create polygonal pipe from curve",
                "Vipers", "Viper.surface")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve","C","Base curve",GH_ParamAccess.item);
            pManager.AddNumberParameter("Radius","R","Pipe radius",GH_ParamAccess.item,20);
            pManager.AddIntegerParameter("Sides", "S", "Number of sides", GH_ParamAccess.item,6);
            pManager.AddNumberParameter("Angle","A","Rotation angle (radians)",GH_ParamAccess.item,0);
            pManager.HideParameter(0);
            Message = "Polygonal Pipe";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Pipe","P","Resulting pipe",GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve railCurve=null;
            double radius=0;
            int segments=0;
            double angle = 0;
            if(!DA.GetData(0,ref railCurve)) return;
            if (!DA.GetData(1, ref radius)) return;
            if (!DA.GetData(2, ref segments)) return;
            if(!DA.GetData(3,ref angle))return;
            ///////////////////////////////////////////////////
            Curve x = railCurve;
            double y = radius;
            int n = segments;
            Vector3d vc = x.TangentAtStart;
            Plane pl = new Plane(x.PointAtStart, vc);
            Polyline ply = ViperClass.CreatePolyline(pl, y, n);
            ply.Transform(Transform.Rotation(angle,pl.ZAxis,pl.Origin));
            NurbsCurve nc = ply.ToNurbsCurve();
            SweepOneRail swp = new SweepOneRail();
            Brep[] bs = swp.PerformSweep(x, nc);
            DA.SetDataList(0, bs);
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
                //return Resource1.多边形套管;
                return Resource1.surface_套管;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{6abee009-9742-45da-9347-1c8b14d6320f}"); }
        }
    }
}