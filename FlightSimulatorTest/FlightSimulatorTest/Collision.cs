using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Framework
{
    public class Collision
    {
        public enum CollisionStates
        {
            None,
            Collision,
            Contact,
            Penetrating
        }

        /// <summary>
        /// Triangle-Triangle Intersection Test
        /// </summary>
        /// <param name="Tri1V0">Triangle1 Vertex0</param>
        /// <param name="Tri1V1">Triangle1 Vertex1</param>
        /// <param name="Tri1V2">Triangle1 Vertex2</param>
        /// <param name="Tri2V0">Triangle2 Vertex0</param>
        /// <param name="Tri2V1">Triangle2 Vertex1</param>
        /// <param name="Tri2V2">Triangle2 Vertex2</param>
        /// <returns></returns>
        public static CollisionStates TriTriTest(Vector3 Tri1V0, Vector3 Tri1V1, Vector3 Tri1V2, Vector3 Tri2V0, Vector3 Tri2V1, Vector3 Tri2V2)
        {
            Vector3 N2 = Vector3.Cross((Tri2V1 - Tri2V0), (Tri2V2 - Tri2V0));
            Plane P2 = new Plane(N2.X, N2.Y, N2.Z, Vector3.Dot(-N2, Tri2V0));
            float d10 = P2.Dot(Tri1V0);
            float d11 = P2.Dot(Tri1V1);
            float d12 = P2.Dot(Tri1V2);
            if (Math.Abs(d10) < Constants.DistanceTolerance) d10 = 0;
            if (Math.Abs(d11) < Constants.DistanceTolerance) d11 = 0;
            if (Math.Abs(d12) < Constants.DistanceTolerance) d12 = 0;
            if ((d10 < 0 && d11 < 0 && d12 < 0) ||
                (d10 > 0 && d11 > 0 && d12 > 0))
                return CollisionStates.None; // same side


            Vector3 N1 = Vector3.Cross((Tri1V1 - Tri1V0), (Tri1V2 - Tri1V0));
            Plane P1 = new Plane(N1.X, N1.Y, N1.Z, Vector3.Dot(-N1, Tri1V0));
            float d20 = P1.Dot(Tri2V0);
            float d21 = P1.Dot(Tri2V1);
            float d22 = P1.Dot(Tri2V2);
            if (Math.Abs(d20) < Constants.DistanceTolerance) d20 = 0;
            if (Math.Abs(d21) < Constants.DistanceTolerance) d21 = 0;
            if (Math.Abs(d22) < Constants.DistanceTolerance) d22 = 0;
            if ((d20 < 0 && d21 < 0 && d22 < 0) ||
                (d20 > 0 && d21 > 0 && d22 > 0))
                return CollisionStates.None; // same side

            // Coplanar Triangles
            if ((d10 == 0 && d11 == 0 && d12 == 0) ||
                (d20 == 0 && d21 == 0 && d22 == 0))
            {
                Vector3 T0V0;
                Vector3 T0V1;
                Vector3 T0V2;
                Vector3 T1V0;
                Vector3 T1V1;
                Vector3 T1V2;
                if(Math.Abs(N1.X) >= Math.Abs(N1.Y) && Math.Abs(N1.X) >= Math.Abs(N1.Z))
                {
                    T0V0 = new Vector3(Tri1V0.Y, Tri1V0.Z, 0);
                    T0V1 = new Vector3(Tri1V1.Y, Tri1V1.Z, 0);
                    T0V2 = new Vector3(Tri1V2.Y, Tri1V2.Z, 0);
                    T1V0 = new Vector3(Tri2V0.Y, Tri2V0.Z, 0);
                    T1V1 = new Vector3(Tri2V1.Y, Tri2V1.Z, 0);
                    T1V2 = new Vector3(Tri2V2.Y, Tri2V2.Z, 0);
                }else
                    if (Math.Abs(N1.Y) >= Math.Abs(N1.X) && Math.Abs(N1.Y) >= Math.Abs(N1.Z))
                    {
                        T0V0 = new Vector3(Tri1V0.X, Tri1V0.Z, 0);
                        T0V1 = new Vector3(Tri1V1.X, Tri1V1.Z, 0);
                        T0V2 = new Vector3(Tri1V2.X, Tri1V2.Z, 0);
                        T1V0 = new Vector3(Tri2V0.X, Tri2V0.Z, 0);
                        T1V1 = new Vector3(Tri2V1.X, Tri2V1.Z, 0);
                        T1V2 = new Vector3(Tri2V2.X, Tri2V2.Z, 0);
                    }
                    else//Z component
                    {
                        T0V0 = new Vector3(Tri1V0.X, Tri1V0.Y, 0);
                        T0V1 = new Vector3(Tri1V1.X, Tri1V1.Y, 0);
                        T0V2 = new Vector3(Tri1V2.X, Tri1V2.Y, 0);
                        T1V0 = new Vector3(Tri2V0.X, Tri2V0.Y, 0);
                        T1V1 = new Vector3(Tri2V1.X, Tri2V1.Y, 0);
                        T1V2 = new Vector3(Tri2V2.X, Tri2V2.Y, 0);
                    }
                return TriTriTest2D(T0V0, T0V1, T0V2, T1V0, T1V1, T1V2);
            }

            Vector3 D = Vector3.Cross(N1, N2);
            float dx = Math.Abs(D.X);
            float dy = Math.Abs(D.Y);
            float dz = Math.Abs(D.Z);

            float p10 = 0;
            float p11 = 0;
            float p12 = 0;
            float p20 = -1;
            float p21 = -1;
            float p22 = -1;

            if (dx >= dy && dx >= dz)
            {
                p10 = Tri1V0.X;
                p11 = Tri1V1.X;
                p12 = Tri1V2.X;
                p20 = Tri2V0.X;
                p21 = Tri2V1.X;
                p22 = Tri2V2.X;
            }
            else
                if (dy >= dx && dy >= dz)
                {
                    p10 = Tri1V0.Y;
                    p11 = Tri1V1.Y;
                    p12 = Tri1V2.Y;
                    p20 = Tri2V0.Y;
                    p21 = Tri2V1.Y;
                    p22 = Tri2V2.Y;
                }
                else
                    if (dz >= dx && dz >= dy)
                    {
                        p10 = Tri1V0.Z;
                        p11 = Tri1V1.Z;
                        p12 = Tri1V2.Z;
                        p20 = Tri2V0.Z;
                        p21 = Tri2V1.Z;
                        p22 = Tri2V2.Z;
                    }
            float t10 = 0;
            float t11 = 0;
            float t20 = -1;
            float t21 = -1;

            if ((d10 >= 0 && d12 >= 0) || (d10 < 0 && d12 < 0))
                Intervals(ref t10, ref t11,
                    p11, p10, p12,
                    d11, d10, d12);
            else
                if ((d11 >= 0 && d12 >= 0) || (d11 < 0 && d12 < 0))
                    Intervals(ref t10, ref t11,
                        p10, p11, p12,
                        d10, d11, d12);
                else
                    if ((d10 >= 0 && d11 >= 0) || (d10 < 0 && d11 < 0))
                        Intervals(ref t10, ref t11,
                            p12, p10, p11,
                            d12, d10, d11);

            if ((d20 >= 0 && d22 >= 0) || (d20 < 0 && d22 < 0))
                Intervals(ref t20, ref t21,
                    p21, p20, p22,
                    d21, d20, d22);
            else
                if ((d21 >= 0 && d22 >= 0) || (d21 < 0 && d22 < 0))
                    Intervals(ref t20, ref t21,
                        p20, p21, p22,
                        d20, d21, d22);
                else
                    if ((d20 >= 0 && d21 >= 0) || (d20 < 0 && d21 < 0))
                        Intervals(ref t20, ref t21,
                            p22, p20, p21,
                            d22, d20, d21);
            if (t10 <= t21 && t11 >= t20)
                return CollisionStates.Penetrating;

            return CollisionStates.None;
        }

        public static CollisionStates TriTriTest2D(Vector3 Tri1V0, Vector3 Tri1V1, Vector3 Tri1V2, Vector3 Tri2V0, Vector3 Tri2V1, Vector3 Tri2V2)
        {
            Vector3 edge, axis;
            float d0 = 0, d1, d2, d3;
            edge = Tri1V1 - Tri1V0; // Axis
            axis = new Vector3(edge.Y, -edge.X, 0);
            edge = Tri2V0 - Tri1V1;
            d1 = Vector3.Dot(axis, edge);
            edge = Tri2V1 - Tri1V1;
            d2 = Vector3.Dot(axis, edge);
            edge = Tri2V2 - Tri1V1;
            d3 = Vector3.Dot(axis, edge);
            if ((d1 > 0 && d2 > 0 && d3 > 0) ||
                (d1 < 0 && d2 < 0 && d3 < 0))
            {
                edge = Tri1V2 - Tri1V1;
                d0 = Vector3.Dot(axis,edge);
                if (!Overlap(d0, d1, d2, d3))
                    return CollisionStates.None;
            }



            edge = Tri1V2 - Tri1V1; // axis
            axis = new Vector3(edge.Y, -edge.X, 0);
            edge = Tri2V0 - Tri1V2;
            d1 = Vector3.Dot(axis, edge);
            edge = Tri2V1 - Tri1V2;
            d2 = Vector3.Dot(axis, edge);
            edge = Tri2V2 - Tri1V2;
            d3 = Vector3.Dot(axis, edge);
            if ((d1 > 0 && d2 > 0 && d3 > 0) ||
                (d1 < 0 && d2 < 0 && d3 < 0))
            {
                edge = Tri1V0 - Tri1V1;
                d0 = Vector3.Dot(axis, edge);
                if (!Overlap(d0, d1, d2, d3))
                    return CollisionStates.None;
            }


            edge = Tri1V0 - Tri1V2; // axis
            axis = new Vector3(edge.Y, -edge.X, 0);
            edge = Tri2V0 - Tri1V0;
            d1 = Vector3.Dot(axis, edge);
            edge = Tri2V1 - Tri1V0;
            d2 = Vector3.Dot(axis, edge);
            edge = Tri2V2 - Tri1V0;
            d3 = Vector3.Dot(axis, edge);
            if ((d1 < 0 && d2 < 0 && d3 < 0) ||
                (d1 > 0 && d2 > 0 && d3 > 0))
            {
                edge = Tri1V2 - Tri1V1;
                d0 = Vector3.Dot(axis, edge);
                if (!Overlap(d0, d1, d2, d3))
                    return CollisionStates.None;
            }

            //////////////////////////////////
            edge = Tri2V1 - Tri2V0;
            axis = new Vector3(edge.Y, -edge.X, 0);
            edge = Tri1V0 - Tri2V1;
            d1 = Vector3.Dot(axis, edge);
            edge = Tri1V1 - Tri2V1;
            d2 = Vector3.Dot(axis, edge);
            edge = Tri1V2 - Tri2V1;
            d3 = Vector3.Dot(axis, edge);
            if ((d1 < 0 && d2 < 0 && d3 < 0) ||
                (d1 > 0 && d2 > 0 && d3 > 0))
            {
                edge = Tri2V2 - Tri2V0;
                d0 = Vector3.Dot(axis, edge);
                if (!Overlap(d0, d1, d2, d3))
                    return CollisionStates.None;
            }


            edge = Tri2V2 - Tri2V1;
            axis = new Vector3(edge.Y, -edge.X, 0);
            edge = Tri1V0 - Tri2V2;
            d1 = Vector3.Dot(axis, edge);
            edge = Tri1V1 - Tri2V2;
            d2 = Vector3.Dot(axis, edge);
            edge = Tri1V2 - Tri2V2;
            d3 = Vector3.Dot(axis, edge);
            if ((d1 < 0 && d2 < 0 && d3 < 0) ||
                (d1 > 0 && d2 > 0 && d3 > 0))
            {
                edge = Tri2V0 - Tri2V1;
                d0 = Vector3.Dot(axis, edge);
                if (!Overlap(d0, d1, d2, d3))
                    return CollisionStates.None;
            }

            edge = Tri2V0 - Tri2V2;
            axis = new Vector3(edge.Y, -edge.X, 0);
            edge = Tri1V0 - Tri2V0;
            d1 = Vector3.Dot(axis, edge);
            edge = Tri1V1 - Tri2V0;
            d2 = Vector3.Dot(axis, edge);
            edge = Tri1V2 - Tri2V0;
            d3 = Vector3.Dot(axis, edge);
            if ((d1 < 0 && d2 < 0 && d3 < 0) ||
                (d1 > 0 && d2 > 0 && d3 > 0))
            {
                edge = Tri2V1 - Tri2V2;
                d0 = Vector3.Dot(axis, edge);
                if (!Overlap(d0, d1, d2, d3))
                    return CollisionStates.None;
            }


            return CollisionStates.Penetrating;
        }

        private static bool Overlap(float d0, float d1, float d2, float d3)
        {
            float min0 = 0;
            float max0 = d0;
            float min1 = d1;
            float max1 = d2;

            if (max0 < min0) Swap(ref max0, ref min0);
            if (max1 < min1) Swap(ref max1, ref min1);
            if (d3 < min1) min1 = d3;
            if (d3 > max1) max1 = d3;

            return (max0 >= min1 && min0 <= max1);
        }

        private static void Intervals(ref float t1, ref float t2, float p1, float p0, float p2,float d1,float d0,float d2)
        {
            t1 = p0 + (p1 - p0) * (d0 / (d0 - d1));
            t2 = p2 + (p1 - p2) * (d2 / (d2 - d1));
            if (t1 > t2)
                Swap(ref t1, ref t2);
        }

        private static void Swap(ref float t10, ref float t11)
        {
            float tmp = t10;
            t10 = t11;
            t11 = tmp;
        }
    }
}
