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
using NintendoSpy.Readers;

namespace NintendoSpy
{
    public class Keybindings
    {
        class Binding 
        {
            readonly public ushort OutputKey;
            readonly public IReadOnlyList <string> RequiredButtons;

            public bool CurrentlyDepressed;

            public Binding (ushort outputKey, IReadOnlyList <string> requiredButtons) {
                OutputKey = outputKey;
                RequiredButtons = requiredButtons;
            }
        }

        public const string XML_FILE_PATH = "keybindings.xml";

        IControllerReader _reader;
        List <Binding> _bindings = new List <Binding> ();

        public Keybindings (string xmlFilePath, IControllerReader reader)
        {
            var xmlPath = Path.Combine (Environment.CurrentDirectory, xmlFilePath);

            if (! File.Exists (xmlPath)) {
                throw new ConfigParseException ("Could not find "+XML_FILE_PATH);
            }

            var doc = XDocument.Load (xmlPath);

            foreach (var binding in doc.Root.Elements ("binding")) 
            {
                var outputKey = readKeybinding (binding.Attribute ("output-key").Value);
                if (outputKey == 0) continue;

                List <string> requiredButtons = new List <string> ();
                foreach (var input in binding.Elements ("input")) {
                    requiredButtons.Add (input.Attribute ("button").Value);
                }

                if (requiredButtons.Count < 1) continue;

                _bindings.Add (new Binding (outputKey, requiredButtons));
            }

            _reader = reader;
            _reader.ControllerStateChanged += reader_ControllerStateChanged;
        }

        public void Finish ()
        {
            _reader.ControllerStateChanged -= reader_ControllerStateChanged;
        }

        void reader_ControllerStateChanged (IControllerReader reader, ControllerState state)
        {
            foreach (var binding in _bindings) 
            {
                bool allRequiredButtonsDown = true;

                foreach (var requiredButton in binding.RequiredButtons) {
                    allRequiredButtonsDown &= state.Buttons [requiredButton];
                }

                if (allRequiredButtonsDown && !binding.CurrentlyDepressed) {
                    SendKeys.PressKey (binding.OutputKey);
                    binding.CurrentlyDepressed = true;
                }
                else if (!allRequiredButtonsDown && binding.CurrentlyDepressed) {
                    SendKeys.ReleaseKey (binding.OutputKey);
                    binding.CurrentlyDepressed = false;
                }
            }
        }

        static ushort readKeybinding (string name) 
        {
            var upperName = name.ToUpperInvariant ();

            if (Regex.Match (upperName, "^[A-Z0-9]$").Success) {
                return (ushort)upperName.ToCharArray()[0];
            }
            else if (VK_KEYWORDS.ContainsKey (upperName)) {
                return VK_KEYWORDS [upperName];
            } 
            else {
                return 0;
            }
        }

        static ushort vkConvert (Key key) {
            return (ushort) KeyInterop.VirtualKeyFromKey (key);
        }

        static readonly IReadOnlyDictionary <string, ushort> VK_KEYWORDS = new Dictionary <string, ushort> {
            { "ENTER", vkConvert (Key.Enter) },
            { "TAB", vkConvert (Key.Tab) },
            { "ESC", vkConvert (Key.Escape) },
            { "ESCAPE", vkConvert (Key.Escape) },
            { "HOME", vkConvert (Key.Home) },
            { "END", vkConvert (Key.End) },
            { "LEFT", vkConvert (Key.Left) },
            { "RIGHT", vkConvert (Key.Right) },
            { "UP", vkConvert (Key.Up) },
            { "DOWN", vkConvert (Key.Down) },
            { "PGUP", vkConvert (Key.Prior) },
            { "PGDN", vkConvert (Key.Next) },
            { "NUMLOCK", vkConvert (Key.NumLock) },
            { "SCROLLLOCK", vkConvert (Key.Scroll) },
            { "PRTSC", vkConvert (Key.PrintScreen) },
            { "BREAK", vkConvert (Key.Cancel) },
            { "BACKSPACE", vkConvert (Key.Back) },
            { "BKSP", vkConvert (Key.Back) },
            { "BS", vkConvert (Key.Back) },
            { "CLEAR", vkConvert (Key.Clear) },
            { "CAPSLOCK", vkConvert (Key.Capital) },
            { "INS", vkConvert (Key.Insert) },
            { "INSERT", vkConvert (Key.Insert) },
            { "DEL", vkConvert (Key.Delete) },
            { "DELETE", vkConvert (Key.Delete) },
            { "HELP", vkConvert (Key.Help) },
            { "F1", vkConvert (Key.F1) },
            { "F2", vkConvert (Key.F2) },
            { "F3", vkConvert (Key.F3) },
            { "F4", vkConvert (Key.F4) },
            { "F5", vkConvert (Key.F5) },
            { "F6", vkConvert (Key.F6) },
            { "F7", vkConvert (Key.F7) },
            { "F8", vkConvert (Key.F8) },
            { "F9", vkConvert (Key.F9) },
            { "F10", vkConvert (Key.F10) },
            { "F11", vkConvert (Key.F11) },
            { "F12", vkConvert (Key.F12) },
            { "F13", vkConvert (Key.F13) },
            { "F14", vkConvert (Key.F14) },
            { "F15", vkConvert (Key.F15) },
            { "F16", vkConvert (Key.F16) },
            { "NUMPAD0", vkConvert (Key.NumPad0) },
            { "NUMPAD1", vkConvert (Key.NumPad1) },
            { "NUMPAD2", vkConvert (Key.NumPad2) },
            { "NUMPAD3", vkConvert (Key.NumPad3) },
            { "NUMPAD4", vkConvert (Key.NumPad4) },
            { "NUMPAD5", vkConvert (Key.NumPad5) },
            { "NUMPAD6", vkConvert (Key.NumPad6) },
            { "NUMPAD7", vkConvert (Key.NumPad7) },
            { "NUMPAD8", vkConvert (Key.NumPad8) },
            { "NUMPAD9", vkConvert (Key.NumPad9) },
            { "MULTIPLY", vkConvert (Key.Multiply) },
            { "*", vkConvert (Key.Multiply) },
            { "ADD", vkConvert (Key.Add) },
            { "+", vkConvert (Key.Add) },
            { "SUBTRACT", vkConvert (Key.Subtract) },
            { "-", vkConvert (Key.Subtract) },
            { "DIVIDE", vkConvert (Key.Divide) },
            { "/", vkConvert (Key.Divide) },
            { "SPACE", vkConvert (Key.Space) },
            { "LEFTSHIFT", vkConvert (Key.LeftShift) },
            { "RIGHTSHIFT", vkConvert (Key.RightShift) },
            { "LEFTCTRL", vkConvert (Key.LeftCtrl) },
            { "RIGHTCTRL", vkConvert (Key.RightCtrl) },
            { "LEFTALT", vkConvert (Key.LeftAlt) },
            { "RIGHTALT", vkConvert (Key.RightAlt) },
            { "UP", vkConvert (Key.Up) },
            { "DOWN", vkConvert (Key.Down) },
            { "LEFT", vkConvert (Key.Left) },
            { "RIGHT", vkConvert (Key.Right) }
        };
    }
}
