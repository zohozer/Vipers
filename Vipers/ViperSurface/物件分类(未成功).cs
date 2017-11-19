using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers
{
    public class MyComponent57 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent57 class.
        /// </summary>
        public MyComponent57()
            : base("物件分类", "GeometryClassify",
                    "将物件细分为不同种类",
                     "Vipers", "Viper.surface")
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
            pManager.AddGeometryParameter("物件", "geometry", "待分类的物件", GH_ParamAccess.list);
            pManager.HideParameter(0);
            Message = "物件分类";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("", "point", "", GH_ParamAccess.list);
            pManager.AddBrepParameter("", "surface", "", GH_ParamAccess.list);
            pManager.AddBrepParameter("", "solid", "", GH_ParamAccess.list);
            pManager.AddMeshFaceParameter("", "mesh", "", GH_ParamAccess.list);
            pManager.AddCurveParameter("", "nurbs", "", GH_ParamAccess.list);
            pManager.AddCurveParameter("", "line", "", GH_ParamAccess.list);
            pManager.AddCurveParameter("", "polyline", "", GH_ParamAccess.list);
            pManager.AddCurveParameter("", "circle", "", GH_ParamAccess.list);
            pManager.AddCurveParameter("", "arc", "", GH_ParamAccess.list);
            pManager.AddCurveParameter("", "ellipse", "", GH_ParamAccess.list);
            pManager.AddGeometryParameter("", "others", "", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<object> geometry = new List<object>();
            if (!DA.GetDataList(0, geometry)) return;
            List<object> points = new List<object>();
            List<object> surfaces = new List<object>();
            List<object> solids = new List<object>();
            List<object> meshs = new List<object>();
            List<object> curves = new List<object>();
            List<object> lines = new List<object>();
            List<object> polylines = new List<object>();
            List<object> circles = new List<object>();
            List<object> arcs =new List<object>();
            List<object> ellipses =new List<object>();
            List<object> objs = new List<object>();
            for (int i = 0; i < geometry.Count; i++)
            {
                var syt = geometry[i];
                if (syt.GetType().Name=="GH_Point")//////Point3d
                {
                    points.Add(syt);
                    DA.SetDataList(0, points);
                    continue;
                }
                else if (syt.GetType().Name == "GH_Brep")///////Brep
                {
                    #region;
                    Brep bp = (Brep)syt;
                    if (bp.IsSolid)
                    {
                        solids.Add(syt);
                        DA.SetDataList(2, solids);
                        continue;
                    }
                    else if (bp.IsSurface)
                    {
                        surfaces.Add(syt);
                        DA.SetDataList(1, surfaces);
                        continue;
                    }
                    #endregion;
                }
                else if (syt.GetType().Name == "GH_Curve")///////Curve
                {
                    #region;
                    Curve c = (Curve)syt;
                    if (c.IsLinear())
                    {
                        lines.Add(syt);
                        DA.SetDataList(5, lines);
                        continue;
                    }
                    else if (c.IsPolyline())
                    {
                        polylines.Add(syt);
                        DA.SetDataList(6, polylines);
                        continue;
                    }
                    else if (c.IsCircle())
                    {
                        circles.Add(syt);
                        DA.SetDataList(7, circles);
                        continue;
                    }
                    else if (c.IsArc())
                    {
                        arcs.Add(syt);
                        DA.SetDataList(8, arcs);
                        continue;
                    }
                    else if (c.IsEllipse())
                    {
                        ellipses.Add(syt);
                        DA.SetDataList(9, ellipses);
                        continue;
                    }
                    else
                    {
                        curves.Add(syt);
                        DA.SetDataList(4, curves);
                        continue;
                    }
                    #endregion;
                }

                else if (syt.GetType().Name == "GH_Mesh")/////Mesh
                {
                    meshs.Add((Mesh)syt);
                    DA.SetDataList(3, meshs);
                    continue;
                }
                else
                {
                    objs.Add(syt);
                    DA.SetDataList(10, objs);
                }
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
                return Resource1.物件分类;
            }
        }
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{e2d2b867-a459-4cb1-8403-2af18f1ae1da}"); }
        }
    }
}