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
    public class WaveTree : GH_Component, IGH_VariableParameterComponent
    {
        /// <summary>
        /// Initializes a new instance of the WaveTree class.
        /// </summary>
        public WaveTree()
            : base("编织树形数据", "WaveTree",
                "将树形数据按照指定序号编织,右键多选（编织整个树形数据或编织树形数据的分支）",
               "Vipers", "Viper.data")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            int[] moren = {0,1};
            pManager.AddIntegerParameter("索引","P","列表的每个数值对应输入端的树形数据的序号",GH_ParamAccess.list,moren);
            pManager.AddGenericParameter("树形数据", "0", "待编织的树形数据", GH_ParamAccess.tree);
            pManager.AddGenericParameter("树形数据", "1", "待编织的树形数据", GH_ParamAccess.tree);
            Message = "编织树形数据分支";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("树形数据", "T", "编织后的树形数据", GH_ParamAccess.tree);
        }
        bool _wavetree = true;

        public bool _WaveTree
        {
            get { return _wavetree; }
            set
            {
                _wavetree = value;
                if (_wavetree == true)
                    Message = "编织树形数据分支";
                else
                    Message = "编织整个树形数据";
            }
        }
        Color cor1 = Color.FromArgb(61, 200, 44);
        Color cor2 = Color.FromArgb(61, 150, 44);
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            // Append the item to the menu, making sure it's always enabled and checked if Absolute is True.
            ToolStripMenuItem item = Menu_AppendItem(menu, "编织树形数据分支", Menu_AbsoluteClicked, true, _WaveTree);
            ToolStripMenuItem item2 = Menu_AppendItem(menu, "编织整个树形数据", Menu_AbsoluteClicked2, true, !_WaveTree);
            // Specifically assign a tooltip text to the menu item.
            item.ToolTipText = "编织树形数据中的每个分支";
            item.BackColor = cor1;
            item2.ToolTipText = "编织整个树形数据";
            item2.BackColor = cor2;
        }
        private void Menu_AbsoluteClicked(object sender, EventArgs e)
        {
            RecordUndoEvent("Absolute");
            _WaveTree = true;
            ExpireSolution(true);
        }
        private void Menu_AbsoluteClicked2(object sender, EventArgs e)
        {
            RecordUndoEvent("Absolute");
            _WaveTree = false;
            ExpireSolution(true);
        }
        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            List<int> index = new List<int>();////插入索引
            if (!DA.GetDataList(0, index)) return;
            int mem = 0;
            GH_Structure<IGH_Goo> last = new GH_Structure<IGH_Goo>();////结果
            for (int i = 0; i < index.Count; i++)
            {
                if (index[i] >= Params.Input.Count)  return;
            }
            List<GH_Structure<IGH_Goo>> trees = new List<GH_Structure<IGH_Goo>>();
            List<int> find = new List<int>();
            int num=0;////////////重新排序的序号
            for (int i = 1; i < Params.Input.Count; i++)
            {
                GH_Structure<IGH_Goo> test;
                if (DA.GetDataTree(i, out test))
                {
                    ////////////////////////////////////////////////////////////////////////////////////
                    Params.Input[i].NickName = num.ToString();
                    num++;
                    ///////////////////////////////////////////////////////////////////////////////////
                    trees.Add(test);
                    find.Add(0);
                }
            }
            if (_WaveTree)///插分支
            {
                int count = index.Distinct().ToList().Count;//////真正参与计算的单个索引个数
                for (int q = 0; q < index.Count; q++)
                {
                    if (find[index[q]] < trees[index[q]].PathCount)
                    {
                        last.AppendRange(trees[index[q]].Branches[find[index[q]]], new GH_Path(0, mem));
                        find[index[q]] += 1;
                        mem++;
                        if (find[index[q]] == trees[index[q]].PathCount)////该树遍历完
                        {
                            count--;
                        }
                    }
                    if (count == 0)////所有树遍历完
                    {
                        DA.SetDataTree(0, last);
                        return;
                    }
                    if (q == index.Count - 1)
                    {
                        q = -1;
                    }
                }
            }
            else ///插整棵
            {
                for (int i = 0; i<index.Count;i++ )
                {
                    for (int q = 0; q<trees[index[i]].PathCount;q++ )
                    {
                        last.AppendRange(trees[index[i]].Branches[q],new GH_Path(0,mem));
                        mem++;
                    }
                    
                }
                DA.SetDataTree(0, last);
            }
        }
        public override bool Read(GH_IReader reader)
        {
            _WaveTree=reader.GetBoolean("_WaveTree");
            return base.Read(reader);
        }
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("_WaveTree", _WaveTree);
            return base.Write(writer);
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
                return Resource1.data_编织树;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{76bee57f-dd14-420f-b22a-0346bffecb9c}"); }
        }
                #region Methods of IGH_VariableParameterComponent interface


        bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
        {
            return side != GH_ParameterSide.Output && index != 0;
        }

        bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
        {
            if (side == GH_ParameterSide.Input && Params.Input.Count > 3&&index!=0)
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
            Param_GenericObject param = new Param_GenericObject();
            param.Name = "树形数据";
            /////GH_ComponentParamServer.InventUniqueNickname("0123456789", Params.Input);
            param.NickName = param.Name;
            param.Description ="待编织的树形数据";
            param.Access = GH_ParamAccess.tree;
            param.Hidden = true;
            return param;
        }

        bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
        {
            return true;
        }

        void IGH_VariableParameterComponent.VariableParameterMaintenance()
        {
            int num = 1;
            checked
            {
                int num1 = this.Params.Input.Count - 1;
                for (int i = num; i <= num1; i++)
                {
                    IGH_Param iGH_Param = this.Params.Input[i];
                    iGH_Param.Name = "树形数据";
                    iGH_Param.NickName = string.Format("{0}", i - 1);
                    iGH_Param.Description = "待编织的树形数据";
                    iGH_Param.Access = GH_ParamAccess.tree;
                    iGH_Param.MutableNickName = false;
                }
            }
        }
    #endregion
    }
}