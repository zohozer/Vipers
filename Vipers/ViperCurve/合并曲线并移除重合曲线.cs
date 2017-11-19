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

namespace Vipers///TangChi 2015.7.11
{
    public class PolygonMergeRemove : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent26 class.
        /// </summary>
        public PolygonMergeRemove()
            : base("合并曲线并移除重合曲线", "PolygonMergeRemove",
                "适用于将有公共部分的多段线或多段曲线合并，并将公共边移除（公共边必须完全重合）",
                "Vipers", "Viper.curve")
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
            pManager.AddCurveParameter("多段线或曲线", "C", "有公共边的多段线或曲线", GH_ParamAccess.list);
            pManager.AddNumberParameter("公差","T1","可通过调节公差来使模糊筛选公共边",GH_ParamAccess.item,0.001);
            pManager.AddNumberParameter("合并公差", "T2", "可通过调节公差来使端部小于公差值的线段相连", GH_ParamAccess.item, 0);
            pManager.HideParameter(0);
            Message = "合并曲线";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("合并曲线","C","合并之后的曲线（多段线）",GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> polylineCurves = new List<Curve>();
            double curveTolerance = 0;
            double joinTolerance = 0;
            if(!DA.GetDataList(0,polylineCurves)) return;
            if(!DA.GetData(1,ref curveTolerance)) return;
            if(!DA.GetData(2,ref joinTolerance)) return;
            List<Curve> curves = new List<Curve>();
            for (int i = 0; i < polylineCurves.Count; i++)
            {
                Curve c = polylineCurves[i];
                List<double> tt = ViperClass.curveKnot(c);
                Curve[] cs = c.Split(tt);
                curves.AddRange(cs);
            }
            List<Curve> resultCr = ViperClass.deleteSameCurve(curves, curveTolerance);
            Curve[] joinCurves = Curve.JoinCurves(resultCr.ToArray(), joinTolerance);
            DA.SetDataList(0, joinCurves);
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
                //return Resource1.合并曲线并移除重合部分;
                return Resource1.curve_合并曲线;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{01fcdc4d-15db-4cd9-be37-043e6221907e}"); }
        }
    }
}