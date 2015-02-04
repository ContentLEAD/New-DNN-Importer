using System.Xml;
public class category
{

    internal category(int id, string name)
    {
        _id = id;
        _name = name;
    }

    internal category(XmlNode categoryXML)
        : this(int.Parse(categoryXML.SelectSingleNode("id").InnerText), categoryXML.SelectSingleNode("name").InnerText)
    {
    }

    private int _id;
    private string _name;

    public int id
    {
        get { return _id; }
    }

    public string name
    {
        get { return _name; }
    }

}