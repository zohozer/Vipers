using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers////TangChi 2015.11.14
{
    public class OverlapIndex : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent55 class.
        /// </summary>
        public OverlapIndex()
            : base("两组点重合部分序号", "OverlapIndex",
                "找出点列表2中与列表1有重合的点的序号",
               "Vipers", "Viper.point")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("第一组点","PA","用于检索的点",GH_ParamAccess.list);
            pManager.AddPointParameter("第二组点", "PB", "找出第一组点重合的点的序号", GH_ParamAccess.list);
            pManager.AddNumberParameter("距离范围","D","在此范围内的点视为重合",GH_ParamAccess.item,0);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
            Message = "两组点重合部分序号";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("相同点序号", "I1", "第二组点中与第一组点重合的点的序号", GH_ParamAccess.list);
            pManager.AddIntegerParameter("不相同点序号", "I2", "第二组点中与第一组点不重合的点的序号", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            
            List<Point3d> PtList1=new List<Point3d>();
            List<Point3d> PtList2=new List<Point3d>();
            double distance = 0;
            if(!DA.GetDataList(0,PtList1))return;
            if (!DA.GetDataList(1, PtList2)) return;
            if (!DA.GetData(2, ref distance)) return;
            ////////////////////////////////////////////////
            List<Point3d> min = PtList1;
            List<Point3d> max = PtList2;
            Point3d ptmax = new Point3d(0, 0, 1.797693E+308);
            List<int> same = new List<int>(); ;////有重复的点的序号
            for (int i = 0; i < min.Count; i++)
            {
                Point3d pt = min[i];
                for (int j = 0; j < max.Count; j++)
                {
                    if (pt.DistanceTo(max[j]) <= distance)
                    {
                        same.Add(j);
                        max[j] = ptmax;
                    }
                }
            }
            List<int> index = new List<int>();////与第一组点没有重复部分的点的序号
            for (int i = 0; i < max.Count; i++)
            {
                if (max[i] != ptmax)
                {
                    index.Add(i);
                }
            }
            DA.SetDataList(0, same);
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
                //return Resource1.两组点重合部分序号;
                return Resource1.point_两组点重合部分序号;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{41911b70-c87e-4cb5-a627-bb750445d827}"); }
        }
    }
}