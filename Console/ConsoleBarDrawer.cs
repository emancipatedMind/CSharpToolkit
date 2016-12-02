namespace CSharpToolkit.Console {
    using System;
    using System.Collections.Generic;

    public class ConsoleBarDrawer {

        int _bars;
        int _barLength;

        public ConsoleBarDrawer() { }

        public ConsoleColor CompletedColor { set; get; } = ConsoleColor.Green;
        public List<ConsoleColor> ProcessingColors { get; }
            = new List<ConsoleColor> { ConsoleColor.Yellow };

        public virtual void DrawBar(double bars, double barLength) {
            _bars = Convert.ToInt32(bars);
            _barLength = Convert.ToInt32(barLength);
            ValidateBarLength();
            bool last = _bars == _barLength;
            int spaces = _barLength - _bars;
            string bar = "[";
            bar += new string('|', _bars); 
            bar += new string(' ', spaces); 
            bar += "]";
            ClearCurrentConsoleLine();
            var defaultColor = Console.ForegroundColor;
            if (last) Console.ForegroundColor = CompletedColor;
            else if (ProcessingColors.Count != 0) {
                int index = _bars % ProcessingColors.Count;
                Console.ForegroundColor = ProcessingColors[index];
            }
            Console.Write(bar);
            Console.ForegroundColor = defaultColor;
        }

        private void ClearCurrentConsoleLine() {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth)); 
            Console.SetCursorPosition(0, currentLineCursor);
        }

        private void ValidateBarLength() {
            int limit = Console.WindowWidth - 3;
            if (_barLength > limit)
                _barLength = limit;
        }

    }
}