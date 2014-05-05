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

        static Dictionary<String, ushort> VkKeywords;

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
            initVkKeywords();
            var xmlPath = Path.Combine (Environment.CurrentDirectory, xmlFilePath);

            if (! File.Exists (xmlPath)) {
                throw new FileNotFoundException ("Could not find "+XML_FILE_PATH);
            }

             var doc = XDocument.Load (xmlPath);

            _bindings = new Dictionary <string, ushort> ();

             foreach (var binding in doc.Root.Elements ("binding")) {
                 ushort keyboardKey = readKeybinding(binding.Attribute("keyboard-key").Value);
                 if(keyboardKey!=(ushort)0){
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
                SendKeys.PressKey (getKeybinding (buttonName));
                _pressedButtons.Remove (buttonName);
            }
            else if (!_pressedButtons.Contains(buttonName) && pressed && _bindings.ContainsKey(buttonName))
            {
                SendKeys.ReleaseKey (getKeybinding (buttonName));
                _pressedButtons.Add (buttonName);
            }
        }

        ushort readKeybinding (string name) {
            if (Regex.Match(name, "^[A-Za-z0-9]$").Success){
                return (ushort)name.ToUpper().ToCharArray()[0];
            }
            else{
                if(VkKeywords.ContainsKey(name)){
                    return VkKeywords[name];
                }else{
                    return (ushort)0;
                }
            }
        }

        ushort getKeybinding(string name){
            return _bindings[name];
        }

        //There is probably a better way of doing this (Reflection?).
        private void initVkKeywords()
        {
            VkKeywords = new Dictionary<String, ushort>();

            VkKeywords.Add("ENTER", (ushort)KeyInterop.VirtualKeyFromKey(Key.Return));
            VkKeywords.Add("TAB", (ushort)KeyInterop.VirtualKeyFromKey(Key.Tab));
            VkKeywords.Add("ESC", (ushort)KeyInterop.VirtualKeyFromKey(Key.Escape));
            VkKeywords.Add("ESCAPE", (ushort)KeyInterop.VirtualKeyFromKey(Key.Escape));
            VkKeywords.Add("HOME", (ushort)KeyInterop.VirtualKeyFromKey(Key.Home));
            VkKeywords.Add("END", (ushort)KeyInterop.VirtualKeyFromKey(Key.End));
            VkKeywords.Add("LEFT", (ushort)KeyInterop.VirtualKeyFromKey(Key.Left));
            VkKeywords.Add("RIGHT", (ushort)KeyInterop.VirtualKeyFromKey(Key.Right));
            VkKeywords.Add("UP", (ushort)KeyInterop.VirtualKeyFromKey(Key.Up));
            VkKeywords.Add("DOWN", (ushort)KeyInterop.VirtualKeyFromKey(Key.Down));
            VkKeywords.Add("PGUP", (ushort)KeyInterop.VirtualKeyFromKey(Key.Prior));
            VkKeywords.Add("PGDN", (ushort)KeyInterop.VirtualKeyFromKey(Key.Next));
            VkKeywords.Add("NUMLOCK", (ushort)KeyInterop.VirtualKeyFromKey(Key.NumLock));
            VkKeywords.Add("SCROLLLOCK", (ushort)KeyInterop.VirtualKeyFromKey(Key.Scroll));
            VkKeywords.Add("PRTSC", (ushort)KeyInterop.VirtualKeyFromKey(Key.PrintScreen));
            VkKeywords.Add("BREAK", (ushort)KeyInterop.VirtualKeyFromKey(Key.Cancel));
            VkKeywords.Add("BACKSPACE", (ushort)KeyInterop.VirtualKeyFromKey(Key.Back));
            VkKeywords.Add("BKSP", (ushort)KeyInterop.VirtualKeyFromKey(Key.Back));
            VkKeywords.Add("BS", (ushort)KeyInterop.VirtualKeyFromKey(Key.Back));
            VkKeywords.Add("CLEAR", (ushort)KeyInterop.VirtualKeyFromKey(Key.Clear));
            VkKeywords.Add("CAPSLOCK", (ushort)KeyInterop.VirtualKeyFromKey(Key.Capital));
            VkKeywords.Add("INS", (ushort)KeyInterop.VirtualKeyFromKey(Key.Insert));
            VkKeywords.Add("INSERT", (ushort)KeyInterop.VirtualKeyFromKey(Key.Insert));
            VkKeywords.Add("DEL", (ushort)KeyInterop.VirtualKeyFromKey(Key.Delete));
            VkKeywords.Add("DELETE", (ushort)KeyInterop.VirtualKeyFromKey(Key.Delete));
            VkKeywords.Add("HELP", (ushort)KeyInterop.VirtualKeyFromKey(Key.Help));
            VkKeywords.Add("F1", (ushort)KeyInterop.VirtualKeyFromKey(Key.F1));
            VkKeywords.Add("F2", (ushort)KeyInterop.VirtualKeyFromKey(Key.F2));
            VkKeywords.Add("F3", (ushort)KeyInterop.VirtualKeyFromKey(Key.F3));
            VkKeywords.Add("F4", (ushort)KeyInterop.VirtualKeyFromKey(Key.F4));
            VkKeywords.Add("F5", (ushort)KeyInterop.VirtualKeyFromKey(Key.F5));
            VkKeywords.Add("F6", (ushort)KeyInterop.VirtualKeyFromKey(Key.F6));
            VkKeywords.Add("F7", (ushort)KeyInterop.VirtualKeyFromKey(Key.F7));
            VkKeywords.Add("F8", (ushort)KeyInterop.VirtualKeyFromKey(Key.F8));
            VkKeywords.Add("F9", (ushort)KeyInterop.VirtualKeyFromKey(Key.F9));
            VkKeywords.Add("F10", (ushort)KeyInterop.VirtualKeyFromKey(Key.F10));
            VkKeywords.Add("F11", (ushort)KeyInterop.VirtualKeyFromKey(Key.F11));
            VkKeywords.Add("F12", (ushort)KeyInterop.VirtualKeyFromKey(Key.F12));
            VkKeywords.Add("F13", (ushort)KeyInterop.VirtualKeyFromKey(Key.F13));
            VkKeywords.Add("F14", (ushort)KeyInterop.VirtualKeyFromKey(Key.F14));
            VkKeywords.Add("F15", (ushort)KeyInterop.VirtualKeyFromKey(Key.F15));
            VkKeywords.Add("F16", (ushort)KeyInterop.VirtualKeyFromKey(Key.F16));
            VkKeywords.Add("MULTIPLY", (ushort)KeyInterop.VirtualKeyFromKey(Key.Multiply));
            VkKeywords.Add("*", (ushort)KeyInterop.VirtualKeyFromKey(Key.Multiply));
            VkKeywords.Add("ADD", (ushort)KeyInterop.VirtualKeyFromKey(Key.Add));
            VkKeywords.Add("+", (ushort)KeyInterop.VirtualKeyFromKey(Key.Add));
            VkKeywords.Add("SUBTRACT", (ushort)KeyInterop.VirtualKeyFromKey(Key.Subtract));
            VkKeywords.Add("-", (ushort)KeyInterop.VirtualKeyFromKey(Key.Subtract));
            VkKeywords.Add("DIVIDE", (ushort)KeyInterop.VirtualKeyFromKey(Key.Divide));
            VkKeywords.Add("/", (ushort)KeyInterop.VirtualKeyFromKey(Key.Divide));
        }
    }
}
