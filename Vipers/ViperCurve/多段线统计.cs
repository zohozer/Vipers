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

namespace Vipers////TangChi 2016.3.14
{
    public class PolylineClassification : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the 多边形统计 class.
        /// </summary>
        public PolylineClassification()
            : base("多段线统计", "PolylineClassification",
                "将相同（边数，各边长，各内角相同的）多段线归类，可通过调节公差，将近似相同的多边形归为一类",
                "Vipers", "Viper.curve")
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
            pManager.AddCurveParameter("多段线","P","待分类的多边形",GH_ParamAccess.list);
            pManager.AddNumberParameter("长度公差","T","长度在此公差范围内的多段线视为边长相等",GH_ParamAccess.item,0);
            AddNumberInput();
            pManager.HideParameter(0);
            Message = "多段线统计";
        }
        Param_Number p = new Param_Number();
        public void AddNumberInput()////创建number输出端 该输出端可以添加角度选项
        {
            p.AngleParameter = true;
            p.Name = "角度公差";
            p.NickName = "A";
            p.Description = "角度在此公差范围内的多段线视为角度相等";
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
            pManager.AddCurveParameter("多段线","P","归类后的多段线",GH_ParamAccess.tree);
            pManager.AddIntegerParameter("索引", "I", "归类后多边形在原多段线的索引位置", GH_ParamAccess.tree);
            pManager.AddTextParameter("警告","W","如果输入端有非多段线，则不让该曲线参与归类",GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> polylines=new List<Curve>();
            double tolerance1=0;
            double tolerance2=0;
            if(!DA.GetDataList(0,polylines)) return;
            if(!DA.GetData(1,ref tolerance1))return;
            if (!DA.GetData(2, ref tolerance2)) return;
            if (p.UseDegrees)
                tolerance2 = RhinoMath.ToRadians(tolerance2);
            List<Polyline> x = new List<Polyline>();
            List<string> wrong = new List<string>();
            for (int i = 0;i<polylines.Count; i++)
            {
                Polyline pl = null;
                if (polylines[i].TryGetPolyline(out pl))
                {
                    x.Add(pl);
                }
                else 
                {
                    string mm = "序号为" + i + "的曲线不是多段线";
                    wrong.Add(mm);
                }
            }
            double t1 = tolerance1;
            double t2 = tolerance2;
            DataTree<double> length = new DataTree<double>();
            DataTree<double> angle = new DataTree<double>();
            int num = 0;
            for (int i = 0; i < x.Count; i++)
            {
                angle.AddRange(Angles(x[i]), new GH_Path(0, num));
                Line[] lines = x[i].GetSegments();
                for (int j = 0; j < lines.Length; j++)
                {
                    length.Add(lines[j].Length, new GH_Path(0, num));
                }
                num++;
            }
            /////////////////////////////////////////////////////////////////
            DataTree<Polyline> last = new DataTree<Polyline>();
            DataTree<int> index = new DataTree<int>();
            num = 0;
            for (int i = 0; i < x.Count; i++)
            {
                if (x[i] == null)
                {
                    continue;
                }
                last.Add(x[i], new GH_Path(0, DA.Iteration,num));
                index.Add(i, new GH_Path(0, DA.Iteration,num));
                for (int j = 0; j < x.Count; j++)
                {
                    if (x[j] == null || i == j || x[i].SegmentCount != x[j].SegmentCount)
                    {
                        continue;
                    }
                    if (Test(length.Branch(i), length.Branch(j), t1) && Test(angle.Branch(i), angle.Branch(j), t2))
                    {
                        last.Add(x[j], new GH_Path(0,DA.Iteration, num));
                        index.Add(j, new GH_Path(0,DA.Iteration, num));
                        x[j] = null;
                    }
                }
                x[i] = null;
                num++;
            }
            DA.SetDataTree(0, last);
            DA.SetDataTree(1, index);
            DA.SetDataList(2, wrong);
        }
        public List<double> Angles(Polyline polyline) //////////////多边形内角
        {
            if (polyline.IsClosed)
            {
                Line[] lines = polyline.GetSegments();
                List<Point3d> pts = new List<Point3d>();
                for (int i = 0; i < lines.Length; i++)
                {
                    Line l = lines[i];
                    pts.Add(l.From);
                }
                List<Point3d> ptA = new List<Point3d>();
                List<Point3d> ptB = new List<Point3d>();
                for (int i = 0; i < lines.Length; i++)////角点的左边点
                {
                    if (i == lines.Length - 1)
                    {
                        ptA.Add(pts[0]);
                        break;
                    }
                    ptA.Add(pts[i + 1]);
                }
                for (int i = 0; i < lines.Length; i++)////角点的右边点
                {
                    if (i == 0)
                    {
                        ptB.Add(pts[lines.Length - 1]);
                        continue;
                    }
                    ptB.Add(pts[i - 1]);
                }
                List<double> agl = new List<double>();
                List<double> rad = new List<double>();
                for (int i = 0; i < lines.Length; i++)
                {
                    Vector3d v1 = Point3d.Subtract(pts[i], ptA[i]);
                    Vector3d v2 = Point3d.Subtract(pts[i], ptB[i]);
                    agl.Add(Vector3d.VectorAngle(v1, v2) * 180 / Math.PI);
                }
                return agl;
            }
            else
            {
                Line[] lines = polyline.GetSegments();
                List<Point3d> pts = new List<Point3d>();
                for (int i = 0; i < lines.Length; i++)
                {
                    Line l = lines[i];
                    pts.Add(l.From);
                }
                pts.Add(lines[lines.Length - 1].To);/////加入最后一个点
                List<Point3d> ptCenter = new List<Point3d>();
                List<Point3d> ptA = new List<Point3d>();
                List<Point3d> ptB = new List<Point3d>();
                for (int i = 0; i < pts.Count - 2; i++)
                {
                    ptA.Add(pts[i]);
                    ptCenter.Add(pts[i + 1]);
                    ptB.Add(pts[i + 2]);
                }
                List<double> agl = new List<double>();
                List<double> rad = new List<double>();
                for (int i = 0; i < ptA.Count; i++)
                {
                    Vector3d v1 = Point3d.Subtract(ptCenter[i], ptA[i]);
                    Vector3d v2 = Point3d.Subtract(ptCenter[i], ptB[i]);
                    agl.Add(Vector3d.VectorAngle(v1, v2) * 180 / Math.PI);
                }
                return agl;
            }
        }
        public bool Test(List<double> listA, List<double> listB, double tolerance) ///两组列表值全等
        {
            bool mm = true;
            //////////////////////////////////////////////////
            double[] AA = listA.ToArray();
            double[] BB = listB.ToArray();
            List<double> listA2 = AA.ToList();
            List<double> listB2 = BB.ToList();
            //////////////////////////////////////////////////引用类型这里如果不复制一个数组的话会出错
            if (listA2.Count == listB2.Count)
            {
                for (int i = 0; i < listA2.Count; i += 0)
                {
                    double aa = listA2[i];
                    listA2.RemoveAt(i);
                    for (int j = 0; j < listB2.Count; j++)
                    {
                        if (Math.Abs(aa - listB2[j]) <= tolerance + 0.0000001)
                        {
                            listB2.RemoveAt(j);
                            break;
                        }
                    }
                    if (listA2.Count != listB2.Count)
                    {
                        mm = false;
                        break;
                    }
                }
            }
            else
            {
                mm = false;
            }
            return mm;
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
                return Resource1.curve_多段线统计;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{e5a35235-e6d0-4c69-85ba-9e0f577e31e6}"); }
        }
    }
}