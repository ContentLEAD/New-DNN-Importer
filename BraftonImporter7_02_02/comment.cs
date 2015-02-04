using System.Xml;
public class comment
{

    internal comment(int id, string text, string name, string location, System.DateTime postDate)
    {
        _id = id;
        _text = text;
        _name = name;
        _location = location;
        _postDate = postDate;
    }

    internal comment(XmlNode commentXML)
        : this(int.Parse(commentXML.SelectSingleNode("id").InnerText), commentXML.SelectSingleNode("text").InnerText, commentXML.SelectSingleNode("name").InnerText, commentXML.SelectSingleNode("location").InnerText, System.DateTime.Parse(commentXML.SelectSingleNode("publishedDate").InnerText))
    {
    }

    #region "--- Fields ---"
    private int _id;
    private string _text;
    private string _name;
    private string _location;
    private System.DateTime _postDate;
    #endregion


    public int id
    {
        get { return _id; }
    }

    public string text
    {
        get { return _text; }
    }

    public string name
    {
        get { return _name; }
    }

    public string location
    {
        get { return _location; }
    }

    public System.DateTime postDate
    {
        get { return _postDate; }
    }

}