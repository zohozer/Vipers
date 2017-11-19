using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers/////TangChi 2015.10.25
{
    public class PPSort : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent47 class.
        /// </summary>
        public PPSort()
            : base("通过与已知点距离排序", "PPSort",
                "通过参考点与已知点的距离长度排序",
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
            pManager.AddPointParameter("中心点", "P", "参考点将以该点为中心", GH_ParamAccess.item);
            pManager.AddPointParameter("参考点","P","用户设置的参考点",GH_ParamAccess.list);
            pManager.AddBooleanParameter("反转","R","切换排序顺序",GH_ParamAccess.item,false);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
            Message = "通过与已知点距离排序";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("点","P","排序后的点",GH_ParamAccess.list);
            pManager.AddIntegerParameter("索引", "I", "索引值", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d centerPt = new Point3d();
            List<Point3d> points = new List<Point3d>();
            bool reverse = false;
            if(!DA.GetData(0,ref centerPt)) return;
            if(!DA.GetDataList(1,points)) return;
            if(!DA.GetData(2,ref reverse))return;
            List<double> dist = new List<double>();
            List<double> dist2 = new List<double>();
            List<int> index = new List<int>();
            for (int i = 0; i < points.Count; i++)
            {
                dist.Add(centerPt.DistanceTo(points[i]));
                dist2.Add(centerPt.DistanceTo(points[i]));
            }
            dist.Sort();
            if (reverse)
            {
                dist.Reverse();
            }
            for (int i = 0; i < dist.Count; i++)
            {
                for (int j = 0; j < dist2.Count; j++)
                {
                    if (dist[i] == dist2[j])
                    {
                        index.Add(j);
                        dist2[j] = 1.797693E+308;
                        break;
                    }
                }
            }
            List<Point3d> pts = new List<Point3d>();
            for (int i = 0; i < index.Count; i++)
            {
                pts.Add(points[index[i]]);
            }
            DA.SetDataList(0, pts);
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
                //return Resource1.通过与已知点距离排序;
                return Resource1.point_与已知点距离排序;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{ec5e143f-f3f1-4d8a-8bf7-b704f43eefab}"); }
        }
    }
}