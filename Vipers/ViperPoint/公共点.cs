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

namespace Vipers
{
    public class PublicPoints : GH_Component, IGH_VariableParameterComponent
    {
        /// <summary>
        /// Initializes a new instance of the PublicPoints class.
        /// </summary>
        public PublicPoints()
            : base("公共点", "PublicPoints",
                "多组点的公共部分",
                "Vipers", "Viper.point")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddPointParameter("列表A","A","一组点",GH_ParamAccess.list,Point3d.Unset);
            pManager.AddPointParameter("列表B", "B", "一组点", GH_ParamAccess.list,Point3d.Unset);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
            Message = "多组点的公共部分点";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("结果","R","公共部分的点",GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> ob1 = new List<Point3d>();
            List<Point3d> ob2 = new List<Point3d>();
            List<Point3d> last = new List<Point3d>();
            if(!DA.GetDataList(0,ob1))return;
            if(!DA.GetDataList(1, ob2))return;
            for (int i = 0; i < ob1.Count; i++)
            {
                    if (ob2.Contains(ob1[i]))
                    {
                        last.Add(ob1[i]);
                    }
            }
            List<Point3d> collect = new List<Point3d>();
            for (int i = 2; i < Params.Input.Count; i++)
            {
                List<Point3d> ob = new List<Point3d>();
                if (!DA.GetDataList(i, ob)) return;
                if(ob.Count==1&&ob[0]==Point3d.Unset)///////我的默认值
                {
                    continue;
                }
                for (int q = 0; q < ob.Count; q++)
                {
                    if (last.Contains(ob[q]))
                    {
                        collect.Add(ob[q]);
                    }
                }
                last.Clear();
                last.AddRange(collect);
                collect.Clear();
            }
            DA.SetDataList(0, last);
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
                return Resource1.point_公共点;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{7c028760-cd18-4af3-93a3-dc69819de128}"); }
        }

        bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
        {
            return side != GH_ParameterSide.Output;
        }

        bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
        {
            if (side == GH_ParameterSide.Input && Params.Input.Count > 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
        {

            Param_Point param = new Param_Point();
            param.NickName = GH_ComponentParamServer.InventUniqueNickname("ABCDEFGHIJKLMNOPQRSTUVWXYZ", Params.Input);
            param.Name ="列表"+param.NickName;
            param.Description = "一组点";
            param.Access = GH_ParamAccess.list;
            param.SetPersistentData(Point3d.Unset);
            param.Hidden = true;
            return param;
        }

        bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
        {
            return true;
        }
        //string index = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";/////////////////////////////////////////////////////////////////自定义的数组序号
        void IGH_VariableParameterComponent.VariableParameterMaintenance()
        {
            for (int i = 0; i < this.Params.Input.Count; i++)
            {
                Params.Input[i].NickName = null;
            }
            for (int i = 0; i < this.Params.Input.Count; i++)
            {
                IGH_Param iGH_Param = this.Params.Input[i];
                iGH_Param.NickName = GH_ComponentParamServer.InventUniqueNickname("ABCDEFGHIJKLMNOPQRSTUVWXYZ", Params.Input);
                iGH_Param.Name = "列表" + iGH_Param.NickName;
                iGH_Param.Description = "一组点";
                iGH_Param.Access = GH_ParamAccess.list;
                iGH_Param.MutableNickName = false;
            }
        }
    }
}