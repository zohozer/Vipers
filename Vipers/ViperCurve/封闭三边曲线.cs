using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers///TangChi 2015.2.16
{
    public class closeThreeEdge : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public closeThreeEdge()
            : base("封闭三边曲线", "Close3-EdgeCurve",
                "将三条首尾连接的曲线闭合",
                "Vipers", "Viper.curve")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("曲线","C","待连接的曲线",GH_ParamAccess.list);
            pManager.AddNumberParameter("公差","T","在此范围内自动连接",GH_ParamAccess.item,0);
            pManager.HideParameter(0);
            Message = "封闭三边曲线";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("曲线","C","封闭的三边曲线", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            
            List<Curve> curves = new List<Curve>();
            double tolerance = 0;
            if (!DA.GetDataList(0,curves)) return;
            if (!DA.GetData(1, ref tolerance)) return;
            int num = curves.Count;
            List<Curve> curs = new List<Curve>();
            for (int i = 0; i < num - 2; i++)
            {
                for (int j = i + 1; j < num - 1; j++)
                {
                    Rhino.Geometry.Intersect.CurveIntersections cin = Rhino.Geometry.Intersect.Intersection.CurveCurve(curves[i], curves[j], 0, 0);
                    int mm = cin.Count;
                    if (mm >= 1)
                    {
                        for (int k = j + 1; k < num; k++)
                        {
                            Rhino.Geometry.Intersect.CurveIntersections cin2 = Rhino.Geometry.Intersect.Intersection.CurveCurve(curves[k], curves[j], 0, 0);
                            int nn = cin2.Count;
                            if (nn >= 1)
                            {
                                Curve[] cr3d = { curves[i], curves[j], curves[k] };
                                Curve[] clr = Curve.JoinCurves(cr3d, tolerance);
                                if (clr[0].IsClosed)
                                {
                                    curs.Add(clr[0]);
                                }

                            }

                        }
                    }

                }

            }
            DA.SetDataList(0,curs);
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
                //return Resource1.封闭三边线段;
                return Resource1.curve_三边曲线封面;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{67806e1e-859e-468e-8551-2d005b16c667}"); }
        }
    }
}