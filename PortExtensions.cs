using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmodemProtocol {
    public static class PortExtensions {
        /// <summary>
        /// Writes the specified byte to the port.
        /// </summary>
        /// <param name="p">The port to use.</param>
        /// <param name="buffer">The byte to be written to the port.</param>
        public static void Write(this SerialPort p, byte buffer) {
            p.Write(new byte[] { buffer }, 0, 1);
        }

        /// <summary>
        /// Writes all bytes to the port.
        /// </summary>
        /// <param name="p">The port to use.</param>
        /// <param name="buffer">The byte array to be written to the port.</param>
        public static void Write(this SerialPort p, byte[] buffer) {
            p.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Flushes out of memory both the input buffer, and output buffer.
        /// </summary>
        /// <param name="p"></param>
        public static void Flush(this SerialPort p) {
            p.DiscardInBuffer();
            p.DiscardOutBuffer();
        }

        public static void Write(this SerialPort p, List<byte> buffer) {
            byte[] _buffer = buffer.ToArray();
            p.Write(_buffer, 0, _buffer.Length);
        }

    }
}