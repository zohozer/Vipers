using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers ///TangChi 2015.5.21
{
    public class ViperPlaneLine : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent4 class.
        /// </summary>
        public ViperPlaneLine()
            : base("找出与指定平面不正交的线段", "CurveOrthogonality",
                "通过用户设置的平面，找出与平面x,y,z轴都不平行的线段（z轴通过布尔值设置）",
                "Vipers", "Viper.curve")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddLineParameter("待检查的线段", "L", "待检查的线段", GH_ParamAccess.list);
            pManager.AddPlaneParameter("用户指定平面", "P", "如果线段不平行于平面的x，y，z轴中任一，则被视为不正交线段", GH_ParamAccess.list, Plane.WorldXY);
            pManager.AddNumberParameter("角度","A","与x,y,z轴任意角度在angle以内的，视为正交",GH_ParamAccess.item,0);
            pManager.AddBooleanParameter("Z向量","Z","是否加入Z方向向量，如果是，则选择true",GH_ParamAccess.item,false);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
            Message = "线段是否与指定平面正交";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("不共线的线段", "C1", "找出的与指定向量不共线的线段", GH_ParamAccess.list);
            pManager.AddLineParameter("共线的线段", "C2", "找出的与指定向量共线的线段", GH_ParamAccess.list);
            pManager.AddIntegerParameter("索引1", "I1", "不共线线段的索引位置", GH_ParamAccess.list);
            pManager.AddIntegerParameter("索引2", "I2", "共线线段的索引位置", GH_ParamAccess.list);
            pManager.HideParameter(1);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Line> lines = new List<Line>();
            List<Plane> plane = new List<Plane>();
            double angle = 0;
            bool direction=false;
            if (!DA.GetDataList(0, lines)) return;
            if (!DA.GetDataList(1, plane)) return;
            if (!DA.GetData(2, ref angle)) return;
            if (!DA.GetData(3, ref direction)) return;
            List<Vector3d> vectors = new List<Vector3d>();
            for (int i = 0; i<plane.Count;i++ )
            {
                vectors.Add(plane[i].XAxis);
                vectors.Add(plane[i].YAxis);
                if (direction)
                {
                    vectors.Add(plane[i].ZAxis);
                }
            }
            List<Line> goodline = new List<Line>();
            List<Line> badline = new List<Line>();
            List<int> indexgood = new List<int>();
            List<int> indexbad = new List<int>();
            for (int i = 0; i < lines.Count; i++)
            {
                Line li = lines[i];
                for (int q = 0; q < vectors.Count; q++)
                {
                    double degree = Vector3d.VectorAngle(li.Direction, vectors[q]);
                    if (degree > Math.PI * 0.5)
                    {
                        degree = Math.PI - degree;
                    }
                    if (degree <= angle + 0.000000001)
                    {
                        goodline.Add(li);
                        indexgood.Add(i);
                        lines[i] = Line.Unset;
                        break;
                    }
                }
                if (lines[i] != Line.Unset)
                {
                    badline.Add(lines[i]);
                    indexbad.Add(i);
                }
            }
            DA.SetDataList(0, badline);
            DA.SetDataList(1, goodline);
            DA.SetDataList(2, indexbad);
            DA.SetDataList(3, indexgood);
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
                //return Resource1.线段与平面正交;
                return Resource1.curve_与指定平面不成交线段;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{b0759c85-fb19-47c6-b0ac-74bc37d81d77}"); }
        }
    }
}