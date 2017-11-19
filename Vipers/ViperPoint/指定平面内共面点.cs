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

namespace Vipers//////// TangChi 2015.1.18(2016.4.16改)
{
    public class ViperPlanePoints : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent4 class.
        /// </summary>
        public ViperPlanePoints()
            : base("指定平面内共面点分组", "CoplanarPoints",
                "根据用户指定的平面分别找出共面点（如果只有一个指定平面，则通过计算，将共面的点分为一组，没有共面的点，单独一组。如果指定了多个平面，则找出位于这些平面上的所有点，同时输出不在这些平面上的点的序号）",
                "Vipers", "Viper.point")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("筛选点","P","参与筛选的点",GH_ParamAccess.list);
            pManager.AddPlaneParameter("指定平面", "P", "用户指定的平面（如果只有一个指定平面，则通过计算，将共面的点分为一组，没有共面的点，单独一组。如果指定了多个平面，则找出位于这些平面上的所有点，同时输出不在这些平面上的点的序号）", GH_ParamAccess.list, Plane.WorldXY);
            pManager.AddNumberParameter("公差","T","点到平面的距离在此范围内视为共面点",GH_ParamAccess.item,0);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
            Message = "指定平面内共面点分组";
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("共面点", "P", "在指定平面的点", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("序号1", "I1", "共面点在原来点中的序号", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("序号2","I2","不在任何指定平面内的点的序号（只针对输入多个指定平面的情况）",GH_ParamAccess.list);
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            List<Point3d> points=new List<Point3d>();
            List<Plane> planes=new List<Plane>();
            double tolerance=0;
            if(!DA.GetDataList(0,points))return;
            if (!DA.GetDataList(1, planes)) return;
            if (!DA.GetData(2, ref tolerance)) return;
            GH_Structure<GH_Point> last = new GH_Structure<GH_Point>();
            GH_Structure<GH_Integer> index1 = new GH_Structure<GH_Integer>();
            List<int> index2 = new List<int>();
            int mem = 0;
            int branch = DA.Iteration;
            if (planes.Count <= 1)
            {
                Plane pln = planes[0];
                for (int i = 0; i < points.Count; i++)
                {
                    if (points[i] == Point3d.Unset) continue;
                    pln.Origin = points[i];
                    last.Append(new GH_Point(points[i]), new GH_Path(0, branch, mem));
                    index1.Append(new GH_Integer(i), new GH_Path(0, branch, mem));
                    points[i] = Point3d.Unset;
                    for (int k = 0; k < points.Count; k++)
                    {
                        if (points[k] == Point3d.Unset)continue;
                        if (Math.Abs(pln.DistanceTo(points[k])) <= tolerance + 0.00000001)
                        {
                            last.Append(new GH_Point(points[k]), new GH_Path(0, branch, mem));
                            index1.Append(new GH_Integer(k), new GH_Path(0, branch,mem));
                            points[k] = Point3d.Unset;
                        }
                    }
                    mem++;
                }
            }
            else
            {
                for (int i = 0; i < planes.Count; i++)
                {
                    Plane pln = planes[i];
                    for (int k = 0; k < points.Count; k++)
                    {
                        if (points[k] == Point3d.Unset)continue;
                        if ((Math.Abs(pln.DistanceTo(points[k])) <= tolerance + 0.00000001))
                        {
                            last.Append(new GH_Point(points[k]), new GH_Path(0, branch, mem));
                            index1.Append(new GH_Integer(k), new GH_Path(0, branch, mem));
                            points[k] = Point3d.Unset;
                        }
                    }
                    mem++;
                }
                for (int i = 0; i < points.Count; i++)
                {
                    if (points[i] != Point3d.Unset)
                    {
                        index2.Add(i);
                    }
                }
            }
            DA.SetDataTree(0, last);
            DA.SetDataTree(1, index1);
            DA.SetDataList(2, index2);
        }
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Resource1.point_与指定平面共面点分组;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{7d380a3a-7aec-4a27-85bd-522ed94ea2bc}"); }
        }
    }
}