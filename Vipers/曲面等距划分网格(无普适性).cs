using System;
using System.Collections.Generic;
using PanelingTools;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace Vipers//////////////直接复制的PanelingTools插件的代码(无普适性)
{
    public class 曲面等距划分网格 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the 曲面等距划分网格 class.
        /// </summary>
        public 曲面等距划分网格()
            : base("曲面等距划分网格", "Nickname",
                "Description",
                "Category", "Subcategory")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.hidden; }
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("","S","",GH_ParamAccess.item);
            pManager.AddNumberParameter("","D1","",GH_ParamAccess.item);
            pManager.AddNumberParameter("","D2","",GH_ParamAccess.item);
            pManager.AddBooleanParameter("","B","",GH_ParamAccess.item,false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("","P","",GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Surface destination = null;
            double naN = double.NaN;
            double num2 = double.NaN;
            bool flag = true;
            if ((((DA.GetData<Surface>(0, ref destination) && DA.GetData<double>(1, ref naN)) && DA.GetData<double>(2, ref num2)) && DA.GetData<bool>(3, ref flag)) && ((destination.IsValid && (naN > 0.0)) && (num2 > 0.0)))
            {
                if (naN > num2)
                {
                    if ((naN % num2) > 0.0)
                    {
                        this.AddRuntimeMessage((GH_RuntimeMessageLevel)1, "One distance parameter needs to be a multiplier of the other");
                        return;
                    }
                }
                else if ((num2 > naN) && ((num2 % naN) > 0.0))
                {
                    this.AddRuntimeMessage((GH_RuntimeMessageLevel)1, "One distance parameter needs to be a multiplier of the other");
                    return;
                }
                List<Point3d[]> rhGrid = Utility.DivideSurfaceByChordLength(destination, naN, num2, flag, 0.0, 0.0);
                GH_Structure<GH_Point> grid = new GH_Structure<GH_Point>();
                GH_Path path = new GH_Path(DA.ParameterTargetPath(0));
                path = path.AppendElement(DA.ParameterTargetIndex(0));
                if (!PtListArrayToGhTreePoint(rhGrid, ref grid, path))
                {
                    this.AddRuntimeMessage((GH_RuntimeMessageLevel)1, "Failed to convert Rhino list array to GH tree.");
                }
                else
                {
                    DA.SetDataTree(0, grid);
                }
            }
        }
        public static bool PtListArrayToGhTreePoint(List<Point3d[]> rhGrid, ref GH_Structure<GH_Point> grid, GH_Path path)
        {
            int num = 0;
            foreach (Point3d[] pointdArray in rhGrid)
            {
                if (pointdArray.Length > 0)
                {
                    List<GH_Point> data = new List<GH_Point>();
                    foreach (Point3d pointd in pointdArray)
                    {
                        data.Add(new GH_Point(pointd));
                    }
                    grid.AppendRange(data, path.AppendElement(num++));
                }
            }
            return true;
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
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{712c8c60-0504-4896-a1ad-43ebd26ab257}"); }
        }
    }
}