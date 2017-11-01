namespace CSharpToolkit.Diagnostics {
    using System;
    using System.Linq;
    public class RandomDataGenerator {

        static Random rand = new Random();

        public int Offset { get; set; } = 0x20;
        public int Domain { get; set; } = 0x5E;

        public byte[] GetRandomData(int length) =>
            Enumerable.Repeat(0, length).Select(x => (byte)(rand.Next(Domain) + Offset)).ToArray();
    }
}
