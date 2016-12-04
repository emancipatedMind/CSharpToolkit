namespace CSharpToolkit.Algorithms {
    using EventArgs;

    public class RunsLessThanSeed : PeriodicMethodInvoker {

        double _loop = 0;

        protected override void RunAlgorithm() {
            if (_timesIterated >= _loop) {
                FireActionEvent(new ActionEventArgs(_runs)); 
                _loop = _factor * ++_runs;
            }
        }

        public override void Reset() {
            base.Reset();
            _loop = 0;
        }

        protected override void CalculateFactor() => _factor = Seed / RunsDesired;
        protected override void InitializeSeed() => Seed = 100;
    }
}