using gp;
using System;
using System.Collections.Generic;
using System.Drawing;
using TopAbs;

namespace OCCTest
{
    /// <summary>
    /// can have only one point ( face in construction )
    /// don't add vectors to a face defined by points
    /// or don't add points to a face defined by vectors
    /// TODO: add a list of vectors and a list of points + adapt the behavior of functions depending on what list is filled
    /// </summary>
    public class Face 
    {
        public TopAbs_Orientation orientation; // orientation of the face
        public List<Pnt> pts = new List<Pnt>(); // pts of the face

        public List<Face> subFaces = new List<Face>(); // subfaces of this face
        public List<int> triangles = new List<int>(); // triangles made out of this face

        // constructors
        public Face(List<gp_Pnt> pts)
        {
            this.pts = ToPnt(pts);
        }
        public Face(List<Pnt> pts)
        {
            this.pts = pts;
        }
        public Face(List<Pnt> pts, TopAbs_Orientation orientation)
        {
            this.pts = pts;
            this.orientation = orientation;
        }
        public Face()
        {
        }
        public Face(Pnt v1, Pnt v2)
        {
            pts.Add(v1);
            pts.Add(v2);
        }
        public Face(Face f)
        {
            pts = f.pts;
            orientation = f.orientation;
        }

    
        /// <summary>
        /// centroid of the face
        /// </summary>
        public Pnt Centroid
        {
            get
            {
                return Sum() / pts.Count;
            }
        }

        /// <summary>
        /// doesn't verify that the point is included but verifies if the point is on the same plan
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool Contains(Pnt p)
        {
            if(pts.Count>=3)
            {
                Edge e = new Edge(pts[0], p);
                return e.v.DotProduct(this.Normale) == 0;

            }
            return true;
        }

        /// <summary>
        /// doesn't verify that the point is included but verifies if the point is on the same plan
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool Contains(gp_Pnt p)
        {
            if (pts.Count<3) // fuck it if it was a face containing vectors and not points TODO: implement it 
            {
                return true; // if we do not have enough points or vectors then it means that we are constructing a face so any point would fit in our new face
            } else
            {
                Edge e = new Edge(pts[0], ToPnt(p));
                return e.v.DotProduct(this.Normale) == 0;
            }
        }

        /// <summary>
        /// sum of all the pts
        /// </summary>
        /// <returns></returns>
        public Pnt Sum()
        {
            double a = 0;
            double b = 0;
            double c = 0;
            foreach (Pnt pt in pts)
            {
                a = a + pt.x;
                b = b + pt.y;
                c = c + pt.z;
            }
            return new Pnt(a, b, c);
        }

        /// <summary>
        /// add a point to the face
        /// </summary>
        /// <param name="v"></param>
        public void Add(gp_Pnt v)
        {
            pts.Add(ToPnt(v));
        }
        /// <summary>
        /// add a point to the face
        /// </summary>
        /// <param name="v"></param>
        public void Add(Pnt v)
        {
            pts.Add(v);
        }

        /// <summary>
        /// returns the normale of the face
        /// </summary>
        public Pnt Normale
        {
            get
            {
                if (pts.Count > 2)
                {
                    return new Triangle(pts[0], pts[1], pts[2]).Normal;
                }
                else if (pts.Count == 2)
                {
                    //Debug.Log("Only 2 pts, they will be considered as two vectors defining a plan in order to return the normale you asked");
                    return Pnt.Cross(pts[0], pts[1]); // it's not Points, it's vectors defining the surface
                }
                else
                {
                    //Debug.Log("Not enough points in face to calculate a normale...");
                    return new Pnt(0, 0, 0);
                }
            }
        }

        /// <summary>
        /// translate the face with the vector p
        /// </summary>
        /// <param name="p"></param>
        public void Translate(Pnt p)
        {
            for(int i=0; i<pts.Count;i++)
            {
                pts[i] = new Pnt(pts[i] + p);
            }
            foreach(Face f in subFaces)
            {
                f.Translate(p);
            }
        }
       
        
        /// <summary>
        /// returns 3 Pnt giving 3vectors defining the coordinate system of the face
        /// </summary>
        public List<Pnt> CoordinateSystem
        {
            get
            {
                return new List<Pnt> { this.Normale, this.YAxis, this.ZAxis }; // Vector3 are not enough precise
            }
        }

        /// <summary>
        /// Y axis of the face
        /// </summary>
        public Pnt YAxis
        {
            get
            {
                return pts[1] - pts[0]; // it's not always Points, it can also be vectors defining the surface
            }
        }
        /// <summary>
        /// z axis of the face
        /// </summary>
        public Pnt ZAxis
        {
            get
            {
                return new Face(this.Normale, this.YAxis).Normale;
            }
        }

        
        public Pnt ToPnt(gp_Pnt v)
        {
            return new Pnt(v);
        }
        public List<Pnt> ToPnt(List<gp_Pnt> vec)
        {

            List<Pnt> output = new List<Pnt>();
            foreach (gp_Pnt v in vec)
            {
                output.Add(ToPnt(v));
            }
            return output;
        }

        /// <summary>
        /// performs graham on the points of your face and returns triangles
        /// </summary>
        /// <param name="computeCompletely">true-> returns triangles / false -> only applies graham</param>
        /// <returns>returns triangles</returns>
        public Face ToTrianglesGraham(bool computeCompletely)
        {

            UniqueVertices();
            Face f = Graham();
            
            if (computeCompletely)
            {
                if (f.pts.Count == 4)
                {
                    // __________________________________________________________
                    List<int> tempList1 = new List<int>();
                    Face tempFace1 = new Face();

                    tempList1.Add(f.RetrievesIndex(f.pts[0]));
                    tempFace1.Add(f.pts[0]);

                    tempList1.Add(f.RetrievesIndex(f.pts[1]));
                    tempFace1.Add(f.pts[1]);

                    tempList1.Add(f.RetrievesIndex(f.pts[3]));
                    tempFace1.Add(f.pts[3]);

                    tempFace1.orientation = f.orientation;

                    f.subFaces.Add(tempFace1);
                    f.triangles.AddRange(tempList1);
                    // __________________________________________________________
                    List<int> tempList2 = new List<int>();
                    Face tempFace2 = new Face();

                    tempList2.Add(f.RetrievesIndex(f.pts[1]));
                    tempFace2.Add(f.pts[1]);

                    tempList2.Add(f.RetrievesIndex(f.pts[2]));
                    tempFace2.Add(f.pts[2]);

                    tempList2.Add(f.RetrievesIndex(f.pts[3]));
                    tempFace2.Add(f.pts[3]);

                    tempFace2.orientation = f.orientation;

                    f.subFaces.Add(tempFace2);
                    f.triangles.AddRange(tempList2);
                    // __________________________________________________________
                }
                else
                {
                    Pnt centroid = f.Centroid;
                    for (int i = 0; i < f.pts.Count - 1; i++) // the -1 may sound weird but it is because we are adding the centroid to the points ;)
                    {

                        List<int> tempList = new List<int>();
                        Face tempFace = new Face();

                        //
                        tempList.Add(f.RetrievesIndex(f.pts[i]));
                        tempFace.Add(f.pts[i]);
                        //
                        if (i == f.pts.Count - 1 - 1) // same reason here
                        {
                            tempList.Add(f.RetrievesIndex(f.pts[0]));
                            tempFace.Add(f.pts[0]);
                        }
                        else
                        {
                            tempList.Add(f.RetrievesIndex(f.pts[i + 1]));
                            tempFace.Add(f.pts[i + 1]);
                        }
                        //
                        tempList.Add(f.RetrievesIndex(centroid));
                        tempFace.Add(centroid);
                        //
                        tempFace.orientation = f.orientation;
                        f.subFaces.Add(tempFace);
                        f.triangles.AddRange(tempList);
                    }
                } // looks like it never occurs
                
            return f; // ordonned points with triangles set
            }

            //*/
            return f; // only ordonned pts
        }

        /// <summary>
        /// retrieves the index of the vertex 
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public int RetrievesIndex(Pnt pt)
        {
            // make sure u applied Graham before
            if (Index(pt) == -1)
            {
                this.Add(pt);
                return pts.Count - 1;
            }
            else
            {
                return Index(pt);
            }
        }

        /// <summary>
        /// this contains all the points, f2 contains the points that we dont want
        /// </summary>
        /// <param name="f2"></param>
        /// <returns></returns>
        public Face Difference(Face unique)
        {
            Face toReturn = new Face();
            foreach(Pnt pt in unique.pts) // for each unique point in unique
            {
                if (!this.Includes(pt)) // if f1 (this) doesn't include the unique point, it means it belongs to f2 (toReturn)
                    toReturn.Add(pt);
            }
            return toReturn;
        }


        /// <summary>
        /// appplies graham on points
        /// </summary>
        /// <returns></returns>
        public Face Graham()
        {
            // nothing verifies that points are really in the same face, be careful


            // calculation of the new orthogonal coordinate system
            Face f = new Face(this);
            List<Pnt> c = f.CoordinateSystem; // coordinate system of our face

            // calculation of the transformations into this coordinate system
            double alpha = Pnt.Angle(new Pnt(c[0].x, c[0].y, 0), new Pnt(1, 0, 0)); // rotation around z axis on the plan X,Y between x' and x
                                                                                    // new Vector3(c[0].x, c[0].y,0) is the x vector with its z coordinates put to 0
            f = TransformToNewCoordinateSystem(f, 0, 0, -alpha);
            c = f.CoordinateSystem;

            double beta = Pnt.Angle(new Pnt(0, c[2].y, c[2].z), new Pnt(0, 0, 1)); // rotation around the new x axis on the plan Y',Z'=Z  between z'' and z'=z
                                                                                   // new Vector3(0, c[2].y, c[2].z) is the z vector with its x coordinate put to 0
            f = TransformToNewCoordinateSystem(f, -beta, 0, 0);
            c = f.CoordinateSystem;


            double gamma = Pnt.Angle(new Pnt(c[0].x, 0, c[0].z), new Pnt(1, 0, 0)); // rotation around new y axis on the plan x,z between x'' and x'
            f = TransformToNewCoordinateSystem(f, 0, -gamma, 0);
            // http://ads.harvard.edu/books/1989fcm..book/Chapter2.pdf page 27 definitions of Goldstein

            //new Face(new List<Pnt>() { f.pts[0], f.pts[1], f.pts[2] }).Normale.Log();
            //new Face(new List<Pnt>() { f.pts[3], f.pts[1], f.pts[2] }).Normale.Log();
            // we get the same results, which implies that they are on the same plan, which is kind of rassurant

            // calculation of the superior hull (at this point it's like if points were in 2D )
            List<int> ordonnedIndices = new List<int> { 0 }; // we decide that the first point will be also the first indice
            bool gotMyFirstEdge = false;
            //while (ordonnedIndices.Count < pts.Count) // as long as our indices are not all added... NONONO it must stop when we get back to our first point
            while (ordonnedIndices[0] != ordonnedIndices[ordonnedIndices.Count-1] || ordonnedIndices.Count==1)
            {

                // we must get the first segment ( i mean the second point )
                if (!gotMyFirstEdge)
                {
                    int i = 1;
                    Edge e1 = new Edge(f.pts[0], f.pts[i]);
                    while (e1.Length <= 0)
                    {
                        if (i == f.pts.Count - 1) // we could not find any valid edge (Length>0)
                            return null;
                        i++;
                        e1.Replace(f.pts[0], f.pts[i]);
                    }
                    int indice = i;
                    //we got our edge that has a valid length

                    // we got the first edge, but is this one good ? i mean: is it a border or is it crossing our face ?
                    Edge e2 = new Edge();
                    for (int j = i+1; j < f.pts.Count; j++)
                    {
                        e2.Replace(f.pts[0], f.pts[j]);
                        if (e2.Length > 0)
                        {
                            double d = e1.Angle(e2, f.Normale);
                            // we got a valid edge, let's see if the angle is a good one to keep 
                            if (e1.Angle(e2, f.Normale) > 0)
                            { // if it turns right (or maybe left, but let's imagine it is right)
                                e1 = new Edge(e2); // we update e1, we found a better edge
                                indice = j;
                            }
                        }
                    }

                    // we got our edge, did we ?
                    if (e1.Length > 0)
                    {
                        // yes we did !! it's the most on the right (in regards of pts[0])
                        gotMyFirstEdge = true;
                        ordonnedIndices.Add(indice);
                    }

                }
                else // now that we got our first edge we must add all others 
                {
                    // we update our latest edge
                    Edge e1 = new Edge(f.pts[ordonnedIndices[ordonnedIndices.Count - 1]], f.pts[ordonnedIndices[ordonnedIndices.Count - 2]]);
                    double angle = 0;
                    int indice = 0;
                    // we find the new best match
                    for (int j = 0; j < f.pts.Count; j++)
                    {

                        Edge e2 = new Edge(f.pts[ordonnedIndices[ordonnedIndices.Count - 1]], f.pts[j]);
                        //Edge e2= new Edge(f.pts[1], f.pts[j]);
                        if (e2.Length > 0)
                        {
                            // we got a valid edge, let's see if the angle is a good one to keep 
                            if (e1.Angle(e2, f.Normale) > angle && !e1.SameDirectionAndPoints(e2))
                            { // if it turns right (or maybe left, but let's imagine it is right)
                                indice = j; // we update indice, we found a better edge
                                angle = e1.Angle(e2, f.Normale);
                            } // TODO maybe we need to modify the condition on the angle depending on the orientation of the face ?
                        }

                    }

                    // i hope u checked it was really a face otherwise u may get a weird result
                    ordonnedIndices.Add(indice);
                }


            }

            List<Pnt> ordonnedPoints = new List<Pnt>();
            foreach (int indice in ordonnedIndices)
            {
                ordonnedPoints.Add(pts[indice]); // we add all our points in the right order
            }
            Face g = new Face(ordonnedPoints.GetRange(0, ordonnedPoints.Count-1)); // last point is the first point, we dont need it
            if (!this.SameOrder(ordonnedPoints))
            {// not same order
                if (this.orientation == TopAbs_Orientation.TopAbs_FORWARD)
                    g.orientation = TopAbs_Orientation.TopAbs_REVERSED;
                else
                    g.orientation = TopAbs_Orientation.TopAbs_FORWARD;
            }

            return g;
        }

        /// <summary>
        /// verifies if points are ordonned the same way
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public bool SameOrder(List<Pnt> points)
        {
            if (points.Count != pts.Count)
                return false;
            for(int i=0; i<points.Count; i++)
            {
                if (!points[i].Equals(pts[i]))
                    return false;
            }
            return true;

        }

        /// <summary>
        /// get unique vertices
        /// </summary>
        public void UniqueVertices()
        {

            Face uv = new Face();
            foreach (Pnt pt in this.pts)
            {
                if (!uv.Includes(pt))
                {
                    uv.Add(pt);
                }
            }
            
            pts = uv.pts;
        }

        /// <summary>
        /// checks if pts includes the Pnt pt, returns true in that case, false otherwise
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public bool Includes(Pnt pt)
        {
            foreach( Pnt p in this.pts)
            {
                if (pt.Equals(p))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// get the index of the vertex
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public int Index(Pnt pt)
        {
            for (int i=0; i< this.pts.Count; i++)
            {
                if (pts[i].Equals(pt))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// transforms to the new coordinate system with the 3 angles given
        /// </summary>
        /// <param name="face"></param>
        /// <param name="alpha">in degree</param>
        /// <param name="beta">in degree</param>
        /// <param name="gamma">in degree</param>
        /// <returns></returns>
        private Face TransformToNewCoordinateSystem(Face face, double alpha, double beta, double gamma)
        {

            Face f = new Face();
            foreach (Pnt pt in face.pts)
            {
                // first rotation
                Pnt temp = Rotate(pt, alpha, "x");

                // second rotation
                temp = Rotate(temp, beta, "y");

                // third rotation
                temp = Rotate(temp, gamma, "z");

                // add it to the new face
                f.Add(temp);
            }


            return f;
        }


        /// <summary>
        /// rotate the face with the given angle, around the axis
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="alpha">in degree</param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public Pnt Rotate(Pnt pt, double alpha, string axis)
        {
            alpha = alpha * 2 * Math.PI / 360;
            // results in here are a bit too much approximated
            // it's even wrong as fk
            if (axis == "z")
            {
                double a = pt.x * Math.Cos(alpha) - pt.y * Math.Sin(alpha);
                double b = pt.x * Math.Sin(alpha) + pt.y * Math.Cos(alpha);
                double c = pt.z;
                return new Pnt(a, b, c);
            }
            else if (axis == "y")
            {
                double a = pt.x * Math.Cos(alpha) + pt.z * Math.Sin(alpha);
                double b = pt.y;
                double c = -pt.x * Math.Sin(alpha) + pt.z * Math.Cos(alpha); ;
                return new Pnt(a, b, c);

            }
            else if (axis == "x")
            {
                double a = pt.x;
                double b = pt.y * Math.Cos(alpha) - pt.z * Math.Sin(alpha);
                double c = pt.y * Math.Sin(alpha) + pt.z * Math.Cos(alpha);
                return new Pnt(a, b, c);

            }
            //Debug.Log("The Vector3 returned by Rotate() is wrong, u gave a wrong axis name: please use \"x\", \"y\" ou \"z\"");
            return new Pnt(0, 0, 0);
        }
    }
}
