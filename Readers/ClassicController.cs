using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy.Readers
{
	static public class ClassicController
	{
		const int PACKET_SIZE = 48;

		static readonly string[] BUTTONS = {
			null, "r", "plus", "home", "minus", "l", "down", "right", "up", "left", "zr", "x", "a", "y", "b", "zl"
		};

		static float readLeftStick(int input)
		{
			return (float)(input - 30) / 30;
		}

		static float readRightStick(int input)
		{
			return (float)(input - 15) / 15;
		}

		static float readTrigger(int input)
		{
			return (float)(input) / 31;
		}

		static byte unencrypt(byte packet)
		{
			return (byte)((byte)(packet ^ 0x17) + 0x17);
		}

		static public ControllerState ReadFromPacket(byte[] packet)
		{
			if (packet.Length < PACKET_SIZE) return null;

			byte[] data = new byte[PACKET_SIZE / 8];
			for (int i = 0; i < PACKET_SIZE / 8; i++) {
				data[i] = unencrypt(SignalTool.readByte(packet, i * 8));
			}

			var state = new ControllerStateBuilder();

			for (int i = 0; i < BUTTONS.Length; ++i)
			{
				if (string.IsNullOrEmpty(BUTTONS[i])) continue;
				state.SetButton(BUTTONS[i], (data[4 + i / 8] & (0x01 << (i % 8))) == 0x00);
			}

			state.SetAnalog("lstick_x", readLeftStick(data[0] & 0x3F));
			state.SetAnalog("lstick_y", readLeftStick(data[1] & 0x3F));
			state.SetAnalog("rstick_x", readRightStick(((data[0] & 0xC0) >> 3) | ((data[1] & 0xC0) >> 5) | ((data[2] & 0x80) >> 7)));
			state.SetAnalog("rstick_y", readRightStick(data[2] & 0x1F));
			state.SetAnalog("trig_l", readTrigger(((data[2] & 0x60) >> 2) | ((data[3] & 0xE0) >> 5)));
			state.SetAnalog("trig_r", readTrigger(data[3] & 0x1F));

			return state.Build();
		}
	}
}
