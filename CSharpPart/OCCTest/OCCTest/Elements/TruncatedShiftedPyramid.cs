using BRepBuilderAPI;
using gp;
using System;
using System.Collections.Generic;
using System.Text;
using TopAbs;
using TopoDS;

namespace OCCTest.Elements
{
    public class TruncatedShiftedPyramid: BasicElement
    {
        /// <summary>
        /// create a pyramid beheaded and with a small shift angle
        /// </summary>
        /// <param name="widthBase">width of the base</param>
        /// <param name="widthTop">width of the top</param>
        /// <param name="myHeight">height of the pyramid</param>
        /// <param name="alpha">angle</param>
        public TruncatedShiftedPyramid(double widthBase, double widthTop, double myHeight, double alpha =0)
        {

            // initialisation
            double rad = alpha * 2 * Math.PI / (double)360;
            double e = (widthBase - widthTop) / 2;
            double x = myHeight * Math.Tan(rad); 
            double L = e + x; 
           

            // LEFT PART
            gp_Pnt aPnt11 = new gp_Pnt( widthBase/2,        -widthBase/2,       0);
            gp_Pnt aPnt12 = new gp_Pnt( widthBase / 2 - e,  -widthBase / 2 + L, myHeight);
            gp_Pnt aPnt13 = new gp_Pnt( -widthBase/2+e,     -widthBase/2 + L,   myHeight);
            gp_Pnt aPnt14 = new gp_Pnt( -widthBase / 2,     -widthBase / 2,     0);


            // RIGHT PART
            gp_Pnt aPnt21 = new gp_Pnt( widthBase/2,        widthBase/2,                    0);
            gp_Pnt aPnt22 = new gp_Pnt( widthBase / 2 - e,  -widthBase / 2 + L+ widthTop,   myHeight);
            gp_Pnt aPnt23 = new gp_Pnt( -widthBase/2+e,     -widthBase / 2 + L+ widthTop,   myHeight);
            gp_Pnt aPnt24 = new gp_Pnt( -widthBase / 2,     widthBase / 2,                  0);

            
            //________________________________________________________________

            List<List<gp_Pnt>> faces = new List<List<gp_Pnt>> { new List<gp_Pnt> { aPnt11 , aPnt12 , aPnt13 , aPnt14 }, new List<gp_Pnt> { aPnt21, aPnt22, aPnt23, aPnt24 } };

            // sadly u must know how to orientate faces, the algorithm can't determine it by itself for now
            List <TopAbs_Orientation> orientations = new List<TopAbs_Orientation> { TopAbs_Orientation.TopAbs_REVERSED, TopAbs_Orientation.TopAbs_FORWARD};


            List<Face> tempFaces = new List<Face>();


            Face f1 = new Face(faces[0]);
            f1.orientation = orientations[1];
            tempFaces.Add(f1);

            Face f2 = new Face(faces[1]);
            f2.orientation = orientations[0];
            tempFaces.Add(f2);

            // lateral face
            List<gp_Pnt> allPoints = faces[0];
            allPoints.AddRange(faces[1]);
            Surface surface = new Surface(allPoints, orientations[0]);
            f1 = new Face(surface.f1);
            f2 = new Face(surface.f2);
            tempFaces.AddRange(ComputeFaces(f1, f2, surface.orientation));

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
