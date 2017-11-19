using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers///TangChi 2015.6.12
{
    public class ViperAchimedeanSpira2 : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public ViperAchimedeanSpira2()
            : base("阿基米德螺线Ⅱ", "AchimedeanSpiraⅡ",
                "通过中心点，终止点，平面，圈数，总控制点数量创建的阿基米德螺线",
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
            pManager.AddPointParameter("螺旋形中心点", "P1", "阿基米德螺旋线的中点", GH_ParamAccess.item, new Point3d(0, 0, 0));
            pManager.AddPointParameter("螺旋形终止点", "P2", "阿基米德螺旋线的中点", GH_ParamAccess.item, new Point3d(-200, 0, 0));
            pManager.AddPlaneParameter("螺旋线平面", "P", "阿基米德螺旋线的平面", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddIntegerParameter("周期", "N", "旋转周期", GH_ParamAccess.item, 3);
            pManager.AddIntegerParameter("每圈控制点的数量", "N", "阿基米德曲线每圈的控制点数量", GH_ParamAccess.item, 120);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
            pManager.HideParameter(2);
            Message = "阿基米德螺线Ⅱ(中点/终点/平面/圈数/控制点总数)";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("生成的螺旋点", "P", "生成结果点", GH_ParamAccess.list);
            pManager.AddCurveParameter("阿基米德螺旋线", "C", "生成结果曲线", GH_ParamAccess.item);
            pManager.HideParameter(0);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //centerPoint:螺线中心点
            //endPoint:螺线终止点
            //plane:螺线所在平面
            //cycles:圈数
            //controlPointsNumber:控制点数量
            //controlPointNumber:每圈控制点数量
            ///////////////////////////////////////////////////////////////////////////////////////////////////声明变量
            Point3d centerPoint = new Point3d(0, 0, 0);
            Point3d endPoint = new Point3d(-200, 0, 0);
            Plane plane = new Plane(new Point3d(0, 0, 0), new Vector3d(0, 0, 1));
            int cycles = 1;
            int controlPointsNumber = 60;
            ///////////////////////////////////////////////////////////////////////////////////////////////////检测输入端是否合理
            if (!DA.GetData(0, ref centerPoint)) return;
            if (!DA.GetData(1, ref endPoint)) return;
            if (!DA.GetData(2, ref plane)) return;
            if (!DA.GetData(3, ref cycles)) return;
            if (!DA.GetData(4, ref controlPointsNumber)) return;
            ///////////////////////////////////////////////////////////////////////////////////////////////////
            List<Point3d> pts = new List<Point3d>();
            double distance = centerPoint.DistanceTo(endPoint);
            double sgementlength = distance / (controlPointsNumber - 1);
            Vector3d vstart = Point3d.Subtract(centerPoint, endPoint);
            vstart.Reverse();
            double mm = 2 * Math.PI * cycles / (controlPointsNumber - 1);////平分后的度数
            double length = sgementlength;
            for (double i = 0; i < controlPointsNumber; i++)//每一度的增量为y
            {
                if (i == 0)
                {
                    pts.Add(centerPoint);
                    continue;
                }
                Transform ts = Transform.Rotation(mm, plane.ZAxis, centerPoint);
                vstart.Transform(ts);
                Line l = new Line(centerPoint, vstart, length);
                pts.Add(l.To);
                length += sgementlength;
            }
            DA.SetDataList(0, pts);
            DA.SetData(1, Curve.CreateInterpolatedCurve(pts, 3));
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                //return Vipers.Resource1.阿基米德螺旋2;
                return Vipers.Resource1.point_螺旋曲线;

            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{11c3478f-a7be-4539-bf64-400d52d72dcd}"); }
        }
    }
}