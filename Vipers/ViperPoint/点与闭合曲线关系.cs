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

namespace Vipers///TangChi 2015.7.21
{
    public class CurvePointRelation : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent28 class.
        /// </summary>
        public CurvePointRelation()
            : base("点与闭合曲线关系", "CurvePointRelation",
                "outside-在曲线外部，coincident-在曲线上，inside-在曲线内部",
                "Vipers", "Viper.point")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("点", "P", "用于判断关系的点", GH_ParamAccess.list);
            pManager.AddCurveParameter("曲线","C","用于判断的闭合曲线",GH_ParamAccess.list);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
            Message = "点与闭合曲线关系";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("外部点", "P1", "位于曲线外部的点", GH_ParamAccess.list);
            pManager.AddPointParameter("曲线上的点", "P2", "位于曲线上的点", GH_ParamAccess.list);
            pManager.AddPointParameter("内部点", "P3", "位于曲线内部的点", GH_ParamAccess.list);
            pManager.AddIntegerParameter("外部点序号","I1","外部点在原点中的序号",GH_ParamAccess.list);
            pManager.AddIntegerParameter("曲线上的点序号", "I2", "位于曲线上的点在原点中的序号", GH_ParamAccess.list);
            pManager.AddIntegerParameter("内部点序号", "I3", "内部点在原点中的序号", GH_ParamAccess.list);
            pManager.HideParameter(0);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> points=new List<Point3d>();
            List<Curve> curves=new List<Curve>();
            if(!DA.GetDataList(0,points)) return;
            if (!DA.GetDataList(1,curves)) return;
            List<Point3d> x = points;
            List<Curve> y = curves;
            List<int> indexA = new List<int>();
            List<int> indexB = new List<int>();
            List<int> indexC = new List<int>();
            List<Point3d> pta = new List<Point3d>();
            List<Point3d> ptb = new List<Point3d>();
            List<Point3d> ptc = new List<Point3d>();
            Point3d ptmax = new Point3d(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
            for (int i = 0; i < y.Count; i++)
            {
                Curve c = y[i];
                for (int j = 0; j < x.Count; j++)
                {
                    if (c.Contains(x[j]) == PointContainment.Inside)
                    {
                        indexC.Add(j);
                        ptc.Add(x[j]);
                        x[j] = ptmax;
                    }
                    else if (c.Contains(x[j]) == PointContainment.Coincident)
                    {
                        indexB.Add(j);
                        ptb.Add(x[j]);
                        x[j] = ptmax;
                    }
                }
            }
            for (int i = 0; i < x.Count; i++)
            {
                if (x[i] == ptmax)
                {
                    continue;
                }
                indexA.Add(i);
                pta.Add(x[i]);
            }
            DA.SetDataList(0, pta);
            DA.SetDataList(1, ptb);
            DA.SetDataList(2, ptc);
            DA.SetDataList(3,indexA);
            DA.SetDataList(4,indexB);
            DA.SetDataList(5,indexC);
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
                //return Resource1.点与闭合曲线位置关系;
                return Resource1.point_点与闭合曲线关系;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{8e2e242a-89f3-4aa5-adc0-3822f57d974e}"); }
        }
    }
}