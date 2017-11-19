using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers ///TangChi 2015.5.21
{
    public class Viperxyz : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent2 class.
        /// </summary>
        public Viperxyz()
            : base("求三元一次方程整数解", "TernaryEequation",
                "求出满足条件的x，y，z的整数解",
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
            pManager.AddNumberParameter("总数", "S", "三元一次方程总和", GH_ParamAccess.item);
            pManager.AddNumberParameter("x项系数", "a", "x项系数", GH_ParamAccess.item);
            pManager.AddNumberParameter("y项系数", "b", "y项系数", GH_ParamAccess.item);
            pManager.AddNumberParameter("z项系数", "c", "z项系数", GH_ParamAccess.item);
            pManager.AddNumberParameter("常数项", "d", "三元一次方程常数项", GH_ParamAccess.item);
            Message = "S=ax+by+cz+d";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("x的整数解", "NX", "x的整数解", GH_ParamAccess.list);
            pManager.AddIntegerParameter("y的整数解", "NY", "y的整数解", GH_ParamAccess.list);
            pManager.AddIntegerParameter("z的整数解", "NZ", "z的整数解", GH_ParamAccess.list);
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
            double c = 0;
            double d = 0;
            if (!DA.GetData(0, ref S)) return;
            if (!DA.GetData(1, ref a)) return;
            if (!DA.GetData(2, ref b)) return;
            if (!DA.GetData(3, ref c)) return;
            if (!DA.GetData(4, ref d)) return;
            List<int> xx = new List<int>();
            List<int> yy = new List<int>();
            List<int> zz = new List<int>();
            if (a != 0 && b != 0 && c != 0)
            {
                for (int i = 0; a * i <= S; i++)
                {
                    for (int j = 0; a * i + b * j <= S; j++)
                    {
                        for (int k = 0; a * i + b * j + c * k <= S; k++)
                        {
                            if (a * i + b * j + c * k + d == S)
                            {
                                xx.Add(i);
                                yy.Add(j);
                                zz.Add(k);
                            }
                        }
                    }
                }

                DA.SetDataList(0, xx);
                DA.SetDataList(1, yy);
                DA.SetDataList(2, zz);
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
                //return Resource1.v15;
                return Resource1.math_三次方程;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{8bf2258b-90b9-45c6-a156-d0e9b5125d38}"); }
        }
    }
}