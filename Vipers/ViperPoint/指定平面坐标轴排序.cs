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

namespace Vipers
{
    public class 指定平面坐标轴排序 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the 指定平面坐标轴排序 class.
        /// </summary>
        public 指定平面坐标轴排序()
            : base("指定平面坐标轴排序", "PointsSort",
                "根据用户设置的平面沿X,Y,Z轴排序，右键菜单指定排序轴",
                "Vipers", "Viper.point")
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
            pManager.AddPointParameter("点","P","排序点",GH_ParamAccess.list);
            pManager.AddPlaneParameter("平面","P","排序参考平面",GH_ParamAccess.item,Plane.WorldXY);
            pManager.AddBooleanParameter("反向","R","反向排序",GH_ParamAccess.item,false);
            pManager.HideParameter(0);
            pManager.HideParameter(1);
            Message = "X轴方向排序";
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("点","P","排序后的点",GH_ParamAccess.list);
            pManager.AddIntegerParameter("索引", "I", "排序后的点在原来点的索引位置", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> points = new List<Point3d>();
            Plane plane = new Plane();
            bool reverse = false;
            if(!DA.GetDataList(0,points))return;
            if(!DA.GetData(1,ref plane))return;
            if (!DA.GetData(2, ref reverse)) return;
            List<Point3d> remap = new List<Point3d>();
            List<double> xxx1 = new List<double>();
            List<double> yyy1 = new List<double>();
            List<double> zzz1 = new List<double>();
            List<double> xxx2 = new List<double>();
            List<double> yyy2 = new List<double>();
            List<double> zzz2 = new List<double>();
            for (int i = 0; i < points.Count; i++)
            {
                Point3d pt = Point3d.Unset;
                plane.RemapToPlaneSpace(points[i], out pt);
                remap.Add(pt);
                xxx1.Add(pt.X);////原x坐标
                yyy1.Add(pt.Y);////原y坐标
                zzz1.Add(pt.Z);////原z坐标
                xxx2.Add(pt.X);
                yyy2.Add(pt.Y);
                zzz2.Add(pt.Z);
            }
            xxx2.Sort();
            yyy2.Sort();
            zzz2.Sort();
            if (reverse)
            {
                xxx2.Reverse();
                yyy2.Reverse();
                zzz2.Reverse();
            }
            List<double> test1 = new List<double>();///原坐标
            List<double> test2 = new List<double>();///排序后坐标
            if (_VectorX)///////如果是x方向排序
            {
                test1 = xxx1;//////////////////////////////////////默认为x坐标
                test2 = xxx2;//////////////////////////////////////默认为x坐标
            }
            else if (_VectorY)
            {
                test1 = yyy1;
                test2 = yyy2;
            }
            else if (_VectorZ)
            {
                test1 = zzz1;
                test2 = zzz2;
            }
            List<Point3d> last = new List<Point3d>();////排序后的点
            List<int> index = new List<int>();////序号
            for (int i = 0; i < test2.Count; i++)
            {
                for (int q = 0; q < test1.Count; q++)
                {
                    if (test1[q] == double.MaxValue)
                    {
                        continue;
                    }
                    if (test2[i] == test1[q])
                    {
                        index.Add(q);
                        test1[q] = double.MaxValue;
                        last.Add(points[q]);
                    }
                }

            }
            DA.SetDataList(0, last);
            DA.SetDataList(1, index);
        }
        private bool _vectorx = true;
        private bool _vectory = false;
        private bool _vectorz = false;
        public bool _VectorX/////////////////稀疏
        {
            get { return _vectorx; }
            set
            {
                _vectorx = value;
                if ((_vectorx))
                {
                    Message = "X轴方向排序";
                }
            }
        }
        public bool _VectorY///////////////普通
        {
            get { return _vectory; }
            set
            {
                _vectory = value;
                if ((_vectory))
                {
                    Message = "Y轴方向排序";
                }
            }
        }
        public bool _VectorZ///////////////致密
        {
            get { return _vectorz; }
            set
            {
                _vectorz = value;
                if ((_vectorz))
                {
                    Message = "Z轴方向排序";
                }
            }
        }

        public override bool Read(GH_IReader reader)
        {
            _VectorX = reader.GetBoolean("_VectorX");
            _VectorY = reader.GetBoolean("_VectorY");
            _VectorZ = reader.GetBoolean("_VectorZ");
            return base.Read(reader);
        }
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("_VectorX", _VectorX);
            writer.SetBoolean("_VectorY", _VectorY);
            writer.SetBoolean("_VectorZ", _VectorZ);
            return base.Write(writer);
        }

        Color cor1 = Color.FromArgb(61, 200, 44);
        Color cor2 = Color.FromArgb(61, 150, 44);
        Color cor3 = Color.FromArgb(61, 100, 44);
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            // Append the item to the menu, making sure it's always enabled and checked if Absolute is True.
            ToolStripMenuItem item = Menu_AppendItem(menu, "X轴方向排序", Menu_AbsoluteClicked, true, _VectorX);
            ToolStripMenuItem item2 = Menu_AppendItem(menu, "Y轴方向排序", Menu_AbsoluteClicked2, true, _VectorY);
            ToolStripMenuItem item3 = Menu_AppendItem(menu, "Z轴方向排序", Menu_AbsoluteClicked3, true, _VectorZ);
            // Specifically assign a tooltip text to the menu item.
            item.BackColor = cor1;
            item2.BackColor = cor2;
            item3.BackColor = cor3;
        }
        private void Menu_AbsoluteClicked(object sender, EventArgs e)
        {
            RecordUndoEvent("Absolute");
            _VectorX = true;
            _VectorY = false;
            _VectorZ = false;
            ExpireSolution(true);
        }
        private void Menu_AbsoluteClicked2(object sender, EventArgs e)
        {
            RecordUndoEvent("Absolute");
            _VectorX = false;
            _VectorY = true;
            _VectorZ = false;
            ExpireSolution(true);
        }
        private void Menu_AbsoluteClicked3(object sender, EventArgs e)
        {
            RecordUndoEvent("Absolute");
            _VectorX = false ;
            _VectorY = false;
            _VectorZ = true ;
            ExpireSolution(true);
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
                return Resource1.point_点沿指定平面轴排序;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{45fa336d-066f-494c-8820-b5193de4b705}"); }
        }
    }
}