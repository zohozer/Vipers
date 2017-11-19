using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers////TangChi 2015.3.1
{
    public class closeFourEdge : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public closeFourEdge()
            : base("封闭四边曲线", "Close4-EdgeCurve",
                "将四条首尾连接的曲线闭合",
                "Vipers", "Viper.curve")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("曲线", "C", "待连接的曲线", GH_ParamAccess.list);
            pManager.AddNumberParameter("公差", "T", "在此范围内自动连接", GH_ParamAccess.item, 0);
            pManager.HideParameter(0);
            Message = "封闭四边曲线";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("曲线", "C", "封闭的四边曲线", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> curves = new List<Curve>();   
            double tolerance = 0;
            if (!DA.GetDataList(0, curves)) return;
            if (!DA.GetData(1, ref tolerance)) return;
            int num = curves.Count;
            List<Curve> curs = new List<Curve>();
            for (int i = 0; i < num; i++)
            {
                for (int j = i + 1; j < num; j++)
                {
                    for (int k = j + 1; k < num; k++)
                    {
                        Curve[] cs = { curves[i], curves[j], curves[k] };
                        if (ThreeEdges(cs) == 2)
                        {
                            for (int g = k + 1; g < num; g++)
                            {
                                Curve[] cr3d = { curves[i], curves[j], curves[k], curves[g] };
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
            DA.SetDataList(0, curs);
        }
        private int ThreeEdges(Curve[] x) //给定三条曲线，通过数值判断相互之间的关系（0：互不相干，1：有一对有交点，2：有两对有交点，3：封闭三边）
        {
            Rhino.Geometry.Intersect.CurveIntersections cin1 = Rhino.Geometry.Intersect.Intersection.CurveCurve(x[0], x[1], 0, 0);
            Rhino.Geometry.Intersect.CurveIntersections cin2 = Rhino.Geometry.Intersect.Intersection.CurveCurve(x[0], x[2], 0, 0);
            Rhino.Geometry.Intersect.CurveIntersections cin3 = Rhino.Geometry.Intersect.Intersection.CurveCurve(x[1], x[2], 0, 0);
            int a = 0;
            int b = 0;
            int c = 0;
            if (cin1.Count == 1)
            {
                a = 1;
            }
            if (cin2.Count == 1)
            {
                b = 1;
            }
            if (cin3.Count == 1)
            {
                c = 1;
            }
            int mm = a + b + c;
            return mm;
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
                //return Resource1.封闭四边线段;
                return Resource1.curve_四边曲线封面;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{8dcf4a2e-6140-4ba9-8fb4-508b6c149406}"); }
        }
    }
}