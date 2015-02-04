using System.Xml;
using System.Collections.Generic;
using System;
using System.Collections;
public class newsItem
{

    private XmlNode _fullItemXml;

    internal newsItem(int id, string headline, System.DateTime publishDate, string href)
    {
        _id = id;
        _headline = headline;
        _publishDate = publishDate;
        this.fullItemHref = href;
    }

    internal newsItem(XmlNode newListItemXML)
        : this(int.Parse(newListItemXML.SelectSingleNode("id").InnerText), newListItemXML.SelectSingleNode("headline").InnerText, System.DateTime.Parse(newListItemXML.SelectSingleNode("publishDate").InnerText), newListItemXML.Attributes.GetNamedItem("href").InnerText)
    {
    }

    private XmlElement FullItemXml
    {
        get
        {
            XmlDocument NewsItemXMLDoc = default(XmlDocument);
            if (_fullItemXml == null)
            {
                NewsItemXMLDoc = new XmlDocument();
                try
                {
                    NewsItemXMLDoc.Load(fullItemHref);
                    _fullItemXml = NewsItemXMLDoc.SelectSingleNode("newsItem");
                    if (_fullItemXml == null)
                    {
                        throw new NullReferenceException("parsing the 'newsItem' element failed.");
                    }
                }
                catch (Exception ex)
                {
                    throw new ApiExceptions.ApiNotAvailableException(ex);
                }
            }
            return (XmlElement)_fullItemXml;
        }
    }


    //Private Sub FetchFullItem()
    //    If Not FullItemLoaded Then
    //        'Load other fields
    //        'Dim NewsItemXMLDoc As New XmlDocument
    //        'Try
    //        '    NewsItemXMLDoc.Load(fullItemHref)
    //        'Catch ex As Exception
    //        '    Throw New Exceptions.ApiNotAvailableException(ex)
    //        'End Try
    //        Try
    //            Dim NewsItem As XmlNode = NewsItemXMLDoc.Item("newsItem")
    //            If NewsItem.Item("extract") IsNot Nothing Then _
    //                _extract = NewsItem.Item("extract").InnerText
    //            _text = NewsItem.Item("text").InnerText
    //            _format = NewsItem.Item("text").Attributes("format").InnerText
    //            If NewsItem.Item("byLine") IsNot Nothing Then _
    //                _byLine = NewsItem.Item("byLine").InnerText
    //            If NewsItem.Item("tweetText") IsNot Nothing Then _
    //                _tweetText = NewsItem.Item("tweetText").InnerText
    //            If NewsItem.Item("source") IsNot Nothing Then _
    //                _source = NewsItem.Item("source").InnerText
    //            If NewsItem.Item("clientQuote") IsNot Nothing Then _
    //                _clientQuote = NewsItem.Item("clientQuote").InnerText
    //            _createdDate = Date.Parse(NewsItem.Item("createdDate").InnerText)
    //            _lastModifiedDate = Date.Parse(NewsItem.Item("lastModifiedDate").InnerText)
    //            If NewsItem.Item("htmlTitle") IsNot Nothing Then _
    //                _htmlTitle = NewsItem.Item("htmlTitle").InnerText
    //            If NewsItem.Item("htmlMetaDescripiton") IsNot Nothing Then _
    //                _htmlMetaDescripiton = NewsItem.Item("htmlMetaDescripiton").InnerText
    //            If NewsItem.Item("htmlMetaKeywords") IsNot Nothing Then _
    //                _htmlMetaKeywords = NewsItem.Item("htmlMetaKeywords").InnerText
    //            If NewsItem.Item("htmlMetaLanguage") IsNot Nothing Then _
    //                _htmlMetaLanguage = NewsItem.Item("htmlMetaLanguage").InnerText
    //            If NewsItem.Item("tags") IsNot Nothing Then _
    //                _tags = NewsItem.Item("tags").InnerText
    //            If NewsItem.Item("priority") IsNot Nothing Then _
    //                Integer.TryParse(NewsItem.Item("priority").InnerText, _priority)
    //            If NewsItem.Item("categories") IsNot Nothing Then _
    //                categoriesHref = NewsItem.Item("categories").Attributes("href").InnerText
    //            If NewsItem.Item("photos") IsNot Nothing Then _
    //                photosHref = NewsItem.Item("photos").Attributes("href").InnerText
    //            If NewsItem.Item("comments") IsNot Nothing Then _
    //                commentsHref = NewsItem.Item("comments").Attributes("href").InnerText
    //            _encoding = NewsItem.Attributes("encoding").InnerText
    //            FullItemLoaded = True
    //        Catch ex As Exception
    //            Throw New Exceptions.ApiDecodeException(ex)
    //        End Try
    //    End If
    //End Sub

    #region "--- Fields ---"



    private bool FullItemLoaded = false;

    private string _encoding;
    private string fullItemHref;
    private int _id;
    private string _headline;
    private string _extract;
    private string _text;
    private System.DateTime _publishDate;
    private string _byLine;
    private string _tweetText;
    private string _source;
    private string _clientQuote;
    private System.DateTime? _createdDate;
    private System.DateTime? _lastModifiedDate;
    private string _htmlTitle;
    private string _htmlMetaDescripiton;
    private string _htmlMetaKeywords;
    private string _htmlMetaLanguage;
    private string _tags;
    private enumeratedTypes.enumNewsItemState _state;
    private string _format;
    private int? _priority;
    private List<category> _categories;
    private List<photo> _photos;
    private List<comment> _comments;
    private string _categoriesHref;
    private string _photosHref;
    private string _commentsHref;

    #endregion

    public string encoding
    {
        get
        {
            //FetchFullItem()
            if (string.IsNullOrEmpty(_encoding))
            {
                if (this.FullItemXml.SelectSingleNode("encoding") != null)
                {
                    _encoding = this.FullItemXml.SelectSingleNode("encoding").InnerText;
                }
            }
            return _encoding;
        }
    }

    public int id
    {
        get { return _id; }
    }

    public string headline
    {
        get { return _headline; }
    }

    public string extract
    {
        get
        {
            if (string.IsNullOrEmpty(_extract))
            {
                if (this.FullItemXml.SelectSingleNode("extract") != null)
                {
                    _extract = this.FullItemXml.SelectSingleNode("extract").InnerText;
                }
            }
            return _extract;
        }
    }

    public string text
    {
        get
        {
            if (string.IsNullOrEmpty(_text))
            {
                if (this.FullItemXml.SelectSingleNode("text") != null)
                {
                    _text = this.FullItemXml.SelectSingleNode("text").InnerText;
                }
            }
            return _text;
        }
    }

    public System.DateTime publishDate
    {
        get { return _publishDate; }
    }

    public string byLine
    {
        get
        {
            if (string.IsNullOrEmpty(_byLine))
            {
                if (this.FullItemXml.SelectSingleNode("byLine") != null)
                {
                    _byLine = this.FullItemXml.SelectSingleNode("byLine").InnerText;
                }
            }
            return _byLine;
        }
    }

    public string tweetText
    {
        get
        {
            if (string.IsNullOrEmpty(_tweetText))
            {
                if (this.FullItemXml.SelectSingleNode("tweetText") != null)
                {
                    _tweetText = this.FullItemXml.SelectSingleNode("tweetText").InnerText;
                }
            }
            return _tweetText;
        }
    }

    public string source
    {
        get
        {
            if (string.IsNullOrEmpty(_source))
            {
                if (this.FullItemXml.SelectSingleNode("source") != null)
                {
                    _source = this.FullItemXml.SelectSingleNode("source").InnerText;
                }
            }
            return _source;
        }
    }

    public string clientQuote
    {
        get
        {
            if (string.IsNullOrEmpty(_clientQuote))
            {
                if (this.FullItemXml.SelectSingleNode("clientQuote") != null)
                {
                    _clientQuote = this.FullItemXml.SelectSingleNode("clientQuote").InnerText;
                }
            }
            return _clientQuote;
        }
    }

    public System.DateTime createdDate
    {
        get
        {
            if (!_createdDate.HasValue)
            {
                if (this.FullItemXml.SelectSingleNode("createdDate") != null)
                {
                    _createdDate = System.DateTime.Parse(this.FullItemXml.SelectSingleNode("createdDate").InnerText);
                }
            }
            return _createdDate.Value;
        }
    }

    public System.DateTime lastModifiedDate
    {
        get
        {
            if (!_lastModifiedDate.HasValue)
            {
                if (this.FullItemXml.SelectSingleNode("lastModifiedDate") != null)
                {
                    _lastModifiedDate = System.DateTime.Parse(this.FullItemXml.SelectSingleNode("lastModifiedDate").InnerText);
                }
            }
            return _lastModifiedDate.Value;
        }
    }

    public string htmlTitle
    {
        get
        {
            if (string.IsNullOrEmpty(_htmlTitle))
            {
                if (this.FullItemXml.SelectSingleNode("htmlTitle") != null)
                {
                    _htmlTitle = this.FullItemXml.SelectSingleNode("htmlTitle").InnerText;
                }
            }
            return _htmlTitle;
        }
    }

    public string htmlMetaDescription
    {
        get
        {
            if (string.IsNullOrEmpty(_htmlMetaDescripiton))
            {
                if (this.FullItemXml.SelectSingleNode("htmlMetaDescripiton") != null)
                {
                    _htmlMetaDescripiton = this.FullItemXml.SelectSingleNode("htmlMetaDescripiton").InnerText;
                }
            }
            return _htmlMetaDescripiton;
        }
    }

    public string htmlMetaKeywords
    {
        get
        {
            if (string.IsNullOrEmpty(_htmlMetaKeywords))
            {
                if (this.FullItemXml.SelectSingleNode("htmlMetaKeywords") != null)
                {
                    _htmlMetaKeywords = this.FullItemXml.SelectSingleNode("htmlMetaKeywords").InnerText;
                }
            }
            return _htmlMetaKeywords;
        }
    }

    public string htmlMetaLanguage
    {
        get
        {
            if (string.IsNullOrEmpty(_htmlMetaLanguage))
            {
                if (this.FullItemXml.SelectSingleNode("htmlMetaLanguage") != null)
                {
                    _htmlMetaLanguage = this.FullItemXml.SelectSingleNode("htmlMetaLanguage").InnerText;
                }
            }
            return _htmlMetaLanguage;
        }
    }

    public string tags
    {
        get
        {
            if (_state == enumeratedTypes.enumNewsItemState.None)
            {
                if (this.FullItemXml.SelectSingleNode("tags") != null)
                {
                    _tags = this.FullItemXml.SelectSingleNode("tags").InnerText;
                }
            }
            return _tags;
        }
    }

    public int priority
    {
        get
        {
            if (!_priority.HasValue)
            {
                if (this.FullItemXml.SelectSingleNode("priority") != null)
                {
                    _priority = int.Parse(this.FullItemXml.SelectSingleNode("priority").InnerText);
                }
                else
                {
                    _priority = 0;
                }
            }
            return _priority.Value;
        }
    }

    public enumeratedTypes.enumNewsItemState state
    {
        get
        {
            if (string.IsNullOrEmpty(_state.ToString()))
            {
                if (this.FullItemXml.SelectSingleNode("state") != null)
                {
                    _state = (enumeratedTypes.enumNewsItemState)Enum.Parse(
                        typeof(enumeratedTypes.enumNewsItemState), this.FullItemXml.SelectSingleNode("state").InnerText, true);
                }
            }
            return _state;
        }
    }

    public string format
    {
        get
        {
            if (string.IsNullOrEmpty(_format))
            {
                if (this.FullItemXml.SelectSingleNode("text").Attributes.GetNamedItem("format") != null)
                {
                    _format = this.FullItemXml.SelectSingleNode("text").Attributes.GetNamedItem("format").InnerText;
                }
            }
            return _format;
        }
    }

    public string CategoriesHref
    {
        get
        {
            if (string.IsNullOrEmpty(_categoriesHref))
            {
                try
                {
                    _categoriesHref = this.FullItemXml.SelectSingleNode("categories").Attributes.GetNamedItem("href").InnerText;
                }
                catch (NullReferenceException ex)
                {
                    throw new Exception("Failed to parse 'categories/@href' attribute", ex);
                }
            }
            return _categoriesHref;
        }
    }

    public string PhotosHref
    {
        get
        {
            if (string.IsNullOrEmpty(_photosHref))
            {
                try
                {
                    _photosHref = this.FullItemXml.SelectSingleNode("photos").Attributes.GetNamedItem("href").InnerText;
                }
                catch (NullReferenceException ex)
                {
                    throw new Exception("Failed to parse 'photos/@href' attribute", ex);
                }
            }
            return _photosHref;
        }
    }

    public string CommentsHref
    {
        get
        {
            if (string.IsNullOrEmpty(_commentsHref))
            {
                try
                {
                    _commentsHref = this.FullItemXml.SelectSingleNode("comments").Attributes.GetNamedItem("href").InnerText;
                }
                catch (NullReferenceException ex)
                {
                    throw new Exception("Failed to parse 'comments/@href' attribute", ex);
                }
            }
            return _commentsHref;
        }
    }

    public IEnumerable<category> categories
    {
        get
        {

            if (_categories == null)
            {
                _categories = new List<category>();
                if (!string.IsNullOrEmpty(this.CategoriesHref))
                {
                    //Load categories from service
                    XmlDocument CategoriesXMLDoc = new XmlDocument();
                    try
                    {
                        CategoriesXMLDoc.Load(this.CategoriesHref);
                    }
                    catch (Exception ex)
                    {
                        throw new ApiExceptions.ApiNotAvailableException(ex);
                    }
                    try
                    {
                        IEnumerator CategoryEnumerator = CategoriesXMLDoc.SelectSingleNode("categories").ChildNodes.GetEnumerator();
                        while (CategoryEnumerator.MoveNext())
                        {
                            XmlNode CategoryXML = (XmlNode)CategoryEnumerator.Current;
                            _categories.Add(new category(CategoryXML));
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new ApiExceptions.ApiDecodeException(ex);
                    }
                }
            }
            return _categories;
        }
    }

    public IEnumerable<photo> photos
    {
        get
        {
            if (_photos == null)
            {
                _photos = new List<photo>();
                if (!string.IsNullOrEmpty(this.PhotosHref))
                {
                    //Load photos from service
                    XmlDocument PhotosXMLDoc = new XmlDocument();
                    try
                    {
                        PhotosXMLDoc.Load(this.PhotosHref);
                    }
                    catch (Exception ex)
                    {
                        throw new ApiExceptions.ApiNotAvailableException(ex);
                    }
                    try
                    {
                        foreach (XmlNode PhotoNode in PhotosXMLDoc.SelectSingleNode("photos").ChildNodes)
                        {

                            _photos.Add(new photo(PhotoNode));
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new ApiExceptions.ApiDecodeException(ex);
                    }
                }
            }
            return _photos;
        }
    }

    public IEnumerable<comment> comments
    {
        get
        {
            if (_comments == null)
            {
                _comments = new List<comment>();
                if (!string.IsNullOrEmpty(CommentsHref))
                {
                    //Load categories from service
                    XmlDocument CommentsXMLDoc = new XmlDocument();
                    try
                    {
                        CommentsXMLDoc.Load(CommentsHref);
                    }
                    catch (Exception ex)
                    {
                        throw new ApiExceptions.ApiNotAvailableException(ex);
                    }
                    try
                    {
                        IEnumerator CommentEnumerator = CommentsXMLDoc.SelectSingleNode("comments").ChildNodes.GetEnumerator();
                        while (CommentEnumerator.MoveNext())
                        {
                            XmlNode CommentNode = (XmlNode)CommentEnumerator.Current;
                            _comments.Add(new comment(CommentNode));
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new ApiExceptions.ApiDecodeException(ex);
                    }
                }
            }
            return _comments;
        }
    }

}
