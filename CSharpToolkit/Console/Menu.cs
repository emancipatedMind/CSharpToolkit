namespace CSharpToolkit.Console {
    using System;
    public class Menu {
        public static int Construct(string prompt, params string[] options) {

            if (options == null || options.Length == 0)
                throw new InvalidOperationException("The options have not been initialized.");

            int startLeft = 0;
            int startTop = 0;
            ConsoleColor mainColor = Console.ForegroundColor == ConsoleColor.Gray ? ConsoleColor.Gray : ConsoleColor.White;

            var currentIndex = -1;
            bool originalCursorVisibility = Console.CursorVisible;
            var originalForegroundColor = Console.ForegroundColor;
            var originalBackgroundColor = Console.BackgroundColor;
            var result = 0;
            var highlighted = -1;
            var entry = "";

            Action<int, string, bool> ShowMenuItem = (index, text, highlight) => {
                var label = (index + 1).ToString();

                Console.CursorLeft = startLeft;
                Console.CursorTop = startTop + index;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = mainColor;
                Console.Out.Write($"[{label}] ");
                if (highlight)
                {
                    Console.BackgroundColor = mainColor;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.Out.Write(text);
            };

            Action<int> Highlight = i => {
                if (highlighted != i) {
                    if (highlighted >= 0) {
                        // Unhighlight old
                        ShowMenuItem(highlighted, options[highlighted], false);
                    }

                    if (i >= 0 && i < options.Length) {
                        // Highlight new
                        highlighted = i;
                        ShowMenuItem(highlighted, options[highlighted], true);
                    }
                    else {
                        highlighted = -1;
                    }
                }
            };

            if (Console.CursorLeft != 0)
                Console.Out.WriteLine();

            if (!String.IsNullOrEmpty(prompt))
            {
                Console.WriteLine(prompt);
                Console.WriteLine();
            }

            startLeft = Console.CursorLeft;
            startTop = Console.CursorTop;

            Boolean choiceMade = false;

            for (int i = 0; i < options.Length; i++)
            {
                ShowMenuItem(i, options[i], false);
            }

            while (!choiceMade)
            {
                Console.CursorVisible = false;
                Console.BackgroundColor = originalBackgroundColor;
                Console.ForegroundColor = originalForegroundColor;
                Console.CursorLeft = startLeft;
                Console.CursorTop = startTop + options.Length + 1;
                Console.Write("> " + entry.PadRight(options.Length));
                Console.CursorLeft = 2 + entry.Length;
                Console.CursorTop = startTop + options.Length + 1;
                Console.CursorVisible = true;

                var input = Console.ReadKey(true);
                var key = input.Key;

                Console.CursorVisible = false;

                if (key.Equals(ConsoleKey.DownArrow) && currentIndex < options.Length - 1)
                {
                    currentIndex++;
                    entry = (currentIndex + 1).ToString();
                }
                else if (key.Equals(ConsoleKey.UpArrow) && currentIndex > 0)
                {
                    currentIndex--;
                    entry = (currentIndex + 1).ToString();
                }
                else if (key.Equals(ConsoleKey.Home))
                {
                    currentIndex = 0;
                    entry = (currentIndex + 1).ToString();
                }
                else if (key.Equals(ConsoleKey.End))
                {
                    currentIndex = options.Length - 1;
                    entry = (currentIndex + 1).ToString();
                }
                else if (key.Equals(ConsoleKey.PageDown))
                {
                    currentIndex = Math.Min(currentIndex + 10, options.Length - 1);
                    entry = (currentIndex + 1).ToString();
                }
                else if (key.Equals(ConsoleKey.PageUp))
                {
                    currentIndex = Math.Max(currentIndex - 10, 0);
                    entry = (currentIndex + 1).ToString();
                }
                else if (key >= ConsoleKey.D0 && key <= ConsoleKey.D9 && entry.Length < options.Length.ToString().Length)
                {
                    entry = entry + (key - ConsoleKey.D0).ToString();
                    currentIndex = Int32.Parse(entry) - 1;
                }
                else if (key >= ConsoleKey.NumPad0 && key <= ConsoleKey.NumPad9 && entry.Length < options.Length.ToString().Length)
                {
                    entry = entry + (key - ConsoleKey.NumPad0).ToString();
                    currentIndex = Int32.Parse(entry) - 1;
                }
                else if (key.Equals(ConsoleKey.Backspace) && entry.Length > 0)
                {
                    entry = entry.Substring(0, entry.Length - 1);
                    currentIndex = String.IsNullOrEmpty(entry) ? -1 : Int32.Parse(entry) - 1;
                }
                else if (key.Equals(ConsoleKey.Enter))
                {
                    choiceMade = true;
                    result = currentIndex + 1;
                }

                Highlight(currentIndex);
            }

            Console.CursorVisible = originalCursorVisibility;
            Console.CursorTop = startTop + options.Length + 3;
            Console.CursorLeft = 0;
            Console.BackgroundColor = originalBackgroundColor;
            Console.ForegroundColor = originalForegroundColor;

            return result;
        }
    }

}
