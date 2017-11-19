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

namespace Vipers///////TangChi 2016.8.10
{
    public class RandomGroup : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the 物件随机分组 class.
        /// </summary>
        public RandomGroup()
            : base("物件随机分组", "RandomGroup",
                "将一组物件按指定的组数均分，如果不能平均分组，则采用物件数量除以分组数得到的商为每组的物件数量，余数量的物件则依次从第一组加到最后一组，每组加一个，加完为止。通过设置布尔值选择物件分组情况，true：随机分组，false：直接分组",
               "Vipers", "Viper.data")
        {
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            Message = "物件随机分组";
            pManager.AddGenericParameter("物件","L","一组待分组的物件",GH_ParamAccess.list);
            pManager.AddIntegerParameter("组数","N","物件共分多少组",GH_ParamAccess.item,2);
            pManager.AddBooleanParameter("切换","B","通过布尔值选择分组方式，true：随机分组，false：直接分组",GH_ParamAccess.item,true);
            pManager.AddIntegerParameter("种子","S","随机种子，更改后会改变分组的情况，但只对随机分组有效",GH_ParamAccess.item,5);
            pManager.HideParameter(0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("物件","T","按要求分组后的物件（树形数据）",GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            List<IGH_Goo> x = new List<IGH_Goo>();
            int n = 0;
            bool b = false;
            int s = 0;
            if(!DA.GetDataList(0,x)||!DA.GetData(1,ref n)||!DA.GetData(2,ref b)||!DA.GetData(3,ref s))return;
            Random rand = new Random(s);
            if (n < 2)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "随机分组不能少于2组");
                return;
            }
            DataTree<object> last = new DataTree<object>();
            List<int> index = new List<int>();
            for (int i = 0; i < x.Count; i++)
            {
                index.Add(i);
            }
            if (b)
            {
                while (x.Count != 0)
                {
                    for (int k = 0; k < n; k++)
                    {
                        if (x.Count == 0) break;
                        int test = rand.Next() % x.Count;
                        last.Add(x[test], new GH_Path(0,DA.Iteration,k));
                        x.RemoveAt(test);
                    }
                }
                DA.SetDataTree(0, last);
            }
            else/////非随机分组
            {
                List<int> mems = newGroups(x.Count, n);
                int count = 0;
                for (int i = 0; i < mems.Count; i++)
                {
                    if (x.Count == 0)
                    {
                        DA.SetDataTree(0, last);
                        return;
                    }
                    for (int q = 0; q < mems[i]; q++)
                    {
                        if (x.Count == 0)
                        {
                            DA.SetDataTree(0, last);
                            return;
                        }
                        last.Add(x[0], new GH_Path(0, DA.Iteration,count));
                        x.RemoveAt(0);
                    }
                    count++;
                }
                DA.SetDataTree(0, last);
            }
        }
        public List<int> newGroups(int x, int n) /////将x分为n份，如果不能等分，则从前往后依次加1，加完余数为止
        {
            int each = (x - x % n) / n;
            int rest = x % n;
            List<int> groups = new List<int>();
            for (int i = 0; i < n; i++)
            {
                if (rest != 0)
                {
                    groups.Add(each + 1);
                    rest--;
                }
                else
                {
                    groups.Add(each);
                }
            }
            return groups;
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
                return Resource1.data_物件随机分组;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{78db91e9-55a8-4878-a69c-2987cffff3eb}"); }
        }
    }
}