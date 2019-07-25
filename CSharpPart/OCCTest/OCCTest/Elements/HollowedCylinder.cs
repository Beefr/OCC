using TopoDS;
using BRep;
using BRepBuilderAPI;
using Geom;
using gp;
using System;
using System.Collections.Generic;
using System.Text;
using TopExp;
using GC;
using BRepPrimAPI;
using TopAbs;
using TopTools;
using BRepOffsetAPI;
using BRepPrim;
using STEPControl;
using IFSelect;
using AIS;
using BOPAlgo;
using BRepAlgoAPI;
using ShapeFix;
using OCCTest.Elements;

namespace OCCTest.Elements
{
    public class HollowedCylinder: BasicElement
    {


        /// <summary>
        /// generate a hollowed cylinder
        /// </summary>
        /// <param name="myDiameter">diameter</param>
        /// <param name="myLength">length</param>
        /// <param name="myThickness">thickness</param>
        public HollowedCylinder(double myDiameter, double myLength, double myThickness)
        {
            // external part
            BRepPrimAPI_MakeCylinder aMakeCylinder = new BRepPrimAPI_MakeCylinder(new gp_Ax2(new gp_Pnt(0, 0, 0), new gp_Dir(0, 0, 1)), myDiameter/2, myLength);
            TopoDS_Shape myBody = aMakeCylinder.Shape();
            
            // internal part
            BRepPrimAPI_MakeCylinder aMakeHollowedPart = new BRepPrimAPI_MakeCylinder(new gp_Ax2(new gp_Pnt(0, 0, 0), new gp_Dir(0, 0, 1)), myDiameter/2 - myThickness, myLength);
            TopoDS_Shape hollowedPart = aMakeHollowedPart.Shape();

            // cut
            BOPAlgo_BOP test = new BOPAlgo_BOP();
            test.AddArgument(myBody);
            TopTools_ListOfShape LS = new TopTools_ListOfShape();
            LS.Append(hollowedPart);
            test.SetTools(LS);
            test.SetRunParallel(true);
            test.SetOperation(BOPAlgo_Operation.BOPAlgo_CUT);
            test.Perform();
            myBody = test.Shape();
           


            // ______________ triangulation ______________
            myFaces = Triangulation(myBody, 0.7f);
        }


    }
}
