using BRep;
using Geom;
using GeomAdaptor;
using gp;
using Standard;
using System;
using System.Collections.Generic;
using System.Text;
using TopAbs;
using TopExp;
using TopoDS;

namespace OCCTest
{
    public static class ToVertices
    {


        public static List<List<gp_Pnt>> toVertices(TopoDS_Shape myBody)
        {
            
            List<List<gp_Pnt>> myPoints = new List<List<gp_Pnt>>();

            
            
            // we explore each face
            for (TopExp_Explorer aFaceExplorer = new TopExp_Explorer(myBody, TopAbs_ShapeEnum.TopAbs_FACE); aFaceExplorer.More(); aFaceExplorer.Next())
            {
                TopoDS_Face face = TopoDS.TopoDS.ToFace(aFaceExplorer.Current());
               
                List<gp_Pnt> tempList = new List<gp_Pnt>();
                // in each face we extract the vertices
                for (TopExp_Explorer exp = new TopExp_Explorer(face.Oriented(TopAbs_Orientation.TopAbs_FORWARD), TopAbs_ShapeEnum.TopAbs_VERTEX); exp.More(); exp.Next())
                {

                    TopoDS_Vertex vertex = TopoDS.TopoDS.ToVertex(exp.Current());
                    gp_Pnt pt = BRep_Tool.Pnt(vertex);
                    tempList.Add(pt);

                }
                myPoints.Add(tempList);
                

            }//*/
            

            return myPoints;
        }

        public static List<string> TypeOfSurfaces(TopoDS_Shape myBody)
        {
            List<string> myStrings = new List<string>();

            for (TopExp_Explorer aFaceExplorer = new TopExp_Explorer(myBody, TopAbs_ShapeEnum.TopAbs_FACE); aFaceExplorer.More(); aFaceExplorer.Next())
            {

                //TopoDS_Face aFace = TopoDS::Face(aFaceExplorer.Current());
                TopoDS_Face aFace = new TopoDS_Face();
                //aFace = (TopoDS_Face)aFaceExplorer.Current();
                aFace = TopoDS.TopoDS.ToFace(aFaceExplorer.Current());


                // Check if <aFace> is the top face of the bottle's neck 
                //Handle(Geom_Surface) aSurface = BRep_Tool::Surface(aFace);
                Geom_Surface aSurface = BRep_Tool.Surface(aFace);

                GeomAdaptor_Surface adap = new GeomAdaptor_Surface(aSurface);


                myStrings.Add(aSurface.GetType().ToString());
                myStrings.Add(Geom_Plane.get_type_name());




            }

            return myStrings;
        }
        

        public static List<TopAbs_Orientation> GetOrientation(TopoDS_Shape myBody)
        {
            List<TopAbs_Orientation> or = new List<TopAbs_Orientation>();

            

            // we explore each face
            for (TopExp_Explorer aFaceExplorer = new TopExp_Explorer(myBody, TopAbs_ShapeEnum.TopAbs_FACE); aFaceExplorer.More(); aFaceExplorer.Next())
            {
                // in each face we get all the wires
                TopoDS_Face face = TopoDS.TopoDS.ToFace(aFaceExplorer.Current());
                or.Add(face.Orientation());
                


            }//*/


            return or;
        }

    }    
}