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

namespace Vipers//////TangChi 2015.8.28
{
    public class ListGrid : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent39 class.
        /// </summary>
        public ListGrid()
            : base("列表数值网格", "ListGrid",
                "根据列表中的的数值作为相邻点之间的间距设置网格",
                "Vipers", "Viper.point")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            double[] lists={1,2,3,4,5,6,7,8,9,8,7,6,5,4,3,2,1};
            pManager.AddPlaneParameter("平面","P","设置参考平面",GH_ParamAccess.item,Plane.WorldXY);
            pManager.AddPointParameter("点","P","设置起始点",GH_ParamAccess.item,Point3d.Origin);
            pManager.AddNumberParameter("x列表","LX","设置x方向的长度数值列表",GH_ParamAccess.list,lists);
            pManager.AddNumberParameter("y列表","LY","设置y方向的长度数值列表",GH_ParamAccess.list, lists);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
            Message = "通过列表数值设置网格";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("网格点","P","根据列表生成的网格点",GH_ParamAccess.tree);
            pManager.AddRectangleParameter("矩形","C","与网格点对应的矩形阵列",GH_ParamAccess.tree);
            pManager.HideParameter(0);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane startPlane=new Plane();
            Point3d startPoint=new Point3d();
            List<double> xLengthList=new List<double>();
            List<double> yLengthList=new List<double>();
            if(!DA.GetData(0,ref startPlane))return;
            if(!DA.GetData(1, ref startPoint))return;
            if(!DA.GetDataList(2,xLengthList))return;
            if(!DA.GetDataList(3, yLengthList))return;
            Plane pl = startPlane;
            Point3d p = startPoint;
            List<double> x = xLengthList;
            List<double> y = yLengthList;
            Vector3d vx = pl.XAxis;
            Vector3d vy = pl.YAxis;
            DataTree<Point3d> ptree = new DataTree<Point3d>();
            DataTree<Rectangle3d> recs = new DataTree<Rectangle3d>();
            List<Point3d> pt = new List<Point3d>();
            pt.Add(p);
            for (int i = 0; i < x.Count; i++)
            {
                p.Transform(Transform.Translation(vx * x[i]));
                pt.Add(p);
            }
            ptree.AddRange(pt, new GH_Path(0, 0));
            PointCloud cloud = new PointCloud(pt);
            int index = 1;
            for (int i = 0; i < y.Count; i++)
            {
                cloud.Transform(Transform.Translation(vy * y[i]));
                Point3d[] ptss = cloud.GetPoints();
                ptree.AddRange(ptss, new GH_Path(0, index));
                index++;
            }
            //////////////////////////////////////////////////////////////////创建矩形
            int index2 = 0;
            for (int i = 0; i < y.Count; i++)
            {
                for (int j = 0; j < x.Count; j++)
                {
                    Point3d ppp = ptree.Branch(i)[j];
                    Plane pln = new Plane(ppp, pl.XAxis, pl.YAxis);
                    recs.Add(new Rectangle3d(pln, x[j], y[i]), new GH_Path(0, index2));
                }
                index2++;
            }
            DA.SetDataTree(0, ptree);
            DA.SetDataTree(1, recs);
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
                //return Resource1.通过列表数值设置网格;
                return Resource1.point_列表转换网格;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{ce3aae8c-a6ae-443d-875d-1d7d2d042aa4}"); }
        }
    }
}