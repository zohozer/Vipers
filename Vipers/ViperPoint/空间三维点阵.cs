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

namespace Vipers////TangChi 2015.7.1
{
    public class ViperMatrix : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent13 class.
        /// </summary>
        public ViperMatrix()
            : base("空间三维点阵", "PointMatrix",
                "可控制数量与间距的空间三维点阵",
                "Vipers", "Viper.point")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("起始点","P","设置起始点",GH_ParamAccess.item,new Point3d(0,0,0));
            pManager.AddPlaneParameter("起始平面","P","设置起始平面",GH_ParamAccess .item,Plane.WorldXY);
            pManager.AddIntegerParameter("x方向数量","NX","x方向数量",GH_ParamAccess.item,5);
            pManager.AddIntegerParameter("y方向数量","NY","y方向数量",GH_ParamAccess.item,5);
            pManager.AddIntegerParameter("z方向数量","NZ","z方向数量",GH_ParamAccess.item,5);
            pManager.AddNumberParameter("x方向的间距", "LX", "x方向的间距", GH_ParamAccess.item, 10);
            pManager.AddNumberParameter("y方向的间距", "LY", "y方向的间距", GH_ParamAccess.item, 10);
            pManager.AddNumberParameter("z方向的间距", "LZ", "z方向的间距", GH_ParamAccess.item, 10);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
            Message = "空间三维点阵";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("三维点阵","matrix","空间三维点阵",GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d startPt = new Point3d();
            Plane plane = new Plane();
            int xNumber = 0;
            int yNumber = 0;
            int zNumber = 0;
            double xLength = 0;
            double yLength = 0;
            double zLength = 0;
            if (!DA.GetData(0,ref startPt)) return;
            if (!DA.GetData(1, ref plane)) return;
            if (!DA.GetData(2, ref xNumber)) return;
            if (!DA.GetData(3, ref yNumber)) return;
            if (!DA.GetData(4, ref zNumber)) return;
            if (!DA.GetData(5, ref xLength)) return;
            if (!DA.GetData(6, ref yLength)) return;
            if (!DA.GetData(7, ref zLength)) return;
            Point3d s = startPt;
            Plane p = plane;
            int x = xNumber;
            int y = yNumber;
            int z = zNumber;
            double lx = xLength;
            double ly = yLength;
            double lz = zLength;
            DataTree<Point3d> pts = new DataTree<Point3d>();
            Vector3d vx = p.XAxis;
            Vector3d vy = p.YAxis;
            Vector3d vz = p.ZAxis;
            List<Point3d> ptx = new List<Point3d>();
            ptx.Add(s);
            for (int i = 0; i < x - 1; i++)////x方向阵列
            {
                Vector3d va = vx * lx;
                Transform tsa = Transform.Translation(va);
                s.Transform(tsa);
                ptx.Add(s);
            }
            ////////////////////////////////////////////////////////////////////
            for (int i = 0; i < ptx.Count; i++)////y方向阵列
            {
                Point3d pt = ptx[i];
                double ly2 = ly;
                for (int j = 0; j < y; j++)
                {
                    if (j == 0)
                    {
                        pts.Add(pt, new GH_Path(0, i));
                        continue;
                    }
                    Vector3d vb = vy * ly2;
                    Transform tsb = Transform.Translation(vb);
                    pt.Transform(tsb);
                    pts.Add(pt, new GH_Path(0, i));
                }
            }
            ///////////////////////////////////////////////////////////////////
            pts.Graft(true);
            List<GH_Path> path = new List<GH_Path>(pts.Paths);
            DataTree<Point3d> ptss = new DataTree<Point3d>();
            for (int i = 0; i < pts.BranchCount; i++)
            {
                Point3d pt = pts.Branch(i)[0];
                double lz2 = lz;
                for (int k = 0; k < z; k++)
                {
                    if (k == 0)
                    {
                        ptss.Add(pt, path[i]);
                        continue;
                    }
                    Vector3d vc = vz * lz2;
                    Transform tsc = Transform.Translation(vc);
                    pt.Transform(tsc);
                    ptss.Add(pt, path[i]);
                }
            }
            ///////////////////////////////////////////////////////////////////
            DA.SetDataTree(0, ptss);
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
                //return Resource1.三维点阵;
                return Resource1.point_三维点阵;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{6f412801-b9f1-457a-bbb6-c8cd3741db83}"); }
        }
    }
}