using BOPAlgo;
using BRepBuilderAPI;
using BRepOffsetAPI;
using BRepPrimAPI;
using Geom;
using GeomAPI;
using GeomPlate;
using gp;
using System;
using System.Collections.Generic;
using System.Text;
using TColgp;
using TColStd;
using TopoDS;
using TopTools;

namespace OCCTest.Elements
{
    /// <summary>
    /// coude: BasicElement
    /// </summary>
    public class Elbow: BasicElement
    {
        /// <summary>
        /// put in MyFaces all the faces that we compute. Note that L1 doesn't exist in BaseElement but i don't figure out on how we can modelize it without L1, there also could be a L2
        /// </summary>
        /// <param name="L1">length between the origin and the most right part of the tube ( same for the top)</param>
        /// <param name="bendingRadius">radius of the pipe</param>
        /// <param name="wallThickness">thickness of the pipe</param>
        /// <param name="bendingAngle"> angle from 0*pi/180 to bendingAngle*pi/180</param>
        /// <param name="n">parameter that modifies the number of triangles</param>
        public Elbow(double L1, double bendingRadius, double wallThickness, double bendingAngle, double n)
        {

            // generate the POINTS for the spline
            double smallShift = Math.PI / 16;
            TColgp_Array1OfPnt array1 = GenerateSpline(L1, bendingRadius, wallThickness, 0);
            TColgp_Array1OfPnt array2 = GenerateSpline(L1, bendingRadius, wallThickness, smallShift);

            // create the SPLINE with the points
            GeomAPI_PointsToBSpline aSpline1 = new GeomAPI_PointsToBSpline(array1);
            GeomAPI_PointsToBSpline aSpline2 = new GeomAPI_PointsToBSpline(array2);
            Geom_BSplineCurve connectionSpline1 = aSpline1.Curve();
            Geom_BSplineCurve connectionSpline2 = aSpline2.Curve();

            // create EXTERNAL shape with spline
            TopoDS_Shape myBody3 = Build(connectionSpline1, bendingAngle, bendingRadius, n, 0);

            // create INTERNAL shape with spline
            TopoDS_Shape myBody32 = Build(connectionSpline2, bendingAngle, bendingRadius - wallThickness, n, smallShift);//*/
            
            

            
            // ______________ hollowing ______________
            BOPAlgo_BOP cutter = new BOPAlgo_BOP();
            cutter.AddArgument(myBody3);
            TopTools_ListOfShape LSC = new TopTools_ListOfShape();
            LSC.Append(myBody32);
            cutter.SetTools(LSC);
            cutter.SetRunParallel(true);
            cutter.SetOperation(BOPAlgo_Operation.BOPAlgo_CUT);
            cutter.Perform();
            myBody3 = cutter.Shape();//*/




            // ______________ triangulation ______________
            myFaces = Triangulation(myBody3, 0.7f); 

        }

        /// <summary>
        /// build the shape of the elbow (internal or external) by building the sliding face and building the wire with the given spline
        /// </summary>
        /// <param name="connectionSpline"> the spline to slide on</param>
        /// <param name="bendingAngle">the angle of our future elbow</param>
        /// <param name="bendingRadius"> the radius of the elbow</param>
        /// <param name="n">modifies the number of triangles</param>
        /// <param name="shift">makes the shape begin before 0*pi/180 and end after bendingAngle, we use it to hollow correctly the shape (put 0 for the external part, and something like pi/16 for the internal part)</param>
        /// <returns>the shape of the elbow (external or internal part)</returns>
        private TopoDS_Shape Build(Geom_BSplineCurve connectionSpline, double bendingAngle, double bendingRadius, double n, double shift)
        {

            bool firstIteration = true; // check if it is the first iteration
            BRepBuilderAPI_MakeWire aMakeWire = new BRepBuilderAPI_MakeWire(); // initialize our wire 
            gp_Pnt lastPnt = new gp_Pnt(); // initialize our last point

            double angle = bendingAngle * Math.PI / 180; // our angle in radian
            double lp = connectionSpline.LastParameter(); // often 1
            double fp = connectionSpline.FirstParameter(); // often 0
            double percentage = (angle + 2 * shift) / (2 * Math.PI); // percentage of the spline to get ( because our spline goes from 0 to 2pi, but we dont want all)
            double pas = (lp * percentage - fp) / n; // the step for the iteration on the spline
            for (double i = fp; i < lp * percentage ; i = i + pas) // fp already includes the small shift if it got any
            {
                if (firstIteration)
                { // we get our first point
                    lastPnt = connectionSpline.Value(i);
                    firstIteration = false;
                }
                else
                { // and now we add a new edge(last point, current point) on our wire 
                    aMakeWire.Add(new BRepBuilderAPI_MakeEdge(lastPnt, connectionSpline.Value(i)).Edge());
                    lastPnt = connectionSpline.Value(i);
                }
            }

            // create the pipe with the spline and the section
            TopoDS_Wire W = MakeCircularHollowedWire(bendingRadius); // the face to be slided
            BRepOffsetAPI_MakePipeShell piper = new BRepOffsetAPI_MakePipeShell(aMakeWire.Wire()); // initialize with the wire to slide on
            BRepBuilderAPI_TransitionMode Mode = new BRepBuilderAPI_TransitionMode();
            Mode = BRepBuilderAPI_TransitionMode.BRepBuilderAPI_RoundCorner;
            piper.SetTransitionMode(Mode); // to have a curved shape
            piper.Add(W, true, true); // first= true to get a pipe and not something else really weird
            piper.Build(); // create the shape
            piper.MakeSolid();
            return piper.Shape();



        }


        /// <summary>
        /// generate the spline on which we will slide to compute our pipe
        /// </summary>
        /// <param name="L1">length between the origin and the most right part of the tube ( same for the top)</param>
        /// <param name="bendingRadius">radius of the pipe</param>
        /// <param name="wallThickness">thickness of the pipe</param>
        /// <param name="shift"> makes the shape begin before 0*pi/180 and end after bendingAngle, we use it to hollow correctly the shape (put 0 for the external part, and something like pi/16 for the internal part)</param>
        /// <returns>returns a set of points that can be used to generate a spline</returns>
        private TColgp_Array1OfPnt GenerateSpline(double L1, double bendingRadius, double wallThickness, double shift)
        {
            double angle =  2 * Math.PI; // Math.PI*1.5;// 360 * Math.PI / 180;
            int numberOfPoints = 36;
            TColgp_Array1OfPnt array = new TColgp_Array1OfPnt(0, numberOfPoints);
            double temp = L1 - bendingRadius;
            if (shift!=0) // internal part
            {
                double totalAngle = angle + 2*shift; // calculate the total angle
                double pas = totalAngle / numberOfPoints;

                for (int i = 0; i <= numberOfPoints; i++)
                {
                    //Pnt pt = new Pnt((temp - wallThickness) * (float)Math.Cos(i * pas - smallShift), (temp - wallThickness) * (float)Math.Sin(i * pas - smallShift), 0);
                    //array.SetValue(i, pt.ToGp());
                    array.SetValue(i, new gp_Pnt((temp + wallThickness) * (float)Math.Cos(i * pas - shift), (temp + wallThickness) * (float)Math.Sin(i * pas - shift), 0));
                }
            } else // external part
            {
                double totalAngle = angle; // calculate the total angle
                double pas = totalAngle / numberOfPoints;

                for (int i = 0; i <= numberOfPoints; i++)
                {
                    //Pnt pt = new Pnt(temp * (float)Math.Cos(i * pas), temp * (float)Math.Sin(i * pas), 0);
                    //array.SetValue(i, pt.ToGp());
                    array.SetValue(i, new gp_Pnt(temp * (float)Math.Cos(i * pas), temp * (float)Math.Sin(i * pas), 0));
                }
            }

            return array;
        }

        /// <summary>
        /// create the face to be slided
        /// </summary>
        /// <param name="r">the radius of the circular face</param>
        /// <returns>a wire that is our face to be slided</returns>
        private TopoDS_Wire MakeCircularHollowedWire(double r)
        {
            /*gp_Circ2d cir1 = new gp_Circ2d(new gp_Ax2d(new gp_Pnt2d(new gp_XY(0f, 0f)), new gp_Dir2d(1, 0)), r);

            BRepBuilderAPI_MakeEdge2d mee1 = new BRepBuilderAPI_MakeEdge2d(cir1);
            TopoDS_Edge e1 = mee1.Edge();

            BRepBuilderAPI_MakeWire aMakeWire1 = new BRepBuilderAPI_MakeWire(e1);
            TopoDS_Wire W = aMakeWire1.Wire();//*/


            gp_Circ cir1 = new gp_Circ(new gp_Ax2(new gp_Pnt(0, 0, 0), new gp_Dir(1, 0, 0)), r);
            BRepBuilderAPI_MakeEdge aMakeEdge = new BRepBuilderAPI_MakeEdge(cir1);
            BRepBuilderAPI_MakeWire aMakeWire1 = new BRepBuilderAPI_MakeWire(aMakeEdge.Edge());
            TopoDS_Wire W = aMakeWire1.Wire();

            return W;
        }
    }
}
