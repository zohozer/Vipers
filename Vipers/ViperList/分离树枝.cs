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

namespace Vipers//////TangChi 2016.9.2
{
    public class SeparationTree : GH_Component, IGH_VariableParameterComponent
    {
        /// <summary>
        /// Initializes a new instance of the 分离树枝 class.
        /// </summary>
        public SeparationTree()
            : base("分离树干", "SeparationTree",
                "将多种路径混合的树形数据分成若干单一路径的树形数据(右键选择生成所有单一路径)",
               "Vipers", "Viper.data")
        {
            Message = "分离树干";
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("树形数据", "G", "多种路径混合的树形数据", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("序号", "I", "分类后，指定单一路径树形数据的序号", GH_ParamAccess.item, 0);
            pManager[0].Optional = true;
            pManager.HideParameter(0);
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("树形数据","G","在指定序号下的单一路径树形数据，",GH_ParamAccess.tree);
            pManager.AddGenericParameter("空数据","-","无数据输出",GH_ParamAccess.tree);
            this.VariableParameterMaintenance();
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int indexs=0;
            if(!DA.GetData(1,ref indexs))return;
            #region
            GH_Structure<IGH_Goo> x = (GH_Structure<IGH_Goo>)this.Params.Input[0].VolatileData;
            List<GH_Structure<IGH_Goo>> last = new List<GH_Structure<IGH_Goo>>();
            List<int> contains = new List<int>();
            for (int i = 0; i < x.PathCount; i++)
            {
                if (contains.Contains(i)) continue;//重复的index
                GH_Structure<IGH_Goo> tree = new GH_Structure<IGH_Goo>();
                GH_Path path = x.get_Path(i);
                tree.AppendRange(x.Branches[i], path);
                contains.Add(i);
                string testPath = System.Text.RegularExpressions.Regex.Replace(path.ToString(), @";\d+}", "");
                for (int q = 0; q < x.PathCount; q++)
                {
                    if (i == q || contains.Contains(q)) continue;
                    GH_Path path2 = x.get_Path(q);
                    string testPath2 = System.Text.RegularExpressions.Regex.Replace(path2.ToString(), @";\d+}", "");
                    if (testPath == testPath2)
                    {
                        tree.AppendRange(x.Branches[q], path2);
                        contains.Add(q);
                    }
                }
                last.Add(tree);
            }
            NewTreePath = last.Count;///////
            #endregion
            DA.DisableGapLogic();
            if (DA.Iteration <= 0 && (x != null) && !x.IsEmpty)
            {
                if (last.Count > this.Params.Output.Count-1&&this.Params.Output.Count>1)
                {
                    int count=newTreePath-this.Params.Output.Count+1;
                    string warning = string.Format("还有{0}个单一路径树形数据未显示在输出端",count);
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, warning);
                }
                int num2 = this.Params.Output.Count;
                for (int i = 1; i < num2; i++)///从第二项开始
                {
                    GH_Structure<IGH_Goo> structure2 = (GH_Structure<IGH_Goo>)this.Params.Output[i].VolatileData;
                    structure2.Clear();
                    if (i >last.Count)
                    {
                        this.Params.Output[i].NickName = "-";
                        this.Params.Output[i].Name = "空数据";
                        this.Params.Output[i].Description = "无数据输出";
                    }
                    else
                    {
                        string newName = System.Text.RegularExpressions.Regex.Replace(last[i - 1].get_Path(0).ToString(), @"\d+}", "");
                        this.Params.Output[i].NickName = newName + "xx}";
                        string newName2 = System.Text.RegularExpressions.Regex.Replace(this.Params.Output[i].NickName, @"\d+}", "");
                        this.Params.Output[i].Description = string.Format("路径类型：{0}", newName2);
                        this.Params.Output[i].Name = "树形数据";
                        DA.SetDataTree(i,last[i-1]);
                    }
                }
            }
            if ((x != null) && !x.IsEmpty)
                DA.SetDataTree(0, last[indexs]);
        }
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Resource1.data_分离树干;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{7f79fc9f-0d96-495f-af06-562caf1ac256}"); }
        }

        #region IGH_VariableParameterComponent 成员

        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            return (side == GH_ParameterSide.Output && index!=0);
        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            return (side == GH_ParameterSide.Output && index!=0);
        }

        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            return new Param_GenericObject();
        }

        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            return true;
        }
        /// <summary>
        /// ///////////////////////////
        /// </summary>
        private int newTreePath = 0;
        public int NewTreePath
        {
            get { return newTreePath; }
            set { newTreePath = value; }
        }
        /// <summary>
        /// /////////////////////////
        /// </summary>
        public void VariableParameterMaintenance()
        {
            this.Params.Input[0].Optional = true;
            this.Params.Input[0].Access = GH_ParamAccess.tree;
            int num2 = this.Params.Output.Count - 1;
            for (int i = 1; i <= num2; i++)
            {
                this.Params.Output[i].Name = "空数据";
                this.Params.Output[i].NickName = "-";
                this.Params.Output[i].Description = "无数据输出";
                this.Params.Output[i].MutableNickName = false;
                this.Params.Output[i].Access = GH_ParamAccess.tree;
            }
        }
        Color cor = Color.FromArgb(61, 200, 44);
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            ToolStripMenuItem item = Menu_AppendItem(menu, "显示所有单一路径", new EventHandler(this.Menu_AutoCreateTree_Clicked));
            item.BackColor = cor;
        }
        private void Menu_AutoCreateTree_Clicked(object sender, EventArgs e)
        {
            if (this.Params.Output.Count != newTreePath+1)
            {
                this.RecordUndoEvent("Explode Matches");
                if (this.Params.Output.Count < newTreePath+1)
                {
                    while (this.Params.Output.Count < newTreePath+1)
                    {
                        IGH_Param param = this.CreateParameter(GH_ParameterSide.Output, this.Params.Output.Count);
                        this.Params.RegisterOutputParam(param);
                    }
                }
                else if (this.Params.Output.Count > newTreePath+1)
                {
                    while (this.Params.Output.Count > 1)
                    {
                        this.Params.UnregisterOutputParameter(this.Params.Output[this.Params.Output.Count - 1]);
                    }
                }
                this.Params.OnParametersChanged();
                this.VariableParameterMaintenance();
                this.ExpireSolution(true);
            }
        }
        #endregion
    }
}