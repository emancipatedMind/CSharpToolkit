namespace CSharpToolkit.Geometry {
    using System;
    public class ThreeDLineSegment {

        public static ThreeDLineSegment FromThreeDPoints(ThreeDPoint point2, ThreeDPoint point1) =>
            CalculateFromCartesianCoordinates(point2.X, point2.Y, point2.Z, point1.X, point1.Y, point1.Z);

        public static ThreeDLineSegment FromThreeDPoints(ThreeDPoint point) =>
            CalculateFromCartesianCoordinates(point.X, point.Y, point.Z, 0, 0, 0);

        public static ThreeDLineSegment FromCartesianCoordinates(double x2, double y2, double z2, double x1, double y1, double z1) =>
            CalculateFromCartesianCoordinates(x2, y2, z2, x1, y1, z1);

        public static ThreeDLineSegment FromCartesianCoordinates(double x, double y, double z) =>
            CalculateFromCartesianCoordinates(x, y, z, 0, 0, 0);

        public static ThreeDLineSegment FromSphericalCoordinates(double length2, double theta2, double phi2, double length1, double theta1, double phi1) =>
            CalculateFromSphericalCoordinates(length2, theta2, phi2, length1, theta1, phi1);

        public static ThreeDLineSegment FromSphericalCoordinates(double length, double theta, double phi) =>
            CalculateFromSphericalCoordinates(length, theta, phi, 0, 0, 0);

        private static ThreeDLineSegment CalculateFromSphericalCoordinates(double length2, double theta2, double phi2, double length1, double theta1, double phi1) =>
            CalculateFromCartesianCoordinates(
                length2 * Math.Cos(theta2) * Math.Sin(phi2),
                length2 * Math.Sin(theta2) * Math.Sin(phi2),
                length2 * Math.Cos(phi2),
                length1 * Math.Cos(theta1) * Math.Sin(phi1),
                length1 * Math.Sin(theta1) * Math.Sin(phi1),
                length1 * Math.Cos(phi1)
            );

        private static ThreeDLineSegment CalculateFromCartesianCoordinates(double x2, double y2, double z2, double x1, double y1, double z1) {
            double length = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2) + Math.Pow(z2 - z1, 2));
            var xyLine = TwoDLineSegment.FromCartesianCoordinates(x2, y2, x1, y1);
            var xyzLine = TwoDLineSegment.FromCartesianCoordinates(z2, xyLine.Length, z1, 0);
            return new ThreeDLineSegment(
                x1,
                y1,
                z1,
                x2,
                y2,
                z2,
                length,
                xyLine.Theta,
                xyzLine.Theta
                );
        }

        public ThreeDLineSegment(
            double x1,
            double y1,
            double z1,
            double x2,
            double y2,
            double z2,
            double length,
            double theta,
            double phi
        ) {
            X1 = x1;
            Y1 = y1;
            Z1 = z1;
            X2 = x2;
            Y2 = y2;
            Z2 = z2;
            Length = length;
            Theta = theta;
            Phi = phi;
        }

        public double X1 { get; }
        public double Y1 { get; }
        public double Z1 { get; }
        public double X2 { get; }
        public double Y2 { get; }
        public double Z2 { get; }
        public double Length { get; }
        public double Theta { get; }
        public double Phi { get; }

    }
}
