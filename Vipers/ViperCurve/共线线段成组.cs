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

namespace Vipers
{
    public class 共线线段成组 : GH_Component ////TangChi 2016.12.22
    {
        /// <summary>
        /// Initializes a new instance of the 共线线段成组 class.
        /// </summary>
        public 共线线段成组()
            : base("共线线段成组", "LineGroups",
    "将近似共线的线段成组，相邻线段之间可以没有连接，通过调节角度公差和距离公差来确定相邻线段是否共线",
    "Vipers", "Viper.curve")
        {
            Message = "共线线段成组";
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddLineParameter("线段","L","待分组的线段",GH_ParamAccess.list);
            AddNumberInput();
            pManager.AddNumberParameter("距离公差", "T", "相邻线段如果距离在此公差范围内，则视为共线", GH_ParamAccess.item, 0);
            pManager.HideParameter(0);
        }
        Param_Number p = new Param_Number();
        public void AddNumberInput()////创建number输出端 该输出端可以添加角度选项
        {
            p.AngleParameter = true;
            p.Name = "角度公差";
            p.NickName = "A";
            p.Description = "相邻线段如果角度在此公差范围内，则视为共线";
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
            pManager.AddLineParameter("分组线段","G","根据条件共线线段分组",GH_ParamAccess.tree);
            pManager.AddLineParameter("剩余线段","R","不满足条件的独立线段",GH_ParamAccess.list);
            pManager.AddIntegerParameter("索引","I1","共线线段的索引位置",GH_ParamAccess.tree);
            pManager.AddIntegerParameter("索引", "I2", "独立线段的索引位置", GH_ParamAccess.list);
            pManager.HideParameter(1);
        }
        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Line> lines = new List<Line>();
            double t1 = 0;
            double t2 = 0;
            if(!DA.GetDataList(0,lines)||!DA.GetData(1,ref t1)||!DA.GetData(2,ref t2))return;
            if(p.UseDegrees)
              t1=  RhinoMath.ToRadians(t1);
            ////t1 角度公差  t2 两条相邻直线间的距离公差
            DataTree<Line> last = new DataTree<Line>();
            DataTree<int> lastIndex = new DataTree<int>();
            List<Line> restLine = new List<Line>();
            List<int> restIndex = new List<int>();
            List<Line> collects = new List<Line>();
            List<int> indexes = new List<int>();
            Line collect = Line.Unset;
            int count = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i] == Line.Unset) continue;
                collects.Add(lines[i]);
                indexes.Add(i);
                int mem = i;
                bool flag = true;
                while (true)///找出满足ln1的线段 注意，这只是单方向
                {
                    Line ln1 = collects[collects.Count - 1];
                    Point3d center1 = ln1.ToNurbsCurve().PointAtNormalizedLength(0.5);///中点
                    double distance = double.MaxValue;
                    for (int q = 0; q < lines.Count; q++)/////从剩余的选择满足条件的最近线段
                    {
                        if (lines[q] == Line.Unset || q == mem || q == i) continue;
                        Line ln2 = lines[q];
                        Point3d center2 = ln2.ToNurbsCurve().PointAtNormalizedLength(0.5);///中点
                        if (CoLine(ln1, ln2, t1, t2) && distance > center1.DistanceTo(center2))
                        {
                            collect = ln2;
                            distance = center1.DistanceTo(center2);
                            mem = q;
                        }
                    }
                    distance = double.MaxValue;
                    if (collect != Line.Unset)///说明上式选出了满足条件的线段
                    {
                        collects.Add(collect);
                        indexes.Add(mem);
                        lines[mem] = Line.Unset;
                        collect = Line.Unset;
                        continue;
                    }
                    if (collects.Count > 1 && flag)/////第二次遍历
                    {
                        flag = false;
                        collects.Reverse();
                        indexes.Reverse();
                        continue;
                    }
                    break;////没有就退出
                }
                if (collects.Count > 1)
                {
                    last.AddRange(collects, new GH_Path(0, count));
                    lastIndex.AddRange(indexes, new GH_Path(0, count));
                    lines[i] = Line.Unset;
                    count++;
                }
                collects.Clear();
                indexes.Clear();
            }
            for (int i = 0; i < lines.Count; i++)////剩余线段
            {
                if (lines[i] == Line.Unset) continue;
                restLine.Add(lines[i]);
                restIndex.Add(i);
            }
            DA.SetDataTree(0, last);
            DA.SetDataTree(2, lastIndex);
            DA.SetDataList(1, restLine);
            DA.SetDataList(3, restIndex);
        }
        public bool CoLine(Line x, Line y, double t1, double t2) //////判断两条线段是否在公差范围内共线
        {
            /////////////////////通过角度和距离判断两条线段是否共线
            Vector3d vc1 = x.Direction;
            Vector3d vc2 = y.Direction;
            Point3d center = y.ToNurbsCurve().PointAtNormalizedLength(0.5);
            Point3d closet = x.ClosestPoint(center, false);
            double angle = Vector3d.VectorAngle(vc1, vc2);
            if (angle > Math.PI * 0.5) angle = Math.PI - angle;
            double distance = center.DistanceTo(closet);
            if (angle <= t1 + 0.000001 && distance <= t2 + 0.0000001)
                return true;
            else
                return false;
        }
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Resource1.curve_共线线段成组;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{3ef7ab27-145c-4801-857a-01b301574fb4}"); }
        }
    }
}