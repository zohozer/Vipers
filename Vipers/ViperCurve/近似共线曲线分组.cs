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

namespace Vipers/////TangChi 2016.3.26
{
    public class SimilarCollinear : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SimilarCollinear class.
        /// </summary>
        public SimilarCollinear()
            : base("近似共线线段分组", "SimilarCollinear",
                "线段夹角如果在设置的角度范围内，则将这些线段分为一组",
                "Vipers", "Viper.curve")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("线段", "C", "待分组的线段", GH_ParamAccess.list);
            AddNumberInput();
            pManager.HideParameter(0);
            Message = "近似共线线段分组";
        }
        Param_Number p = new Param_Number();
        public void AddNumberInput()////创建number输入端 该输出端可以添加角度选项
        {
            p.AngleParameter = true;
            p.Name = "角度";
            p.NickName = "A";
            p.Description = "在此角度(弧度数)范围的线段为一组";
            p.Access = GH_ParamAccess.item;
            p.SetPersistentData(Math.PI*0.1);
            Params.RegisterInputParam(p);
            p.Optional = true;
        }
        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("线段", "C", "分组后的线段", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("索引", "I", "分组后的线段在原来线段中的索引位置", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> curves = new List<Curve>();
            double angle = 0;
            if(!DA.GetDataList(0,curves))return;
            if(!DA.GetData(1,ref angle))return;
            if (p.UseDegrees)///设置的是角度则转换为弧度
                angle = RhinoMath.ToRadians(angle);
            DataTree<Curve> last = new DataTree<Curve>();
            DataTree<int> indexes = new DataTree<int>();
            List<Curve> collect = new List<Curve>();
            List<int> index = new List<int>();
            int mem = 0;
            int branch = DA.Iteration;
            bool flag1 = false;
            bool flag2 = false;
            bool flag3 = true;
            Curve c1 = null;
            for (int i = 0; i < curves.Count; i++)
            {
                if (curves[i] == null)
                {
                    continue;
                }
                if (!flag1)
                {
                    c1 = curves[i];
                }
                if (flag3)
                {
                    collect.Add(curves[i]);
                    index.Add(i);
                }
                flag1 = false;
                Vector3d v1 = Point3d.Subtract(c1.PointAtEnd, c1.PointAtStart);
                for (int q = 0; q < curves.Count; q++)
                {
                    if (curves[q] == null || i == q)
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
                    flag3 = false;
                    i--;
                    continue;
                }
                //////////////////////////////////////////////////////////////////先判断第一个方向所有满足条件的线
                //////////////////////////////////////////////////////////////////
                if (!flag2)
                {
                    flag1 = false;
                    flag2 = true;
                    i--;
                    flag3 = false;
                    index.Reverse();
                    collect.Reverse();
                    continue;
                }
                last.AddRange(collect, new GH_Path(0, branch,mem));
                indexes.AddRange(index, new GH_Path(0,branch,mem));
                mem++;
                curves[i] = null;
                collect.Clear();
                index.Clear();
                flag3 = true;
                flag2 = false;
                flag1 = false;
            }
            DA.SetDataTree(0, last);
            DA.SetDataTree(1, indexes);
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
        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Resource1.curve_近似共线曲线成组;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{508b76f6-7090-431e-8967-a83c376bc745}"); }
        }
    }
}