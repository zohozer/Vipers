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
namespace Vipers///////TangChi 2016.01.10
{
    public class rectangle : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the rectangle class.
        /// </summary>
        public rectangle()
            : base("矩形网格", "Rectangle",
                "传统类型的矩形网格",
                "Vipers", "Viper.surface")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quarternary; }
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("曲面", "S", "待划分的曲面", GH_ParamAccess.item);
            pManager.AddIntegerParameter("u方向数量", "U", "u方向的网格数量", GH_ParamAccess.item, 50);
            pManager.AddIntegerParameter("v方向数量", "V", "v方向的网格数量", GH_ParamAccess.item, 50);
            Message = "TC-1-05\n矩形网格";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("网格", "C", "生成的矩形网格", GH_ParamAccess.tree);
            pManager.AddPointParameter("网格点", "P", "矩形网格对应的点", GH_ParamAccess.tree);
            pManager.HideParameter(1);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Surface surface = null;
            int uCount = 10;
            int vCount = 10;
            if (!DA.GetData(0, ref surface)) return;
            if (!DA.GetData(1, ref uCount)) return;
            if (!DA.GetData(2, ref vCount)) return;
            /////////////////////////////////
            surface.SetDomain(0, new Interval(0, 1));
            surface.SetDomain(1, new Interval(0, 1));
            double u = 1.0 / uCount;
            double v = 1.0 / vCount;
            double start = 0;
            List<double> us = new List<double>();
            List<double> vs = new List<double>();
            for (int i = 0; i < uCount + 1; i++)
            {
                us.Add(start);
                start += u;
            }
            start = 0;
            for (int i = 0; i < vCount + 1; i++)
            {
                vs.Add(start);
                start += v;
            }
            ////////////////////////////////////////////////////////////////
            DataTree<Point3d> pts = new DataTree<Point3d>();
            int nums = 0;
            for (int i = 0; i < us.Count; i++)
            {
                for (int j = 0; j < vs.Count; j++)
                {
                    pts.Add(surface.PointAt(us[i], vs[j]), new GH_Path(0,DA.Iteration ,nums));
                }
                nums++;
            }
            DataTree<Polyline> plys = new DataTree<Polyline>();
            int mems = 0;
            for (int i = 0; i < pts.BranchCount - 1; i++)
            {
                for (int j = 0; j < pts.Branch(i).Count - 1; j++)
                {
                    List<Point3d> test = new List<Point3d>();
                    test.Add(pts.Branch(i)[j]);
                    test.Add(pts.Branch(i)[j + 1]);
                    test.Add(pts.Branch(i + 1)[j + 1]);
                    test.Add(pts.Branch(i + 1)[j]);
                    test.Add(pts.Branch(i)[j]);
                    plys.Add(new Polyline(test), new GH_Path(0, DA.Iteration, mems));
                }
                mems++;
            }
            DA.SetDataTree(0, plys);
            DA.SetDataTree(1, pts);
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
                return Resource1.surface_网格表皮E;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{fb48640a-1dde-4ffc-8ddf-d6e4fa28bb3c}"); }
        }
    }
}