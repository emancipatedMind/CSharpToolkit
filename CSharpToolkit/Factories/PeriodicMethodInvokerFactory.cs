namespace CSharpToolkit.Factories {
    using System;
    using EventArgs;
    using Algorithms;

    class PeriodicMethodInvokerFactory {

        readonly RunsLessThanSeed _runsLessThanSeed = new RunsLessThanSeed();
        readonly SeedLessThanRuns _seedLessThanRuns = new SeedLessThanRuns();

        public PeriodicMethodInvoker GetPeriodicMethodInvoker(double seed, double runsDesired) {
            PeriodicMethodInvoker _product;
            if (seed < runsDesired) {
                _product = _seedLessThanRuns;
            }
            else {
                _product = _runsLessThanSeed;
            }
            _product.Seed = seed;
            _product.RunsDesired = runsDesired;
            _product.Reset();
            return _product;
        } 

        public EventHandler<ActionEventArgs> SetAction {
            set {
                _runsLessThanSeed.Action += value;
                _seedLessThanRuns.Action += value;
            }
        }
    }
}