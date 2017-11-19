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
using Rhino;
using Rhino.Geometry;

namespace Vipers///////TangChi 2016.5.10
{
    public class PolylineFromEdge : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent6 class.
        /// </summary>
        public PolylineFromEdge()
            : base("根据边长确定多边形", "PolylineFromEdge",
                "根据输入的边长和边数确定指定平面上的多边形",
                "Vipers", "Viper.curve")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary | GH_Exposure.obscure; }
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("平面","P","参考平面",GH_ParamAccess.item,Rhino.Geometry.Plane.WorldXY);
            pManager.AddNumberParameter("边长","L","多边形边长",GH_ParamAccess.item,100);
            pManager.AddIntegerParameter("边数","N","多边形边数",GH_ParamAccess.item,6);
            pManager.AddNumberParameter("角度(弧度数)","A","多边形在参考平面的旋转角度",GH_ParamAccess.item,0);
            pManager.HideParameter(0);
            Message = "根据边长确定多边形";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("多边形","C","根据条件生成的多边形",GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane p=Plane.Unset;
            double l=0;
            int n=6;
            double a=0;
            if(!DA.GetData(0,ref p ))return;
            if(!DA.GetData(1,ref l))return;
            if(!DA.GetData(2,ref n))return;
            if(!DA.GetData(3,ref a))return;
            if (n <= 2) return;
            double angle = Math.PI / n;
            double r = l / 2 / Math.Sin(angle);
            Circle ccc = new Circle(p, r);
            Point3d[] pts;
            ccc.ToNurbsCurve().DivideByCount(n, true, out pts);
            List<Point3d> last = pts.ToList();
            last.Add(last[0]);
            Polyline poly = new Polyline(last);
            poly.Transform(Transform.Rotation(a, p.ZAxis, p.Origin));
            DA.SetData(0, poly);
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
                return Resource1.curve_边长确定多边形;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{0d5142ed-1a4f-4c84-b1f2-39e00a865a4b}"); }
        }
    }
}