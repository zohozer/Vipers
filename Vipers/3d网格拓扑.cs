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

namespace Vipers
{
    public class _3d网格拓扑 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the _3d网格拓扑 class.
        /// </summary>
        public _3d网格拓扑()
            : base("3d网格拓扑", "Hull3D",
                "将空间点转换为网格面",
                "Vipers", "viper.mesh")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter ("点", "P", "一组空间点", GH_ParamAccess.list);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("网格", "M", "生成的网格",GH_ParamAccess.item);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> list = new List<Point3d>();
            if (DA.GetDataList<Point3d>(0, list))
            {
                Point3d pointd2 = list[0];
                if (pointd2.IsValid)
                {
                    Mesh mesh = new Mesh();
                    if (list.Count > 3)
                    {
                        List<Point3d> list2 = new List<Point3d>();
                        list2.AddRange(list);
                        Random random = new Random();
                        while (mesh.Vertices.Count < 4)
                        {
                            bool flag = true;
                            int index = random.Next(0, list2.Count - 1);
                            if (mesh.Vertices.Count == 2)
                            {
                                flag = this.checkIfOnLine(mesh.Vertices[0], mesh.Vertices[1], list2[index]);
                            }
                            if (flag)
                            {
                                mesh.Vertices.Add(list2[index]);
                                list2.RemoveAt(index);
                            }
                        }
                        mesh.Faces.AddFace(0, 1, 2);
                        mesh.Faces.AddFace(1, 2, 3);
                        mesh.Faces.AddFace(2, 3, 0);
                        mesh.Faces.AddFace(3, 0, 1);
                        mesh.Vertices.CombineIdentical(true, true);
                        mesh.FaceNormals.ComputeFaceNormals();
                        mesh.UnifyNormals();
                        mesh.Normals.ComputeNormals();
                        while (list2.Count > 0)
                        {
                            int num2 = random.Next(0, list2.Count - 1);
                            Point3d point = new Point3d(list2[num2]);
                            if (!this.IsPtInMesh(point, mesh))
                            {
                                List<int> list3 = new List<int>();
                                list3 = this.seenFaces(point, mesh);
                                list3.Sort();
                                for (int i = list3.Count - 1; i >= 0; i += -1)
                                {
                                    mesh.Faces.RemoveAt(list3[i]);
                                }
                                mesh = this.createNewFaces(point, mesh);
                            }
                            list2.RemoveAt(num2);
                        }
                    }
                    DA.SetData(0, mesh);
                }
            }
        }
        public bool checkIfOnLine(Point3d pt1, Point3d pt2, Point3d pttested)
        {
            double num = pttested.DistanceTo(pt1);
            double num2 = pttested.DistanceTo(pt2);
            double num3 = pt1.DistanceTo(pt2);
            return ((num + num2) > num3);
        }
        public Mesh createNewFaces(Point3d pt, Mesh mesh)
        {
            List<Polyline> list2 = new List<Polyline>();
            list2.AddRange(mesh.GetNakedEdges());
            List<Line> list = new List<Line>();
            int num3 = list2.Count - 1;
            for (int i = 0; i <= num3; i++)
            {
                Polyline polyline = new Polyline();
                list.AddRange(list2[i].GetSegments());
            }
            Mesh other = new Mesh();
            int num4 = list.Count - 1;
            for (int j = 0; j <= num4; j++)
            {
                Mesh mesh4 = new Mesh();
                Line line = list[j];
                mesh4.Vertices.Add(line.From);
                mesh4.Vertices.Add(line.To);
                mesh4.Vertices.Add(pt);
                mesh4.Faces.AddFace(0, 1, 2);
                other.Append(mesh4);
            }
            mesh.Append(other);
            mesh.Vertices.CombineIdentical(true, true);
            mesh.UnifyNormals();
            return mesh;
        }
        public double dotProduct(Vector3d v1, Vector3d v2)
        {
            return (((v2.X * v1.X) + (v2.Y * v1.Y)) + (v2.Z * v1.Z));
        }
        public bool IsPtInMesh(Point3d point, Mesh mesh)
        {
            mesh.FaceNormals.ComputeFaceNormals();
            mesh.UnifyNormals();
            mesh.Normals.ComputeNormals();
            BoundingBox boundingBox = mesh.GetBoundingBox(true);
            if (!boundingBox.Contains(point))
            {
                return false;
            }
            Polyline points = new Polyline();
            points.Add(point);
            Vector3d vectord = new Vector3d(100.0, 100.0, 100.0);
            points.Add(boundingBox.Max + vectord);
            int[] faceIds = null;
            Point3d[] pointdArray = Intersection.MeshPolyline(mesh, new PolylineCurve(points), out faceIds);
            if (pointdArray == null)
            {
                return false;
            }
            if ((pointdArray.Length % 2) == 0)
            {
                return false;
            }
            return true;
        }
        public List<int> seenFaces(Point3d point, Mesh mesh)
        {
            List<int> list2 = new List<int>();
            int num2 = mesh.Faces.Count - 1;
            for (int i = 0; i <= num2; i++)
            {
                Vector3d vectord = mesh.FaceNormals[i];
                Point3d faceCenter = mesh.Faces.GetFaceCenter(i);
                Vector3d vectord2 = this.vec2Pts(point, faceCenter);
                if (this.dotProduct(vectord2, vectord) < 0.001)
                {
                    list2.Add(i);
                }
            }
            return list2;
        }
        public Vector3d vec2Pts(Point3d pt1, Point3d pt2)
        {
            return new Vector3d(pt2.X - pt1.X, pt2.Y - pt1.Y, pt2.Z - pt1.Z);
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
                return Resource1.mesh_网格;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{1ebc57ed-f9e8-4c5f-a3a5-d50025f187f0}"); }
        }
    }
}