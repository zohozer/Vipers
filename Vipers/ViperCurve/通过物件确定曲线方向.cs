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


namespace Vipers////////TangChi 2016.10.13
{
    public class CurveToGeometry : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public CurveToGeometry()
            : base("通过物件确定曲线方向", "CurveToGeometry",
                "将曲线最接近的物件的一端作为曲线的起点或终点",
                "Vipers", "Viper.curve")
        {
            Message = "通过物件确定曲线方向";
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("曲线","C","待确定方向的曲线",GH_ParamAccess.item);
            pManager.AddGeometryParameter("物件","G","一组影响曲线方向的物件（点，线，面，体及mesh）",GH_ParamAccess.list);
            pManager.AddBooleanParameter("反转","R","true：则靠近物件的一端为终点 。false：靠近物件的一端为起点",GH_ParamAccess.item,false);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("曲线","C","确定方向后的曲线",GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve cs = null;
            List<IGH_GeometricGoo> gs = new List<IGH_GeometricGoo>();
            bool re = false;
            if(!DA.GetData(0,ref cs)||!DA.GetDataList(1,gs)||!DA.GetData(2,ref re))return;
            double distance = double.MaxValue;
            Point3d pt1 = cs.PointAtStart;
            Point3d pt2 = cs.PointAtEnd;
            for (int i = 0; i < gs.Count; i++)
            {
                if (gs[i] is GH_Curve)
                {
                    GH_Curve ccc = (GH_Curve)gs[i];
                    Curve ccc2 = ccc.Value;
                    double t1 = 0;
                    double t2 = 0;
                    ccc2.ClosestPoint(pt1, out t1);
                    ccc2.ClosestPoint(pt2, out t2);
                    Point3d test1 = ccc2.PointAt(t1);
                    Point3d test2 = ccc2.PointAt(t2);
                    double distanceA = test1.DistanceTo(pt1);
                    double distanceB = test2.DistanceTo(pt2);
                    if (distance > distanceA)
                    {
                        distance = distanceA;
                    }
                    if (distance > distanceB)
                    {
                        distance = distanceB;
                        cs.Reverse();
                    }
                }
                else if (gs[i] is GH_Brep)
                {
                    GH_Brep bbb = (GH_Brep)gs[i];
                    Brep bbb2 = bbb.Value;
                    Point3d test1 = bbb2.ClosestPoint(pt1);
                    Point3d test2 = bbb2.ClosestPoint(pt2);
                    double distanceA = test1.DistanceTo(pt1);
                    double distanceB = test2.DistanceTo(pt2);
                    if (distance > distanceA)
                    {
                        distance = distanceA;
                    }
                    if (distance > distanceB)
                    {
                        distance = distanceB;
                        cs.Reverse();
                    }
                }
                else if (gs[i] is GH_Point)
                {
                    GH_Point ppp = (GH_Point)gs[i];
                    Point3d ppp2 = ppp.Value;
                    double distanceA = ppp2.DistanceTo(pt1);
                    double distanceB = ppp2.DistanceTo(pt2);
                    if (distance > distanceA)
                    {
                        distance = distanceA;
                    }
                    if (distance > distanceB)
                    {
                        distance = distanceB;
                        cs.Reverse();
                    }
                }
                else if (gs[i] is GH_Mesh)
                {
                    GH_Mesh mmm = (GH_Mesh)gs[i];
                    Mesh mmm2 = mmm.Value;
                    Point3d test1 = mmm2.ClosestPoint(pt1);
                    Point3d test2 = mmm2.ClosestPoint(pt2);
                    double distanceA = test1.DistanceTo(pt1);
                    double distanceB = test2.DistanceTo(pt2);
                    if (distance > distanceA)
                    {
                        distance = distanceA;
                    }
                    if (distance > distanceB)
                    {
                        distance = distanceB;
                        cs.Reverse();
                    }
                }
                pt1 = cs.PointAtStart;
                pt2 = cs.PointAtEnd;
            }
            if (re) cs.Reverse();
            DA.SetData(0, cs);        
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
                return Resource1.curve_通过物件确定曲线方向;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{06dea369-3481-4b8c-87cc-91378401e21a}"); }
        }
    }
}