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
    public class TreeBranch : GH_Component
    {
        public TreeBranch ()
            : base("筛选树形数据分支", "TreeBranch",
                "通过制定的筛选模式筛选树形数据的分支，T1端输出满足筛选条件的分支，T2为不满足条件的分支，右键切换筛选模式",
               "Vipers", "Viper.data")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("{8464EACA-CFC2-470E-8DFC-D97EDAC94E55}"); }
        }
       protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Resource1.data_筛选树枝;
            }
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter ("源树形数据", "T", "用于筛选的源树形数据", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("整数列表", "I", "一列整数数据，如果源树形数据的分支索引值在该列表中，则由T1端输出，反之由T2端输出", GH_ParamAccess.list);
            pManager.HideParameter(0);
            Message = "以整数列表筛选分支";
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter ("树形数据1", "T1", "源树形数据中满足筛选条件的数据分支", GH_ParamAccess.tree);
            pManager.AddGenericParameter("树形数据2", "T2", "源树形数据中不满足筛选条件的数据分支", GH_ParamAccess.tree);
            pManager.HideParameter(1);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Structure<IGH_Goo> x = new GH_Structure<IGH_Goo>();
            if (!DA.GetDataTree(0, out x)) return;
            /////////////////////////////////////////////////////////////////////
            GH_Structure<IGH_Goo> last1 = new GH_Structure<IGH_Goo>();////结果1
            GH_Structure<IGH_Goo> last2= new GH_Structure<IGH_Goo>();////结果2
            int index1 = 0;
            int index2 = 0;
            if(IntList)///整数列表
            {
                List<int> y = new List<int>();
                if(!DA.GetDataList(1,y))return;
                for (int i = 0; i < x.PathCount; i++)
                {
                    if (y.Contains(i))
                    {
                        last1.AppendRange(x.Branches[i], new GH_Path(0, index1));
                        index1++;
                    }
                    else
                    {
                        last2.AppendRange(x.Branches[i], new GH_Path(0, index2));
                        index2++;
                    }
                }
                DA.SetDataTree(0,last1);
                DA.SetDataTree(1, last2);
            }
            else if (BoolList)///布尔列表
            {
                List<bool> y = new List<bool>();
                if (!DA.GetDataList(1, y)) return;
                for (int i = 0; i < x.PathCount; i++)
                {
                    if (y[i % y.Count])
                    {
                        last1.AppendRange(x.Branches[i], new GH_Path(0, index1));
                        index1++;
                    }
                    else
                    {
                        last2.AppendRange(x.Branches[i],new GH_Path(0, index2));
                        index2++;
                    }
                }
                DA.SetDataTree(0, last1);
                DA.SetDataTree(1, last2);
            }
            else////区间列表
            {
                List<Rhino.Geometry.Interval> y = new List<Rhino.Geometry.Interval>();
                if (!DA.GetDataList(1, y)) return;
                for (int i = 0; i < x.PathCount; i++)
                {
                    bool flag = true;
                    for (int q = 0; q < y.Count; q++)
                    {
                        if (y[q].IncludesParameter(i))
                        {
                            last1.AppendRange(x.Branches[i], new GH_Path(0, index1));
                            index1++;
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        last2.AppendRange(x.Branches[i], new GH_Path(0, index2));
                        index2++;
                    }
                }
                DA.SetDataTree(0, last1);
                DA.SetDataTree(1, last2);
            }
        }
        bool intList = true;

        public bool IntList
        {
            get { return intList; }
            set 
            { 
                intList = value;
                if(IntList)
                {
                    Message = "以整数列表筛选分支";
                }
            }
        }
        bool boolList = false;

        public bool BoolList
        {
            get { return boolList; }
            set 
            { 
                boolList = value;
                if (BoolList)
                {
                    Message = "以布尔列表筛选分支";
                }
            }
        }
        bool intervalList = false;

        public bool IntervalList
        {
            get { return intervalList; }
            set 
            { 
                intervalList = value;
                if (IntervalList)
                {
                    Message = "以区间列表筛选分支";
                }
            }
        }
        public override bool Read(GH_IReader reader)
        {
            IntList = reader.GetBoolean("IntList");
            BoolList = reader.GetBoolean("BoolList");
            IntervalList = reader.GetBoolean("IntervalList");
            return base.Read(reader);
        }
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("IntList", IntList);
            writer.SetBoolean("BoolList", BoolList);
            writer.SetBoolean("IntervalList", IntervalList);
            return base.Write(writer);
        }
        #region menu override
        Color cor1 = Color.FromArgb(61, 200, 44);
        Color cor2 = Color.FromArgb(61, 150, 44);
        Color cor3 = Color.FromArgb(61, 100, 44);
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            ToolStripMenuItem item1=Menu_AppendItem(menu, "整数列表筛选分支", AdvancedMenuClicked1, true, IntList);
            ToolStripMenuItem item2 = Menu_AppendItem(menu, "布尔列表筛选分支", AdvancedMenuClicked2, true, BoolList);
            ToolStripMenuItem item3 = Menu_AppendItem(menu, "区间列表筛选分支", AdvancedMenuClicked3, true, IntervalList);
            item1.BackColor = cor1;
            item2.BackColor = cor2;
            item3.BackColor = cor3;
        }
        private void AdvancedMenuClicked1(object sender, EventArgs e)/////整数列表筛选分支
        {
            RecordUndoEvent("TreeBranch");
            IntForTree();
            IntList = true;
            BoolList = false;
            IntervalList = false;
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void AdvancedMenuClicked2(object sender, EventArgs e)/////布尔列表筛选分支
        {
            RecordUndoEvent("TreeBranch");
            BoolForTree();
            IntList = false;
            BoolList = true;
            IntervalList = false;
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void AdvancedMenuClicked3(object sender, EventArgs e)/////区间列表筛选分支
        {
            RecordUndoEvent("TreeBranch");
            IntervalForTree();
            IntList = false;
            BoolList = false;
            IntervalList = true;
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        #endregion

        #region (un)folding logic
        private void IntForTree()/////数字列表
        {
            Params.UnregisterInputParameter(Params.Input[1], true);///移除第二个输入端
            IGH_Param p = new Param_Integer();
            p.Name = "整数列表";
            p.NickName = "I";
            p.Description = "一列整数数据，如果源树形数据的分支索引值在该列表中，则有T1端输出，反之由T2端输出";
            p.Access = GH_ParamAccess.list;
            Params.RegisterInputParam(p);
            p.Optional = true;
            
        }
        private void BoolForTree()/////布尔列表
        {
            Params.UnregisterInputParameter(Params.Input[1], true);///移除第二个输入端
            IGH_Param p = new Param_Boolean();
            p.Name = "布尔列表";
            p.NickName = "B";
            p.Description = "一列布尔数据，遍历布尔列表，true从T1端输出，反之由T2端输出";
            p.Access = GH_ParamAccess.list;
            Params.RegisterInputParam(p);
            p.Optional = true;
            
        }
        private void IntervalForTree()/////区间列表
        {
            Params.UnregisterInputParameter(Params.Input[1], true);///移除第二个输入端
            IGH_Param p = new Param_Interval();
            p.Name = "区间列表";
            p.NickName = "D";
            p.Description = "一列区间数据，分支的索引位置在区间列表中任意一个区间（包括区间首尾值）中的，从T1端输出，反之由T2端输出";
            p.Access = GH_ParamAccess.list;
            Params.RegisterInputParam(p);
            p.Optional = true;
        }
        #endregion
    }
}