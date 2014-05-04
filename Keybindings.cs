using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NintendoSpy
{
    public class Keybindings
    {
        public const string XML_FILE_PATH = "keybindings.xml";

        static Keybindings s_instance;
        static public Keybindings Instance {
            get {
                if (s_instance == null) {
                    s_instance = new Keybindings (XML_FILE_PATH);
                }
                return s_instance;
            }
        }

        HashSet <string> _pressedButtons = new HashSet <string> ();

        Dictionary <string, ushort> _bindings;
        public IReadOnlyDictionary <string, ushort> Bindings { get { return _bindings; } }

        public Keybindings (string xmlFilePath)
        {
            var xmlPath = Path.Combine (Environment.CurrentDirectory, xmlFilePath);

            if (! File.Exists (xmlPath)) {
                throw new FileNotFoundException ("Could not find "+XML_FILE_PATH);
            }

             var doc = XDocument.Load (xmlPath);

            _bindings = new Dictionary <string, ushort> ();

             foreach (var binding in doc.Root.Elements ("binding")) {
                 _bindings.Add (
                     binding.Attribute ("gamepad-input").Value,
                     readKeybinding (binding.Attribute ("keyboard-key").Value)
                 );
             }
        }

        public void NotifyButtonState (string buttonName, bool pressed)
        {
            if (_pressedButtons.Contains (buttonName) && !pressed) {
                SendKeys.PressKey (readKeybinding (buttonName));
                _pressedButtons.Remove (buttonName);
            }
            else if (!_pressedButtons.Contains (buttonName) && pressed) {
                SendKeys.ReleaseKey (readKeybinding (buttonName));
                _pressedButtons.Add (buttonName);
            }
        }

        ushort readKeybinding (string name) {
            return (ushort)name.ToUpper().ToCharArray()[0];
        }
    }
}
