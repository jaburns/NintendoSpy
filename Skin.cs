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
using System.Xml;

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
            public enum DirectionValue { Up, Down, Left, Right }
            public ElementConfig Config;
            public string Name;
            public DirectionValue Direction;
            public bool IsReversed;
        }


        public string Name { get; private set; }
        public string Author { get; private set; }
        public string Version { get; private set; }
        public InputSource Type { get; private set; }

        public BitmapImage BackgroundImage { get; private set; }


        List <Button> _buttons = new List <Button> ();
        public IReadOnlyList <Button> Buttons { get { return _buttons; } }

        List <AnalogStick> _analogSticks = new List <AnalogStick> ();
        public IReadOnlyList <AnalogStick> AnalogSticks { get { return _analogSticks; } }

        List <AnalogTrigger> _analogTriggers = new List <AnalogTrigger> ();
        public IReadOnlyList <AnalogTrigger> AnalogTriggers { get { return _analogTriggers; } }


        string _skinPath;

        string fullPath (string fileName) {
            return Path.Combine (_skinPath, fileName);
        }

    // ----------------------------------------------------------------------------------------------------------------
        
        public Skin (string folder)
        {
            _skinPath = Path.Combine (Environment.CurrentDirectory, folder);

            if (! File.Exists (fullPath ("skin.xml"))) {
                throw new SkinParseException ("Could not find skin.xml for skin at '"+folder+"'.");
            }
            var doc = XDocument.Load (fullPath ("skin.xml"));

            Name = readStringAttr (doc.Root, "name");
            Author = readStringAttr (doc.Root, "author");
            Version = readStringAttr (doc.Root, "version");
            Type = InputSource.ALL.First (x => x.TypeTag == readStringAttr (doc.Root, ("type")));

            if (Type == null) {
                throw new SkinParseException ("Illegal value specified for skin attribute 'type'.");
            }

            var bgElem = doc.Root.Elements ("background").First();
            BackgroundImage = loadImage (readStringAttr (bgElem, "image"));

            foreach (var elem in doc.Root.Elements ("button")) {
                _buttons.Add (new Button {
                    Config = parseStandardConfig (elem),
                    Name = readStringAttr (elem, "name")
                });
            }

            foreach (var elem in doc.Root.Elements ("stick")) {
                _analogSticks.Add (new AnalogStick {
                    Config = parseStandardConfig (elem),
                    XName = readStringAttr (elem, "xname"),
                    YName = readStringAttr (elem, "yname"),
                    XRange = readUintAttr (elem, "xrange"),
                    YRange = readUintAttr (elem, "yrange")
                });
            }

            foreach (var elem in doc.Root.Elements ("analog")) 
            {
                var directionAttrs = elem.Attributes ("direction");
                if (directionAttrs.Count() < 1) throw new XmlException ("Element 'analog' needs attribute 'direction'.");

                AnalogTrigger.DirectionValue dir;

                switch (directionAttrs.First().Value) {
                    case "up": dir = AnalogTrigger.DirectionValue.Up; break;
                    case "down": dir = AnalogTrigger.DirectionValue.Down; break;
                    case "left": dir = AnalogTrigger.DirectionValue.Left; break;
                    case "right": dir = AnalogTrigger.DirectionValue.Right; break;
                    default: throw new XmlException ("Element 'analog' attribute 'direction' has illegal value. Valid values are 'up', 'down', 'left', 'right'.");
                }

                _analogTriggers.Add (new AnalogTrigger {
                    Config = parseStandardConfig (elem),
                    Name = readStringAttr (elem, "name"),
                    Direction = dir,
                    IsReversed = boolAttrWithDefault (elem.Attributes ("reverse"), false)
                });
            }
        }

        static string readStringAttr (XElement elem, string attrName)
        {
            var attrs = elem.Attributes (attrName);
            if (attrs.Count() == 0) throw new XmlException ("Required attribute '"+attrName+"' not found on element '"+elem.Name+"'.");
            return attrs.First().Value;
        }

        static uint readUintAttr (XElement elem, string attrName)
        {
            uint ret;
            if (! uint.TryParse (readStringAttr (elem, attrName), out ret)) {
                throw new SkinParseException ("Failed to parse number for property '"+attrName+"' in element '"+elem.Name+"'.");
            }
            return ret;
        }

        BitmapImage loadImage (string fileName) 
        {
            try {
                return new BitmapImage (new Uri (fullPath (fileName)));
            } catch (Exception e) {
                throw new SkinParseException ("Could not load image '"+fileName+"'.", e);
            }
        }

        ElementConfig parseStandardConfig (XElement elem)
        {
            var imageAttr = elem.Attributes ("image");
            if (imageAttr.Count() == 0) throw new XmlException ("Attribute 'image' missing for element '"+elem.Name+"'.");

            var image = loadImage (imageAttr.First().Value);

            uint width = (uint)image.PixelWidth;
            var widthAttr = elem.Attributes("width");
            if (widthAttr.Count() > 0) width = uint.Parse (widthAttr.First().Value);

            uint height = (uint)image.PixelHeight;
            var heightAttr = elem.Attributes("height");
            if (heightAttr.Count() > 0) height = uint.Parse (heightAttr.First().Value);

            uint x = readUintAttr (elem, "x");
            uint y = readUintAttr (elem, "y");

            return new ElementConfig {
                X = x, Y = y, Image = image, Width = width, Height = height
            };
        }

        static bool boolAttrWithDefault (IEnumerable <XAttribute> attr, bool dfault) {
            if (attr.Count() == 0) return dfault;
            if (attr.First().Value == "true") return true;
            if (attr.First().Value == "false") return false;
            return dfault;
        }

    // ----------------------------------------------------------------------------------------------------------------

        public class LoadResults {
            public List <Skin> SkinsLoaded;
            public List <string> ParseErrors;
        }

        static public LoadResults LoadAllSkinsFromParentFolder (string path)
        {
            var skins = new List <Skin> ();
            var errs = new List <string> ();

            foreach (var skinDir in Directory.GetDirectories(path)) {
                try {
                    var skin = new Skin (skinDir);
                    skins.Add (skin);
                }
                catch (FileNotFoundException e) {
                    errs.Add (skinDir + " :: " + e.Message);
                }
                catch (XmlException e) {
                    errs.Add (skinDir + " :: " + e.Message);
                }
            }

            return new LoadResults {
                SkinsLoaded = skins,
                ParseErrors = errs
            };
        }
    }
}
