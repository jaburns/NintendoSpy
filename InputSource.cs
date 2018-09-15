using NintendoSpy.Readers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintendoSpy
{
    public class InputSource
    {
        static public readonly InputSource NES = new InputSource ("nes", "NES", true, false, port => new SerialControllerReader (port, SuperNESandNES.ReadFromPacket_NES));
        static public readonly InputSource SNES = new InputSource ("snes", "Super NES", true, false, port => new SerialControllerReader (port, SuperNESandNES.ReadFromPacket_SNES));
        static public readonly InputSource N64 = new InputSource ("n64", "Nintendo 64", true, false, port => new SerialControllerReader (port, Nintendo64.ReadFromPacket));
        static public readonly InputSource GAMECUBE = new InputSource ("gamecube", "GameCube", true, false, port => new SerialControllerReader (port, GameCube.ReadFromPacket));
        static public readonly InputSource PC360 = new InputSource ("pc360", "PC 360", false, true, controllerId => new XInputReader (uint.Parse(controllerId)));
        static public readonly InputSource PAD = new InputSource ("generic", "Generic PC Gamepad", false, true, controllerId => new GamepadReader (int.Parse(controllerId)));
        static public readonly InputSource PADATOD = new InputSource ("genericanalogtodpad", "Generic PC Gamepad AnalogToDPad", false, true, controllerId => new GamepadReaderAnalogToDPad (int.Parse(controllerId)));
        static public readonly InputSource SEGA = new InputSource("sega", "Sega Genesis", true, false, port => new SerialControllerReader(port, Sega.ReadFromPacket));
        static public readonly InputSource CLASSIC = new InputSource("classic", "Atari/SMS", true, false, port => new SerialControllerReader(port, Classic.ReadFromPacket));
        static public readonly InputSource PLAYSTATION = new InputSource("playstation", "Playstation", true, false, port => new SerialControllerReader(port, Playstation.ReadFromPacket));
        static public readonly InputSource TG16 = new InputSource("tg16", "Turbographx 16", true, false, port => new SerialControllerReader(port, Tg16.ReadFromPacket));
        static public readonly InputSource SATURN = new InputSource("saturn", "Sega Saturn", true, false, port => new SerialControllerReader(port, SS.ReadFromPacket));

        static public readonly IReadOnlyList <InputSource> ALL = new List <InputSource> {
            NES, SNES, N64, GAMECUBE, PC360, PAD, PADATOD, SEGA, CLASSIC, PLAYSTATION, TG16, SATURN
        };

        static public readonly InputSource DEFAULT = NES;

        public string TypeTag { get; private set; }
        public string Name { get; private set; }
        public bool RequiresComPort { get; private set; }
        public bool RequiresId { get; private set; }

        public Func <string, IControllerReader> BuildReader { get; private set; }

        InputSource (string typeTag, string name, bool requiresComPort, bool requiresId, Func <string, IControllerReader> buildReader) {
            TypeTag = typeTag;
            Name = name;
            RequiresComPort = requiresComPort;
            RequiresId = requiresId;
            BuildReader = buildReader;
        }
    }
}
