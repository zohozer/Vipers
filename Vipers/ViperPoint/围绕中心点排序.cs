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

namespace Vipers/////TangChi 2015.9.29
{
    public class SortCenterPt : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent48 class.
        /// </summary>
        public SortCenterPt()
            : base("围绕中心点排序", "SortCenterPt",
                "点云围绕指定点排序，如果没有指定点则以点云的中点为中心排序",
                "Vipers", "Viper.point")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("待排序点","P","用户设置的待排序的点",GH_ParamAccess.list);
            pManager.AddPointParameter("参考点", "P", "", GH_ParamAccess.item, Point3d.Unset);
            pManager.AddBooleanParameter("反向","R","是否反方向排序",GH_ParamAccess.item,false);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
            Message = "围绕中心点排序";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("排序后的点","P","排序后的点",GH_ParamAccess.list);
            pManager.AddIntegerParameter("索引值", "I", "排序后的点的索引值", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> points = new List<Point3d>();
            Point3d point = new Point3d();
            bool reverse = false;
            if(!DA.GetDataList(0,points)) return;
            if(!DA.GetData(1,ref point)) return;
            if (!DA.GetData(2, ref reverse)) return;
            List<Point3d> x = points;
            bool y = reverse;
            //////////////////////////////////////////////////////
            Point3d c = ViperClass.center(x);
            if (point != Point3d.Unset)////如果指定点则以指定点为中心
            {
                c = point;
            }
            Plane pl;
            Plane.FitPlaneToPoints(x, out pl);
            pl.Origin = c;
            double dist = 0;///以所有点离中心点距离的平均值为半径
            for (int i = 0; i < x.Count; i++)
            {
                dist += c.DistanceTo(x[i]);
            }
            Circle cir = new Circle(pl, dist / x.Count);
            ///////////////////////////////////////////////////
            List<double> ts = new List<double>();
            List<double> ts2 = new List<double>();
            for (int i = 0; i < x.Count; i++)
            {
                double t;
                cir.ClosestParameter(x[i], out t);
                ts.Add(t);
                ts2.Add(t);
            }
            ts.Sort();
            if (reverse)
            {
                ts.Reverse();
            }
            List<int> index = new List<int>();///找出对应的索引值
            for (int i = 0; i < x.Count; i++)
            {
                for (int j = 0; j < x.Count; j++)
                {
                    if (ts[i] == ts2[j])
                    {
                        index.Add(j);
                        ts2[j] = double.PositiveInfinity;
                        break;
                    }
                }
            }
            List<Point3d> ptnew = new List<Point3d>();
            for (int i = 0; i < x.Count; i++)
            {
                ptnew.Add(x[index[i]]);
            }
            DA.SetDataList(0, ptnew);
            DA.SetDataList(1, index);
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
                //return Resource1.围绕中心点排序;
                return Resource1.point_围绕中心点排序;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{86deca5c-bd11-4989-a0c4-8868aa58c9f6}"); }
        }
    }
}