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

namespace Vipers /////TangChi 2015.5.4
{
    public class ViperCurveIsSimilar : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent2 class.
        /// </summary>
        public ViperCurveIsSimilar()
            : base("判断曲线是否相似", "CurveSimilar",
                "通过曲线的控制点判断曲线是否相似",
                "Vipers", "Viper.curve")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("曲线A", "CA", "待比较的曲线A", GH_ParamAccess.item);
            pManager.AddCurveParameter("曲线B", "CB", "待比较的曲线B", GH_ParamAccess.item);
            Message = "判断曲线是否相似";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("比较结果", "R", "如果是相似多边形则返回true，否则返回false", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////声明变量
            Curve c1 = null;
            Curve c2 = null;
            ///////////////////////////////////////////////////////////////////////////////////////////////////检测输入端是否合理
            if (!DA.GetData(0, ref c1)) return;
            if (!DA.GetData(1, ref c2)) return;
            List<Point3d> ptcor1 = ViperClass.Ptsss(c1);
            List<Point3d> ptcor2 = ViperClass.Ptsss(c2);
            Polyline polylineA = new Polyline(ptcor1);
            Polyline polylineB = new Polyline(ptcor2);
            List<double> angles1 = new List<double>();
            List<double> angles2 = new List<double>();
            List<double> ag1 = new List<double>();
            List<double> ag2 = new List<double>();
            int n1 = polylineA.SegmentCount;
            int n2 = polylineB.SegmentCount;
            bool flag = false;
            if (n1 == n2)
            {
                flag = true;
                Line[] ls1 = polylineA.GetSegments();
                Line[] ls2 = polylineB.GetSegments();
                for (int i = 0; i < n1; i++)
                {
                    if (i == n1 - 1)
                    {
                        Line la11 = ls1[i];
                        Line la22 = ls1[0];
                        double aa = Math.Round(ViperClass.getangle(la11, la22), 3);
                        angles1.Add(aa);
                        Line lb11 = ls2[i];
                        Line lb22 = ls2[0];
                        double bb = Math.Round(ViperClass.getangle(lb11, lb22), 3);
                        angles2.Add(bb);
                        continue;
                    }
                    Line la1 = ls1[i];
                    Line la2 = ls1[i + 1];
                    double a = Math.Round(ViperClass.getangle(la1, la2), 3);
                    angles1.Add(a);
                    Line lb1 = ls2[i];
                    Line lb2 = ls2[i + 1];
                    double b = Math.Round(ViperClass.getangle(lb1, lb2), 3);
                    angles2.Add(b);
                }
                angles1.Sort();
                angles2.Sort();
                for (int i = 0; i < n1; i++)
                {
                    if (angles1[i] != angles2[i])
                    {
                        flag = false;
                        break;
                    }
                }
            }
            DA.SetData(0,flag);
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
                //return Resource1.v25;
                return Resource1.curve_曲线相似;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{08de17fa-ef4a-44d1-a950-d81ca3e9eee6}"); }
        }
    }
}