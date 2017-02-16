namespace CSharpToolkit.Algorithms {
    using System;
    using EventArgs;

    public abstract class PeriodicMethodInvoker {

        public PeriodicMethodInvoker() {
            InitializeSeed();
            CalculateFactor();
        }

        public event EventHandler<ActionEventArgs> Action;

        protected double _factor;
        double _seed;
        double _runsDesired = 20;

        protected double _timesIterated = 0;
        protected double _runs = 0;

        public double Seed {
            get { return _seed; }
            set {
                if (value < 1) value = 1;
                _seed = value;
                CalculateFactor();
            }
        }

        public double RunsDesired {
            get { return _runsDesired; }
            set {
                if (value < 1) value = 1;
                _runsDesired = value;
                CalculateFactor();
            }
        }

        public void Iterate() {
            _timesIterated++;
            RunAlgorithm();
            if (_runs > RunsDesired) Reset();
        }

        public virtual void Reset() {
            _timesIterated = 0;
            _runs = 0;
        }

        protected void FireActionEvent(ActionEventArgs e) => Action?.Invoke(this, e);

        protected abstract void CalculateFactor();
        protected abstract void RunAlgorithm();
        protected abstract void InitializeSeed();
    }
}
