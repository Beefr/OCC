using BRep;
using Geom;
using gp;
using Poly;
using System;
using System.Collections.Generic;
using System.Text;
using TColgp;
using TopAbs;
using TopExp;
using TopoDS;

namespace OCCTest.Elements
{
    public class BasicElement
    {
        public List<Face> myFaces = new List<Face>(); // faces of the mesh

        public BasicElement() { }

        /// <summary>
        /// triangulate using the opencascade's triangulation
        /// </summary>
        /// <param name="shape">input shape</param>
        /// <param name="deflection">parameter to define the maximum angle</param>
        /// <returns>a list of faces</returns>
        public List<Face> Triangulation(TopoDS_Shape shape, double deflection)
        {
            List<Face> faces = new List<Face>();

            BRepMesh.BRepMesh_IncrementalMesh im = new BRepMesh.BRepMesh_IncrementalMesh(shape, deflection); // 0.7 it controls the number of triangles
            im.Perform();
            if (im.IsDone())
            {
                for (TopExp_Explorer aFaceExplorer = new TopExp_Explorer(im.Shape(), TopAbs_ShapeEnum.TopAbs_FACE); aFaceExplorer.More(); aFaceExplorer.Next())
                {

                    TopoDS_Face face = TopoDS.TopoDS.ToFace(aFaceExplorer.Current());


                    TopLoc.TopLoc_Location L = new TopLoc.TopLoc_Location();
                    Poly_Triangulation tri = BRep.BRep_Tool.Triangulation(face, ref L);
                    bool isDone = true; 

                    if (isDone)
                    {
                        Poly_Array1OfTriangle triangles = tri.Triangles();
                        TColgp_Array1OfPnt nodes = tri.Nodes();
                        for (int i = tri.Triangles().Lower(); i < tri.Triangles().Upper() + 1; i++)
                        {
                            Poly_Triangle triangle = triangles.Value(i);

                            int node1 = 0, node2 = 0, node3 = 0;
                            triangle.Get(ref node1, ref node2, ref node3);

                            gp_Pnt v1 = nodes.Value(node1);
                            gp_Pnt v2 = nodes.Value(node2);
                            gp_Pnt v3 = nodes.Value(node3);
                            // don't forget about face orientation :)
                            Face f = new Face(new List<gp_Pnt> { v1, v2, v3 })
                            {
                                orientation = face.Orientation()
                            };
                            faces.Add(f);
                        }


                    }



                }

            }
            return faces;
        }
        
       
        /// <summary>
        /// translate the element 
        /// </summary>
        /// <param name="pt">the vector for the translation</param>
        public void Translate(Pnt pt)
        {
            for(int i=0; i< myFaces.Count; i++)
            {
                for (int j=0; j<myFaces[i].pts.Count; j++)
                {
                    myFaces[i].pts[j] = myFaces[i].pts[j] + pt;
                }
            }
        }


        /// <summary>
        /// takes a single set of vertices and transforms it into a set of faces describing the face joining the 2wires contained inside the set
        /// </summary>
        /// <param name="face1">first face</param>
        /// <param name="face2">second face</param>
        /// <param name="orientation">orientation of the face</param>
        /// <returns>a set of faces</returns>
        public List<Face> ComputeFaces(Face face1, Face face2, TopAbs_Orientation orientation)
        {

            // let's compose the face between those 2 faces
            List<Face> subFaces = new List<Face>();
            int limit = Math.Min(face1.pts.Count - 1, face2.pts.Count - 1);
            for (int i = 0; i < limit; i++)
            {
                Face tempFace = new Face();
                tempFace.Add(face1.pts[i]);
                tempFace.Add(face1.pts[i + 1]);
                tempFace.Add(face2.pts[i + 1]);
                tempFace.Add(face2.pts[i]);
                tempFace.orientation = orientation;
                subFaces.Add(tempFace);
            }

            Face temp = new Face();
            temp.Add(face1.pts[limit]);
            temp.Add(face1.pts[0]);
            temp.Add(face2.pts[0]);
            temp.Add(face2.pts[limit]);
            temp.orientation = orientation; 
            subFaces.Add(temp);
            return subFaces;
        }
        
    }
}
