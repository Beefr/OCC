using gp;
using System;
using System.Collections.Generic;


namespace OCCTest
{

    public class Edge
    {
        public Pnt v = new Pnt();

        public Pnt p1;
        public Pnt p2;
        public List<Pnt> pts = new List<Pnt>();

        // constructors
        public Edge(List<gp_Pnt> pts)
        {

            this.pts = ToPnt(pts);
            p1 = ToPnt(pts[0]);
            p2 = ToPnt(pts[1]);

            v = ToPnt(pts[1]) - ToPnt(pts[0]);
        }
        public Edge()
        {
        }
        public Edge(gp_Pnt p1, gp_Pnt p2)
        {
            v = ToPnt(p2) - ToPnt(p1);


            this.pts.Clear();
            this.pts.Add(ToPnt(p1));
            this.pts.Add(ToPnt(p2));
            this.p1 = ToPnt(p1);
            this.p2 = ToPnt(p2);
        }
        public Edge(Pnt p1, Pnt p2)
        {
            v = p2 - p1;


            this.pts.Clear();
            this.pts.Add(p1);
            this.pts.Add(p2);
            this.p1 = p1;
            this.p2 = p2;
        }
        public Edge(Edge e)
        {
            v = e.v;
            this.p1 = e.p1;
            this.p2 = e.p2;
            this.pts = e.pts;
        }

        /// <summary>
        /// angle between two edges
        /// </summary>
        /// <param name="e"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public double Angle(Edge e, Pnt axis)
        {
            return this.v.IsEqual(e.v) ? 0 : Pnt.SignedAngle(v, e.v, axis);
        }

        
        /// <summary>
        /// length of the edge
        /// </summary>
        public float Length
        {
            get
            {
                double d = (p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y) + (p1.z - p2.z) * (p1.z - p2.z);
                return (float)Math.Sqrt((float)d);
            }
        }

        /// <summary>
        /// modifies the edge
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public void Replace(Pnt p1, Pnt p2)
        {
            v = p2 - p1;


            this.pts.Clear();
            this.pts.Add(p1);
            this.pts.Add(p2);
            this.p1 = p1;
            this.p2 = p2;
        }
        /// <summary>
        /// modifies the edge
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public void Replace(gp_Pnt p1, gp_Pnt p2)
        {
            v = ToPnt(p2) - ToPnt(p1);


            this.pts.Clear();
            this.pts.Add(ToPnt(p1));
            this.pts.Add(ToPnt(p2));
            this.p1 = ToPnt(p1);
            this.p2 = ToPnt(p2);
        }

        /// <summary>
        /// returns true if the edges equals this
        /// </summary>
        /// <param name="e2"></param>
        /// <returns></returns>
        public bool Equals(Edge e2)
        {
            if (Equals(this.p1, e2.p1) && Equals(this.p2, e2.p2))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// returns true if the points are the same, even though it is not in the same order
        /// </summary>
        /// <param name="e2"></param>
        /// <returns></returns>
        public bool SameDirectionAndPoints(Edge e2)
        {
            if (this.Equals(e2) || this.Equals(e2.Reverse()))
                return true;
            return false;
        }

        /// <summary>
        /// reverse the edge
        /// </summary>
        /// <returns></returns>
        public Edge Reverse()
        {
            return new Edge(this.p2, this.p1);
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


    }
}
