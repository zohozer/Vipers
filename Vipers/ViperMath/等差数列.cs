using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.ComponentModel;

namespace Vipers
{
    public class ViperSn : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent2 class.
        /// </summary>
        public ViperSn()
            : base("等差数列给定三个未知数求出第四个未知数", "SuperSeries",
                "如输入Sn,a1,d,则Result输出n值，如果输入a1,d,n,则Result输出Sn值，如果输入Sn,a1,d,n,则判断该数列是否成立，Result输出true或false",
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
            pManager.AddNumberParameter("等差数列之和", "Sn", "等差数列之和", GH_ParamAccess.item, double .Epsilon);
            pManager.AddNumberParameter("起始数", "a1", "等差数列起始数", GH_ParamAccess.item, double.Epsilon);
            pManager.AddNumberParameter("公差", "d", "等差数列公差", GH_ParamAccess.item, double.Epsilon);
            pManager.AddNumberParameter("项数", "n", "等差数列项数", GH_ParamAccess.item, double.Epsilon);
            Message = "Sn=a1+a2+……+an";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("等差数列之和", "Sn", "等差数列之和", GH_ParamAccess.item);
            pManager.AddNumberParameter("起始数", "a1", "等差数列起始数", GH_ParamAccess.item);
            pManager.AddNumberParameter("公差", "d", "等差数列公差", GH_ParamAccess.item);
            pManager.AddIntegerParameter("项数", "n", "等差数列项数", GH_ParamAccess.item);
            pManager.AddBooleanParameter("判断结果", "checking", "如果四个输入端都有数据，则判断该公式是否成立", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double Sn = 0;
            double a1 = 0;
            double d = 0;
            double n = 0;
            if (!DA.GetData(0, ref Sn)) return;
            if (!DA.GetData(1, ref a1)) return;
            if (!DA.GetData(2, ref d)) return;
            if (!DA.GetData(3, ref n)) return;
            if (Sn == double.Epsilon && a1 != double.Epsilon && d != double.Epsilon && n != double.Epsilon)
            {
                Sn = (2 * a1 + (n - 1) * d) * n / 2;
                DA.SetData(0, Sn);
            }
            else if (Sn != double.Epsilon && a1 == double.Epsilon && d != double.Epsilon && n != double.Epsilon)
            {
                a1 = (2 * Sn / n - (n - 1) * d) / 2;
                DA.SetData(1, a1);
            }
            else if (Sn != double.Epsilon && a1 != double.Epsilon && d == double.Epsilon && n != double.Epsilon)
            {
                d = (2 * Sn / n - 2 * a1) / (n - 1);
                DA.SetData(2, d);
            }
            else if (Sn != double.Epsilon && a1 != double.Epsilon && d != double.Epsilon && n == double.Epsilon)
            {
                if (a1 == 0 && d == 0)
                {

                }
                else
                {
                    for (int i = 0; (2 * a1 + (i - 1) * d) * i / 2 <= Sn; i++)
                    {
                        DA.SetData(3, i);
                    }
                }
            }
            else if (Sn != double.Epsilon && a1 != double.Epsilon && d != double.Epsilon && n != double.Epsilon)
            {
                if ((2 * a1 + (n - 1) * d) * n / 2 == Sn)
                {
                    DA.SetData(4, true);
                }
                else
                {
                    DA.SetData(4, false);
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
                //return Resource1.v19;
                return Resource1.math_等差数列;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{7cbaf34f-4c49-4e26-8925-3490f349b11e}"); }
        }
    }
}
