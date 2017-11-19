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

namespace Vipers///TangChi 2015.6.9
{
    public class CurveRelation : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent2 class.
        /// </summary>
        public CurveRelation()
            : base("曲线位置关系", "CurveRelation",
                "判断曲线B与闭合曲线A的位置关系",
                "Vipers", "Viper.curve")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("闭合曲线A", "CA", "曲线必须是闭合的曲线", GH_ParamAccess.item);
            pManager.AddCurveParameter("曲线B", "CB", "判断与曲线A位置关系的曲线", GH_ParamAccess.list);
            pManager.HideParameter(1);
            Message = "曲线位置关系";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("表达", "E", "中文阐述两者关系", GH_ParamAccess.list);
            pManager.AddIntegerParameter("评估值", "V", "0:在曲线内，1:与曲线相交，2:在曲线外，1000:两者不在同一平面", GH_ParamAccess.list);
            pManager.AddCurveParameter("内部曲线", "C1", "在曲线A内部的曲线", GH_ParamAccess.list);
            pManager.AddCurveParameter("相交曲线", "C2", "与曲线A相交的曲线", GH_ParamAccess.list);
            pManager.AddCurveParameter("外部曲线", "C3", "在曲线A外部的曲线", GH_ParamAccess.list);
            pManager.AddCurveParameter("不共面曲线", "C4", "与曲线A不共面的曲线", GH_ParamAccess.list);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve curveA = null;
            List<Curve> curveB = new List<Curve>();
            if(!DA.GetData(0,ref curveA)) return;
            if (!DA.GetDataList(1,curveB)) return;
            if (curveA.IsClosed == true && curveA.IsPlanar() == true)
            {
                List<int> judge = new List<int>();
                List<string> answer = new List<string>();
                List<Curve> cIn = new List<Curve>();
                List<Curve> cOut = new List<Curve>();
                List<Curve> cOn = new List<Curve>();
                List<Curve> cNo = new List<Curve>();
                for (int k = 0; k < curveB.Count; k++)
                {
                    List<Point3d> pts = Ptsss(curveA);
                    Plane pln = new Plane(pts[0], pts[1], pts[2]);
                    if (curveB[k].IsInPlane(pln) == true)
                    {
                        Point3d ptA = curveB[k].PointAtStart;
                        Point3d ptB = curveB[k].PointAtEnd;
                        Rhino.Geometry.Intersect.CurveIntersections cin = Rhino.Geometry.Intersect.Intersection.CurveCurve(curveA, curveB[k], 0, 0);
                        int AA = curveA.Contains(ptA).GetHashCode();
                        int BB = curveA.Contains(ptB).GetHashCode();
                        if (cin.Count >= 1)//相交
                        {
                            answer.Add("相交");
                            judge.Add(1);
                            cOn.Add(curveB[k]);
                        }
                        else if (cin.Count == 0 && AA == 2 && BB == 2)//在外
                        {
                            answer.Add("在外");
                            judge.Add(2);
                            cOut.Add(curveB[k]);
                        }
                        else if (cin.Count == 0 && AA == 1 && BB == 1)//在内
                        {
                            answer.Add("在内");
                            judge.Add(0);
                            cIn.Add(curveB[k]);
                        }
                    }
                    else if (curveB[k].IsInPlane(pln) == false)
                    {
                        answer.Add("curveB与curveA不在同一平面");
                        judge.Add(1000);
                        cNo.Add(curveB[k]);
                    }
                }
                DA.SetDataList(0,answer);
                DA.SetDataList(1, judge);
                DA.SetDataList(2 ,cIn);
                DA.SetDataList(3, cOn);
                DA.SetDataList(4, cOut);
                DA.SetDataList(5 ,cNo);
            }
            else
            {
                DA.SetData(0, "curveA必须为封闭的平面曲线");
            }
        }
        public List<Point3d> Ptsss(Curve x) //给定曲线找出其控制点
        {
            NurbsCurve mm = x.ToNurbsCurve();
            ControlPoint list = new ControlPoint();
            List<Point3d> ptss = new List<Point3d>();
            Rhino.Geometry.Collections.NurbsCurvePointList listA = mm.Points;
            for (int i = 0; i < listA.Count; i++)
            {
                list = listA.ElementAt(i);
                Point3d pt = list.Location;
                ptss.Add(pt);
            }
            return ptss;
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
                //return Resource1.v1;
                return Resource1.curve_曲线位置关系;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{67a83c4a-f049-44e0-8a0b-7b3c17456656}"); }
        }
    }
}