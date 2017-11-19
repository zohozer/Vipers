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

namespace Vipers////TangChi 2016.6.14
{
    public class addition : GH_Component, IGH_VariableParameterComponent
    {
        /// <summary>
        /// Initializes a new instance of the addition class.
        /// </summary>
        public addition()
            : base("乘法", "VariableAddition",
                "同GH自带乘法运算器，可增加减少输入端",
                "Vipers", "Viper.math")
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
            pManager.AddNumberParameter("乘数","A","参与乘法计算的数字",GH_ParamAccess.item,1);
            pManager.AddNumberParameter("被乘数", "B", "参与乘法计算的数字", GH_ParamAccess.item,1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("积","R","乘积",GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double x=0;
            double y=0;
            if(!DA.GetData(0,ref x ))return;
            if(!DA.GetData(1,ref y ))return;
            double last=x*y;
            for (int i = 2; i<Params.Input.Count;i++ )
            {
                double test=0;
                if(DA.GetData(i,ref test ))
                {
                    last = last * test;
                }
            }
            DA.SetData(0,last);
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
                return Resource1.math_乘法;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{d69dcd32-93c2-4289-bdef-0b19171fac99}"); }
        }
        bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
        {
            return side != GH_ParameterSide.Output ;
        }

        bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
        {
            if (side == GH_ParameterSide.Input &&Params.Input.Count>2)
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
            Param_Number param = new Param_Number();
            param.Name = "被乘数";
            param.NickName = GH_ComponentParamServer.InventUniqueNickname("ABCDEFGHIJKLMNOPQRSTUVWXYZ", Params.Input);
            param.Description = "参与乘法计算的数字";
            param.Access = GH_ParamAccess.item;
            param.SetPersistentData(1);
            return param;
        }

        bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
        {
            return true;
        }
        //string index = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";/////////////////////////////////////////////////////////////////自定义的数组序号
        void IGH_VariableParameterComponent.VariableParameterMaintenance()
        {
            for (int i = 0; i<this.Params.Input.Count;i++ )
            {
                Params.Input[i].NickName = null;
            }
            checked
            {
                for (int i =0; i <this.Params.Input.Count; i++)
                {
                    IGH_Param iGH_Param = this.Params.Input[i];
                    iGH_Param.Name = "被乘数";
                    iGH_Param.NickName = GH_ComponentParamServer.InventUniqueNickname("ABCDEFGHIJKLMNOPQRSTUVWXYZ", Params.Input);
                    iGH_Param.Description = "参与乘法计算的数字";
                    iGH_Param.Access = GH_ParamAccess.item;
                    iGH_Param.MutableNickName = false;
                }
            }
        }
    }
}