using System.Xml;
using System;
using System.Collections.Generic;

public class photo
{
    internal photo(XmlNode photoXML)
        : base()
    {
        _id = int.Parse(photoXML.SelectSingleNode("id").InnerText);
        _htmlAlt = photoXML.SelectSingleNode("htmlAlt") == null ? null : photoXML.SelectSingleNode("htmlAlt").InnerText;
        _caption = photoXML.SelectSingleNode("caption") == null ? null : photoXML.SelectSingleNode("caption").InnerText;
        _orientationValue = photoXML.SelectSingleNode("orientation").InnerText;
        _photoXML = photoXML;
    }

    private int _id;
    private string _htmlAlt;
    private string _caption;
    private enumeratedTypes.enumPhotoOrientation _orientation;
    private string _orientationValue;
    private XmlNode _photoXML;
    private List<Instance> _instances;

    private XmlNode photoXML
    {
        get { return _photoXML; }
    }

    public int id
    {
        get { return _id; }
    }

    public string htmlAlt
    {
        get { return _htmlAlt; }
    }

    public string caption
    {
        get {return _caption; }
    }
    
    public enumeratedTypes.enumPhotoOrientation orientation
    {
        get
        {
            switch (_orientationValue)
            {
                case "Landscape":
                    _orientation = enumeratedTypes.enumPhotoOrientation.Landscape;
                    break;
                case "Portrait":
                    _orientation = enumeratedTypes.enumPhotoOrientation.Portrait;
                    break;
                default:
                    throw new Exception("A matching Photo Orientation was not found!");
            }
            return _orientation;
        }
    }

    public IEnumerable<Instance> Instances
    {
        get
        {
            if (_instances == null)
            {

                if (this.photoXML.SelectSingleNode("instances") != null)
                {
                    _instances = new List<Instance>();
                    foreach (XmlNode PhotoInstanceNode in this.photoXML.SelectSingleNode("instances").ChildNodes)
                    {
                        _instances.Add(new Instance(PhotoInstanceNode));
                    }

                }
            }

            return _instances;
        }
    }

    public class Instance
    {

        internal Instance(int width, int height, string type, string url)
        {
            _width = width;
            _height = height;
            _typeValue = type;
            _url = url;
        }

        internal Instance(XmlNode instaceXML)
            : this(int.Parse(instaceXML.SelectSingleNode("width").InnerText), int.Parse(instaceXML.SelectSingleNode("height").InnerText), instaceXML.SelectSingleNode("type").InnerText, instaceXML.SelectSingleNode("url").InnerText)
        {
        }

        private int _width;
        private int _height;
        private enumeratedTypes.enumPhotoInstanceType _type;
        private string _typeValue;
        private string _url;

        public int width
        {
            get { return _width; }
        }

        public int height
        {
            get { return _height; }
        }

        public enumeratedTypes.enumPhotoInstanceType type
        {
            get
            {
                switch (_typeValue)
                {
                    case "Thumbnail":
                        _type = enumeratedTypes.enumPhotoInstanceType.Thumbnail;
                        break;
                    case "Large":
                        _type = enumeratedTypes.enumPhotoInstanceType.Large;
                        break;
                    case "HighRes":
                        _type = enumeratedTypes.enumPhotoInstanceType.HighRes;
                        break;
                    case "Custom":
                        _type = enumeratedTypes.enumPhotoInstanceType.Custom;
                        break;
                    case "Small":
                        _type = enumeratedTypes.enumPhotoInstanceType.Small;
                        break;
                    case "Medium":
                        _type = enumeratedTypes.enumPhotoInstanceType.Medium;
                        break;
                    default:
                        throw new Exception("A matching Photo Instance Type was not found");
                }

                return _type;
            }
        }

        public string url
        {
            get { return _url; }
        }

    }

}
