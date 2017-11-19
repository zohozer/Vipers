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
    public class CutStartEnd : GH_Component
    {
        public CutStartEnd()
            : base("删除首尾项", "CutStartEnd",
                "删除线性数据(List)或者树形数据(Tree)的首尾项，右键选择删除模式（线性数据删除的是列表中的首尾项(item)，树形数据删除的是首尾的整个列表(list)）",
               "Vipers", "Viper.data")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("{8564EACA-CFC2-470E-8DFC-D9711AC94E55}"); }
        }
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Resource1.data_删除列表首尾项;
            }
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("列表","L","待删除首尾项的列表",GH_ParamAccess.list);
            pManager.AddBooleanParameter("首项", "S", "true：删除列表中的第一项", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("尾项","E","true：删除列表中的最后一项",GH_ParamAccess.item,false);
            pManager.HideParameter(0);
            Message = "删除线性数据首尾";
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("列表", "L", "删除结果", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (_CutStartEnd)///删除列表
            {
                List<object> list = new List<object>();
                if (!DA.GetDataList(0, list)) return;
                bool start = false;
                bool end = false;
                if(!DA.GetData(1,ref start))return;
                if (!DA.GetData(2, ref end)) return;
                ///////////////////////////////////////////
                if(start)
                {
                    list.RemoveAt(0);
                }
                if(end)
                {
                    list.RemoveAt(list.Count - 1);
                }
                DA.SetDataList(0, list);
            }
            else ///删除树
            {
                GH_Structure<IGH_Goo> list = new GH_Structure<IGH_Goo>();
                GH_Structure<IGH_Goo> last = new GH_Structure<IGH_Goo>();
                if (!DA.GetDataTree(0, out list)) return;
                bool start = false;
                bool end = false;
                if (!DA.GetData(1, ref start)) return;
                if (!DA.GetData(2, ref end)) return;
                ///////////////////////////////////////////
                if (start)
                {
                    for (int i = 1; i < list.PathCount; i++)
                    {
                        last.AppendRange(list.Branches[i], list.Paths[i - 1]);
                    }
                }
                else
                {
                    for (int i = 0; i < list.PathCount; i++)
                    {
                        last.AppendRange(list.Branches[i], list.Paths[i]);
                    }
                }
                GH_Structure<IGH_Goo> last2= new GH_Structure<IGH_Goo>();
                if (end)
                {
                    for (int i = 0; i < last.PathCount-1; i++)
                    {
                        last2.AppendRange(last.Branches[i], last.Paths[i]);
                    }
                }
                else
                {
                    last2 = last;
                }
                DA.SetDataTree(0, last2);
            }
        }

        bool _cutstartend = true;
        public bool _CutStartEnd
        {
            get { return _cutstartend; }
            set 
            {
                _cutstartend = value;
                if (_CutStartEnd)
                    Message = "删除线性数据首尾";
                else
                    Message = "删除树形数据首尾";
            }
        }

        public override bool Read(GH_IReader reader)
        {
            _CutStartEnd = reader.GetBoolean("_CutStartEnd");
            return base.Read(reader);
        }
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("_CutStartEnd", _CutStartEnd);
            return base.Write(writer);
        }

        #region menu override
        Color cor1 = Color.FromArgb(61, 200, 44);
        Color cor2 = Color.FromArgb(61, 150, 44);
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            ToolStripMenuItem item1 = Menu_AppendItem(menu, "删除线性数据首尾", AdvancedMenuClicked1, true, _CutStartEnd);
            ToolStripMenuItem item2 = Menu_AppendItem(menu, "删除树形数据首尾", AdvancedMenuClicked2, true, !_CutStartEnd);
            item1.BackColor = cor1;
            item2.BackColor = cor2;
        }
        private void AdvancedMenuClicked1(object sender, EventArgs e)
        {
            RecordUndoEvent("Advanced Parameters1");
            CutForList();
            _CutStartEnd = true;
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void AdvancedMenuClicked2(object sender, EventArgs e)
        {
            RecordUndoEvent("Advanced Parameters1");
            CutForTree();
            _CutStartEnd = false;
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        #endregion

        #region (un)folding logic
        private void CutForList()/////删除项目
        {
            Params.UnregisterInputParameter(Params.Input[0], true);
            Params.UnregisterInputParameter(Params.Input[0], true);
            Params.UnregisterInputParameter(Params.Input[0], true);
            Params.UnregisterOutputParameter(Params.Output[0], true);
            Param_GenericObject p1 = new Param_GenericObject();
            Param_Boolean p2 = new Param_Boolean();
            Param_Boolean p3 = new Param_Boolean();
            Param_GenericObject last = new Param_GenericObject();
            last.Access = GH_ParamAccess.list;
            last.Name = "列表";
            last.NickName = "L";
            last.Description = "删除结果";
            p1.Name = "列表";
            p1.NickName = "L";
            p1.Description = "待删除首尾项的列表";
            p1.Access = GH_ParamAccess.list;
            p2.Name = "首项";
            p2.NickName = "S";
            p2.Description = "删除列表中的第一项";
            p2.Access = GH_ParamAccess.item;
            p2.SetPersistentData(false);
            p3.Name = "尾项";
            p3.NickName = "E";
            p3.Description = "删除列表中的最后项";
            p3.Access = GH_ParamAccess.item;
            p3.SetPersistentData(false);
            Params.RegisterInputParam(p1);
            Params.RegisterInputParam(p2);
            Params.RegisterInputParam(p3);
            Params.RegisterOutputParam(last);
            p1.Optional = true;
            p2.Optional = true;
            p3.Optional = true;
            last.Optional = true;
        }
        private void CutForTree()/////删除列表
        {
            Params.UnregisterInputParameter(Params.Input[0], true);
            Params.UnregisterInputParameter(Params.Input[0], true);
            Params.UnregisterInputParameter(Params.Input[0], true);
            Params.UnregisterOutputParameter(Params.Output[0], true);
            Param_GenericObject p1 = new Param_GenericObject();
            Param_Boolean p2 = new Param_Boolean();
            Param_Boolean p3 = new Param_Boolean();
            Param_GenericObject last = new Param_GenericObject();
            last.Access = GH_ParamAccess.tree;
            last.Name = "树形数据";
            last.NickName = "T";
            last.Description = "删除结果";
            p1.Name = "树形数据";
            p1.NickName = "T";
            p1.Description = "待删除首尾项的列表的树形数据";
            p1.Access = GH_ParamAccess.tree;
            p2.Name = "首项";
            p2.NickName = "S";
            p2.Description = "删除树形数据中的第一个列表";
            p2.Access = GH_ParamAccess.item;
            p2.SetPersistentData(false);
            p3.Name = "尾项";
            p3.NickName = "E";
            p3.Description = "删除树形数据中的最后一个列表";
            p3.Access = GH_ParamAccess.item;
            p3.SetPersistentData(false);
            Params.RegisterInputParam(p1);
            Params.RegisterInputParam(p2);
            Params.RegisterInputParam(p3);
            Params.RegisterOutputParam(last);
            p1.Optional = true;
            p2.Optional = true;
            p3.Optional = true;
            last.Optional = true;
        }
        #endregion
    }
}