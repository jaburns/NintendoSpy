using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    static public class Dreamcast
    {
        const int FRAME_HEADER_SIZE = 32;

        static readonly string[] BUTTONS = {
            "right", "left", "down", "up", "start", "a", "b", "x", "y"
        };

        static float readTrigger(byte input)
        {
            return (float) (input) / 256;
        }
        static float readStick(byte input)
        {
            return (float)(input - 128) / 128;
        }

        static public ControllerState ReadFromPacket(byte[] packet)
        {
            if (packet.Length < FRAME_HEADER_SIZE) return null;

            var state = new ControllerStateBuilder();

            for (int i = 0; i < BUTTONS.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS[i])) continue;
                state.SetButton(BUTTONS[i], true);
            }

            state.SetAnalog("stick_x", 0);
            state.SetAnalog("stick_y", 0);
            state.SetAnalog("trig_l", 1.0f);
            state.SetAnalog("trig_r", 1.0f);

            return state.Build();
        }
    }
}
