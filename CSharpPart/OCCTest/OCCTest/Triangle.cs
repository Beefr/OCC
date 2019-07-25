
namespace OCCTest
{

    public class Triangle
    {
        public Pnt p1, p2, p3;

        
        /// <summary>
        /// constructor sets the points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        public Triangle(Pnt p1, Pnt p2, Pnt p3)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }
        
        /// <summary>
        /// get the centroid of the 3 points
        /// </summary>
        public Pnt Centroid
        {
            get
            {
                return (p1 + p2 + p3) / 3;
            }
        }

        /// <summary>
        /// get the normal of the face made of the 3 points
        /// </summary>
        public Pnt Normal
        {
            get
            {
                return Pnt.Normalize(Pnt.Cross(p2 - p1, p3 - p1));
            }
        }
        
    }
}
