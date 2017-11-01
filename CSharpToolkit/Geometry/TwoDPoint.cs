namespace CSharpToolkit.Geometry {
    public struct TwoDPoint {

        public double X { get; private set; }
        public double Y { get; private set; }

        public TwoDPoint(double x, double y) : this() {
            X = x;
            Y = y;
        }

        public override string ToString() => $"{nameof(X)}:{X};{nameof(Y)}:{Y};";
        public override int GetHashCode() => ToString().GetHashCode();
        public override bool Equals(object obj) => obj is TwoDPoint && obj.GetHashCode() == GetHashCode();

    }
}
