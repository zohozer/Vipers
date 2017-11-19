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

namespace Vipers//////TangChi 2015.7.24
{
    public class FunctionCurve : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent29 class.
        /// </summary>
        public FunctionCurve()
            : base("创建sin和cos函数曲线", "FunctionCurve",
                "通过设置参考平面，控制点数量，曲线周期，极值，长度创建sin或cos函数曲线",
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
            pManager.AddPlaneParameter("平面","P","参考平面",GH_ParamAccess.item,Plane.WorldXY);
            pManager.AddIntegerParameter("数量", "N", "控制点数量", GH_ParamAccess.item,60);
            pManager.AddNumberParameter("周期", "P", "曲线的周期", GH_ParamAccess.item,5);
            pManager.AddNumberParameter("极值", "E", "曲线的极值", GH_ParamAccess.item,10);
            pManager.AddNumberParameter("长度", "L", "曲线两端点之间的净长", GH_ParamAccess.item,200);
            pManager.AddBooleanParameter("切换", "R", "切换为sin或cos曲线", GH_ParamAccess.item,true);
            pManager.HideParameter(0);
            Message = "Sin&Cos曲线";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("控制点", "P", "曲线上的控制点", GH_ParamAccess.list);
            pManager.AddCurveParameter("函数曲线", "C", "sin或cos函数曲线", GH_ParamAccess.item);
            pManager.HideParameter(0);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane plane=new Plane();
            int numberPt=0;
            double period=0;
            double extremum=0;
            double length = 0;
            bool isSwitch=true;
            if(!DA.GetData(0,ref plane)) return;
            if(!DA.GetData(1, ref numberPt)) return;
            if (!DA.GetData(2, ref period)) return;
            if (!DA.GetData(3, ref extremum)) return;
            if (!DA.GetData(4, ref length)) return;
            if (!DA.GetData(5, ref isSwitch)) return;
            Plane p = plane;
            int n = numberPt;
            double y = period;
            double z = extremum;
            bool u = isSwitch;
            List<Point3d> pts = new List<Point3d>();
            double pi = 2 * Math.PI * y / n;
            double len = 0;
            Transform ts = Transform.PlaneToPlane(Plane.WorldXY, p);
            if (u == true && y != 0&&n>0)
            {
                for (double i = 0; i <= 2 * Math.PI * y+0.5*pi;i += pi)
                {
                    Point3d pt = new Point3d(len, Math.Sin(i) * z, 0);
                    pt.Transform(ts);
                    pts.Add(pt);
                    len += length / n;
                }
                DA.SetDataList(0, pts);
            }
            else if (u == false && y != 0 && n > 0)
            {
                for (double i = 0; i <= 2 * Math.PI * y + 0.5 * pi; i += pi)
                {
                    Point3d pt = new Point3d(len, Math.Cos(i) * z, 0);
                    pt.Transform(ts);
                    pts.Add(pt);
                    len += length / n;
                }
                DA.SetDataList(0, pts);
            }
            Curve result = Curve.CreateInterpolatedCurve(pts, 3);
            DA.SetData(1, result);
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
                //return Resource1.sin_cos函数曲线;
                return Resource1.point_sincos函数曲线;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{61cacf14-61c9-41a3-8559-7ebfd1a3a157}"); }
        }
    }
}