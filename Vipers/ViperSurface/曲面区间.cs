using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers
{
    public class SurfaceDomain : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SurfaceDomain class.
        /// </summary>
        public SurfaceDomain()
            : base("曲面区间", "SurfaceDomain",
                "更改曲面区间（默认输出曲面的原区间）",
                "Vipers", "Viper.surface")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("曲面","S","待修改区间的曲面",GH_ParamAccess.item);
            pManager.AddIntervalParameter("U区间", "U", "用户设置u方向区间（如果不更改，则输出原曲面u方向区间）", GH_ParamAccess.item, new Interval(0, 0));
            pManager.AddIntervalParameter("V区间","V","用户设置v方向区间（如果不更改，则输出原曲面v方向区间）",GH_ParamAccess.item,new Interval(0,0));
            pManager.HideParameter(0);
            Message = "曲面区间";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter("曲面", "S", "修改后的曲面", GH_ParamAccess.item);
            pManager.AddIntervalParameter("U区间", "U", "修改后u方向区间", GH_ParamAccess.item);
            pManager.AddIntervalParameter("V区间", "V", "修改后v方向区间", GH_ParamAccess.item);
            pManager.AddIntegerParameter("闭合情况","R","如果是闭合曲面，则判断是哪里闭合(0：U方向闭合  1：V方向闭合)",GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Surface srf = null;
            Interval u = new Interval();
            Interval v = new Interval();
            if (!DA.GetData(0,ref srf))return;
            if (!DA.GetData(1, ref u)) return;
            if (!DA.GetData(2, ref v)) return;
            if(u.ToString() !=new Interval(0,0).ToString())
            {
                srf.SetDomain(0,u);
            }
            else if (v.ToString() != new Interval(0, 0).ToString())
            {
                srf.SetDomain(0, v);
            }
            if(srf.IsClosed(0))
            {
                DA.SetData(3,0);
            }
             else if (srf.IsClosed(1))
            {
                DA.SetData(3, 1);
            }
            DA.SetData(0,srf);
            DA.SetData(1, srf.Domain(0));
            DA.SetData(2, srf.Domain(1));
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
                return Resource1.surface_曲面区间;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{bdec90ab-4122-47b1-8c1f-1a4a94df5bf7}"); }
        }
    }
}