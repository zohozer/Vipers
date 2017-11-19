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
    public class FastMesh: GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the brep转网格 class.
        /// </summary>
        public FastMesh()
            : base("brep转网格", "FastMesh",
                "将brep转换为网格",
                "Vipers", "viper.mesh")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter ("体块", "B", "待转换成网格的体块", GH_ParamAccess.list);
            pManager.AddNumberParameter ("边长", "L", "最大网格边长度", GH_ParamAccess.item,1000.0);
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
            double num=0;
            List<Brep> list = new List<Brep>();
            if (DA.GetDataList<Brep>(0, list) && DA.GetData<double>(1, ref num))
            {
                Brep[] brepArray = null;
                brepArray = Brep.CreateBooleanUnion(list.ToArray(), 0.1);
                MeshingParameters meshingParameters = new MeshingParameters
                {
                    MaximumEdgeLength = num
                };
                Mesh[] meshArray = Mesh.CreateFromBrep(brepArray[0], meshingParameters);
                Mesh msh = new Mesh();
                int num3 = meshArray.Length - 1;
                for (int i = 0; i <= num3; i++)
                {
                    msh.Append(meshArray[i]);
                }
                msh.Vertices.CullUnused();
                msh.Vertices.CombineIdentical(true, true);
                msh.UnifyNormals();
                msh = this.triangulation(msh);
                DA.SetData(0, msh);
            }
        }
        public Mesh triangulation(Mesh msh)
        {
            Mesh mesh = new Mesh();
            int num2 = msh.Faces.Count - 1;
            for (int i = 0; i <= num2; i++)
            {
                Mesh other = new Mesh();
                MeshFace face = new MeshFace();
                face = msh.Faces[i];
                Point3d vertex = new Point3d(msh.Vertices[face.A]);
                Point3d pointd2 = new Point3d(msh.Vertices[face.B]);
                Point3d pointd3 = new Point3d(msh.Vertices[face.C]);
                other.Vertices.Add(vertex);
                other.Vertices.Add(pointd2);
                other.Vertices.Add(pointd3);
                MeshFace face2 = msh.Faces[i];
                if (face2.IsQuad)
                {
                    Point3d pointd4 = new Point3d(msh.Vertices[face.D]);
                    other.Vertices.Add(pointd4);
                    if (vertex.DistanceTo(pointd3) <= pointd2.DistanceTo(pointd4))
                    {
                        other.Faces.AddFace(0, 1, 2);
                        other.Faces.AddFace(0, 2, 3);
                    }
                    else
                    {
                        other.Faces.AddFace(0, 1, 3);
                        other.Faces.AddFace(1, 2, 3);
                    }
                }
                else
                {
                    other.Faces.AddFace(0, 1, 2);
                }
                mesh.Append(other);
            }
            mesh.Vertices.CombineIdentical(true, true);
            mesh.UnifyNormals();
            return mesh;
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
                return Resource1.mesh_brep转网格;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{1b21a6bc-145b-499d-9e28-2499ec36b45b}"); }
        }
    }
}