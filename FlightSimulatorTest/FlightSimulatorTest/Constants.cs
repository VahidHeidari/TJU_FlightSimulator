using System;

namespace Framework
{
    class SpeedConvertor
    {
        public static float Knot2ft_sec(float Speed_Knot)
        {
            return Speed_Knot * 1.689f;
        }
        public static float Knot2mile_hr(float Speed_Knot)
        {
            return Speed_Knot * 1.51f;
        }
        public static float Knot2Km_hr(float Speed_Knot)
        {
            return Speed_Knot * 1.852f;
        }
        public static float Knot2m_s(float Speed_Knot)
        {
            return Speed_Knot * 0.5144f;
        }
        public static float Ft_sec2m_sec(float Speed_ft_sec)
        {
            return Speed_ft_sec * 0.3048f;
        }
        public static float Ft_sec2Knot(float Speed_ft_sec)
        {
            return Speed_ft_sec * 1.0f / 1.689f;
        }
        public static float Mach_ft_sec(float Speed_ft_sec)
        {
            return SpeedConvertor.Ft_sec2m_sec(Speed_ft_sec) / Constants.am_s;
        }
    }
    class Constants
    {
        public static float g = -32.174f;  // gravity constant
        public static float e = 0.5f;   // coefficient restitution

        public static float DistanceTolerance = 0.1f;
        public static float TimeTolerance = 0.0001f;
        public static float VelocityTolerance = 0.01f;


        public static float AirPressur0N_m2 = 101325;
        public static float AirPressur0psi = 14.7f;
        public static float AirPressur0lb_ft2 = 2116.2f;

        public static float T0kelvin = 288.15f;
        public static float T0celsius = 15;

        public static float AirDencity0kg_m3 = 1.225f;
        public static float AirDencity0slug_ft3 = 0.002378f;

        public static float u0kg_sec_m = 1.783E-5f;

        public static float am_s = 340.4f;
        //public static float aft_sec = 

    }
}
