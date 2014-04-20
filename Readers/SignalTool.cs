using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    static internal class SignalTool
    {
        /// <summary>
        /// Reads a byte of data from a string of 8 bits in a controller data packet.
        /// </summary>
        public static byte readByte (byte[] packet, int offset)
        {
            byte val = 0;
            for (int i = 0 ; i < 8 ; ++i) {
                if ((packet[i+offset] & 0x0F) != 0) {
                    val |= (byte)(1<<(7-i));
                }
            }
            return val;
        }
    }
}
