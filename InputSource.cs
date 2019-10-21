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
        static public readonly InputSource CLASSIC = new InputSource("classic", "Atari/Commodore/SMS", true, false, false, false, port => new SerialControllerReader(port, Classic.ReadFromPacket));
        static public readonly InputSource DRIVINGCONTROLLER = new InputSource("drivingcontroller", "Atari Driving Controller", true, false, false, false, port => new SerialControllerReader(port, DrivingController.ReadFromPacket));
        static public readonly InputSource ATARIKEYBOARD = new InputSource("atarikeyboard", "Atari Keyboard Controller", true, false, false, false, port => new SerialControllerReader(port, AtariKeyboard.ReadFromPacket));
        static public readonly InputSource PADDLES = new InputSource("paddles", "Atari Paddles", true, false, false, true, (port, port2) => new SerialControllerReader2(port, port2, Paddles.ReadFromPacket, Paddles.ReadFromSecondPacket));
        static public readonly InputSource JAGUAR = new InputSource("jaguar", "Atari Jaguar", true, false, false, false, port => new SerialControllerReader(port, SuperNESandNES.ReadFromPacket_Jaguar));


        static public readonly InputSource COLECOVISION = new InputSource("colecovision", "ColecoVision", true, false, false, false, port => new SerialControllerReader(port, ColecoVision.ReadFromPacket));

        static public readonly InputSource CD32 = new InputSource("cd32", "Commodore Amiga CD32", true, false, false, false, port => new SerialControllerReader(port, SuperNESandNES.ReadFromPacket_CD32));
        static public readonly InputSource C64MINI = new InputSource("c64mini", "The C64 Mini", false, false, true, false, hostname => new SSHControllerReader(hostname, "-z", C64mini.ReadFromPacket));

        static public readonly InputSource INTELLIVISION = new InputSource("intellivision", "Mattel Intellivision", true, false, false, false, port => new SerialControllerReader(port, SuperNESandNES.ReadFromPacket_Intellivision));

        //static public readonly InputSource XBOX = new InputSource("xbox", "Microsoft Xbox", false, true, controllerId => new XboxReader(int.Parse(controllerId)));
        static public readonly InputSource XBOX = new InputSource("xbox", "Microsoft Xbox", false, false, true, hostname => new SSHControllerReader(hostname, "sudo pkill -9 usb-mitm ; sudo usb-mitm 2> /dev/null -x", XboxReaderV2.ReadFromPacket));
        static public readonly InputSource XBOX360 = new InputSource("xbox360", "Microsoft Xbox 360", false, false, true, hostname => new SSHControllerReader(hostname, "sudo pkill -9 usb-mitm ; sudo usb-mitm 2> /dev/null -b", Xbox360Reader.ReadFromPacket));

        static public readonly InputSource TG16 = new InputSource("tg16", "NEC Turbographx 16", true, false, false, false, port => new SerialControllerReader(port, Tg16.ReadFromPacket));

        static public readonly InputSource NES = new InputSource ("nes", "Nintendo NES", true, false, false, port => new SerialControllerReader (port, SuperNESandNES.ReadFromPacket_NES));
        static public readonly InputSource SNES = new InputSource ("snes", "Nintendo SNES", true, false, false, port => new SerialControllerReader (port, SuperNESandNES.ReadFromPacket_SNES));
        static public readonly InputSource N64 = new InputSource ("n64", "Nintendo 64", true, false, false, port => new SerialControllerReader (port, Nintendo64.ReadFromPacket));
        static public readonly InputSource GAMECUBE = new InputSource ("gamecube", "Nintendo GameCube", true, false, false, port => new SerialControllerReader (port, GameCube.ReadFromPacket));
        static public readonly InputSource WII = new InputSource("wii", "Nintendo Wii", true, false, false, port => new SerialControllerReader(port, WiiReaderV2.ReadFromPacket));
        static public readonly InputSource SWITCH = new InputSource("switch", "Nintendo Switch", false, false, true, hostname => new SSHControllerReader(hostname, "sudo pkill -9 usb-mitm ; sudo usb-mitm 2> /dev/null -z", SwitchReader.ReadFromPacket));

        static public readonly InputSource THREEDO = new InputSource("3do", "Panasonic 3DO", true, false, false, false, port => new SerialControllerReader(port, ThreeDO.ReadFromPacket));

        static public readonly InputSource PC360 = new InputSource ("pc360", "PC 360 Controller", false, true, false, false, controllerId => new XInputReader (uint.Parse(controllerId)));
        static public readonly InputSource PAD = new InputSource ("generic", "PC Generic Gamepad", false, true, false, false, controllerId => new GamepadReader (int.Parse(controllerId)));
        static public readonly InputSource PADATOD = new InputSource ("genericanalogtodpad", "PC Generic Gamepad with AnalogToDPad", false, true, false, false, controllerId => new GamepadReaderAnalogToDPad (int.Parse(controllerId)));
        static public readonly InputSource PS2KEYBOARD = new InputSource("ps2keyboard", "PC PS/2 Keyboard", true, false, false, false, port => new SerialControllerReader(port, PS2Keyboard.ReadFromPacket));

        static public readonly InputSource CDI = new InputSource("cdi", "Phillips CD-i", true, false, false, false, port => new SerialControllerReader(port, CDi.ReadFromPacket));

        static public readonly InputSource SEGA = new InputSource("genesis", "Sega Genesis", true, false, false, false, port => new SerialControllerReader(port, Sega.ReadFromPacket));
        static public readonly InputSource SATURN3D = new InputSource("saturn", "Sega Saturn", true, false, false, false, port => new SerialControllerReader(port, SS3D.ReadFromPacket));
        static public readonly InputSource DREAMCAST = new InputSource("dreamcast", "Sega Dreamcast", true, false, false, false, port => new SerialControllerReader(port, Dreamcast.ReadFromPacket));
        static public readonly InputSource GENMINI = new InputSource("genesismini", "Sega Genesis Mini", false, false, true, false, hostname => new SSHControllerReader(hostname, "-z", GenesisMiniReader.ReadFromPacket));

        static public readonly InputSource NEOGEO = new InputSource("neogeo", "SNK NeoGeo", true, false, false, false, port => new SerialControllerReader(port, NeoGeo.ReadFromPacket));
        static public readonly InputSource NEOGEOMINI = new InputSource("neogeomini", "SNK NeoGeo Mini", false, false, true, false, hostname => new SSHControllerReader(hostname, "-g", NeoGeoMini.ReadFromPacket));

        static public readonly InputSource PLAYSTATION2 = new InputSource("playstation", "Sony Playstation 1/2", true, false, false, port => new SerialControllerReader(port, Playstation2.ReadFromPacket));
        static public readonly InputSource PS3 = new InputSource("playstation3", "Sony PlayStation 3", false, false, true, hostname => new SSHControllerReader(hostname, "sudo pkill -9 usb-mitm ; sudo usb-mitm 2> /dev/null -y", SuperNESandNES.ReadFromPacket_PSClassic));
        static public readonly InputSource PSCLASSIC = new InputSource("psclassic", "Sony PlayStation Classic", false, false, true, hostname => new SSHControllerReader(hostname, "sudo pkill -9 usb-mitm ; sudo usb-mitm 2> /dev/null -u", PS3Reader.ReadFromPacket));

        //static public readonly InputSource PLAYSTATION = new InputSource("playstation", "Playstation", true, false, port => new SerialControllerReader(port, Playstation.ReadFromPacket));
        //static public readonly InputSource MOUSETESTER = new InputSource("mousetester", "Mouse Tester", true, false, port => new MouseTester(port));
        //static public readonly InputSource WII = new InputSource("wii", "Nintendo Wii", false, true, controllerId => new WiiReaderV1(int.Parse(controllerId)));

        static public readonly IReadOnlyList <InputSource> ALL = new List <InputSource> {
            CLASSIC, DRIVINGCONTROLLER, ATARIKEYBOARD, PADDLES, JAGUAR, COLECOVISION, CD32, C64MINI, INTELLIVISION, XBOX, XBOX360, TG16, NES, SNES, N64, GAMECUBE, WII, SWITCH, THREEDO, PC360, PAD, PADATOD, PS2KEYBOARD, CDI, SEGA, SATURN3D, DREAMCAST, GENMINI, NEOGEO, NEOGEOMINI, PLAYSTATION2, PS3, PSCLASSIC
        };

        static public readonly InputSource DEFAULT = NES;

        public string TypeTag { get; private set; }
        public string Name { get; private set; }
        public bool RequiresComPort { get; private set; }
        public bool RequiresComPort2 { get; private set; }
        public bool RequiresId { get; private set; }
        public bool RequiresHostname { get; private set; }

        public Func <string, IControllerReader> BuildReader { get; private set; }

        public Func<string, string, IControllerReader> BuildReader2 { get; private set; }

        InputSource (string typeTag, string name, bool requiresComPort, bool requiresId, bool requiresHostname, bool requiresComPort2, Func <string, IControllerReader> buildReader) {
            TypeTag = typeTag;
            Name = name;
            RequiresComPort = requiresComPort;
            RequiresComPort2 = requiresComPort2;
            RequiresId = requiresId;
            RequiresHostname = requiresHostname;
            BuildReader = buildReader;
        }

        InputSource(string typeTag, string name, bool requiresComPort, bool requiresId, bool requiresHostname, bool requiresComPort2, Func<string, string, IControllerReader> buildReader2)
        {
            TypeTag = typeTag;
            Name = name;
            RequiresComPort = requiresComPort;
            RequiresComPort2 = requiresComPort2;
            RequiresId = requiresId;
            RequiresHostname = requiresHostname;
            BuildReader2 = buildReader2;
        }
    }
}
