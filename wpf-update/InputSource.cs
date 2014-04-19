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
        static public readonly IReadOnlyList <InputSource> ALL = new List <InputSource> {
            new InputSource ("nes", "NES", true, port => new SerialControllerReader (port, new NES ())),
            new InputSource ("snes", "Super NES", true, port => new SerialControllerReader (port, new SuperNES ())),
            new InputSource ("n64", "Nintendo 64", true, port => new SerialControllerReader (port, new Nintendo64 ())),
            new InputSource ("gcn", "GameCube", true, port => new SerialControllerReader (port, new GameCube ())),
            new InputSource ("pc360", "PC 360", false, _ => new XInputReader ()),
            new InputSource ("pad", "Other Gamepad", false, _ => new GamepadReader ())
        };

        static public readonly InputSource DEFAULT = ALL [0];

        public string FolderPrefix { get; private set; }
        public string Name { get; private set; }
        public bool RequiresComPort { get; private set; }

        public Func <string, IControllerReader> BuildReader { get; private set; }

        InputSource (string folderPrefix, string name, bool requiresComPort, Func <string, IControllerReader> buildReader) {
            FolderPrefix = folderPrefix;
            Name = name;
            RequiresComPort = requiresComPort;
            BuildReader = buildReader;
        }
    }
}
