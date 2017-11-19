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

namespace Vipers//////TangChi 2016.12.30
{
    public class GeometryClassify : GH_Component, IGH_VariableParameterComponent
    {
        public GeometryClassify()
            : base("物件分类", "GeometryClassify",
                "将输入的物件分类后（点，线，面，体，网格）分配给各个输出端",
                "Vipers", "viper.data")
        {
            Message = "物件分类";
        }
        bool point = false;
        bool curve = false;
        bool surface = false;
        bool brep = false;
        bool mesh = false;
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("物件", "O", "待分类的物件", GH_ParamAccess.tree);
            pManager.HideParameter(0);
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

        }
        protected override void BeforeSolveInstance()
        {
            ClearOutput();
            point = false;
            curve = false;
            surface = false;
            brep = false;
            mesh = false;
            IGH_Structure dd = Params.Input[0].VolatileData;
            foreach (IGH_Goo geo in dd.AllData(true))
            {
                if (geo is GH_Point)
                {
                    point = true;
                }
                else if (geo is GH_Curve)////curve
                {
                    curve = true;
                }
                else if (geo is GH_Brep)
                {
                    GH_Brep bp = (GH_Brep)geo;
                    if (bp.Value.IsSurface)///surface
                    {
                        surface = true;
                    }
                    else//////brep
                    {
                        brep = true;
                    }
                }
                else if (geo is GH_Mesh)
                {
                    mesh = true;
                    GH_Mesh me = (GH_Mesh)geo;
                }
            }
            AddOutput();
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Structure<IGH_Goo> obj = new GH_Structure<IGH_Goo>();
            if (!DA.GetDataTree(0,out obj)) return;
            DataTree<Point3d> pts = new DataTree<Point3d>();
            DataTree<Curve> crs = new DataTree<Curve>();
            DataTree<Surface> sfs = new DataTree<Surface>();
            DataTree<Brep> bps = new DataTree<Brep>();
            DataTree<Mesh> mes = new DataTree<Mesh>();
            for (int i = 0; i < obj.PathCount; i++)/////////////////////判断输入端的类型，分别赋值给上面创建的datatree中
            {
                for (int q = 0; q < obj.Branches[i].Count; q++)
                {
                    IGH_Goo geo = obj.Branches[i][q];
                    if(geo==null)continue;
                    if (geo is GH_Point)
                    {
                        GH_Point ghpt = (GH_Point)geo;
                        pts.Add(ghpt.Value, obj.Paths[i]);
                    }
                    else if (geo is GH_Curve)////curve
                    {
                        GH_Curve ghcr = (GH_Curve)geo;
                        crs.Add(ghcr.Value, obj.Paths[i]);
                    }
                    else if (geo is GH_Brep)
                    {
                        GH_Brep bp = (GH_Brep)geo;
                        if (bp.Value.IsSurface)///surface
                        {
                            sfs.Add(bp.Value.Surfaces[0], obj.Paths[i]);
                        }
                        else//////brep
                        {
                            bps.Add(bp.Value, obj.Paths[i]);
                        }
                    }
                    else if (geo is GH_Mesh)
                    {
                        GH_Mesh me = (GH_Mesh)geo;
                        mes.Add(me.Value, obj.Paths[i]);
                    }
                }
            }
            for (int i = 0; i < Params.Output.Count; i++)
            {
                if (point)
                {
                    DA.SetDataTree(i,pts);
                    point = false;
                    continue;
                }
                if (curve)
                {
                    DA.SetDataTree(i, crs);
                    curve = false;
                    continue;
                }
                if (surface)
                {
                    DA.SetDataTree(i, sfs);
                    surface = false;
                    continue;
                }
                if (brep)
                {
                    DA.SetDataTree(i, bps);
                    brep = false;
                    continue;
                }
                if (mesh)
                {
                    DA.SetDataTree(i, mes);
                    mesh = false;
                    continue;
                }
            }
        }
        public void AddOutput()
        {
            if (point)
            {
                AddPointOutput();
                Params.OnParametersChanged();
            }
            if (curve)
            {
                AddCurveOutput();
                Params.OnParametersChanged();
            }
            if (surface)
            {
                AddSurfaceOutput();
                Params.OnParametersChanged();
            }
            if (brep)
            {
                AddBrepOutput();
                Params.OnParametersChanged();
            }
            if (mesh)
            {
                AddMeshOutput();
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
        public void AddPointOutput()////创建点输出端
        {
            IGH_Param p = new Param_Point();
            p.Name = "Point";
            p.NickName = "Pt";
            p.Description = "点";
            p.Access = GH_ParamAccess.tree;
            Params.RegisterOutputParam(p);
            p.Optional = true;
        }
        public void AddCurveOutput()/////创建曲线输出端
        {
            IGH_Param p = new Param_Curve();
            p.Name = "Curve";
            p.NickName = "Cr";
            p.Description = "曲线";
            p.Access = GH_ParamAccess.tree;
            Params.RegisterOutputParam(p);
            p.Optional = true;
        }
        public void AddSurfaceOutput()//////创建曲面输出端
        {
            IGH_Param p = new Param_Surface();
            p.Name = "Surface";
            p.NickName = "Sf";
            p.Description = "曲面";
            p.Access = GH_ParamAccess.tree;
            Params.RegisterOutputParam(p);
            p.Optional = true;
        }
        public void AddBrepOutput()//////创建brep输出端
        {
            IGH_Param p = new Param_Brep();
            p.Name = "Brep";
            p.NickName = "Bp";
            p.Description = "实体";
            p.Access = GH_ParamAccess.tree;
            Params.RegisterOutputParam(p);
            p.Optional = true;
        }
        public void AddMeshOutput()///////创建网格输出端
        {
            IGH_Param p = new Param_Mesh();
            p.Name = "Mesh";
            p.NickName = "Ms";
            p.Description = "网格";
            p.Access = GH_ParamAccess.tree;
            Params.RegisterOutputParam(p);
            p.Optional = true;
        }
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Resource1.data_物件分类;
            }
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("{fd42c08a-d0c7-4994-9540-9dbb7bd31617}"); }
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