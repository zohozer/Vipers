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
using Grasshopper.Kernel.Parameters;

namespace Vipers//////TangChi 2016.4.5
{
    public class 指定线段近似共线 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the 指定线段近似共线 class.
        /// </summary>
        public 指定线段近似共线()
            : base("找出共线线段", "SpecifiedCollinear",
                "找出与用户指定线段共线的线段，调节角度值（angle），可将在此角度范围的线段找出",
                "Vipers", "Viper.curve")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            Message = "找出指定共线线段";
            pManager.AddCurveParameter("源线段","C","将从中找出与指定线段共线（近似共线）的线段",GH_ParamAccess.list);
            pManager.AddCurveParameter("指定线段","C","用户指定的线段",GH_ParamAccess.list);
            AddNumberInput();
            pManager.HideParameter(0);
            pManager.HideParameter(1);
        }
        Param_Number p = new Param_Number();
        public void AddNumberInput()////创建number输出端 该输出端可以添加角度选项
        {
            p.AngleParameter = true;
            p.Name = "角度范围";
            p.NickName = "A";
            p.Description = "如果源线段中有线段与指定线段角度小于此范围的，则被选出";
            p.Access = GH_ParamAccess.item;
            p.SetPersistentData(0);
            Params.RegisterInputParam(p);
            p.Optional = true;
        }
        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("筛选结果","C","找出符合条件的线段",GH_ParamAccess.list);
            pManager.AddIntegerParameter("序号","I","符合条件的线段在源线段中的序号",GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> curves = new List<Curve>();
            List<Curve> c = new List<Curve>();
            double angle = 0;
            if(!DA.GetDataList(0,curves))return;
            if (!DA.GetDataList(1, c)) return;
            if (!DA.GetData(2, ref angle)) return;
            if(p.UseDegrees)
               angle= RhinoMath.ToRadians(angle);
            //////////////////////////////////////////////////////////
            List<Curve> collect = new List<Curve>();
            List<int> index = new List<int>();
            bool flag1 = false;
            bool flag2 = false;
            Curve c1 = null;
            List<Curve> same = new List<Curve>();
            List<int> sameindex = chongfu(curves, c, out same);////删除与指定曲线重合部分后的源曲线
            for (int k = 0; k < c.Count; k++)
            {
                c1 = c[k];
                while (true)
                {
                    for (int q = 0; q < curves.Count; q++)
                    {
                        flag1 = false;
                        Vector3d v1 = Point3d.Subtract(c1.PointAtEnd, c1.PointAtStart);
                        if (curves[q] == null)
                        {
                            continue;
                        }
                        Curve c2 = curves[q];
                        Vector3d v2 = Point3d.Subtract(c2.PointAtEnd, c2.PointAtStart);
                        if (Rhino.Geometry.Intersect.Intersection.CurveCurve(c1, c2, 0, 0).Count == 1 && zhefan(c1, c2))///有一个交点
                        {
                            double ag = Vector3d.VectorAngle(v1, v2);
                            if (ag > Math.PI * 0.5)
                            {
                                ag = Math.PI - ag;
                            }
                            if (ag <= angle)////角度满足要求
                            {
                                index.Add(q);
                                collect.Add(curves[q]);
                                c1 = collect[collect.Count - 1];
                                curves[q] = null;
                                flag1 = true;
                                break;
                            }
                        }
                    }
                    if (flag1)////如果有满足情况的曲线，则继续执行上一步操作
                    {
                        continue;
                    }
                    //////////////////////////////////////////////////////////////////先判断第一个方向所有满足条件的线
                    //////////////////////////////////////////////////////////////////
                    if (!flag2)
                    {
                        c1 = c[k];
                        flag1 = false;
                        flag2 = true;
                        continue;
                    }
                    if (collect.Count == 0)/////如果没有匹配的曲线
                    {
                        flag2 = false;
                        flag1 = false;
                        break;
                    }
                    flag2 = false;
                    flag1 = false;
                    break;
                }
            }
            collect.AddRange(same);
            index.AddRange(sameindex);
            DA.SetDataList(0, collect);
            DA.SetDataList(1, index);
        }
        public bool zhefan(Curve x, Curve y) ///////////////简易判断曲线是否为折返曲线
        {
            Point3d p1 = x.PointAtEnd;
            Point3d p2 = x.PointAtStart;
            Point3d p3 = y.PointAtEnd;
            Point3d p4 = y.PointAtStart;
            double length1 = p1.DistanceTo(p2);
            double length2 = p3.DistanceTo(p4);
            double length3 = 0;
            if (p1 == p3)
            {
                length3 = p2.DistanceTo(p4);
            }
            else if (p1 == p4)
            {
                length3 = p2.DistanceTo(p3);
            }
            else if (p2 == p3)
            {
                length3 = p1.DistanceTo(p4);
            }
            else if (p2 == p4)
            {
                length3 = p1.DistanceTo(p3);
            }
            if (length3 > length1 && length3 > length2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public List<int> chongfu(List<Curve> cs, List<Curve> c, out List<Curve> same) ///////删除与指定曲线重合的曲线
        {
            List<int> index = new List<int>();
            List<Curve> getsame = new List<Curve>();
            for (int i = 0; i < c.Count; i++)
            {
                for (int q = 0; q < cs.Count; q++)
                {
                    if (cs[q] == null)
                    {
                        continue;
                    }
                    if (Rhino.Geometry.Intersect.Intersection.CurveCurve(c[i], cs[q], 0, 0).Count == 1)
                    {
                        if (Rhino.Geometry.Intersect.Intersection.CurveCurve(c[i], cs[q], 0, 0)[0].OverlapA == c[i].Domain && Rhino.Geometry.Intersect.Intersection.CurveCurve(c[i], cs[q], 0, 0)[0].OverlapB == cs[q].Domain)
                        {
                            getsame.Add(cs[q]);
                            cs[q] = null;
                            index.Add(q);
                            break;
                        }
                    }
                }
            }
            same = getsame;
            return index;
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
                return Resource1.curve_找出共线线段;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{b4becdb4-55cf-4285-b2ab-86c5b222ce5e}"); }
        }
    }
}