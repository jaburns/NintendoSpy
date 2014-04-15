using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Windows.Media.Imaging;

namespace NintendoSpy
{
    public class Skin
    {
        public struct ElementConfig {
            public string Name;
            public BitmapImage Image;
            public int X, Y, Width, Height;
        }

        public struct Button {
            public ElementConfig Config;
        }

        public struct AnalogStick {
            public ElementConfig Config;
            public float XRange, YRange;
        }

        public struct AnalogTrigger {
            public ElementConfig Config;
            public bool IsHorizontal;
            public float Range;
        }


        public string Name { get; private set; }
        public string Author { get; private set; }
        public string Version { get; private set; }

        public BitmapImage BackgroundImage { get; private set; }
        public Color BackgroundColor { get; private set; }


        List <Button> _buttons = new List <Button> ();
        public IReadOnlyList <Button> Buttons { get { return _buttons.AsReadOnly (); } }

        List <AnalogStick> _analogSticks = new List <AnalogStick> ();
        public IReadOnlyList <AnalogStick> AnalogSticks { get { return _analogSticks.AsReadOnly (); } }

        List <AnalogTrigger> _analogTriggers = new List <AnalogTrigger> ();
        public IReadOnlyList <AnalogTrigger> AnalogTriggers { get { return _analogTriggers.AsReadOnly (); } }


        public Skin (string folder)
        {
            Func <string, string> skinPath = x =>
                Path.Combine (Environment.CurrentDirectory, folder, x);

            Func <string, BitmapImage> loadImage = x =>
                new BitmapImage (new Uri (skinPath (x)));

            var doc = XDocument.Load (skinPath ("skin.xml"));

            Name = doc.Root.Attributes("name").First().Value;
            Author = doc.Root.Attributes("author").First().Value;
            Version = doc.Root.Attributes("version").First().Value;

            var bgElem = doc.Root.Elements ("background").First();
            BackgroundImage = loadImage (bgElem.Attributes("image").First().Value);
            BackgroundColor = (Color)ColorConverter.ConvertFromString (bgElem.Attributes("color").First().Value);

            foreach (var button in doc.Root.Elements ("button")) {
                _buttons.Add (new Button {
                    Config = new ElementConfig {
                        Name = button.Attributes("name").First().Value,
                        Image = loadImage (button.Attributes("image").First().Value),
                        X = Int32.Parse (button.Attributes("x").First().Value),
                        Y = Int32.Parse (button.Attributes("y").First().Value),
                        Width = Int32.Parse (button.Attributes("width").First().Value),
                        Height = Int32.Parse (button.Attributes("height").First().Value),
                    }
                });
            }
        }

        static public List<Skin> LoadAllSkinsFromParentFolder (string path)
        {
            var ret = new List <Skin> ();
            foreach (var skinDir in Directory.GetDirectories(path)) {
                ret.Add (new Skin (skinDir));
            }
            return ret;
        }
    }
}
