using gp;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OCCTest
{
    public class Pnt 
    {
        // coordinates
        public List<double> d = new List<double>();
        public double x, y, z;

        // constructors
        public Pnt()
        {
            d.Add(0);
            d.Add(0);
            d.Add(0);
            x = 0;
            y = 0;
            z = 0;
        }
        public Pnt(gp_Pnt gp)
        {
            d.Add(gp.X());
            d.Add(gp.Y());
            d.Add(gp.Z());
            x = gp.X();
            y = gp.Y();
            z = gp.Z();
        }
        public Pnt(Pnt p)
        {
            x = p.x;
            y = p.y;
            z = p.z;
            d.Clear();
            d.Add(p.x);
            d.Add(p.y);
            d.Add(p.z);
        }
        public Pnt(double d1, double d2, double d3)
        {
            d.Add(d1);
            d.Add(d2);
            d.Add(d3);
            x = d1;
            y = d2;
            z = d3;
        }
        
        // comparison
        public bool IsEqual(Pnt p2)
        {
            if (this.x == p2.x && this.y == p2.y && this.z == p2.z)
                return true;
            return false;
        }
        
       // operators
        public static Pnt operator +(Pnt p1, Pnt p2)
        {
            return new Pnt(p1.d[0] + p2.d[0], p1.d[1] + p2.d[1], p1.d[2] + p2.d[2]);
        }
        public static Pnt operator -(Pnt p1, Pnt p2)
        {
            return new Pnt(p1.d[0] - p2.d[0], p1.d[1] - p2.d[1], p1.d[2] - p2.d[2]);
        }
        public static Pnt operator /(Pnt p1, int i)
        {
            return new Pnt(p1.d[0] / i, p1.d[1] / i, p1.d[2] / i);
        }
        public static Pnt operator /(Pnt p1, double i)
        {
            return new Pnt(p1.d[0] / i, p1.d[1] / i, p1.d[2] / i);
        }

        /// <summary>
        /// cross product between the two vectors
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Pnt Cross(Pnt p1, Pnt p2)
        {
            return new Pnt(p1.y * p2.z - p1.z * p2.y, p1.z * p2.x - p1.x * p2.z, p1.x * p2.y - p1.y * p2.x);
        }
        /// <summary>
        /// normalization of the vector
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Pnt Normalize(Pnt p)
        {
            double L = Math.Sqrt(p.x * p.x + p.y * p.y + p.z * p.z);
            return L == 0 ? p : p / L;
        }
        /// <summary>
        /// signed angle between two vectors defined by p1 and p2
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="axis">normale of the plane defined by the two vectors</param>
        /// <returns></returns>
        public static double SignedAngle(Pnt p1, Pnt p2, Pnt axis)
        {
            double angle = Math.Acos(DotProduct(Normalize(p1), Normalize(p2)));
            Pnt cross = Cross(p1, p2);
            if (DotProduct(axis, cross) < 0)
            { // Or > 0
                angle = -angle;
            }
            return angle * 360 / (2 * Math.PI); // in degree
        }
        /// <summary>
        /// angle between the two vectors
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double Angle(Pnt p1, Pnt p2)
        {
            double angle = Math.Acos(DotProduct(Normalize(p1), Normalize(p2)));

            return angle * 360 / (2 * Math.PI); // in degree;
        }
        /// <summary>
        /// dot product of the two vectors
        /// </summary>
        /// <param name="e1"></param>
        /// <param name="e2"></param>
        /// <returns></returns>
        public static double DotProduct(Pnt e1, Pnt e2)
        {
            return e1.x * e2.x + e1.y * e2.y + e1.z * e2.z;
        }
        /// <summary>
        /// dot product between this and the vector e2
        /// </summary>
        /// <param name="e2"></param>
        /// <returns></returns>
        public double DotProduct(Pnt e2)
        {
            return x * e2.x + y * e2.y + z * e2.z;
        }
        /// <summary>
        /// returns true if it is null
        /// </summary>
        /// <returns></returns>
        public bool IsNull()
        {
            if (x==0 && y==0 && z==0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// returns the distance between this and pt
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public double Distance(Pnt pt)
        {
            return Math.Sqrt((x - pt.x) * (x - pt.x) + (y - pt.y) * (y - pt.y) + (z - pt.z) * (z - pt.z));
        }

        /// <summary>
        /// rotate this from the angle "angle" around the axis "axis"
        /// </summary>
        /// <param name="angle">in degree</param>
        /// <param name="axis"></param>
        public void Rotated(double angle, Pnt axis)
        {
            angle = angle * Math.PI / 180; // converting to radians

            
            double rx = x * (Math.Cos(angle) + axis.x * axis.x * (1 - Math.Cos(angle))) + y * (axis.x * axis.y * (1 - Math.Cos(angle)) - axis.z * Math.Sin(angle)) + z * (axis.x * axis.z * (1 - Math.Cos(angle)) + axis.y * Math.Sin(angle));
            double ry = x * (axis.y * axis.x * (1 - Math.Cos(angle)) + axis.z * Math.Sin(angle)) + y * (Math.Cos(angle) + axis.y * axis.y * (1 - Math.Cos(angle))) + z * (axis.y * axis.z * (1 - Math.Cos(angle)) - axis.x * Math.Sin(angle));
            double rz = x * (axis.z * axis.x * (1 - Math.Cos(angle) - axis.y * Math.Sin(angle))) + y * (axis.z * axis.y * (1 - Math.Cos(angle)) + axis.x * Math.Sin(angle)) + z * (Math.Cos(angle) + axis.z * axis.z * (1 - Math.Cos(angle)));

            x = rx;
            y = ry;
            z = rz;
            d.Clear();
            d.Add(rx);
            d.Add(ry);
            d.Add(rz);
        }

        /// <summary>
        ///  returns a pnt rotated from angle degrees around the axis
        /// </summary>
        /// <param name="angle">in defree</param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public Pnt Rotate(double angle, Pnt axis)
        {
            Pnt p = this;
            p.Rotated(angle, axis);
            return p;
        }

    
        public gp_Dir ToDir()
        {
            return new gp_Dir(x, y, z);
        }

        public gp_Pnt ToGp()
        {
            return new gp_Pnt(x, y, z);
        }

    }
}
