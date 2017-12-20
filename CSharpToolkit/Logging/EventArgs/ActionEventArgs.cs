namespace CSharpToolkit.EventArgs {

    using System;

    public class ActionEventArgs : EventArgs {
        internal ActionEventArgs(double iteration) {
            Iteration = iteration;
        }

        public double Iteration { get; private set; }
    }
}
