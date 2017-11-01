namespace CSharpToolkit.Geometry {
    public struct ThreeDPoint {

        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public ThreeDPoint(double x, double y, double z) : this() {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString() => $"{nameof(X)}:{X};{nameof(Y)}:{Y};{nameof(Z)}:{Z};";
        public override int GetHashCode() => ToString().GetHashCode();
        public override bool Equals(object obj) => obj is ThreeDPoint && obj.GetHashCode() == GetHashCode();

    }
}
