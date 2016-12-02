namespace CSharpToolkit.Console {

    using System;
    using System.Collections.Generic;
    using Algorithms;
    using Factories;

    public class BarDrawerByPercentage {

        readonly ConsoleBarDrawer _drawer = new ConsoleBarDrawer();
        PeriodicMethodInvokerFactory _factory = new PeriodicMethodInvokerFactory();

        PeriodicMethodInvoker _pmi;
        double _seed = 100.0;
        double _barLength = 20.0;
        bool _firstCall = true;

        public BarDrawerByPercentage() {
            _factory.SetAction = (s, e) => _drawer.DrawBar(e.Iteration, _barLength);
        }

        public void UpdateBar() {
            if (_firstCall) Configure();
            _pmi.Iterate();
        }

        public double Seed {
            get { return _seed; }
            set {
                if (value < 1) value = 1.0;
                else value = Math.Ceiling(value); 
                _seed = value;
                Reset();
            }
        }

        public double BarLength {
            get { return _barLength; }
            set {
                if (value < 1) value = 1.0;
                else value = Math.Ceiling(value); 
                _barLength = value;
                Reset();
            }
        }

        public ConsoleColor CompletedColor {
            set { _drawer.CompletedColor = value; }
        }

        public List<ConsoleColor> ProcessingColors => _drawer.ProcessingColors;

        public void Reset() => _firstCall = true;

        private void Configure() {
            _pmi = _factory.GetPeriodicMethodInvoker(Seed, BarLength);
            _pmi.Reset();
            _firstCall = false;
        }
    }
}