namespace CSharpToolkit.Algorithms {
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    /// <summary>
    /// Used to check if a certain position matches a sequence.
    /// </summary>
    public class SequenceChecker {

        /// <summary>
        /// Bytes to be matched. '\r\n' by default. 
        /// </summary>
        public byte[] Sequence { get; set; } = Encoding.ASCII.GetBytes("\r\n");

        /// <summary>
        /// Checks to see if source array matches Sequence at index 0.
        /// </summary>
        /// <param name="source">Array to check for Sequence.</param>
        /// <returns>Whether Sequence was found at index 0.</returns>
        public bool IsSequenceMatchedByLowestBytesIn(byte[] source) {
            byte[] dest = new byte[Sequence.Length];
            Array.Copy(source, dest, dest.Length);
            return Sequence.SequenceEqual(dest);
        }

        /// <summary>
        /// Checks to see if source array matches Sequence at index 0.
        /// </summary>
        /// <param name="stream">Stream to check for Sequence.</param>
        /// <param name="restoreStream">Whether stream is restored to pre-check position.</param>
        /// <returns>Whether Sequence was found at current position.</returns>
        public bool IsSequenceMatchedByLowestBytesIn(MemoryStream stream, bool restoreStream = true) {
            byte[] dest = new byte[Sequence.Length];
            long position = stream.Position;
            stream.Read(dest, 0, dest.Length);
            if (restoreStream)
                stream.Position = position;
            return Sequence.SequenceEqual(dest);
        }
    }
}
