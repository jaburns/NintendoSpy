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
            public BitmapImage Image;
            public uint X, Y, Width, Height;
        }

        public struct Button {
            public ElementConfig Config;
            public string Name;
        }

        public struct AnalogStick {
            public ElementConfig Config;
            public string XName, YName;
            public uint XRange, YRange;
        }

        public struct AnalogTrigger {
            public ElementConfig Config;
            public string Name;
            public bool IsHorizontal;
            public float Range;
        }


        public string Name { get; private set; }
        public string Author { get; private set; }
        public string Version { get; private set; }
        public InputSource Type { get; private set; }

        public BitmapImage BackgroundImage { get; private set; }
        public Color BackgroundColor { get; private set; }


        List <Button> _buttons = new List <Button> ();
        public IReadOnlyList <Button> Buttons { get { return _buttons; } }

        List <AnalogStick> _analogSticks = new List <AnalogStick> ();
        public IReadOnlyList <AnalogStick> AnalogSticks { get { return _analogSticks; } }

        List <AnalogTrigger> _analogTriggers = new List <AnalogTrigger> ();
        public IReadOnlyList <AnalogTrigger> AnalogTriggers { get { return _analogTriggers; } }


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
            Type = InputSource.ALL.First (x => x.TypeTag == doc.Root.Attributes("type").First().Value);

            var bgElem = doc.Root.Elements ("background").First();
            BackgroundImage = loadImage (bgElem.Attributes("image").First().Value);
            BackgroundColor = (Color)ColorConverter.ConvertFromString (bgElem.Attributes("color").First().Value);

            foreach (var button in doc.Root.Elements ("button"))
            {
                string name = button.Attributes("name").First().Value;

                var image = loadImage (button.Attributes("image").First().Value);

                uint width = (uint)image.PixelWidth;
                var widthAttr = button.Attributes("width");
                if (widthAttr.Count() > 0) width = uint.Parse (widthAttr.First().Value);

                uint height = (uint)image.PixelHeight;
                var heightAttr = button.Attributes("height");
                if (heightAttr.Count() > 0) height = uint.Parse (heightAttr.First().Value);

                _buttons.Add (new Button {
                    Name = name,
                    Config = new ElementConfig {
                        X = uint.Parse (button.Attributes("x").First().Value),
                        Y = uint.Parse (button.Attributes("y").First().Value),
                        Image = image,
                        Width = width,
                        Height = height
                    }
                });
            }

            foreach (var button in doc.Root.Elements ("stick"))
            {
                string xname = button.Attributes("xname").First().Value;
                string yname = button.Attributes("yname").First().Value;

                var image = loadImage (button.Attributes("image").First().Value);

                uint width = (uint)image.PixelWidth;
                var widthAttr = button.Attributes("width");
                if (widthAttr.Count() > 0) width = uint.Parse (widthAttr.First().Value);

                uint height = (uint)image.PixelHeight;
                var heightAttr = button.Attributes("height");
                if (heightAttr.Count() > 0) height = uint.Parse (heightAttr.First().Value);

                _analogSticks.Add (new AnalogStick {
                    XName = xname,
                    YName = yname,
                    XRange = uint.Parse (button.Attributes("xrange").First().Value),
                    YRange = uint.Parse (button.Attributes("yrange").First().Value),
                    Config = new ElementConfig {
                        X = uint.Parse (button.Attributes("x").First().Value),
                        Y = uint.Parse (button.Attributes("y").First().Value),
                        Image = image,
                        Width = width,
                        Height = height
                    }
                });
            }

            // TODO Get triggers
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
