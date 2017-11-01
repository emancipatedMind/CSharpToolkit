namespace CSharpToolkit.Geometry {
    using System;
    public static class AngleConversions {
        public static decimal RadiansToDegrees(decimal radians) => radians * 180m / Convert.ToDecimal(Math.PI);
        public static double RadiansToDegrees(double radians) => radians * 180 / Math.PI;
        public static decimal DegreesToRadians(decimal degrees) => degrees * Convert.ToDecimal(Math.PI) / 180m;
        public static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;

        public static decimal RadiansToGradians(decimal radians) => radians * 200m / Convert.ToDecimal(Math.PI);
        public static double RadiansToGradians(double radians) => radians * 200 / Math.PI;
        public static decimal GradiansToRadians(decimal gradians) => gradians * Convert.ToDecimal(Math.PI) / 200m;
        public static double GradiansToRadians(double gradians) => gradians * Math.PI / 200;

        public static decimal GradiansToDegrees(decimal gradians) => gradians * .9m;
        public static double GradiansToDegrees(double gradians) => gradians * .9;
        public static decimal DegreesToGradians(decimal degrees) => degrees / .9m;
        public static double DegreesToGradians(double degrees) => degrees / .9;
    }
}
