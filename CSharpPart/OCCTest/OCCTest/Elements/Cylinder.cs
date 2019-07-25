using BRepPrimAPI;
using gp;
using System;
using System.Collections.Generic;
using System.Text;
using TopAbs;
using TopoDS;

namespace OCCTest.Elements
{
    public class Cylinder : BasicElement
    {
        /// <summary>
        /// generate a cylinder
        /// </summary>
        /// <param name="myDiameter">diameter</param>
        /// <param name="myHeight">height</param>
        public Cylinder(double myDiameter, double myHeight) {
            BRepPrimAPI_MakeCylinder aMakeCylinder = new BRepPrimAPI_MakeCylinder(new gp_Ax2(new gp_Pnt(0, 0, 0), new gp_Dir(0, 0, 1)), myDiameter/2, myHeight);
            TopoDS_Shape myBody = aMakeCylinder.Shape();


            // ______________ triangulation ______________
            myFaces = Triangulation(myBody, 0.7f);

        }


    }
}
