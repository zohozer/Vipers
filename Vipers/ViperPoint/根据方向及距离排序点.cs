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

namespace Vipers/////TangChi 2016.01.13
{
    public class SortByDD : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public SortByDD()
            : base("根据方向及距离排序点", "SortByDD",
                "通过设置起始点及第二点，确定点群的排序方向",
                "Vipers", "Viper.point")
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
            pManager.AddPointParameter("排序点","P","待排序的点",GH_ParamAccess.list);
            pManager.AddPointParameter("起始点","PA","排序的起始点",GH_ParamAccess.item);
            pManager.AddPointParameter("第二点", "PB", "排序的第二点，排序的方向将以该A,B两点的向量作为排序方向", GH_ParamAccess.item);
            AddNumberInput();
            pManager.HideParameter(0);
            pManager.HideParameter(1);
            pManager.HideParameter(2);
            Message = "根据方向及距离排序点";
        }
        Param_Number p = new Param_Number();
        public void AddNumberInput()////创建number输出端 该输出端可以添加角度选项
        {
            p.AngleParameter = true;
            p.Name = "角度范围";
            p.NickName = "A";
            p.Description = "调整角度范围，两向量角度在公差范围则被视为连续的点(默认为弧度数)";
            p.Access = GH_ParamAccess.item;
            p.SetPersistentData(0.2);
            Params.RegisterInputParam(p);
            p.Optional = true;
        }
        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("排序后的点", "P", "排序后得到的点", GH_ParamAccess.list);
            pManager.AddIntegerParameter("序号","I","排序后的点在原来点中的序号",GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> points = new List<Point3d>();
            Point3d point0 = new Point3d();
            Point3d point1 = new Point3d();
            double tolerance = 0;
            if(!DA.GetDataList(0,points))return;
            if (!DA.GetData(1, ref point0)) return;
            if (!DA.GetData(2, ref point1)) return;
            if (!DA.GetData(3, ref tolerance)) return;
            Vector3d vc2 = Point3d.Subtract(point1, point0);
            if (p.UseDegrees)
                tolerance = RhinoMath.ToRadians(tolerance);
            /////////////////////////////////////////////////////////
            List<int> mems = new List<int>();
            List<Point3d> pty = new List<Point3d>();
            bool flag = true;///终止循环
            pty.Add(point0);
            pty.Add(point1);
            for (int i = 0; i < 2; i++)///用户输入点的序号
            {
                for (int j = 0; j < points.Count; j++)
                {
                    if (points[j] == pty[i])
                    {
                        mems.Add(j);
                        break;
                    }
                }
            }
            #region
            while (flag)////////////////////该循环每循环一次，则找出最近的满足条件的一个点直到找完为止
            {
                double max = double.MaxValue;
                Point3d getPt = Point3d.Unset;
                int index = int.MaxValue;
                for (int i = 0; i < points.Count; i++)
                {
                    if (pty.Contains(points[i]))
                    {
                        continue;
                    }
                    Vector3d test = Point3d.Subtract(points[i], pty[pty.Count - 1]);
                    double angle = Vector3d.VectorAngle(test, vc2);
                    double dist = points[i].DistanceTo(pty[pty.Count - 1]);
                    if (angle <= tolerance && max > dist)
                    {
                        getPt = points[i];
                        max = dist;
                        index = i;
                    }
                }
                if (getPt != Point3d.Unset)
                {
                    vc2 = Point3d.Subtract(getPt, pty[pty.Count - 1]);
                    pty.Add(getPt);
                    mems.Add(index);
                }
                else
                {
                    flag = false;
                }
            }
            #endregion;
            DA.SetDataList(0, pty);
            DA.SetDataList(1, mems);
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
                return Resource1.point_根据方向及距离排序点;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{9b50fa95-e57e-41b8-8ebc-321e6614debd}"); }
        }
    }
}