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
using Rhino.Geometry.Intersect;

namespace Vipers.ViperCurve
{
    public class CloseCurves : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the 生成闭合曲线 class.
        /// </summary>
        public CloseCurves()
            : base("生成闭合曲线", "CloseCurves",
                "将指定曲线的围合部分生成闭合曲线（该电池直接盗版Heteroptera中的GeometricRegion电池，如支持正版，请使用Heteroptera）",
                "Vipers", "Viper.curve")
        {
            Message = "生成闭合曲线";
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("曲线","C","一组曲线",GH_ParamAccess.list);
            pManager.HideParameter(0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("曲线", "C", "生成的闭合曲线", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> list = new List<Curve>();
            DA.GetDataList<Curve>(0, list);
            Plane worldXY = Plane.WorldXY;
            List<Curve> list2 = new List<Curve>();
            for (int i = 0; i <= (list.Count - 1); i++)
            {
                List<double> t = new List<double>();
                for (int k = 0; k <= (list.Count - 1); k++)
                {
                    if (i != k)
                    {
                        foreach (IntersectionEvent event2 in Intersection.CurveCurve(list[i], list[k], RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance))
                        {
                            t.Add(event2.ParameterA);
                        }
                    }
                }
                Curve[] collection = list[i].Split(t);
                list2.AddRange(collection);
            }
            List<Point3d> list3 = new List<Point3d>();
            List<Curve> list4 = new List<Curve>();
            List<int> source = new List<int>();
            List<int> list6 = new List<int>();
            List<int> list7 = new List<int>();
            List<int> list8 = new List<int>();
            List<int> list9 = new List<int>();
            List<Plane> list10 = new List<Plane>();
            List<bool> list11 = new List<bool>();
            DataTree<Curve> tree = new DataTree<Curve>();
            DataTree<int> tree2 = new DataTree<int>();
            foreach (Curve curve in list2)
            {
                for (int m = 0; m <= 2; m += 2)
                {
                    list4.Add(curve);
                    source.Add(source.Count);
                    list6.Add(source.Count - m);
                    list7.Add(-1);
                    list9.Add(-1);
                    list11.Add(false);
                    int num7 = -1;
                    for (int n = 0; n <= (list3.Count - 1); n++)
                    {
                        Point3d pointd = list3[n];
                        if (pointd.DistanceTo(curve.PointAtStart) < RhinoDoc.ActiveDoc.ModelAbsoluteTolerance)
                        {
                            num7 = n;
                            break;
                        }
                    }
                    if (num7 > -1)
                    {
                        list8.Add(num7);
                        tree2.Add(source.Last<int>(), new GH_Path(num7));
                    }
                    else
                    {
                        list8.Add(list3.Count);
                        tree2.Add(source.Last<int>(), new GH_Path(list3.Count));
                        list3.Add(curve.PointAtStart);
                    }
                    Plane plane = new Plane(list3[list8.Last<int>()], worldXY.ZAxis);
                    double angle = Vector3d.VectorAngle(plane.XAxis, curve.TangentAtStart, plane);
                    plane.Rotate(angle, plane.ZAxis);
                    list10.Add(plane);
                    curve.Reverse();
                }
            }
            foreach (GH_Path path in tree2.Paths)
            {
                if (tree2.Branch(path).Count == 1)
                {
                    list11[tree2.Branch(path)[0]] = true;
                    list11[list6[tree2.Branch(path)[0]]] = true;
                }
            }
            foreach (int num9 in source)
            {
                Plane plane3 = list10[list6[num9]];
                int num10 = -1;
                double num11 = 6.2831853071795862;
                foreach (int num12 in tree2.Branch(list8[list6[num9]]))
                {
                    if (((num12 != list6[num9]) && !list11[num9]) && !list11[num12])
                    {
                        Plane plane4 = list10[num12];
                        double num13 = Vector3d.VectorAngle(plane3.XAxis, plane4.XAxis, plane3);
                        if (num13 < num11)
                        {
                            num10 = num12;
                            num11 = num13;
                        }
                    }
                }
                list7[num9] = num10;
            }
            List<int> list12 = new List<int>();
            List<int> list13 = new List<int>();
            foreach (int num14 in source)
            {
                int num15 = 0;
                if (list9[num14] != -1)
                {
                    continue;
                }
                int num16 = 1;
                int count = tree.Paths.Count;
                int num18 = num14;
                tree.Add(list4[num18], new GH_Path(count));
                list9[num18] = count;
            Label_046C:
                if (list7[num18] == -1)
                {
                    list13.Add(count);
                }
                else
                {
                    num18 = list7[num18];
                    tree.Add(list4[num18], new GH_Path(count));
                    num16++;
                    list9[num18] = count;
                    if (list7[num18] != num14)
                    {
                        num15++;
                        if (num15 != (list2.Count - 1))
                        {
                            goto Label_046C;
                        }
                    }
                }
                list12.Add(num16);
            }
            int item = -1;
            int num2 = -1;
            for (int j = 0; j <= (list12.Count - 1); j++)
            {
                if (list12[j] > num2)
                {
                    item = j;
                    num2 = list12[j];
                }
            }
            list13.Add(item);
            int num3 = 0;
            List<Curve> data = new List<Curve>();
            foreach (GH_Path path2 in tree.Paths)
            {
                if (!list13.Contains(path2.Indices[0]))
                {
                    data.Add(Curve.JoinCurves(tree.Branch(path2))[0]);
                    num3++;
                }
            }
            DA.SetDataList(0, data);
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
                return Resource1.curve_生成闭合曲线;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{d0a64879-04c4-4ef5-ab4c-8251b8e51c5c}"); }
        }
    }
}