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

namespace Vipers/////TangChi 2015.7.11
{
    public class SeparatOverlapCurve : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent27 class.
        /// </summary>
        public SeparatOverlapCurve()
            : base("分离重合曲线", "SeparatOverlapCurve",
                "将完全重合的曲线分为一组",
                "Vipers", "Viper.curve")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("曲线","C","待分离的曲线",GH_ParamAccess.list);
            pManager.AddNumberParameter("公差", "T", "可通过调节公差来使模糊筛选公共边", GH_ParamAccess.item, 0.001);
            pManager.HideParameter(0);
            Message = "分离重合曲线";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("曲线","C","没有重合的曲线",GH_ParamAccess.list);
            pManager.AddCurveParameter("曲线", "C", "有重合的曲线", GH_ParamAccess.tree);
            pManager.HideParameter(1);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            List<Curve> curves = new List<Curve>();
            double tolerance = 0;
            if(!DA.GetDataList(0,curves)) return;
            if(!DA.GetData(1,ref tolerance)) return;
            List<Curve> x = curves;
            List<Curve> afterCr = new List<Curve>();
            DataTree<Curve> deleteCr = new DataTree<Curve>();
            int index = 0;
            int branch = DA.Iteration;
            for (int i = 0; i < x.Count; i ++)
            {
                if(x[i]==null)continue;
                int notOne = 0;
                afterCr.Add(x[i]);
                Curve a = afterCr[afterCr.Count-1];
                x[i] = null;
                for (int j = 0; j < x.Count; j++)
                {
                    if(x[j]==null)continue;
                    Curve b = x[j];
                    Rhino.Geometry.Intersect.CurveIntersections cin = Rhino.Geometry.Intersect.Intersection.CurveCurve(a, b, 0, 0);
                    int mm = cin.Count;
                    double toler = Math.Abs(a.GetLength() - b.GetLength());
                    Point3d pa = a.PointAtNormalizedLength(0.5);
                    Point3d pb = b.PointAtNormalizedLength(0.5);
                    double distance = Math.Abs(pa.DistanceTo(pb));
                    if (mm == 1 && toler <= tolerance && distance <= tolerance)
                    {
                        notOne += 1;
                        deleteCr.Add(x[j],new GH_Path(0,branch,index));
                        x[j] = null;
                    }
                }
                if (notOne > 0)
                {
                    deleteCr.Add(afterCr[afterCr.Count - 1],new GH_Path(0,branch,index));
                    afterCr.RemoveAt(afterCr.Count - 1);
                    index++;
                }
            }
            DA.SetDataList(0, afterCr);
            DA.SetDataTree(1, deleteCr);
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
                //return Resource1.删除所有重合曲线;
                return Resource1.curve_分离重合曲线;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{89e6064e-3f91-43ac-9c64-20c0efff93fe}"); }
        }
    }
}