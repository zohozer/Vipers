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
namespace Vipers/////////TangChi 2016.4.11
{
    public class CurveColour : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the 曲线上色 class.
        /// </summary>
        public CurveColour()
            : base("Colour Curve", "CurveColour",
                "Displays custom colours for the curves",
                "Vipers", "Viper.tint")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves","C","Curves to color",GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Thickness","T","Curve thickness to display",GH_ParamAccess.item,5);
            pManager.AddColourParameter("Colour", "C", "Colours to display",GH_ParamAccess.item,Color.FromArgb(255,255,128));
            pManager.AddIntegerParameter("Seed","S", "Seed of random colours for each branch", GH_ParamAccess.item,0);
            pManager.HideParameter(0);
            Message = "Colour Curve";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Structure<GH_Curve> curves = new GH_Structure<GH_Curve>();
            int thickness = 0;
            Color color = Color.Pink;
            int seed = 0;
            if(!DA.GetDataTree(0,out curves))return;
            if(!DA.GetData(1,ref thickness))return;
            if (!DA.GetData(2, ref color)) return;
            if (!DA.GetData(3, ref seed)) return;
            w = thickness;
            c = color;
            trees = curves;
            if (curves.PathCount>= 1)
            {
                List<Color> colors = new List<Color>();
                Random rand = new Random(seed);
                int aaa = 0;
                int bbb = 0;
                int ccc = 0;
                for (int i = 0; i < trees.PathCount; i++)
                {
                    aaa = rand.Next(255);
                    bbb = rand.Next(255);
                    ccc = rand.Next(255);
                    colors.Add(Color.FromArgb(aaa, bbb, ccc));
                }
                cos = colors;
            }
        }
        List<GH_Curve> all = new List<GH_Curve>();
        List<Color> cos = new List<Color>();
        int w = 1;
        Color c = Color.Pink;
        GH_Structure<GH_Curve> trees= new GH_Structure<GH_Curve>();
        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            if(trees.PathCount==0)
            {
                return;
            }
            if (this.Attributes.Selected)///////电池被选中后曲线的颜色
            {
                for (int i = 0; i < trees.PathCount; i++)
                {
                    all = trees.Branches[i];
                    for (int k = 0; k < all.Count; k++)
                    {
                        if (all[k] == null)
                        {
                            continue;
                        }
                        args.Display.DrawCurve(all[k].Value, args.WireColour_Selected, w);
                    }
                }
                return;
            }

            if (trees.PathCount == 1)
            {
                all = trees.Branches[0];
                for (int i = 0; i < all.Count; i++)
                {
                    if(all[i]==null)
                    {
                        continue;
                    }
                    args.Display.DrawCurve(all[i].Value, c, w);
                }
            }
            else
            {
                for (int i = 0; i < trees.PathCount; i++)
                {
                    all = trees.Branches[i];
                    for (int k = 0; k < all.Count; k++)
                    {
                        if (all[k] == null)
                        {
                            continue;
                        }
                        args.Display.DrawCurve(all[k].Value, cos[i], w);
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
                return Resource1.tint_曲线上色;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{909f939c-53f9-48c3-badd-7e69c521952a}"); }
        }
    }
}