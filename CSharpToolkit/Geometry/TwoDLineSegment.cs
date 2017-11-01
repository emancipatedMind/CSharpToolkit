namespace CSharpToolkit.Geometry {
    using System;
    public class TwoDLineSegment {

        public static TwoDLineSegment FromTwoDPoints(TwoDPoint point2, TwoDPoint point1) =>
            CalculateFromCartesianCoordinates(point2.X, point2.Y, point1.X, point1.Y);

        public static TwoDLineSegment FromTwoDPoints(TwoDPoint point) =>
            CalculateFromCartesianCoordinates(point.X, point.Y, 0, 0);

        public static TwoDLineSegment FromCartesianCoordinates(double x2, double y2, double x1, double y1) =>
            CalculateFromCartesianCoordinates(x2, y2, x1, y1);

        public static TwoDLineSegment FromCartesianCoordinates(double x, double y) =>
            CalculateFromCartesianCoordinates(x, y, 0, 0);

        public static TwoDLineSegment FromPolarCoordinates(double length2, double theta2, double length1, double theta1) =>
            CalculateFromPolarCoordinates(length2, theta2, length1, theta1);

        public static TwoDLineSegment FromPolarCoordinates(double length, double theta) =>
            CalculateFromPolarCoordinates(length, theta, 0, 0);

        private static TwoDLineSegment CalculateFromPolarCoordinates(double length2, double theta2, double length1, double theta1) =>
            CalculateFromCartesianCoordinates(length2 * Math.Cos(theta2), length2 * Math.Sin(theta2), length1 * Math.Cos(theta1), length1 * Math.Sin(theta1));

        private static TwoDLineSegment CalculateFromCartesianCoordinates(double x2, double y2, double x1, double y1) {
            double deltaY = y2 - y1;
            double deltaX = x2 - x1;

            double slope;
            double yIntercept;
            double theta;
            if (deltaX == 0) {
                slope = double.PositiveInfinity;
                yIntercept = double.PositiveInfinity;
                theta = (deltaY < 0 ? 1.5 * Math.PI : .5 * Math.PI);
            }
            else {
                slope = deltaY / deltaX;
                yIntercept = y2 - (slope * x2);
                theta = Math.Atan(Convert.ToDouble(deltaY / deltaX));
                theta += deltaX < 0 ? Math.PI : (deltaY < 0 ? 2 * Math.PI : 0);
            }

            double length = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            return new TwoDLineSegment(x1, y1, x2, y2, slope, yIntercept, theta, length);
        }

        public TwoDLineSegment(
            double x1,
            double y1,
            double x2,
            double y2,
            double slope,
            double yIntercept,
            double theta,
            double length
        ) {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            Slope = slope;
            YIntercept = yIntercept;
            Theta = theta;
            Length = length;
        }

        public double X1 { get; }
        public double Y1 { get; }
        public double X2 { get; }
        public double Y2 { get; }
        public double Slope { get; }
        public double YIntercept { get; }
        public double Theta { get; }
        public double Length { get; }

    }
}
