namespace CSharpToolkit.Algorithms {
    using System.Text;
    using System.IO;
    using System.Linq;
    /// <summary>
    /// Used to seek a sequence of bytes in a stream;
    /// </summary>
    public class SequenceSeeker {

        SequenceChecker _sequenceChecker = new SequenceChecker {
            Sequence = Encoding.ASCII.GetBytes("\r\n")
        };

        /// <summary>
        /// Sequence to be matched. Defaulted to CRLF.
        /// </summary>
        public byte[] Sequence {
            get => _sequenceChecker.Sequence;
            set => _sequenceChecker.Sequence = value;
        }

        /// <summary>
        /// Resets the stream and seeks the very first instance of Sequence. Stream is left just after Sequence if found. If not, stream is left reset.
        /// </summary>
        /// <param name="stream">MemoryStream to search for Sequence in.</param>
        /// <returns>Whether Sequence was found.</returns>
        public bool SeekFirstSequence(MemoryStream stream) {
            stream.Position = 0;
            return SeekNextSequence(stream);
        }

        /// <summary>
        /// Seeks the next instance of Sequence. Stream is left just after Sequence if found. If not, stream is left at position found in.
        /// </summary>
        /// <param name="stream">MemoryStream to search for Sequence in.</param>
        /// <returns>Whether Sequence was found.</returns>
        public bool SeekNextSequence(MemoryStream stream) {
            long originalPosition = stream.Position;

            while (stream.Length - stream.Position >= Sequence.Length) {
                if (_sequenceChecker.IsSequenceMatchedByLowestBytesIn(stream)) {
                    stream.Position += Sequence.Length;
                    return true;
                }
                stream.Position++;
            }

            stream.Position = originalPosition;
            return false;
        }
    }
}
