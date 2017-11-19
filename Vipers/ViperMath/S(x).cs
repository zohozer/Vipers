using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers///// TangChi 2015.4.20
{
    public class Sn : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent41 class.
        /// </summary>
        public Sn()
            : base("S=a×n+b×(n+k)", "S=a×n+b×(n+k)",
                "已知总和S以及数值a（或b）以及a,b的数量之差，求出数值b(或a)以及a,b的数量。如果四个输入端都有数据，则找出是否有满足a，b数量关系的值存在",
                "Vipers", "Viper.math")
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
            pManager.AddNumberParameter("总和", "S", "表达式之和", GH_ParamAccess.item, double.Epsilon);
            pManager.AddNumberParameter("数值a", "a", "表达式中a的值", GH_ParamAccess.item, double.Epsilon);
            pManager.AddNumberParameter("数值b", "b", "表达式中b的值", GH_ParamAccess.item, double.Epsilon);
            pManager.AddIntegerParameter("k值", "k", "表达式中k的值（整数）", GH_ParamAccess.item, 1);
            Message = "S=a×n+b×(n+k)";
            
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("数值a", "a", "如果a为未知数则求出a的值", GH_ParamAccess.list);
            pManager.AddNumberParameter("数值b", "b", "如果b为未知数则求出b的值", GH_ParamAccess.list);
            pManager.AddIntegerParameter("a的数量", "Na", "满足条件的a的数量", GH_ParamAccess.list);
            pManager.AddIntegerParameter("b的数量", "Nb", "满足条件的b的数量", GH_ParamAccess.list);
            pManager.AddBooleanParameter("检验","C","如果四个输入端都有数据则判断等式是否成立",GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            double S = 0;
            double a = 0;
            double b = 0;
            int k = 1;
            if(!DA.GetData(0,ref S))return;
            if (!DA.GetData(1, ref a)) return;
            if (!DA.GetData(2, ref b)) return;
            if (!DA.GetData(3, ref k)) return;
            if (S != double.Epsilon && a != double.Epsilon && b == double.Epsilon)
            {
                List<int> N1 = new List<int>();
                List<int> N2 = new List<int>();
                List<double> bb = new List<double>();
                for (int i = 1; S > a * i; i++)
                {
                    double t1 = (S - i * a) / (i + k);
                    double t2 = Math.Round(t1, 4);
                    if (t1 == t2)
                    {
                        N1.Add(i);
                        N2.Add(i + k);
                        bb.Add(t1);
                    }
                }
                DA.SetData(0, a);
                DA.SetDataList(1, bb);
                DA.SetDataList(2, N1);
                DA.SetDataList(3, N2);
            }
            else if (S != double.Epsilon && a == double.Epsilon && b != double.Epsilon)
            {
                List<int> N1 = new List<int>();
                List<int> N2 = new List<int>();
                List<double> bb = new List<double>();
                for (int i = 1; S > b * i; i++)
                {
                    double t1 = (S - i * b) / (i - k);
                    double t2 = Math.Round(t1, 4);
                    if (t1 == t2)
                    {
                        N1.Add(i);
                        N2.Add(i - k);
                        bb.Add(t1);
                    }
                }
                DA.SetDataList(0, bb);
                DA.SetData(1, b);
                DA.SetDataList(2, N1);
                DA.SetDataList(3, N2);
            }
            else if (S != double.Epsilon && a != double.Epsilon && b != double.Epsilon && b != 0 && a != 0)
            {
                for (int i = 1; S > a * i; i++)
                {
                    double ss = a * i + b * (i + k);
                    if (ss == S)
                    {
                        DA.SetData(0, a);
                        DA.SetData(1, b);
                        DA.SetData(2, i);
                        DA.SetData(3, i + k);
                        break;
                    }
                    else
                    {
                        DA.SetData(4, false);
                    }

                }
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
                //return Resource1.S_x_;
                return Resource1.math_sx;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{385f276d-c040-4c34-9cf2-fb3d62c0254e}"); }
        }
    }
}