using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers///////TangChi 2016.4.25
{
    public class ArcFittingEllipse : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the 四段弧拟合椭圆 class.
        /// </summary>
        public ArcFittingEllipse()
            : base("四段弧拟合椭圆", "ArcFittingEllipse",
                "用四段圆弧拟合指定椭圆，椭圆越扁误差越大",
                "Vipers", "Viper.curve")
        {
            Message = "四段圆弧拟合椭圆";
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.obscure; }
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("椭圆","E","待拟合的椭圆",GH_ParamAccess.item);
            pManager.HideParameter(0);
            
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddArcParameter("圆弧","A","拟合结果",GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {


            Curve x = null;
            if(!DA.GetData(0,ref x))return;
            List<Arc> last = new List<Arc>();
            x.Domain = new Interval(0, 1);
            double seg = 0.25 / 500;
            double t = 0;
            double max = double.MaxValue;/////之差绝对值接近0满足要求
            for (int i = 0; i < 500; i++)
            {
                t += seg;
                Arc arc1 = new Arc(x.PointAt(t), x.PointAt(0.25), x.PointAt(0.5 - t));
                Arc arc2 = new Arc(x.PointAt(0.5 - t), x.PointAt(0.5), x.PointAt(0.5 + t));
                double cha = Math.Abs(arc1.Length + arc2.Length - x.GetLength() / 2);///差的绝对值
                if (cha < max)
                {
                    Vector3d v1 = arc1.ToNurbsCurve().TangentAtEnd;
                    Vector3d v2 = arc2.ToNurbsCurve().TangentAtStart;
                    double angle = Vector3d.VectorAngle(v1, v2);
                    if (angle > Math.PI * 0.5)
                    {
                        angle = Math.PI - angle;
                    }
                    if (angle <= 0.01)
                    {
                        max = cha;
                        last.Clear();
                        last.Add(arc1);
                        last.Add(arc2);
                        Arc arc3 = new Arc(x.PointAt(0.5 + t), x.PointAt(0.75), x.PointAt(1 - t));
                        Arc arc4 = new Arc(x.PointAt(1 - t), x.PointAt(0), x.PointAt(t));
                        last.Add(arc3);
                        last.Add(arc4);
                    }
                }
            }
            DA.SetDataList(0, last);
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
                return Resource1.curve_四圆弧拟合椭圆;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{cc9692ef-cc4b-44e1-b60e-610d2886f3bc}"); }
        }
    }
}