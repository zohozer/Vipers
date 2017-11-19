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

namespace Vipers/////TangChi 2015.8.23
{
    public class Fillets : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent38 class.
        /// </summary>
        public Fillets()
            : base("曲线倒多种圆角", "Fillets",
                "根据列表值倒曲线圆角",
                "Vipers", "Viper.curve")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("曲线","C","待倒圆角曲线",GH_ParamAccess.item);
            pManager.AddNumberParameter("圆角列表","L","圆角半径列表值",GH_ParamAccess.list);
            pManager.HideParameter(0);
            Message = "曲线倒多种圆角";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("圆角曲线","C","倒圆角后的曲线",GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve curve=null;
            List<double> radiusList=new List<double>();
            if(!DA.GetData(0,ref curve)) return;
            if(!DA.GetDataList(1,radiusList))return;
            Curve x = curve;
            List<double> y = radiusList;
            Curve[] cs = x.DuplicateSegments();
            List<Arc> arcs = new List<Arc>();
            List<NurbsCurve> ncs = new List<NurbsCurve>();
            if (y.Count < cs.Length)
            {
                for (int i = y.Count - 1; i < cs.Length; i++)
                {
                    y.Add(y[y.Count - 1]);
                }
            }
            for (int i = 0; i < cs.Length; i++)
            {
                if (x.IsClosed == false && i == cs.Length - 1)/////不闭合曲线的情况
                {
                    break;
                }
                if (i == cs.Length - 1)
                {
                    Arc arc2 = Curve.CreateFillet(cs[i], cs[0], y[i], 0, 0);
                    arcs.Add(arc2);
                    Point3d pta = arc2.StartPoint;
                    Point3d ptb = arc2.EndPoint;
                    cs[i].SetEndPoint(pta);
                    cs[0].SetStartPoint(ptb);
                    ncs.Add(arc2.ToNurbsCurve());
                    break;
                }
                Arc arc1 = Curve.CreateFillet(cs[i], cs[i + 1], y[i], 0, 0);
                arcs.Add(arc1);
                Point3d pt1 = arc1.StartPoint;
                Point3d pt2 = arc1.EndPoint;
                cs[i].SetEndPoint(pt1);
                cs[i + 1].SetStartPoint(pt2);
                ncs.Add(arc1.ToNurbsCurve());
            }
            foreach (Curve cc in cs)
            {
                ncs.Add(cc.ToNurbsCurve());
            }
            Curve[] final = Curve.JoinCurves(ncs);
            DA.SetData(0, final[0]);
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
                //return Resource1.倒多种圆角;
                return Resource1.curve_曲线倒多种圆角;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{4955860d-b322-40da-9ac0-1186a28eaea6}"); }
        }
    }
}