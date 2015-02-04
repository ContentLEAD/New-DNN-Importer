using System.Net;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Collections;

public class ApiContext
{
    #region "--- Fields ---"
    private Guid _api_key = Guid.Empty;
    private List<newsItem> _news;
    private string _apiRootUrl;
    private XmlNode _apiRootXml;
    private string _newsurl;
    private string _baseurl;
    #endregion

    /// <summary>
    /// Api_Key and BaseUrl properties much be set
    /// </summary>
    /// <remarks></remarks>
    public ApiContext()
    {
        //Me.api_key = SampleClientLibraryConfigurationSection.Config.ApiKey
    }

    public ApiContext(string api_key, string BaseUrl)
        : this(new Guid(api_key), BaseUrl)
    {
    }

    public ApiContext(Guid api_key, string BaseUrl)
    {
        this.Api_Key = api_key;
        _baseurl = BaseUrl;
    }

    public ApiContext(string ArchiveUrl)
    {
        _newsurl = ArchiveUrl;
    }

    public string BaseUrl
    {
        get { return _baseurl; }
        set { _baseurl = value; }
    }

    public Guid Api_Key
    {
        get { return _api_key; }
        set { _api_key = value; }
    }

    internal string RootUrl
    {
        get { return this.BaseUrl + Api_Key.ToString() + "/"; }
    }

    internal string NewsUrl
    {
        get
        {
            if (string.IsNullOrEmpty(_newsurl))
            {
                try
                {
                    _newsurl = ApiRootXml.SelectSingleNode("news").Attributes.GetNamedItem("href").Value;
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to parse the News URL", ex);
                }
            }

            return _newsurl;
        }
    }

    private XmlNode ApiRootXml
    {
        get
        {
            if (_apiRootXml == null)
            {
                if (!string.IsNullOrEmpty(this.RootUrl))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(this.RootUrl);
                    _apiRootXml = xmlDoc.SelectSingleNode("api");
                }
                else
                {
                    throw new Exception("No API root URL provided!");
                }
            }
            return _apiRootXml;
        }
    }

    /// <summary>
    /// This readonly property presents a lazy-loaded enumeration of newsItems read,
    /// when requested, from the restful api. This throws exceptions if the xml in
    /// api is not able to be reached or the items are not able to be parsed.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    /// <exception cref="Exceptions.ApiDecodeException"></exception>
    /// <exception cref="Exceptions.ApiNotAvailableException"></exception>
    public IEnumerable<newsItem> News
    {
        get
        {
            if (_news == null)
            {
                _news = new List<newsItem>();
                //Load news from service
                XmlDocument NewsXMLDoc = default(XmlDocument);
                try
                {
                    if (string.IsNullOrEmpty(this.NewsUrl))
                    {
                        throw new Exception("News URL is empty!");
                    }
                    else
                    {
                        NewsXMLDoc = new XmlDocument();
                        //-- load the list of News Items
                        NewsXMLDoc.Load(this.NewsUrl);
                    }
                }
                catch (Exception ex)
                {
                    throw new ApiExceptions.ApiNotAvailableException(ex);
                }
                try
                {
                    IEnumerator NewsItemEnum = NewsXMLDoc.SelectSingleNode("news").ChildNodes.GetEnumerator();
                    while (NewsItemEnum.MoveNext())
                    {
                        XmlNode NewsListItem = (XmlNode)NewsItemEnum.Current;
                        if (NewsListItem.Name == "newsListItem")
                        {
                            try
                            {
                                _news.Add(new newsItem(NewsListItem));
                            }
                            catch (NullReferenceException ex)
                            {
                            }
                            //don't throw an exception here if you want to try loading the next item.
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ApiExceptions.ApiDecodeException(ex);
                }
            }
            return _news;
        }
    }

    public bool postComment(int articleID, string name, string location, string text)
    {
        HttpWebRequest postRequest = (HttpWebRequest)HttpWebRequest.Create(NewsUrl + articleID + "/comments/post/");
        postRequest.Method = "POST";
        postRequest.ContentType = "text/xml";

        XmlDocument doc = new XmlDocument();
        XmlElement commentElement = doc.CreateElement("comment");

        XmlElement idElement = doc.CreateElement("id");
        idElement.InnerText = "0";
        XmlElement publishedDateElement = doc.CreateElement("publishedDate");
        publishedDateElement.InnerText = System.DateTime.Now.ToString("s");
        XmlElement nameElement = doc.CreateElement("name");
        nameElement.InnerText = name;
        XmlElement locationElement = doc.CreateElement("location");
        locationElement.InnerText = location;
        XmlElement textElement = doc.CreateElement("text");
        textElement.InnerText = text;

        commentElement.AppendChild(idElement);
        commentElement.AppendChild(publishedDateElement);
        commentElement.AppendChild(nameElement);
        commentElement.AppendChild(locationElement);
        commentElement.AppendChild(textElement);

        doc.AppendChild(commentElement);

        string postData = doc.OuterXml;
        postRequest.ContentLength = postData.Length;

        try
        {
            Stream requestStream = postRequest.GetRequestStream();
            requestStream.Write(Encoding.UTF8.GetBytes(postData.ToCharArray()), 0, postData.Length);
            requestStream.Close();

            WebResponse response = postRequest.GetResponse();
            var responseStatus = ((HttpWebResponse)response).StatusDescription;
            if (responseStatus == "OK")
            {
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                XmlDocument responseAsXML = new XmlDocument();
                responseAsXML.LoadXml(reader.ReadToEnd());

                if (responseAsXML.InnerText == "true")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    //Public Function getNewsItem(ByVal articleID As Integer) As newsItem
    //    Dim newsItemUrl As String = Me.NewsUrl & articleID
    //    Dim NewsXMLDoc As XmlDocument
    //    Try
    //        If String.IsNullOrEmpty(Me.NewsUrl) Then
    //            Throw New Exception("News URL is empty!")
    //        Else
    //            NewsXMLDoc = New XmlDocument
    //            NewsXMLDoc.Load(newsItemUrl) '-- load the list of News Items
    //        End If
    //    Catch ex As Exception
    //        Throw New Exceptions.ApiNotAvailableException(ex)
    //    End Try
    //    Try
    //        Dim id As Integer
    //        Integer.TryParse(articleID, id)
    //        Dim headline As String = NewsXMLDoc.Item("newsItem").Item("headline").InnerText
    //        Dim publishDate As Date
    //        Date.TryParse(NewsXMLDoc.Item("newsItem").Item("publishDate").InnerText, publishDate)

    //        Return New newsItem(id, headline, publishDate, newsItemUrl)
    //    Catch ex As Exception
    //        Throw New Exceptions.ApiDecodeException
    //    End Try
    //End Function
}
