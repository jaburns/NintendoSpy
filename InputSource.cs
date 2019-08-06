using RetroSpy.Readers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy
{
    public class InputSource
    {
        static public readonly InputSource CLASSIC = new InputSource("classic", "Atari/Commodore/SMS Controller", true, false, false, port => new SerialControllerReader(port, Classic.ReadFromPacket));
        static public readonly InputSource DRIVINGCONTROLLER = new InputSource("drivingcontroller", "Atari Driving Controller", true, false, false, port => new SerialControllerReader(port, DrivingController.ReadFromPacket));
        static public readonly InputSource ATARIKEYBOARD = new InputSource("atarikeyboard", "Atari Keyboard Controller", true, false, false, port => new SerialControllerReader(port, AtariKeyboard.ReadFromPacket));
        static public readonly InputSource PADDLES = new InputSource("paddles", "Atari Paddles", true, false, false, port => new SerialControllerReader(port, Paddles.ReadFromPacket));

        static public readonly InputSource CD32 = new InputSource("cd32", "Commodore Amiga CD32", true, false, false, port => new SerialControllerReader(port, SuperNESandNES.ReadFromPacket_CD32));

        static public readonly InputSource INTELLIVISION = new InputSource("intellivision", "Mattel Intellivision", true, false, false, port => new SerialControllerReader(port, SuperNESandNES.ReadFromPacket_Intellivision));

        //static public readonly InputSource XBOX = new InputSource("xbox", "Microsoft Xbox", false, true, controllerId => new XboxReader(int.Parse(controllerId)));
        static public readonly InputSource XBOX = new InputSource("xbox", "Microsoft Xbox", false, false, true, hostname => new SSHControllerReader(hostname, "-x", XboxReaderV2.ReadFromPacket));
        static public readonly InputSource XBOX360 = new InputSource("xbox360", "Microsoft Xbox 360", false, false, true, hostname => new SSHControllerReader(hostname, "-b", Xbox360Reader.ReadFromPacket));

        static public readonly InputSource TG16 = new InputSource("tg16", "NEC Turbographx 16", true, false, false, port => new SerialControllerReader(port, Tg16.ReadFromPacket));

        static public readonly InputSource NES = new InputSource ("nes", "Nintendo NES", true, false, false, port => new SerialControllerReader (port, SuperNESandNES.ReadFromPacket_NES));
        static public readonly InputSource SNES = new InputSource ("snes", "Nintendo SNES", true, false, false, port => new SerialControllerReader (port, SuperNESandNES.ReadFromPacket_SNES));
        static public readonly InputSource N64 = new InputSource ("n64", "Nintendo 64", true, false, false, port => new SerialControllerReader (port, Nintendo64.ReadFromPacket));
        static public readonly InputSource GAMECUBE = new InputSource ("gamecube", "Nintendo GameCube", true, false, false, port => new SerialControllerReader (port, GameCube.ReadFromPacket));
        static public readonly InputSource WII = new InputSource("wii", "Nintendo Wii", true, false, false, port => new SerialControllerReader(port, WiiReaderV2.ReadFromPacket));
        static public readonly InputSource SWITCH = new InputSource("switch", "Nintendo Switch", false, false, true, hostname => new SSHControllerReader(hostname, "-z", SwitchReader.ReadFromPacket));

        static public readonly InputSource THREEDO = new InputSource("3do", "Panasonic 3DO", true, false, false, port => new SerialControllerReader(port, ThreeDO.ReadFromPacket));

        static public readonly InputSource PC360 = new InputSource ("pc360", "PC 360 Controller", false, true, false, controllerId => new XInputReader (uint.Parse(controllerId)));
        static public readonly InputSource PAD = new InputSource ("generic", "PC Generic Gamepad", false, true, false, controllerId => new GamepadReader (int.Parse(controllerId)));
        static public readonly InputSource PADATOD = new InputSource ("genericanalogtodpad", "PC Generic Gamepad with AnalogToDPad", false, true, false, controllerId => new GamepadReaderAnalogToDPad (int.Parse(controllerId)));

        static public readonly InputSource CDI = new InputSource("cdi", "Phillips CD-i", true, false, false, port => new SerialControllerReader(port, CDi.ReadFromPacket));

        static public readonly InputSource SEGA = new InputSource("sega", "Sega Genesis", true, false, false, port => new SerialControllerReader(port, Sega.ReadFromPacket));
        static public readonly InputSource SATURN3D = new InputSource("saturn", "Sega Saturn", true, false, false, port => new SerialControllerReader(port, SS3D.ReadFromPacket));
        static public readonly InputSource DREAMCAST = new InputSource("dreamcast", "Sega Dreamcast", true, false, false, port => new SerialControllerReader(port, Dreamcast.ReadFromPacket));

        static public readonly InputSource NEOGEO = new InputSource("neogeo", "SNK NeoGeo", true, false, false, port => new SerialControllerReader(port, NeoGeo.ReadFromPacket));

        static public readonly InputSource PLAYSTATION2 = new InputSource("playstation", "Sony Playstation 1/2", true, false, false, port => new SerialControllerReader(port, Playstation2.ReadFromPacket));
        static public readonly InputSource PSCLASSIC = new InputSource("psclassic", "Sony PlayStation Classic", false, false, true, hostname => new SSHControllerReader(hostname, "-y", SuperNESandNES.ReadFromPacket_PSClassic));

        //static public readonly InputSource PLAYSTATION = new InputSource("playstation", "Playstation", true, false, port => new SerialControllerReader(port, Playstation.ReadFromPacket));
        //static public readonly InputSource MOUSETESTER = new InputSource("mousetester", "Mouse Tester", true, false, port => new MouseTester(port));
        //static public readonly InputSource WII = new InputSource("wii", "Nintendo Wii", false, true, controllerId => new WiiReaderV1(int.Parse(controllerId)));

        static public readonly IReadOnlyList <InputSource> ALL = new List <InputSource> {
            CLASSIC, DRIVINGCONTROLLER, ATARIKEYBOARD, PADDLES, CD32, INTELLIVISION, XBOX, XBOX360, TG16, NES, SNES, N64, GAMECUBE, WII, SWITCH, THREEDO, PC360, PAD, PADATOD, CDI, SEGA, SATURN3D, DREAMCAST, NEOGEO, PLAYSTATION2, PSCLASSIC
        };

        static public readonly InputSource DEFAULT = NES;

        public string TypeTag { get; private set; }
        public string Name { get; private set; }
        public bool RequiresComPort { get; private set; }
        public bool RequiresId { get; private set; }
        public bool RequiresHostname { get; private set; }

        public Func <string, IControllerReader> BuildReader { get; private set; }

        InputSource (string typeTag, string name, bool requiresComPort, bool requiresId, bool requiresHostname, Func <string, IControllerReader> buildReader) {
            TypeTag = typeTag;
            Name = name;
            RequiresComPort = requiresComPort;
            RequiresId = requiresId;
            RequiresHostname = requiresHostname;
            BuildReader = buildReader;
        }
    }
}
