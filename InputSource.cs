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
        static public readonly InputSource ATARIKEYBOARD = new InputSource("atarikeyboard", "Atari Keyboard Controller", true, false, port => new SerialControllerReader(port, AtariKeyboard.ReadFromPacket));
        //static public readonly InputSource PLAYSTATION = new InputSource("playstation", "Playstation", true, false, port => new SerialControllerReader(port, Playstation.ReadFromPacket));
        static public readonly InputSource PLAYSTATION2 = new InputSource("playstation", "Playstation", true, false, port => new SerialControllerReader(port, Playstation2.ReadFromPacket));
        static public readonly InputSource TG16 = new InputSource("tg16", "Turbographx 16", true, false, port => new SerialControllerReader(port, Tg16.ReadFromPacket));
        static public readonly InputSource PADDLES = new InputSource("paddles", "Atari Paddles", true, false, port => new SerialControllerReader(port, Paddles.ReadFromPacket));
        static public readonly InputSource DRIVINGCONTROLLER = new InputSource("drivingcontroller", "Atari Driving Controller", true, false, port => new SerialControllerReader(port, DrivingController.ReadFromPacket));
        static public readonly InputSource SATURN3D = new InputSource("saturn", "Sega Saturn", true, false, port => new SerialControllerReader(port, SS3D.ReadFromPacket));
        static public readonly InputSource NEOGEO = new InputSource("neogeo", "NeoGeo", true, false, port => new SerialControllerReader(port, NeoGeo.ReadFromPacket));
        static public readonly InputSource THREEDO = new InputSource("3do", "3DO", true, false, port => new SerialControllerReader(port, ThreeDO.ReadFromPacket));
        static public readonly InputSource CDI = new InputSource("cdi", "CD-i", true, false, port => new SerialControllerReader(port, CDi.ReadFromPacket));
        static public readonly InputSource INTELLIVISION = new InputSource("intellivision", "Intellivision", true, false, port => new SerialControllerReader(port, SuperNESandNES.ReadFromPacket_Intellivision));
        static public readonly InputSource CD32 = new InputSource("cd32", "Amiga CD32", true, false, port => new SerialControllerReader(port, SuperNESandNES.ReadFromPacket_CD32));
        static public readonly InputSource DREAMCAST = new InputSource("dreamcast", "Sega Dreamcast", true, false, port => new SerialControllerReader(port, Dreamcast.ReadFromPacket));

        static public readonly IReadOnlyList <InputSource> ALL = new List <InputSource> {
            NES, SNES, N64, GAMECUBE, PC360, PAD, PADATOD, SEGA, CLASSIC, PLAYSTATION2, TG16, SATURN3D, NEOGEO, ATARIKEYBOARD, PADDLES, DRIVINGCONTROLLER, THREEDO, CDI, INTELLIVISION, CD32, DREAMCAST
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
