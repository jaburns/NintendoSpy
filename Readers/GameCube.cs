using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
    static public class GameCube
    {
        const int PACKET_SIZE = 64;
        const int NICOHOOD_PACKET_SIZE = 8;

        static readonly string[] BUTTONS = {
            null, null, null, "start", "y", "x", "b", "a", null, "l", "r", "z", "up", "down", "right", "left"
        };


        // Button order for the Nicohood Nintendo API
        // https://github.com/NicoHood/Nintendo
        // Each byte is reverse from the buttons above
        static readonly string[] NICOHOOD_BUTTONS = {
            "a", "b", "x", "y", "start", null, null, null, "left", "right", "down", "up", "z", "r", "l", null
        };

        static float readStick(byte input)
        {
            return (float)(input - 128) / 128;
        }

        static float readTrigger(byte input)
        {
            return (float)(input) / 256;
        }

        static public ControllerState ReadFromPacket(byte[] packet)
        {
            var state = new ControllerStateBuilder();
            switch (packet.Length)
            {
                // Standard 64 bit packet size
                case PACKET_SIZE:
                    for (int i = 0; i < BUTTONS.Length; ++i)
                    {
                        if (string.IsNullOrEmpty(BUTTONS[i])) continue;
                        state.SetButton(BUTTONS[i], packet[i] != 0x00);
                        Console.WriteLine(i + ": " + packet[i]);
                    }
                    state.SetAnalog("lstick_x", readStick(SignalTool.readByte(packet, BUTTONS.Length)));
                    state.SetAnalog("lstick_y", readStick(SignalTool.readByte(packet, BUTTONS.Length + 8)));
                    state.SetAnalog("cstick_x", readStick(SignalTool.readByte(packet, BUTTONS.Length + 16)));
                    state.SetAnalog("cstick_y", readStick(SignalTool.readByte(packet, BUTTONS.Length + 24)));
                    state.SetAnalog("trig_l", readTrigger(SignalTool.readByte(packet, BUTTONS.Length + 32)));
                    state.SetAnalog("trig_r", readTrigger(SignalTool.readByte(packet, BUTTONS.Length + 40)));
                    break;
                // Packets are written as bytes when writing from the NicoHood API, so we're looking for a packet size of 8 (interpreted as bytes)
                case NICOHOOD_PACKET_SIZE:
                    for (int i = 0; i < 16; i++)
                    {
                        if (string.IsNullOrEmpty(NICOHOOD_BUTTONS[i])) continue;
                        int bitPacket = (packet[i / 8] >> (i % 8)) & 0x1;
                        state.SetButton(NICOHOOD_BUTTONS[i], bitPacket != 0x00);
                    }
                    state.SetAnalog("lstick_x", readStick(packet[2]));
                    state.SetAnalog("lstick_y", readStick(packet[3]));
                    state.SetAnalog("cstick_x", readStick(packet[4]));
                    state.SetAnalog("cstick_y", readStick(packet[5]));
                    state.SetAnalog("trig_l", readTrigger(packet[6]));
                    state.SetAnalog("trig_r", readTrigger(packet[7]));
                    break;
                default:
                    return null;
            }

            return state.Build();
        }
    }
}
