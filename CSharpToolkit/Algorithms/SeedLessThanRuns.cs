namespace CSharpToolkit.Algorithms {
    using EventArgs;

    internal class SeedLessThanRuns : PeriodicMethodInvoker {

        protected override void RunAlgorithm() {
            double limit =  _factor * _timesIterated;
            while (limit >= _runs) {
                FireActionEvent(new ActionEventArgs(_runs));
                _runs++;
            }
        }

        protected override void CalculateFactor() => _factor = RunsDesired / Seed;

        protected override void InitializeSeed() => Seed = 5;
    }
}