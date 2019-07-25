using BOPAlgo;
using BRepPrimAPI;
using gp;
using Poly;
using System;
using System.Collections.Generic;
using System.Text;
using TColgp;
using TopAbs;
using TopExp;
using TopoDS;
using TopTools;

namespace OCCTest.Elements
{
    public class HollowedCylinderPiercedWithHollowedCylinder: BasicElement
    {
        /// <summary>
        /// create a hollowed cylinder that is pierced with another cylinder
        /// </summary>
        /// <param name="myRadius">radius of the big cylinder</param>
        /// <param name="myHeight">height of the big cylinder</param>
        /// <param name="myThickness">thickness of the big cylinder</param>
        /// <param name="myRadius2">radius of the small cylinder</param>
        /// <param name="myHeight2">height of the small cylinder</param>
        /// <param name="myThickness2">thickness of the small cylinder</param>
        /// <param name="pierceHeight">height on which we pierce</param>
        public HollowedCylinderPiercedWithHollowedCylinder(double myRadius, double myHeight, double myThickness, double myRadius2, double myHeight2, double myThickness2, double pierceHeight)
        {
            //  _______first cylinder (the big one) _______
            BRepPrimAPI_MakeCylinder aMakeCylinder = new BRepPrimAPI_MakeCylinder(new gp_Ax2(new gp_Pnt(0, 0, 0), new gp_Dir(0, 0, 1)), myRadius, myHeight);
            TopoDS_Shape myBody = aMakeCylinder.Shape();
            TopoDS_Solid mySolid = aMakeCylinder.Solid();


            // inner cylinder of the bigger cylinder to be hollowed
            BRepPrimAPI_MakeCylinder aMakeHollowedPart = new BRepPrimAPI_MakeCylinder(new gp_Ax2(new gp_Pnt(0, 0, 0), new gp_Dir(0, 0, 1)), myRadius - myThickness, myHeight);
            TopoDS_Shape hollowedPart = aMakeHollowedPart.Shape();

            // hollowing the bigger cylinder
            BOPAlgo_BOP test = new BOPAlgo_BOP();
            test.AddArgument(mySolid);
            TopTools_ListOfShape LS = new TopTools_ListOfShape();
            LS.Append(hollowedPart);
            test.SetTools(LS);
            test.SetRunParallel(true);
            test.SetOperation(BOPAlgo_Operation.BOPAlgo_CUT);
            test.Perform();
            myBody = test.Shape();


            // _______second cylinder (the smaller one) _______
            BRepPrimAPI_MakeCylinder aMakeCylinder2 = new BRepPrimAPI_MakeCylinder(new gp_Ax2(new gp_Pnt(0,-myThickness+  Math.Sqrt(myRadius* myRadius- myRadius2* myRadius2), pierceHeight), new gp_Dir(0, 1, 0)), myRadius2, myHeight2);
            TopoDS_Shape myBody2 = aMakeCylinder2.Shape();
            TopoDS_Solid mySolid2 = aMakeCylinder2.Solid();


            // inner cylinder of the smaller cylinder to be hollowed
            BRepPrimAPI_MakeCylinder aMakeHollowedPart2 = new BRepPrimAPI_MakeCylinder(new gp_Ax2(new gp_Pnt(0, -myThickness + Math.Sqrt(myRadius * myRadius - myRadius2 * myRadius2), pierceHeight), new gp_Dir(0, 1, 0)), myRadius2 - myThickness2, myHeight2);
            TopoDS_Shape hollowedPart2 = aMakeHollowedPart2.Shape();


            // smaller cylinder hollowed
            BOPAlgo_BOP test2 = new BOPAlgo_BOP();
            test2.AddArgument(mySolid2);
            TopTools_ListOfShape LS2 = new TopTools_ListOfShape();
            LS2.Append(hollowedPart2);
            test2.SetTools(LS2);
            test2.SetRunParallel(true);
            test2.SetOperation(BOPAlgo_Operation.BOPAlgo_CUT);
            test2.Perform();
            TopoDS_Shape hollowedSmall = test2.Shape();
            
            // piercing
            BOPAlgo_BOP piercer = new BOPAlgo_BOP();
            piercer.AddArgument(myBody);
            TopTools_ListOfShape LSP = new TopTools_ListOfShape();
            LSP.Append(hollowedPart2);
            piercer.SetTools(LSP);
            piercer.SetRunParallel(true);
            piercer.SetOperation(BOPAlgo_Operation.BOPAlgo_CUT);
            piercer.Perform();
            myBody = piercer.Shape();

            
            // adding the tube
            BOPAlgo_BOP adder = new BOPAlgo_BOP();
            adder.AddArgument(myBody);
            TopTools_ListOfShape LSA = new TopTools_ListOfShape();
            LSA.Append(hollowedSmall);
            adder.SetTools(LSA);
            adder.SetRunParallel(true);
            adder.SetOperation(BOPAlgo_Operation.BOPAlgo_FUSE);
            adder.Perform();
            myBody = adder.Shape();//*/
                                  
            

            // _______triangulation_______
            myFaces = Triangulation(myBody, 0.007f);

        }
    }
}
