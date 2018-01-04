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
namespace Vipers//////TangChi 2015.7.8
{
    public class UnevenSurface : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent23 class.
        /// </summary>
        public UnevenSurface()
            : base("Divide Surface Randomly", "SrfDivideRand",
                "Random division of the surface",
                "Vipers", "Viper.surface")
        {
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
           pManager.AddSurfaceParameter("Surface","S","Surface to divide",GH_ParamAccess.item);
            pManager.AddIntegerParameter("U Count","U","Number of segments in U direction",GH_ParamAccess.item,10);
            pManager.AddIntegerParameter("V Count","V", "Number of segments in V direction", GH_ParamAccess.item,10);
            pManager.AddIntegerParameter("Difference","D", "The larger the number, the more uneven the effect is", GH_ParamAccess.item,20);
            pManager.AddIntegerParameter("Seed","S","Seed of random variation",GH_ParamAccess.item,0);
            Message = "Divide Surface Randomly";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface","S","Dividing surface",GH_ParamAccess.tree);
            pManager.AddInterval2DParameter("Interval","I", "The corresponding interval for each surface", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Surface surface = null;
            int uNumber = 0;
            int vNumber = 0;
            int otherness = 0;
            int seed = 0;
            if(!DA.GetData(0,ref surface)) return;
            if (!DA.GetData(1, ref uNumber)) return;
            if (!DA.GetData(2, ref vNumber)) return;
            if (!DA.GetData(3, ref otherness)) return;
            if (!DA.GetData(4, ref seed)) return;
            Surface x = surface;
            int y = uNumber;
            int z = vNumber;
            if (otherness > 0.5)
            {
                Random rand = new Random(seed);
                List<double> r1 = new List<double>();
                List<double> r2 = new List<double>();
                for (int i = 0; i < y + z; i++)
                {
                    double random = rand.Next(otherness * 10);
                    if (random == 0)
                    {
                        i--;
                        continue;
                    }
                    if (i >= y)
                    {
                        r2.Add(random / 10);
                        continue;
                    }
                    r1.Add(random / 10);
                }
                Interval uu = x.Domain(0);
                Interval vv = x.Domain(1);
                List<Interval> ulist = randInterval(uu, r1);
                List<Interval> vlist = randInterval(vv, r2);
                DataTree<Surface> sss = new DataTree<Surface>();
                DataTree<UVInterval> uv = new DataTree<UVInterval>();
                int index = 0;
                for (int i = 0; i < y; i++)
                {
                    for (int j = 0; j < z; j++)
                    {
                        UVInterval uvinterval = new UVInterval(ulist[i], vlist[j]);
                        sss.Add(x.Trim(ulist[i], vlist[j]), new GH_Path(0, index));
                        uv.Add(uvinterval, new GH_Path(0, index));
                    }
                    index++;
                }
                DA.SetDataTree(0, sss);
                DA.SetDataTree(1, uv);
            }
  
        }
        public List<Interval> randInterval(Interval singleInterval, List<double> listNumbers)
        {
            double t1 = singleInterval.T0;
            double t2 = singleInterval.T1;
            double t = t2 - t1;
            double total = listNumbers.Sum();
            List<double> mems = new List<double>();
            for (int i = 0; i < listNumbers.Count; i++)
            {
                mems.Add(listNumbers[i] / total * t);
            }
            List<Interval> interval = new List<Interval>();
            for (int i = 0; i < listNumbers.Count; i++)
            {
                Interval v = new Interval(t1, t1 + mems[i]);
                t1 += mems[i];
                interval.Add(v);
            }
            return interval;
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
               // return Resource1.不均匀划分曲面;
                return Resource1.surface_随机划分曲面;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{0999216a-3435-4fed-8777-5e28f5922624}"); }
        }
    }
}