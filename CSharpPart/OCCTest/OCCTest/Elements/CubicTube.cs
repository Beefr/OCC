using gp;
using System;
using System.Collections.Generic;
using System.Text;
using TopAbs;

namespace OCCTest.Elements
{
    public class CubicTube: BasicElement
    {
        

        /// <summary>
        /// generate a tube with a cubic shape
        /// </summary>
        /// <param name="myWidth">width</param>
        /// <param name="myHeight">height</param>
        /// <param name="myThickness">thickness</param>
        /// <param name="myLength">length</param>
        public CubicTube(double myWidth, double myHeight, double myThickness, double myLength)
        {// note that u could have achieved the same by creating a wire, then make it slide on a spline to create a pipe, then cut external and internal shapes and finally triangulate it (example in the elbow file)

            // inferior part
            // base part ext
            gp_Pnt aPnt11 = new gp_Pnt(0, 0, 0);
            gp_Pnt aPnt12 = new gp_Pnt(myWidth, 0, 0);
            gp_Pnt aPnt13 = new gp_Pnt(myWidth, myHeight, 0);
            gp_Pnt aPnt14 = new gp_Pnt(0, myHeight, 0);

            // BASE PART INT
            gp_Pnt aPnt15 = new gp_Pnt(  0 + myThickness, 		0 + myThickness, 		0);
            gp_Pnt aPnt16 = new gp_Pnt(myWidth-myThickness, 	0 + myThickness, 		0);
            gp_Pnt aPnt17 = new gp_Pnt(myWidth-myThickness, 	myHeight - myThickness, 	0);
            gp_Pnt aPnt18 = new gp_Pnt(  0 + myThickness, 		myHeight - myThickness, 	0);



            // base part ext
            gp_Pnt aPnt21 = new gp_Pnt(0, 0, myLength);
            gp_Pnt aPnt22 = new gp_Pnt(myWidth, 0, myLength);
            gp_Pnt aPnt23 = new gp_Pnt(myWidth, myHeight, myLength);
            gp_Pnt aPnt24 = new gp_Pnt(0, myHeight, myLength);

            // BASE PART INT
            gp_Pnt aPnt25 = new gp_Pnt(0 + myThickness, 0 + myThickness, myLength);
            gp_Pnt aPnt26 = new gp_Pnt(myWidth - myThickness, 0 + myThickness, myLength);
            gp_Pnt aPnt27 = new gp_Pnt(myWidth - myThickness, myHeight - myThickness, myLength);
            gp_Pnt aPnt28 = new gp_Pnt(0 + myThickness, myHeight - myThickness, myLength);

            List<List<gp_Pnt>> faces = new List<List<gp_Pnt>> { new List<gp_Pnt> { aPnt11, aPnt12, aPnt13, aPnt14 }, new List<gp_Pnt> { aPnt15, aPnt16, aPnt17, aPnt18 }, new List<gp_Pnt> { aPnt21, aPnt22, aPnt23, aPnt24 }, new List<gp_Pnt> { aPnt25, aPnt26, aPnt27, aPnt28 } };

            // sadly u must know how to orientate faces, the algorithm can't determine it by itself for now
            List<TopAbs_Orientation> orientations = new List<TopAbs_Orientation> { TopAbs_Orientation.TopAbs_REVERSED, TopAbs_Orientation.TopAbs_FORWARD };


            List<Face> tempFaces = new List<Face>();

           

            // top face
            List<gp_Pnt> allPoints = faces[0];
            allPoints.AddRange(faces[1]);
            Surface surface = new Surface(allPoints, orientations[1]);
            tempFaces.AddRange(surface.ComputeFaces());


            // bot face
            allPoints = faces[2];
            allPoints.AddRange(faces[3]);
            surface = new Surface(allPoints, orientations[0]);
            tempFaces.AddRange(surface.ComputeFaces());


            // lateral face exterior
            allPoints = faces[0];
            allPoints.AddRange(faces[2]);
            surface = new Surface(allPoints, orientations[1]);
            tempFaces.AddRange(surface.ComputeFaces());

            // lateral face interior
            allPoints = faces[1];
            allPoints.AddRange(faces[3]);
            surface = new Surface(allPoints, orientations[0]);
            tempFaces.AddRange(surface.ComputeFaces());

            // ordonned pts + triangles
            int count = 0;
            foreach (Face f in tempFaces)
            {
                myFaces.AddRange(f.ToTrianglesGraham(true).subFaces);
                count++;
            }

        }
    }
}
