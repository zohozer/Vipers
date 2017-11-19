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
using Grasshopper.Kernel.Parameters;

namespace Vipers/////TangChi 
{
    public class CurvecClassify : GH_Component, IGH_VariableParameterComponent
    {
        /// <summary>
        /// Initializes a new instance of the CurvecClassify class.
        /// </summary>
        public CurvecClassify()
            : base("曲线分类", "CurvecClassify",
                "将曲线按类型分为——曲线，线段，多段线，圆，圆弧，椭圆",
                "Vipers", "viper.curve")
        {
            Message = "曲线分类";
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        /// 
        bool curve = false;
        bool line = false;
        bool polyline = false;
        bool arc = false;
        bool circle = false;
        bool ellipse = false;
        protected override void BeforeSolveInstance()
        {
            ClearOutput();
            curve = false;
            line = false;
            polyline = false;
            arc = false;
            circle = false;
            ellipse = false;
            IGH_Structure dd = Params.Input[0].VolatileData;
            double tolerance = 0;
            foreach (IGH_Goo number in Params.Input[1].VolatileData.AllData(true))
            {
                GH_Number num = (GH_Number)number;
                tolerance = num.Value;
            }
            foreach (IGH_Goo geo in dd.AllData(true))
            {
                GH_Curve crv = (GH_Curve)geo;
                if (crv.Value.IsLinear(tolerance)) line = true;
                else if (crv.Value.IsPolyline()) polyline = true;
                else if (crv.Value.IsCircle(tolerance)) circle = true;
                else if (crv.Value.IsArc(tolerance)) arc = true;
                else if (crv.Value.IsEllipse(tolerance)) ellipse = true;
                else curve = true;
            }
            AddOutput();
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("曲线","C","待分类的曲线",GH_ParamAccess.tree);
            pManager.AddNumberParameter("公差","T","调整公差，可将近似与某些类型的曲线选出",GH_ParamAccess.item,0.01);
            pManager.HideParameter(0);
        }
        public void AddOutput()
        {
            if (curve)
            {
                AddCurveOutput();
                Params.OnParametersChanged();
            }
            if (line)
            {
                AddLineOutput();
                Params.OnParametersChanged();
            }
            if (polyline)
            {
                AddPolylineOutput();
                Params.OnParametersChanged();
            }
            if (arc)
            {
                AddArcOutput();
                Params.OnParametersChanged();
            }
            if (circle)
            {
                AddCircleOutput();
                Params.OnParametersChanged();
            }
            if(ellipse)
            {
                AddEllipseOutput();
                Params.OnParametersChanged();
            }
        }
        public void ClearOutput()//////////////////////////移除所有输出端
        {
            while (this.Params.Output.Count > 0)
            {
                Params.UnregisterOutputParameter(Params.Output[0]);
            }
        }
        public void AddCurveOutput()////创建曲线输出端
        {
            IGH_Param p = new Param_Curve();
            p.Name = "曲线";
            p.NickName = "Cr";
            p.Description = "曲线";
            p.Access = GH_ParamAccess.tree;
            Params.RegisterOutputParam(p);
            p.Optional = true;
        }
        public void AddLineOutput()////创建直线输出端
        {
            IGH_Param p = new Param_Line();
            p.Name = "直线";
            p.NickName = "Li";
            p.Description = "直线";
            p.Access = GH_ParamAccess.tree;
            Params.RegisterOutputParam(p);
            p.Optional = true;
        }
        public void AddPolylineOutput()////创建多段线输出端
        {
            IGH_Param p = new Param_Curve();
            p.Name = "多段线";
            p.NickName = "Pl";
            p.Description = "多段线";
            p.Access = GH_ParamAccess.tree;
            Params.RegisterOutputParam(p);
            p.Optional = true;
        }
        public void AddArcOutput()////创建弧线输出端
        {
            IGH_Param p = new Param_Arc();
            p.Name = "弧线";
            p.NickName = "Ac";
            p.Description = "弧线";
            p.Access = GH_ParamAccess.tree;
            Params.RegisterOutputParam(p);
            p.Optional = true;
        }
        public void AddCircleOutput()////创建圆输出端
        {
            IGH_Param p = new Param_Circle();
            p.Name = "圆";
            p.NickName = "Ce";
            p.Description = "圆";
            p.Access = GH_ParamAccess.tree;
            Params.RegisterOutputParam(p);
            p.Optional = true;
        }
        public void AddEllipseOutput()////创建椭圆输出端
        {
            IGH_Param p = new Param_Curve();
            p.Name = "椭圆";
            p.NickName = "Ep";
            p.Description = "椭圆";
            p.Access = GH_ParamAccess.tree;
            Params.RegisterOutputParam(p);
            p.Optional = true;
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
        /// 
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Structure<GH_Curve> curves = new GH_Structure<GH_Curve>();
            double tolerance = 0;
            if(!DA.GetDataTree(0,out curves)||!DA.GetData(1,ref tolerance))return;
            DataTree<Curve> crs = new DataTree<Curve>();
            DataTree<Line> lis = new DataTree<Line>();
            DataTree<Curve> plys = new DataTree<Curve>();
            DataTree<Arc> acs = new DataTree<Arc>();
            DataTree<Circle> cls = new DataTree<Circle>();
            DataTree<Curve> eps = new DataTree<Curve>();
            for (int i = 0; i<curves.PathCount;i++ )
            {
                for (int q = 0; q < curves.Branches[i].Count; q++)
                {
                    GH_Curve gc = curves.Branches[i][q];
                    if(gc==null)continue;/////////////////////这一步特别重要，如果不用，在模型中删除模型会导致出错
                    if (gc.Value.IsLinear())
                    {
                        Line li = new Line(gc.Value.PointAtStart, gc.Value.PointAtEnd);
                        lis.Add(li, curves.Paths[i]);
                    }
                    else if (gc.Value.IsPolyline())
                    {
                        plys.Add(gc.Value, curves.Paths[i]);
                    }
                    else if (gc.Value.IsCircle(tolerance))
                    {
                        Circle cc = new Circle();
                        gc.Value.TryGetCircle(out cc, tolerance);
                        cls.Add(cc, curves.Paths[i]);
                    }
                    else if (gc.Value.IsArc(tolerance))
                    {
                        Arc ac = new Arc();
                        gc.Value.TryGetArc(out ac, tolerance);
                        acs.Add(ac, curves.Paths[i]);
                    }
                    else if (gc.Value.IsEllipse(tolerance))
                    {
                        Ellipse ep = new Ellipse();
                        gc.Value.TryGetEllipse(out ep, tolerance);
                        eps.Add(ep.ToNurbsCurve(), curves.Paths[i]);
                    }
                    else
                    {
                        crs.Add(gc.Value, curves.Paths[i]);
                    }
                }
            }
            for (int i = 0; i < Params.Output.Count; i++)
            {
                if (curve)
                {
                    DA.SetDataTree(i, crs);
                    curve = false;
                    continue;
                }
                if (line)
                {
                    DA.SetDataTree(i, lis);
                    line = false;
                    continue;
                }
                if (polyline)
                {
                    DA.SetDataTree(i, plys);
                    polyline = false;
                    continue;
                }
                if (arc)
                {
                    DA.SetDataTree(i, acs);
                    arc = false;
                    continue;
                }
                if (circle)
                {
                    DA.SetDataTree(i, cls);
                    circle = false;
                    continue;
                }
                if (ellipse)
                {
                    DA.SetDataTree(i, eps);
                    ellipse = false;
                    continue;
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
                return Resource1.curve_曲线分类;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{46aa2f11-ed7f-4f60-9e26-67cafc4f9476}"); }
        }
        #region IGH_VariableParameterComponent 成员

        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            return (side == GH_ParameterSide.Output && false);
        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            return (side == GH_ParameterSide.Output && false);
        }

        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            return new Param_GenericObject();
        }

        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            return true;
        }

        public void VariableParameterMaintenance()
        {

        }
        #endregion
    }
}