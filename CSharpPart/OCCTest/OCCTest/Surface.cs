using BRep;
using Geom;
using gp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using TopAbs;
using TopExp;
using TopoDS;

namespace OCCTest
{
    /// <summary>
    /// a surface is defined by two sets of points 
    /// those 2sets of points are defined by 2 faces
    /// it can retrieve the surface into List<Face> that can be viewed by unity
    /// </summary>
    class Surface
    {
        public Face f1 = new Face();
        public Face f2 = new Face();
        

        public TopAbs_Orientation orientation;

        public List<Face> subFaces = new List<Face>();

        public Surface() { }
        public Surface(List<gp_Pnt> faces, TopAbs_Orientation orientation)
        {
            // note that if u don't have at least 3 points in a row coming from the first face, this gonna be a total mess
            List<Face> twoFaces = GetFaces(faces);

            f1 = twoFaces[0];
            f2 = twoFaces[1];
            this.orientation = orientation;
        }

        private List<Face> GetFaces(List<gp_Pnt> faces)
        {
            
            bool uniqueFace = true;
            Face unique = new Face();
            foreach(gp_Pnt pt in faces)
            {
                if (unique.Contains(pt))
                    unique.Add(pt);
                else
                    uniqueFace = false;
            }


            Face f1 = new Face();
            Face f2 = new Face();
            if (!uniqueFace)
            {
                foreach (gp_Pnt pt in faces)
                {
                    if (f1.pts.Count < faces.Count / 2)
                    {
                        if (f1.Contains(pt))
                            f1.Add(pt);
                        else
                            f2.Add(pt);
                    }
                    else
                    {
                        if (f2.Contains(pt))
                            f2.Add(pt);
                        else
                            f1.Add(pt);
                    }
                }
            } else
            {
                // we got one unique face... well in fact this face is constituted of two sets of vertices, one exterior and one interior, here we manage to create two faces from this face
                f1=new Face(unique.ToTrianglesGraham(false).pts); // set exterior
                f2= faces.Count-f1.pts.Count>=3 ? new Face(f1.Difference(unique).ToTrianglesGraham(false).pts) : new Face(f1.Difference(unique).pts);
            }//*/

            return new List<Face> { f1, f2 };
        }
        
        public List<Face> ComputeFaces()
        {

            // let's compose the face between those 2 faces
            List<Face> subFaces = new List<Face>();
            int limit = Math.Min(f1.pts.Count - 1, f2.pts.Count - 1);
            if (limit>=1)
            {
                for (int i = 0; i < limit; i++)
                {
                    Face tempFace = new Face();
                    tempFace.Add(f1.pts[i]);
                    tempFace.Add(f1.pts[i + 1]);
                    tempFace.Add(f2.pts[i + 1]);
                    tempFace.Add(f2.pts[i]);
                    tempFace.orientation = orientation;
                    subFaces.Add(tempFace);
                }

                Face temp = new Face();
                temp.Add(f1.pts[limit]);
                temp.Add(f1.pts[0]);
                temp.Add(f2.pts[0]);
                temp.Add(f2.pts[limit]);
                temp.orientation = orientation; 
                subFaces.Add(temp);
            }

            return subFaces;
        }
    }
}
