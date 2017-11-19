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

namespace Vipers///TangChi 2015.2.12
{
    public class ViperPointsByVector : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent2 class.
        /// </summary>
        public ViperPointsByVector()
            : base("根据已知向量找出共线点", "CollinearPoint",
                "根据用户提供的向量找出共线的点",
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
            pManager.AddPointParameter("待选点", "P", "待选出共线的点", GH_ParamAccess.list);
            pManager.AddVectorParameter("指定向量", "V", "用户指定向量，共线的待选点将被选出", GH_ParamAccess.item,Vector3d.XAxis);
            pManager.AddNumberParameter("公差","T","在此范围内视为与向量共线",GH_ParamAccess.item,0);
            pManager.HideParameter(0);
            Message = "通过向量找出共线点";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("与向量共线的点", "P", "与向量共线的点", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("序号", "I", "共线点在原来点中的序号", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> points = new List<Point3d>();
            Vector3d vector = new Vector3d();
            double tolerance = 0;
            if (!DA.GetDataList(0, points)) return;
            if (!DA.GetData(1, ref vector)) return;
            if (!DA.GetData(2, ref tolerance)) return;
            List<Point3d> p = points;
            Vector3d v = vector;
            ///////////////////////////////////////////
            DataTree<int> indexs = new DataTree<int>();//序号
            DataTree<Point3d> pts = new DataTree<Point3d>();//点
            Point3d max = new Point3d(0, 0, double.PositiveInfinity);
            int num = 0;
            for (int i = 0; i < p.Count; i++)
            {
                if (p[i] == max)
                {
                    continue;
                }
                List<Point3d> plist = new List<Point3d>();
                List<int> mem = new List<int>();
                ////////////////////////////////////////
                plist.Add(p[i]);
                mem.Add(i);
                p[i] = max;
                for (int j = i + 1; j < p.Count; j++)
                {
                    if (p[j] == max)
                    {
                        continue;
                    }
                    Vector3d vc = Point3d.Subtract(p[j], plist[0]);
                    double angle = Vector3d.VectorAngle(vc, v);
                    if(angle>Math.PI*0.5)
                    {
                        angle = Math.PI - angle;
                    }
                    if (angle - tolerance <= 0.0000001)
                    {
                        plist.Add(p[j]);
                        mem.Add(j);
                        p[j] = max;
                    }
                }
                pts.AddRange(plist, new GH_Path(0, num));
                indexs.AddRange(mem, new GH_Path(0, num));
                plist.Clear();
                mem.Clear();
                num++;
            }
            DA.SetDataTree(0, pts);
            DA.SetDataTree(1, indexs);
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
                //return Resource1.根据向量找出共线点;
                return Resource1.point_与指定向量平行点;
            }
        }
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{5967b898-2966-411f-bb05-fae678ff50a4}"); }
        }
    }
}