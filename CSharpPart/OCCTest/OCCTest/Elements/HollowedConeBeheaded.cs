using BOPAlgo;
using BRepBuilderAPI;
using BRepPrimAPI;
using gp;
using System;
using System.Collections.Generic;
using System.Text;
using TopAbs;
using TopoDS;
using TopTools;

namespace OCCTest.Elements
{
    public class HollowedConeBeheaded : BasicElement 
    {

        /// <summary>
        /// create a hollowed cone beheaded
        /// </summary>
        /// <param name="myRadius">radius of the base</param>
        /// <param name="myHeight">height of the cone</param>
        /// <param name="myHeadRadius">radius of the top</param>
        /// <param name="myThickness">thickness</param>
        public HollowedConeBeheaded(double baseDiameter, double myHeight, double headDiameter, double myThickness) {
            
            // external part
            BRepPrimAPI_MakeCone aMakeCone = new BRepPrimAPI_MakeCone(new gp_Ax2(new gp_Pnt(0, 0, 0), new gp_Dir(0, 0, 1)), baseDiameter/2, headDiameter/2, myHeight);
            TopoDS_Shape myBody = aMakeCone.Shape();
            TopoDS_Solid mySolid = aMakeCone.Solid();

            // internal part
            BRepPrimAPI_MakeCone aMakeHollowedPart = new BRepPrimAPI_MakeCone(new gp_Ax2(new gp_Pnt(0, 0, 0), new gp_Dir(0, 0, 1)), baseDiameter /2- myThickness, headDiameter /2- myThickness, myHeight);
            TopoDS_Shape hollowedPart = aMakeHollowedPart.Shape();

            // cut
            BOPAlgo_BOP test = new BOPAlgo_BOP();
            test.AddArgument(mySolid);
            TopTools_ListOfShape LS = new TopTools_ListOfShape();
            LS.Append(hollowedPart);
            test.SetTools(LS);
            test.SetRunParallel(true);
            test.SetOperation(BOPAlgo_Operation.BOPAlgo_CUT);
            test.Perform();
            myBody = test.Shape();

            // triangulation
            myFaces = Triangulation(myBody, 0.7f);
        }
    }
}
