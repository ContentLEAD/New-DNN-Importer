using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;

namespace AdferoVideoDotNet.AdferoArticles.ArticlePhotos
{
    /// <summary>
    /// Client that provides article related functions.
    /// </summary>
    public class AdferoArticlePhotosClient
    {
        private string baseUri;
        private AdferoCredentials credentials;

        /// <summary>
        /// Initialises a new instance of the ArticlePhotos Client
        /// </summary>
        /// <param name="baseUri">Uri of the API provided by your account manager</param>
        /// <param name="credentials">Credentials object containing public key and secret key</param>
        public AdferoArticlePhotosClient(string baseUri, AdferoCredentials credentials)
        {
            this.baseUri = baseUri;
            this.credentials = credentials;
        }

        private AdferoArticlePhoto GetArticlePhotoFromXmlString(string xml)
        {
            AdferoArticlePhoto articlePhoto = new AdferoArticlePhoto();
            Dictionary<string, string> fields = new Dictionary<string, string>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNodeList children = doc.SelectNodes("//articlePhoto/node()");

            foreach (XmlNode n in children)
            {
                switch (n.Name)
                {
                    case "id":
                        articlePhoto.Id = int.Parse(n.InnerText);
                        break;

                    case "sourcePhotoId":
                        articlePhoto.SourcePhotoId = int.Parse(n.InnerText);
                        break;

                    case "fields":
                        foreach (XmlNode f in n.ChildNodes)
                            articlePhoto.Fields.Add(f.Attributes["name"].Value, f.InnerText);
                        break;

                    default:
                        break;
                }
            }

            return articlePhoto;
        }

        private string GetUri(int id, string identifier, string format, string[] properties, string[] fields, int? offset, int? limit)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            if (properties != null && properties.Length > 0)
                data["properties"] = string.Join(",", properties);

            if (fields != null && fields.Length > 0)
                data["fields"] = string.Join(",", fields);

            if (!string.IsNullOrEmpty(identifier) || offset != null || limit != null)
            {
                data["offset"] = offset.ToString();
                data["limit"] = limit.ToString();
                data[identifier] = id.ToString();

                List<string> parts = new List<string>();
                foreach (KeyValuePair<string, string> kv in data)
                    parts.Add(string.Format("{0}={1}", kv.Key, kv.Value));
                string queryString = HttpUtility.UrlDecode(string.Join("&", parts.ToArray()));

                return string.Format("{0}articlePhotos.{1}?{2}", this.baseUri, format, queryString);
            }
            else
            {
                List<string> parts = new List<string>();
                foreach (KeyValuePair<string, string> kv in data)
                    parts.Add(string.Format("{0}={1}", kv.Key, kv.Value));
                string queryString = HttpUtility.UrlDecode(string.Join("&", parts.ToArray()));

                return string.Format("{0}articlePhotos/{1}.{2}?{3}", this.baseUri, id, format, queryString);
            }
        }

        private AdferoArticlePhoto GetArticlePhoto(int id, string[] properties, string[] fields)
        {
            string uri = this.GetUri(id, null, "xml", properties, fields, null, null);
            uri = string.Format("http://{0}:{1}@{2}", this.credentials.PublicKey, this.credentials.SecretKey, uri.Replace("http://", string.Empty));
            string xml = AdferoHelpers.GetXmlFromUri(uri);

            return this.GetArticlePhotoFromXmlString(xml);
        }

        public AdferoArticlePhoto Get(int id)
        {
            return this.GetArticlePhoto(id, null, null);
        }

        private string GetArticlePhotoRaw(int id, string[] properties, string[] fields, string format)
        {
            string uri = string.Empty;

            switch (format)
            {
                case "xml":
                    uri = this.GetUri(id, null, "xml", properties, fields, null, null);
                    break;

                case "json":
                    uri = this.GetUri(id, null, "json", properties, fields, null, null);
                    break;

                default:
                    throw new ArgumentException(string.Format("{0} format not supported", format));
            }

            uri = string.Format("http://{0}:{1}@{2}", this.credentials.PublicKey, this.credentials.SecretKey, uri.Replace("http://", string.Empty));
            return AdferoHelpers.GetRawResponse(uri);
        }

        public string GetRaw(int id, string format)
        {
            if (string.IsNullOrEmpty(format))
                throw new ArgumentException("format is required");

            return this.GetArticlePhotoRaw(id, null, null, format);
        }

        private AdferoArticlePhotoList ListArticlePhotosFromXmlString(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            int totalCount = int.Parse(doc.SelectSingleNode("//articlePhotos").Attributes["totalCount"].Value);
            AdferoArticlePhotoList articlesPhotosList = new AdferoArticlePhotoList();
            articlesPhotosList.TotalCount = totalCount;

            foreach (XmlNode n in doc.SelectNodes("//articlePhotos/articlePhoto"))
            {
                foreach (XmlNode na in n.SelectNodes("id"))
                {
                    AdferoArticlePhotoListItem item = new AdferoArticlePhotoListItem();
                    item.Id = int.Parse(na.InnerText);
                    articlesPhotosList.Items.Add(item);
                }
            }

            return articlesPhotosList;
        }

        private AdferoArticlePhotoList ListArticlePhotos(int articleId, int offset, int limit, string[] properties, string[] fields)
        {
            string uri = this.GetUri(articleId, "articleId", "xml", properties, fields, offset, limit);
            uri = string.Format("http://{0}:{1}@{2}", this.credentials.PublicKey, this.credentials.SecretKey, uri.Replace("http://", string.Empty));
            string xml = AdferoHelpers.GetXmlFromUri(uri);

            AdferoArticlePhotoList articlePhotos = this.ListArticlePhotosFromXmlString(xml);
            articlePhotos.Limit = limit;
            articlePhotos.Offset = offset;

            return articlePhotos;
        }

        public AdferoArticlePhotoList ListForArticle(int articleId, int offset, int limit)
        {
            return this.ListArticlePhotos(articleId, offset, limit, null, null);
        }

        private string ListArticlesPhotosRaw(int articleId, int offset, int limit, string[] properties, string[] fields, string format)
        {
            string uri = string.Empty;
            switch (format)
            {
                case "xml":
                    uri = this.GetUri(articleId, "articleId", "xml", properties, fields, offset, limit);
                    break;

                case "json":
                    uri = this.GetUri(articleId, "articleId", "json", properties, fields, offset, limit);
                    break;

                default:
                    throw new ArgumentException(string.Format("{0} format not supported", format));
            }

            uri = string.Format("http://{0}:{1}@{2}", this.credentials.PublicKey, this.credentials.SecretKey, uri.Replace("http://", string.Empty));
            return AdferoHelpers.GetRawResponse(uri);
        }
    }
}