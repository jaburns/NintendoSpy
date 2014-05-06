using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Windows.Input;
using System.Windows.Markup;

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

        Dictionary <string, ushort> _bindings = new Dictionary <string,ushort> ();
        public IReadOnlyDictionary <string, ushort> Bindings { get { return _bindings; } }

        IReadOnlyDictionary <String, ushort> _vkKeywords;
        HashSet <string> _pressedButtons = new HashSet <string> ();

        public Keybindings (string xmlFilePath)
        {
            var xmlPath = Path.Combine (Environment.CurrentDirectory, xmlFilePath);

            if (! File.Exists (xmlPath)) {
                throw new FileNotFoundException ("Could not find "+XML_FILE_PATH);
            }

            var doc = XDocument.Load (xmlPath);

            _vkKeywords = initVkKeywords ();

            foreach (var binding in doc.Root.Elements ("binding")) {
                ushort keyboardKey = readKeybinding (binding.Attribute ("keyboard-key").Value);
                if (keyboardKey != 0) {
                    _bindings.Add (
                        binding.Attribute ("gamepad-input").Value,
                        keyboardKey
                    );
                }
            }
        }

        public void NotifyButtonState (string buttonName, bool pressed)
        {
            if (_pressedButtons.Contains (buttonName) && !pressed && _bindings.ContainsKey(buttonName)) {
                SendKeys.PressKey (_bindings [buttonName]);
                _pressedButtons.Remove (buttonName);
            }
            else if (!_pressedButtons.Contains(buttonName) && pressed && _bindings.ContainsKey(buttonName))
            {
                SendKeys.ReleaseKey (_bindings [buttonName]);
                _pressedButtons.Add (buttonName);
            }
        }

        ushort readKeybinding (string name) 
        {
            var upperName = name.ToUpperInvariant ();

            if (Regex.Match(upperName, "^[A-Z0-9]$").Success) {
                return (ushort)name.ToUpper().ToCharArray()[0];
            }
            else {
                if (_vkKeywords.ContainsKey (upperName)) {
                    return _vkKeywords [upperName];
                } else {
                    return 0;
                }
            }
        }

        static Dictionary <string, ushort> initVkKeywords()
        {
            Func <Key, ushort> vk = x => (ushort) KeyInterop.VirtualKeyFromKey (x);

            return new Dictionary <string, ushort> {
                { "ENTER", vk(Key.Enter) },
                { "TAB", vk(Key.Tab) },
                { "ESC", vk(Key.Escape) },
                { "ESCAPE", vk(Key.Escape) },
                { "HOME", vk(Key.Home) },
                { "END", vk(Key.End) },
                { "LEFT", vk(Key.Left) },
                { "RIGHT", vk(Key.Right) },
                { "UP", vk(Key.Up) },
                { "DOWN", vk(Key.Down) },
                { "PGUP", vk(Key.Prior) },
                { "PGDN", vk(Key.Next) },
                { "NUMLOCK", vk(Key.NumLock) },
                { "SCROLLLOCK", vk(Key.Scroll) },
                { "PRTSC", vk(Key.PrintScreen) },
                { "BREAK", vk(Key.Cancel) },
                { "BACKSPACE", vk(Key.Back) },
                { "BKSP", vk(Key.Back) },
                { "BS", vk(Key.Back) },
                { "CLEAR", vk(Key.Clear) },
                { "CAPSLOCK", vk(Key.Capital) },
                { "INS", vk(Key.Insert) },
                { "INSERT", vk(Key.Insert) },
                { "DEL", vk(Key.Delete) },
                { "DELETE", vk(Key.Delete) },
                { "HELP", vk(Key.Help) },
                { "F1", vk(Key.F1) },
                { "F2", vk(Key.F2) },
                { "F3", vk(Key.F3) },
                { "F4", vk(Key.F4) },
                { "F5", vk(Key.F5) },
                { "F6", vk(Key.F6) },
                { "F7", vk(Key.F7) },
                { "F8", vk(Key.F8) },
                { "F9", vk(Key.F9) },
                { "F10", vk(Key.F10) },
                { "F11", vk(Key.F11) },
                { "F12", vk(Key.F12) },
                { "F13", vk(Key.F13) },
                { "F14", vk(Key.F14) },
                { "F15", vk(Key.F15) },
                { "F16", vk(Key.F16) },
                { "MULTIPLY", vk(Key.Multiply) },
                { "*", vk(Key.Multiply) },
                { "ADD", vk(Key.Add) },
                { "+", vk(Key.Add) },
                { "SUBTRACT", vk(Key.Subtract) },
                { "-", vk(Key.Subtract) },
                { "DIVIDE", vk(Key.Divide) },
                { "/", vk(Key.Divide) }
            };
        }
    }
}
