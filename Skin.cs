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
        public class ElementConfig {
            public BitmapImage Image;
            public uint X, Y, Width, Height;
            public List<string> TargetBackgrounds { get; set; }
            public List<string> IgnoreBackgrounds { get; set; }
        }

        public class Background {
            public string Name { get; set; }
            public BitmapImage Image { get; set; }
            public Color Color { get; set; }
            public uint Width, Height;
        }

        public class Detail
        {
            public string Name { get; set; }
            public ElementConfig Config;
            
        }

        public class Button {
            public ElementConfig Config;
            public string Name;
        }

        public class RangeButton {
            public ElementConfig Config;
            public string Name;
            public float From, To;
        }

        public class AnalogStick {
            public ElementConfig Config;
            public string XName, YName;
            public uint XRange, YRange;
            public bool XReverse, YReverse;
        }

        public class AnalogTrigger {
            public enum DirectionValue { Up, Down, Left, Right }
            public ElementConfig Config;
            public string Name;
            public DirectionValue Direction;
            public bool IsReversed;
            public bool UseNegative;
        }


        public string Name { get; private set; }
        public string Author { get; private set; }
        public InputSource Type { get; private set; }


        List <Background> _backgrounds = new List <Background> ();
        public IReadOnlyList <Background> Backgrounds { get { return _backgrounds; } }

        List<Detail> _details = new List<Detail>();
        public IReadOnlyList <Detail> Details { get { return _details; } }

        List <Button> _buttons = new List <Button> ();
        public IReadOnlyList <Button> Buttons { get { return _buttons; } }

        List <RangeButton> _rangeButtons = new List <RangeButton> ();
        public IReadOnlyList <RangeButton> RangeButtons { get { return _rangeButtons; } }

        List <AnalogStick> _analogSticks = new List <AnalogStick> ();
        public IReadOnlyList <AnalogStick> AnalogSticks { get { return _analogSticks; } }

        List <AnalogTrigger> _analogTriggers = new List <AnalogTrigger> ();
        public IReadOnlyList <AnalogTrigger> AnalogTriggers { get { return _analogTriggers; } }


    // ----------------------------------------------------------------------------------------------------------------

        Skin (string folder)
        {
            var skinPath = Path.Combine (Environment.CurrentDirectory, folder);

            if (! File.Exists (Path.Combine (skinPath, "skin.xml"))) {
                throw new ConfigParseException ("Could not find skin.xml for skin at '"+folder+"'.");
            }
            var doc = XDocument.Load (Path.Combine (skinPath, "skin.xml"));

            Name = readStringAttr (doc.Root, "name");
            Author = readStringAttr (doc.Root, "author");
            Type = InputSource.ALL.First (x => x.TypeTag == readStringAttr (doc.Root, ("type")));

            if (Type == null) {
                throw new ConfigParseException ("Illegal value specified for skin attribute 'type'.");
            }

            var bgElems = doc.Root.Elements ("background");

            if (bgElems.Count() < 1) {
                throw new ConfigParseException ("Skin must contain at least one background.");
            }

            foreach (var elem in bgElems) {
                var imgPath = readStringAttr(elem, "image", false);
                BitmapImage image = null;
                uint width = 0;
                uint height = 0;
                if (!string.IsNullOrEmpty(imgPath)) {
                    image = loadImage(skinPath, imgPath);
                    width = (uint)image.PixelWidth;
                    var widthAttr = elem.Attributes("width");
                    if (widthAttr.Count() > 0) width = uint.Parse(widthAttr.First().Value);
                    height = (uint)image.PixelHeight;
                    var heightAttr = elem.Attributes("height");
                    if (heightAttr.Count() > 0) height = uint.Parse(heightAttr.First().Value);
                }
                else
                {
                    var widthAttr = elem.Attributes("width");
                    if (widthAttr.Count() > 0) width = uint.Parse(widthAttr.First().Value);
                    var heightAttr = elem.Attributes("height");
                    if (heightAttr.Count() > 0) height = uint.Parse(heightAttr.First().Value);
                    if(width == 0 || height == 0)
                    {
                        throw new ConfigParseException("Element 'background' should either define 'image' with optionally 'width' and 'height' or both 'width' and 'height'.");
                    }
                }
                _backgrounds.Add(new Background {
                    Name = readStringAttr(elem, "name"),
                    Image = image,
                    Color = readColorAttr(elem, "color", false),
                    Width = width,
                    Height = height
                });
            }

            foreach (var elem in doc.Root.Elements("detail"))
            {
                _details.Add(new Detail
                {
                    Config = parseStandardConfig(skinPath, elem),
                    Name = readStringAttr(elem, "name"),
                });
            }

            foreach (var elem in doc.Root.Elements ("button")) {
                _buttons.Add (new Button {
                    Config = parseStandardConfig (skinPath, elem),
                    Name = readStringAttr (elem, "name")
                });
            }

            foreach (var elem in doc.Root.Elements ("rangebutton"))
            {
                var from = readFloatConfig (elem, "from");
                var to = readFloatConfig (elem, "to");

                if (from > to) throw new ConfigParseException ("Rangebutton 'from' field cannot be greater than 'to' field.");

                _rangeButtons.Add (new RangeButton {
                    Config = parseStandardConfig (skinPath, elem),
                    Name = readStringAttr (elem, "name"),
                    From = from,
                    To = to
                });
            }

            foreach (var elem in doc.Root.Elements ("stick")) {
                _analogSticks.Add (new AnalogStick {
                    Config = parseStandardConfig (skinPath, elem),
                    XName = readStringAttr (elem, "xname"),
                    YName = readStringAttr (elem, "yname"),
                    XRange = readUintAttr (elem, "xrange"),
                    YRange = readUintAttr (elem, "yrange"),
                    XReverse = readBoolAttr (elem, "xreverse"),
                    YReverse = readBoolAttr (elem, "yreverse")
                });
            }

            foreach (var elem in doc.Root.Elements ("analog"))
            {
                var directionAttrs = elem.Attributes ("direction");
                if (directionAttrs.Count() < 1) throw new ConfigParseException ("Element 'analog' needs attribute 'direction'.");

                AnalogTrigger.DirectionValue dir;

                switch (directionAttrs.First().Value) {
                    case "up": dir = AnalogTrigger.DirectionValue.Up; break;
                    case "down": dir = AnalogTrigger.DirectionValue.Down; break;
                    case "left": dir = AnalogTrigger.DirectionValue.Left; break;
                    case "right": dir = AnalogTrigger.DirectionValue.Right; break;
                    default: throw new ConfigParseException ("Element 'analog' attribute 'direction' has illegal value. Valid values are 'up', 'down', 'left', 'right'.");
                }

                _analogTriggers.Add (new AnalogTrigger {
                    Config = parseStandardConfig (skinPath, elem),
                    Name = readStringAttr (elem, "name"),
                    Direction = dir,
                    IsReversed = readBoolAttr (elem, "reverse"),
                    UseNegative = readBoolAttr (elem, "usenegative")
                });
            }
        }

        static string readStringAttr(XElement elem, string attrName, bool required = true)
        {
            var attrs = elem.Attributes (attrName);
            if (attrs.Count() == 0)
            {
                if (required)
                {
                    throw new ConfigParseException("Required attribute '" + attrName + "' not found on element '" + elem.Name + "'.");
                }
                else
                {
                    return "";
                }
            }
            return attrs.First().Value;
        }

        static List<string> getArrayAttr(XElement elem, string attrName, bool required = true)
        {
            var attrs = elem.Attributes(attrName);
            if (attrs.Count() == 0)
            {
                if (required)
                {
                    throw new ConfigParseException("Required attribute '" + attrName + "' not found on element '" + elem.Name + "'. You can use it with ';' for multiple values.");
                }
                else
                {
                    return new List<string>(0);
                }
            }
            return new List<string>(attrs.First().Value.Split(';'));
        }

        static Color readColorAttr(XElement elem, string attrName, bool required)
        {
            Color result = new Color ();
           
            var attrs = elem.Attributes(attrName);
            if (attrs.Count() == 0)
            {
                if (required)
                {
                    throw new ConfigParseException("Required attribute '" + attrName + "' not found on element '" + elem.Name + "'.");
                }
                else
                {
                    return result;
                }
            }
            var converted = ColorConverter.ConvertFromString(attrs.First().Value);
            if(result != null)
            {
                result = (Color) converted;
            }
            return result;
        }

        static float readFloatConfig (XElement elem, string attrName)
        {
            float ret;
            if (! float.TryParse (readStringAttr (elem, attrName), out ret)) {
                throw new ConfigParseException ("Failed to parse number for property '"+attrName+"' in element '"+elem.Name+"'.");
            }
            return ret;
        }

        static uint readUintAttr (XElement elem, string attrName)
        {
            uint ret;
            if (! uint.TryParse (readStringAttr (elem, attrName), out ret)) {
                throw new ConfigParseException ("Failed to parse number for property '"+attrName+"' in element '"+elem.Name+"'.");
            }
            return ret;
        }

        static BitmapImage loadImage (string skinPath, string fileName)
        {
            try {
                return new BitmapImage (new Uri (Path.Combine (skinPath, fileName)));
            } catch (Exception e) {
                throw new ConfigParseException ("Could not load image '"+fileName+"'.", e);
            }
        }

        static ElementConfig parseStandardConfig (string skinPath, XElement elem)
        {
            var imageAttr = elem.Attributes ("image");
            if (imageAttr.Count() == 0) throw new ConfigParseException ("Attribute 'image' missing for element '"+elem.Name+"'.");

            var image = loadImage (skinPath, imageAttr.First().Value);

            uint width = (uint)image.PixelWidth;
            var widthAttr = elem.Attributes("width");
            if (widthAttr.Count() > 0) width = uint.Parse (widthAttr.First().Value);

            uint height = (uint)image.PixelHeight;
            var heightAttr = elem.Attributes("height");
            if (heightAttr.Count() > 0) height = uint.Parse (heightAttr.First().Value);

            uint x = readUintAttr (elem, "x");
            uint y = readUintAttr (elem, "y");

            var targetBgs = getArrayAttr(elem, "target", false);
            var ignoreBgs = getArrayAttr(elem, "ignore", false);

            return new ElementConfig {
                X = x, Y = y, Image = image, Width = width, Height = height, TargetBackgrounds = targetBgs, IgnoreBackgrounds = ignoreBgs
            };
        }

        static bool readBoolAttr (XElement elem, string attrName, bool dfault = false) {
            var attrs = elem.Attributes (attrName);
            if (attrs.Count() == 0) return dfault;
            if (attrs.First().Value == "true") return true;
            if (attrs.First().Value == "false") return false;
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
                catch (ConfigParseException e) {
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
