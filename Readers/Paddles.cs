using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    static public class Paddles
    {

        const int PACKET_SIZE = 4;

        static readonly string[] BUTTONS = {
            "2", "1"
        };

        static readonly string[] ANALOG_POSITIONS_LEFT =
        {
            "left_0", "left_1", "left_2", "left_3", "left_4", "left_5", "left_6", "left_7", "left_8", "left_9", "left_10",
            "left_11", "left_12", "left_13", "left_14", "left_15", "left_16", "left_17", "left_18", "left_19", "left_20",
            "left_21", "left_22", "left_23", "left_24", "left_25", "left_26", "left_27", "left_28", "left_29", "left_30",
            "left_31", "left_32", "left_33", "left_34", "left_35", "left_36", "left_37", "left_38", "left_39", "left_40",
            "left_41", "left_42", "left_43", "left_45", "left_45", "left_46", "left_47", "left_48", "left_49", "left_50", "left_51"
        };

        static readonly string[] ANALOG_POSITIONS_RIGHT =
        {
            "right_0", "right_1", "right_2", "right_3", "right_4", "right_5", "right_6", "right_7", "right_8", "right_9", "right_10",
            "right_11", "right_12", "right_13", "right_14", "right_15", "right_16", "right_17", "right_18", "right_19", "right_20",
            "right_21", "right_22", "right_23", "right_24", "right_25", "right_26", "right_27", "right_28", "right_29", "right_30",
            "right_31", "right_32", "right_33", "right_34", "right_35", "right_36", "right_37", "right_38", "right_39", "right_40",
            "right_41", "right_42", "right_43", "right_45", "right_45", "right_46", "right_47", "right_48", "right_49", "right_50", "right_51"
        };


        static float readPaddle(ushort input)
        {
            return (float)(input)/256;
        }

        static public ControllerState ReadFromPacket(byte[] packet)
        {
           
            if (packet.Length < PACKET_SIZE) return null;

            var state = new ControllerStateBuilder();

            for (int i = 0; i < BUTTONS.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS[i])) continue;
                state.SetButton(BUTTONS[i], packet[i] != 0x00);
            }

            for(int i = 0; i < ANALOG_POSITIONS_LEFT.Length; ++i)
            {
                state.SetButton(ANALOG_POSITIONS_LEFT[i], packet[2]/5 == i );
            }

            for (int i = 0; i < ANALOG_POSITIONS_RIGHT.Length; ++i)
            {
                state.SetButton(ANALOG_POSITIONS_RIGHT[i], packet[3]/5 == i);
            }

            state.SetAnalog("left", readPaddle(packet[2]));
            state.SetAnalog("right", readPaddle(packet[3]));

            return state.Build();
        }
    }
}
