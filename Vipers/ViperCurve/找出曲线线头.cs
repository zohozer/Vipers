using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers////////TangChi 2015.9.14
{
    public class ThrumOfCurve : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent44 class.
        /// </summary>
        public ThrumOfCurve()
            : base("找出曲线线头", "ThrumOfCurve",
                "找出线头曲线，该曲线的端点有一个或两个不在其它曲线上",
                "Vipers", "Viper.curve")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("曲线","C","待查找是否有线头的曲线",GH_ParamAccess.list);
            pManager.HideParameter(0);
            Message = "找出曲线线头";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("线头","P","不在其它曲线上的点",GH_ParamAccess.list);
            pManager.AddCurveParameter("曲线","C","线头所在的曲线",GH_ParamAccess.list);
            pManager.AddIntegerParameter("索引值", "I", "线头曲线在原曲线中的位置", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> curves=new List<Curve>();
            if(!DA.GetDataList(0,curves))return;
            List<Curve> c = curves;
            List<Curve> cs = new List<Curve>();///有线头的曲线
            List<Point3d> pts = new List<Point3d>();////线头
            List<int> index = new List<int>();/////线头曲线的序号
            for (int i = 0; i < c.Count; i++)
            {
                Curve cr = c[i];///遍历每条曲线
                                if(cr.IsClosed)
                                {
                                    continue;
                                }
                Point3d pt1 = cr.PointAtStart;
                Point3d pt2 = cr.PointAtEnd;
                int flag1 = 0;///与cr没有交点的曲线的数量（当达到count-1个时，就表明是线头）
                int flag2 = 0;
                for (int j = 0; j < c.Count; j++)
                {
                    if (j == i)
                    {
                        continue;
                    }
                    Curve test = c[j];
                    double t1;
                    double t2;
                    test.ClosestPoint(pt1, out t1);
                    test.ClosestPoint(pt2, out t2);
                    Point3d pta = test.PointAt(t1);
                    Point3d ptb = test.PointAt(t2);
                    double dist1 = pt1.DistanceTo(pta);
                    double dist2 = pt2.DistanceTo(ptb);
                    if (dist1 > 0.00001)//两个端点都不在曲线上
                    {
                        flag1++;
                    }
                    if (dist2 > 0.00001)
                    {
                        flag2++;
                    }
                }
                if (flag1 == c.Count - 1)
                {
                    pts.Add(pt1);
                }
                if (flag2 == c.Count - 1)
                {
                    pts.Add(pt2);
                }
                if (flag1 == c.Count - 1 || flag2 == c.Count - 1)
                {
                    cs.Add(cr);
                    index.Add(i);
                }
            }
            DA.SetDataList(0, pts);
            DA.SetDataList(1, cs);
            DA.SetDataList(2, index);
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
                //return Resource1.找出曲线线头;
                return Resource1.curve_曲线线头;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{7668ef38-4b40-4724-bfbb-cc3c4d20837f}"); }
        }
    }
}