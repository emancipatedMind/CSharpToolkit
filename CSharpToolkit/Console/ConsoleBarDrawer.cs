namespace CSharpToolkit.Console {
    using System;
    using System.Collections.Generic;

    public class ConsoleBarDrawer {

        int _bars;
        int _barLength;
        string _bar;
        ConsoleColor _defaultColor;

        public ConsoleBarDrawer() { }

        public ConsoleColor CompletedColor { set; get; } = ConsoleColor.Green;
        public List<ConsoleColor> ProcessingColors { get; }
            = new List<ConsoleColor> { ConsoleColor.Yellow };

        public virtual void DrawBar(double bars, double barLength) {
            _bars = Convert.ToInt32(bars);
            _barLength = Convert.ToInt32(barLength);
            ValidateBarLength();
            ValidateBars();
            BuildBar();
            ClearCurrentConsoleLine();
            SetColor();
            Console.Write(_bar);
            Console.ForegroundColor = _defaultColor;
        }

        private void SetColor() {
            _defaultColor = Console.ForegroundColor;
            if (BarFull) Console.ForegroundColor = CompletedColor;
            else if (ProcessingColors.Count != 0) {
                int index = _bars % ProcessingColors.Count;
                Console.ForegroundColor = ProcessingColors[index];
            }
        }

        private bool BarFull => _bars == _barLength;

        private void BuildBar() {
            int spaces = _barLength - _bars;
            _bar = "[";
            _bar += new string('|', _bars);
            _bar += new string(' ', spaces);
            _bar += "]";
        }

        private void ClearCurrentConsoleLine() {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        private void ValidateBarLength() => ValidateLength(ref _barLength);

        private void ValidateBars() => ValidateLength(ref _bars);

        private void ValidateLength(ref int length) {
            int limit = Console.WindowWidth - 3;
            if (length > limit)
                length = limit;
        }

    }
}
