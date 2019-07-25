using BOPAlgo;
using BRep;
using BRepBuilderAPI;
using BRepPrimAPI;
using GC;
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
using TopTools;

namespace OCCTest.Elements
{
    /// <summary>
    /// it is supposed to be the dished cover but i didn't have any drawings of the element so i improvised something to give u an example on how u should proceed
    /// </summary>
    public class ConvexTankEnd: BasicElement
    {
        /// <summary>
        ///  generate a sort of bowl composed with a cylindrical base ( basically a cylinder) and a curved part
        /// </summary>
        /// <param name="totalHeight">total height of the bowl</param>
        /// <param name="thickness">thickness</param>
        /// <param name="heightCylindricalBase">height of the cylinder</param>
        /// <param name="radius1">we need two radiuses to define the curved part</param>
        /// <param name="radius2">we need two radiuses to define the curved part</param>
        public ConvexTankEnd(double totalHeight, double thickness, double heightCylindricalBase, double radius1, double radius2)
        {
            //  can't work if P == O
            if (radius1 == radius2)
                return;

            // _____________curved part_____________
            // on va définir un certain nombre de points
            // certains avec des intersections de cercles, donc va y avoir un peu de calculs...
            gp_Pnt A = new gp_Pnt(thickness, 0, 0); // point à droite de la base
            gp_Pnt B = new gp_Pnt(0, 0, 0); // point à gauche de la base
            gp_Pnt C = new gp_Pnt(0, heightCylindricalBase, 0); // debut de l'arc exterieur
            gp_Pnt H = new gp_Pnt(thickness, heightCylindricalBase, 0); // debut de l'arc intérieur
            gp_Pnt P = new gp_Pnt(thickness + radius1, heightCylindricalBase, 0); // centre du petit cercle formant l'arc
            gp_Pnt O = new gp_Pnt(thickness + radius2, heightCylindricalBase, 0); // centre du grand cercle formant le capot
            gp_Pnt a1 = new gp_Pnt(radius1 * Math.Cos(3*Math.PI / 4) + thickness + radius1, radius1* Math.Sin(3*Math.PI / 4)+ heightCylindricalBase, 0); // point de l'arc de cercle intérieur 
            gp_Pnt a2 = new gp_Pnt((radius1 + thickness) * Math.Cos(3*Math.PI / 4) + thickness + radius1, (radius1 + thickness) * Math.Sin(3*Math.PI / 4) + heightCylindricalBase, 0); // point de l'arc de cercle extérieur 
            // pour a3 et a4 c'est compliqué, je crois que je dois choisir un angle au hasard et que potentiellement ça peut tout niquer (si R2 trop petit comparé à R1)
            //gp_Pnt a3 = new gp_Pnt(-radius2 * Math.Cos(3 * Math.PI / 4) + thickness + radius2, radius2* Math.Sin(3 * Math.PI / 4)+ totalHeight - thickness - radius2, 0); // point de l'arc du capot intérieur 
            //gp_Pnt a4 = new gp_Pnt(-(radius2 + thickness) * Math.Cos(3 * Math.PI / 4) + thickness + radius2, (radius2 + thickness) * Math.Sin(3 * Math.PI / 4) + totalHeight - thickness - radius2, 0); // point de l'arc du capot extérieur 
            gp_Pnt E = new gp_Pnt(thickness + radius2, totalHeight, 0); // haut du capot
            gp_Pnt F = new gp_Pnt(thickness + radius2, totalHeight - thickness, 0); // haut du capot mais côté intérieur
            // maintenant il faut définir les intersections des arcs de cercle et du capot
            // soit l'intersection du cercle de rayon myRadius1(R1) et de centre P (que l'on va abréger P(R1) ) avec O(R2)
            // et également (myThickness=T) P(R1+T) avec O(R2+T)


        
            gp_Pnt G = new gp_Pnt(P.X(), radius1 * Math.Sin(Math.PI / 2) + heightCylindricalBase, 0); // point de l'arc de cercle intérieur 
            gp_Pnt D = new gp_Pnt(G.X(), G.Y()+thickness,0); // point de l'arc de cercle extérieur 
            

            // maintenant qu'on a tous nos points faut les relier ;)
            TopoDS_Edge AB = new BRepBuilderAPI_MakeEdge(A, B).Edge();
            TopoDS_Edge BC = new BRepBuilderAPI_MakeEdge(B, C).Edge();
            
            GC_MakeArcOfCircle Ca2D = new GC_MakeArcOfCircle(C, a2, D);
            BRepBuilderAPI_MakeEdge meCa2D = new BRepBuilderAPI_MakeEdge(Ca2D.Value());
            TopoDS_Edge CD = meCa2D.Edge();
            
            TopoDS_Edge DE = new BRepBuilderAPI_MakeEdge(D, E).Edge();
            TopoDS_Edge EF = new BRepBuilderAPI_MakeEdge(E, F).Edge();
            TopoDS_Edge FG = new BRepBuilderAPI_MakeEdge(F, G).Edge();
            
            GC_MakeArcOfCircle Ga1H = new GC_MakeArcOfCircle(G, a1, H);
            BRepBuilderAPI_MakeEdge meGa1H = new BRepBuilderAPI_MakeEdge(Ga1H.Value());
            TopoDS_Edge GH = meGa1H.Edge();
            
            TopoDS_Edge HA = new BRepBuilderAPI_MakeEdge(H, A).Edge();


            // creating the wire
            BRepBuilderAPI_MakeWire aMakeWire = new BRepBuilderAPI_MakeWire(AB, BC, CD, DE);
            TopoDS_Wire aWire = aMakeWire.Wire();
            
            aMakeWire = new BRepBuilderAPI_MakeWire(aWire, EF);
            aWire = aMakeWire.Wire();
            aMakeWire = new BRepBuilderAPI_MakeWire(aWire, FG);
            aWire = aMakeWire.Wire();
            aMakeWire = new BRepBuilderAPI_MakeWire(aWire, GH);
            aWire = aMakeWire.Wire();
            aMakeWire = new BRepBuilderAPI_MakeWire(aWire, HA);
            aWire = aMakeWire.Wire();//*/
            
           
            // rotation du wire
            gp_Ax1 Axis = new gp_Ax1(new gp_Pnt(thickness + radius2, 0,0), new gp_Dir(0,1,0)); // origine 0,0,0 avec dir 0,1,0 
            BRepBuilderAPI_MakeFace aMakeFace = new BRepBuilderAPI_MakeFace(aWire);
            TopoDS_Face face = aMakeFace.Face();
            BRepPrimAPI_MakeRevol aMakeRevol = new BRepPrimAPI_MakeRevol(face, Axis,  2*Math.PI);
            aMakeRevol.Build();
            TopoDS_Shape aRotatedShape = aMakeRevol.Shape();
            

            // _____________triangulation_____________
            myFaces = Triangulation(aRotatedShape, 0.007f);
            //*/




        }
    }
}
