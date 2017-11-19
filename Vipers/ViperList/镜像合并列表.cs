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

namespace Vipers///TangChi 2015.10.22
{
    public class MirrorJoinList : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent45 class.
        /// </summary>
        public MirrorJoinList()
            : base("镜像合并列表", "MirrorJoinList",
                "将用户提供的列表镜像后合并，并根据设置的数量增加或减少中间的元素",
                "Vipers", "Viper.data")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("列表","L","用户提供的源列表",GH_ParamAccess.list);
            pManager.AddIntegerParameter("数量","N","生成的列表元素总数",GH_ParamAccess.item);
            pManager.HideParameter(0);
            Message = "镜像合并列表";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("列表","L","镜像合并后的列表",GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<object> list = new List<object>();
            int count = 0;
            if(!DA.GetDataList(0,list))return;
            if(!DA.GetData(1,ref count))return;
            List<object> x = list;
            int y = count;
            int counts = x.Count;
            List<object> newlist = new List<object>();
            ////////////////////////////////////////////////////////////////////////
            if (2 * counts < y)
            {
                newlist.AddRange(x);
                for (int i = 0; i < y - 2 * counts; i++)
                {
                    newlist.Add(x[counts - 1]);
                }
                x.Reverse();
                newlist.AddRange(x);
            }
            ////////////////////////////////////////////////////////////////////////
            else if (2 * counts == y)
            {
                newlist.AddRange(x);
                x.Reverse();
                newlist.AddRange(x);
            }
            ////////////////////////////////////////////////////////////////////////
            else if (2 * counts > y)
            {
                newlist.AddRange(x);
                x.Reverse();
                newlist.AddRange(x);
                double mm = y / 2;
                int num = Convert.ToInt32(Math.Round(mm));
                for (int i = num; newlist.Count > y; i++)
                {
                    newlist.RemoveAt(i);
                    i--;
                }
            }
            DA.SetDataList(0, newlist);
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
                //return Resource1.镜像合并列表;
                return Resource1.data_镜像列表;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{512feba8-2969-4e44-92e6-44c5dac66e12}"); }
        }
    }
}