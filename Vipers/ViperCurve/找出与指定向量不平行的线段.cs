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

namespace Vipers ///TangChi 2015.5.21(2016.3.16)
{
    public class ViperVectorLine : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent4 class.
        /// </summary>
        public ViperVectorLine()
            : base("找出与指定向量不平行的线段", "CurveParallel",
                "通过用户设置的向量，找出与之不平行的线段",
                "Vipers", "Viper.curve")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddLineParameter("待筛选的线段", "L", "待筛选的线段", GH_ParamAccess.list);
            pManager.AddVectorParameter("用户指定向量", "V", "用户指定的向量如果线段不与之平行则被选出", GH_ParamAccess.list,Vector3d.XAxis);
            AddNumberInput();
            pManager.HideParameter(0);
            Message = "与指定向量不平行的线段";
        }
        Param_Number p = new Param_Number();
        public void AddNumberInput()////创建number输出端 该输出端可以添加角度选项
        {
            p.AngleParameter = true;
            p.Name = "角度";
            p.NickName = "A";
            p.Description = "如果直线与指定向量夹角在此范围内，则视为平行";
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
            List<Vector3d> vectors = new List<Vector3d>();
            double angle = 0;
            if(!DA.GetDataList(0,lines)) return;
            if(!DA.GetDataList(1,vectors)) return;
            if(!DA.GetData(2,ref angle))return;
            if (p.UseDegrees)
                angle = RhinoMath.ToRadians(angle);
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
                //return Resource1.v14;
                return Resource1.curve_根据向量找出平行的线段;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{ab3c14f7-c613-49e8-8aaa-e66a6df0671d}"); }
        }
    }
}