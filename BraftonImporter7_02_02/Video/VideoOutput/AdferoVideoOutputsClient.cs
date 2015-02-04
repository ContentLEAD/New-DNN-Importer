using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace AdferoVideoDotNet.AdferoArticlesVideoExtensions.VideoOutputs
{
    public class AdferoVideoOutputsClient
    {
        private string baseUri;
        private AdferoArticles.AdferoCredentials credentials;

        /// <summary>
        /// Initialises a new instance of the VideoOutputs Client
        /// </summary>
        /// <param name="baseUri">Uri of the API provided by your account manager</param>
        /// <param name="credentials">Credentials object containing public key and secret key</param>
        public AdferoVideoOutputsClient(string baseUri, AdferoArticles.AdferoCredentials credentials)
        {
            this.baseUri = baseUri;
            this.credentials = credentials;
        }

        public AdferoVideoOutputsClient(string baseUri, string publicKey, string secretKey)
        {

        }

      

        public AdferoVideoOutput Get(int id)
        {
            return this.GetVideoOutput(id, null, null);
        }

        public string GetRaw(int id, string format)
        {
            if (string.IsNullOrEmpty(format))
                throw new ArgumentException("format is required");

            return this.GetVideoOutputRaw(id, null, null, format);
        }

        public AdferoVideoOutputList ListForArticle(int articleId, int offset, int limit)
        {
            return this.ListVideoOutputsForArticle(articleId, offset, limit, null, null);
        }

        public string ListForArticleRaw(int articleId, int offset, int limit, string format)
        {
            if (string.IsNullOrEmpty(format))
                throw new ArgumentException("format is required");

            return this.ListVideoOutputsForArticleRaw(articleId, offset, limit, null, null, format);
        }

        private AdferoVideoOutput GetVideoOutput(int id, string[] properties, string[] fields)
        {
            string uri = this.GetUri(id, null, "xml", properties, fields, null, null);
            uri = string.Format("http://{0}:{1}@{2}", this.credentials.PublicKey, this.credentials.SecretKey, uri.Replace("http://", string.Empty));
            string xml = AdferoArticles.AdferoHelpers.GetXmlFromUri(uri);

            return this.GetVideoOutputFromXmlString(xml);
        }

        private string GetVideoOutputRaw(int id, string[] properties, string[] fields, string format)
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
            return AdferoArticles.AdferoHelpers.GetRawResponse(uri);
        }

        private string ListVideoOutputsForArticleRaw(int articleId, int offset, int limit, string[] properties, string[] fields, string format)
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
            return AdferoArticles.AdferoHelpers.GetRawResponse(uri);
        }

        private AdferoVideoOutput GetVideoOutputFromXmlString(string xml)
        {
            AdferoVideoOutput videoOutput = new AdferoVideoOutput();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            foreach (XmlNode n in doc.SelectNodes("//videoOutput/node()"))
            {
                switch (n.Name)
                {
                    case "id":
                        videoOutput.Id = int.Parse(n.InnerText);
                        break;

                    case "type":
                        videoOutput.Type = n.InnerText;
                        break;

                    case "width":
                        videoOutput.Width = int.Parse(n.InnerText);
                        break;

                    case "height":
                        videoOutput.Height = int.Parse(n.InnerText);
                        break;

                    case "path":
                        videoOutput.Path = n.InnerText;
                        break;

                    default:
                        break;
                }
            }

            return videoOutput;
        }

        private AdferoVideoOutputList ListVideoOutputsForArticle(int articleId, int offset, int limit, string[] properties, string[] fields)
        {
            string uri = this.GetUri(articleId, "articleId", "xml", properties, fields, offset, limit);
            uri = string.Format("http://{0}:{1}@{2}", this.credentials.PublicKey, this.credentials.SecretKey, uri.Replace("http://", string.Empty));
            string xml = AdferoArticles.AdferoHelpers.GetXmlFromUri(uri);

            AdferoVideoOutputList videoOutputs = this.ListVideoOutputsFromXmlString(xml);
            videoOutputs.Limit = limit;
            videoOutputs.Offset = offset;

            return videoOutputs;
        }

        private AdferoVideoOutputList ListVideoOutputsFromXmlString(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            int totalCount = int.Parse(doc.SelectSingleNode("//videoOutputs").Attributes["totalCount"].Value);
            AdferoVideoOutputList videoOutputs = new AdferoVideoOutputList();
            videoOutputs.TotalCount = totalCount;

            foreach (XmlNode n in doc.SelectNodes("//videoOutputs/videoOutput"))
            {
                foreach (XmlNode na in n.SelectNodes("id"))
                {
                    AdferoVideoOutputListItem videoOutput = new AdferoVideoOutputListItem();
                    videoOutput.Id = int.Parse(na.InnerText);
                    videoOutputs.Items.Add(videoOutput);
                }
            }

            return videoOutputs;
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

                return string.Format("{0}videoOutputs.{1}?{2}", this.baseUri, format, queryString);
            }
            else
            {
                List<string> parts = new List<string>();
                foreach (KeyValuePair<string, string> kv in data)
                    parts.Add(string.Format("{0}={1}", kv.Key, kv.Value));
                string queryString = HttpUtility.UrlDecode(string.Join("&", parts.ToArray()));

                return string.Format("{0}videoOutputs/{1}.{2}?{3}", this.baseUri, id, format, queryString);
            }
        }
    }
}