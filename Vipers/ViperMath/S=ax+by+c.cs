using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers///TangChi 2015.5.18
{
    public class ViperSxy : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent2 class.
        /// </summary>
        public ViperSxy()
            : base("求二元一次方程整数解", "BinaryEquation",
                "求出满足条件的x与y的整数解",
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
            pManager.AddNumberParameter("总数","S","二元一次方程总和",GH_ParamAccess.item);
            pManager.AddNumberParameter("x项系数", "a", "x项系数", GH_ParamAccess.item);
            pManager.AddNumberParameter("y项系数", "b", "y项系数", GH_ParamAccess.item);
            pManager.AddNumberParameter("常数项", "c", "二元一次方程常数项", GH_ParamAccess.item);
            Message = "S=ax+by+c";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("x的整数解","NX","x的整数解",GH_ParamAccess.list);
            pManager.AddIntegerParameter("y的整数解", "NY", "y的整数解", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double S=0;
            double a = 0;
            double b = 0;
            double c = 0;
            if(!DA.GetData(0,ref S)) return;
            if (!DA.GetData(1, ref a)) return;
            if (!DA.GetData(2, ref b)) return;
            if (!DA.GetData(3, ref c)) return;
            List<int> xx = new List<int>();
            List<int> yy = new List<int>();
            if (a != 0 && b != 0)
            {
                for (int i = 0; S > a * i; i++)
                {
                    for (int j = 0; S > a * i + b * j; j++)
                    {
                        if (S == a * i + b * j + c)
                        {
                            xx.Add(i);
                            yy.Add(j);
                        }
                    }
                }
                DA.SetDataList(0, xx);
                DA.SetDataList(1 ,yy);
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
                //return Resource1.v16;
                return Resource1.math_二次方程;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{139d23c4-e119-4808-a2eb-be91e4bf9b6d}"); }
        }
    }
}