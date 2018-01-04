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
using Vipers;

namespace SuperVipers///TangChi 2015.8.5
{
    public class BulgeSkin : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent58 class.
        /// </summary>
        public BulgeSkin()
            : base("Protrusion 1", "BulgeSkin",
                "A flat protrusion on input surface",
                "Vipers", "Viper.surface")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Surface", "S", "Surface to operate on", GH_ParamAccess.item);
            pManager.AddNumberParameter("Height", "H", "Height of protruson", GH_ParamAccess.item, 10);
            pManager.AddNumberParameter("Protrusion", "P", "Degree of protrusion (1 for full enclosure)", GH_ParamAccess.item, 0.5);
            pManager.AddBooleanParameter("Flip", "F", "Flip direction", GH_ParamAccess.item, true);
            pManager.HideParameter(0);
            Message = "Protrusion 1";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "B", "Resulting geometry", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Brep surface = new Brep();
            double height = 0;
            double percent = 0;
            bool isSwitch = true;
            if (!DA.GetData(0, ref surface)) return;
            if (!DA.GetData(1, ref height)) return;
            if (!DA.GetData(2, ref percent)) return;
            if (!DA.GetData(3, ref isSwitch)) return;
            Brep x = surface;
            double h = height;
            double l = percent;
            bool b = isSwitch;
            if (b == false)
            {
                h = h * (-1);
            }
            if (l > 1 || l < 0)
            {
                l = 1;
            }
            Curve[] cs = x.DuplicateEdgeCurves(true);
            Curve[] joins = Curve.JoinCurves(cs);
            Point3d pt = SuperVipersClass.centerPoint(x.DuplicateVertices().ToList());
            Point3d center;/////中点
            ComponentIndex ci;
            double s;
            double t;
            double dist = 0;
            Vector3d vc;///////开口方向
            x.ClosestPoint(pt, out center, out ci, out s, out t, dist, out vc);
            center.Transform(Transform.Translation(vc * h));
            Point3d[] ptss = SuperVipersClass.sortPoints(x.DuplicateVertices().ToList(), joins[0]).ToArray();
            for (int i = 0; i < ptss.Length; i++)
            {
                Vector3d vv = Point3d.Subtract(ptss[i], center);
                ptss[i].Transform(Transform.Translation(vv * l * (-1)));
            }
            Point3d[] ptss2 = SuperVipersClass.sortPoints(x.DuplicateVertices().ToList(), joins[0]).ToArray();
            List<Brep> breps = new List<Brep>();
            for (int i = 0; i < ptss.Length; i++)
            {
                if (i == ptss.Length - 1)
                {
                    NurbsCurve[] n1 = { new Line(ptss[i], ptss[0]).ToNurbsCurve(), new Line(ptss[i], ptss2[i]).ToNurbsCurve(), new Line(ptss2[i], ptss2[0]).ToNurbsCurve(), new Line(ptss[0], ptss2[0]).ToNurbsCurve() };
                    Brep bbp = Brep.CreateEdgeSurface(n1);
                    breps.Add(bbp);
                    continue;
                }
                NurbsCurve[] n2 = { new Line(ptss[i], ptss[i + 1]).ToNurbsCurve(), new Line(ptss[i], ptss2[i]).ToNurbsCurve(), new Line(ptss2[i], ptss2[i + 1]).ToNurbsCurve(), new Line(ptss[i + 1], ptss2[i + 1]).ToNurbsCurve() };
                Brep bbp2 = Brep.CreateEdgeSurface(n2);
                breps.Add(bbp2);
            }
            DA.SetDataList(0, breps);
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
                return Resource1.surface_突出表皮1;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{7a90d9f4-dd7b-491b-bb4d-9e63cab4b931}"); }
        }
    }
}