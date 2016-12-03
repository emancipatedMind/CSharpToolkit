namespace CSharpToolkitPlayground {
    using static System.Console;
    using CSharpToolkit.Console;

    class Program {
        static void Main(string[] args) {

            string title = "Bar Drawer By Percentage Test";
            WriteLine($"***** {title} *****\n");

            var drawer = new BarDrawerByPercentage();

            int[] seeds = { 9, 39 };

            foreach(int seed in seeds) {
                drawer.Seed = seed;
                for (int j = 0; j < 8; j++) {
                    for (int i = 0; i < drawer.Seed; i++) {
                        drawer.UpdateBar();
                        System.Threading.Tasks.Task.Delay(30).Wait(); 
                    }
                    WriteLine();
                }
            } 

            ReadLine();
        }
    }
}
