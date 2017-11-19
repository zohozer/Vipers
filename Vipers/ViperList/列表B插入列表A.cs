using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Vipers///TangChi 2015.12.3
{
    public class ListBInstertListA: GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent56 class.
        /// </summary>
        public ListBInstertListA()
            : base("列表B插入列表A", "ListBInstertListA",
                "将列表B中的项目按照要求依次插入列表A中",
               "Vipers", "Viper.data")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("列表A","LA","被插入的列表",GH_ParamAccess.list);
            pManager.AddGenericParameter("列表B", "LB", "插入的列表", GH_ParamAccess.list);
            pManager.AddIntegerParameter("位置","I","第一个数据插入列表A的位置",GH_ParamAccess.item,1);
            pManager.AddIntegerParameter("列表A项目间隔数量", "NA", "以后每间隔countA个列表A中项目后插入项目B指定个数", GH_ParamAccess.item, 1);
            pManager.AddIntegerParameter("列表B项目间隔数量", "NB", "每次指定countB个列表B中项目后插入列表A中", GH_ParamAccess.item, 1);
            pManager.AddBooleanParameter("列表B剩余项目", "E", "如果插入列表A后列表B还有剩余项，则将剩余项加入到列表A末尾", GH_ParamAccess.item, false);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
            Message = "列表B插入列表A";

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("列表C", "LC", "插入后新的列表", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<object> ListA=new List<object>();
            List<object> ListB=new List<object>();
            int index=1;
            int countA=1;
            int countB=1;
            bool cutEnd = false;
            if(!DA.GetDataList(0,ListA))return;
            if (!DA.GetDataList(1, ListB)) return;
            if (!DA.GetData(2, ref index)) return;
            if (!DA.GetData(3, ref countA)) return;
            if (!DA.GetData(4, ref countB)) return;
            if (!DA.GetData(5, ref cutEnd)) return;
            int cut = ListA.Count;
            for (int i = 0; ListB.Count > 0 && countB != 0; i++)
            {
                for (int j = 0; j < countB && ListB.Count > 0 && countB != 0; j++)
                {
                    ListA.Insert(index, ListB[0]);
                    ListB.RemoveAt(0);
                    index++;
                }
                index += countA;
                cut -= countA;
                if (cut <= 0)
                {
                    break;
                }
            }
            if (!cutEnd)
            {
                ListA.AddRange(ListB);
            }
            DA.SetDataList(0, ListA);
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
                //return Resource1.列表B插入列表A;
                return Resource1.data_列表B插入列表A;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{c1b7bbe0-060c-4fcc-afa3-ff11fb1da6c3}"); }
        }
    }
}