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

namespace Vipers
{
    public class RandomGeometryColor : GH_Component
    {
        public RandomGeometryColor()
            : base("Color Geometry", "GeometryColor",
                "Displays custom colours for Rhino generic geometry",
                "Vipers", "viper.tint")
        {
            Message = "Color Geometry";
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct GH_CustomPreviewItem
        {
            internal Color m_col;
            internal Rhino.Display.DisplayMaterial m_mat;
            internal IGH_PreviewData m_obj;
        }
        public override void ClearData()
        {
            base.ClearData();
            this.m_items = null;
        }
        private List<GH_CustomPreviewItem> m_items;
        public override BoundingBox ClippingBox
        {
            get
            {
                return this.m_clipbox;
            }
        }
        private BoundingBox m_clipbox;
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Geometry", "G", "Generic geometry to color", GH_ParamAccess.list);
            pManager.HideParameter(0);
            pManager.AddColourParameter("Colour", "C", "Colours to display", GH_ParamAccess.item, Color.Yellow);
            pManager.AddIntegerParameter("Seed", "S", "Seed of random colours for each branch", GH_ParamAccess.item, 0);
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (DA.Iteration == 0)
            {
                this.m_items = new List<GH_CustomPreviewItem>();
                this.m_clipbox = BoundingBox.Empty;
            }
            List<IGH_GeometricGoo> destination2 = new List<IGH_GeometricGoo>();
            Color color = new Color();
            int seed = 0;
            if ((DA.GetDataList<IGH_GeometricGoo>(0, destination2) && DA.GetData(1, ref color)) && DA.GetData(2, ref seed))
            {
                Random rand = new Random(DA.Iteration + seed);
                if (DA.Iteration == 0)
                {
                }
                else
                {
                    color = Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255));
                }
                for (int i = 0; i < destination2.Count; i++)
                {
                    Rhino.Display.DisplayMaterial material = new Rhino.Display.DisplayMaterial(color);
                    GH_CustomPreviewItem item = new GH_CustomPreviewItem
                    {
                        m_obj = (IGH_PreviewData)destination2[i],
                        m_mat = material,
                        m_col = material.Diffuse
                    };
                    this.m_items.Add(item);
                    this.m_clipbox.Union(destination2[i].Boundingbox);
                }
            }
        }
        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            if ((this.m_items != null))
            {
                if (this.Attributes.Selected)
                {
                    GH_PreviewMeshArgs args2 = new GH_PreviewMeshArgs(args.Viewport, args.Display, args.ShadeMaterial_Selected, args.MeshingParameters);
                    foreach (GH_CustomPreviewItem item in this.m_items)
                    {
                        item.m_obj.DrawViewportMeshes(args2);
                    }
                }
                else
                {
                    foreach (GH_CustomPreviewItem item2 in this.m_items)
                    {
                        Rhino.Display.DisplayMaterial lastColor = new Rhino.Display.DisplayMaterial(item2.m_col);
                        GH_PreviewMeshArgs args3 = new GH_PreviewMeshArgs(args.Viewport, args.Display, lastColor, args.MeshingParameters);
                        item2.m_obj.DrawViewportMeshes(args3);
                    }
                }
            }
        }
        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            if ((this.m_items != null))
            {
                if (this.Attributes.Selected)
                {
                    GH_PreviewWireArgs args2 = new GH_PreviewWireArgs(args.Viewport, args.Display, args.WireColour_Selected, args.DefaultCurveThickness);
                    foreach (GH_CustomPreviewItem item in this.m_items)
                    {
                        if (!(item.m_obj is GH_Mesh) || CentralSettings.PreviewMeshEdges)
                        {
                            item.m_obj.DrawViewportWires(args2);
                        }
                    }
                }
                else
                {
                    foreach (GH_CustomPreviewItem item2 in this.m_items)
                    {
                        if (!(item2.m_obj is GH_Mesh) || CentralSettings.PreviewMeshEdges)
                        {
                            GH_PreviewWireArgs args3 = new GH_PreviewWireArgs(args.Viewport, args.Display, item2.m_col, args.DefaultCurveThickness);
                            item2.m_obj.DrawViewportWires(args3);
                        }
                    }
                }
            }
        }
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Vipers.Resource1.tint_物件随机上色;
            }
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("{cd429a99-690a-4da6-bf14-43c05e177d8f}"); }
        }
    }
}
