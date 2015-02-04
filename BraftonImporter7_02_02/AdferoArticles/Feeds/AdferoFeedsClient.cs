using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace AdferoVideoDotNet.AdferoArticles.Feeds
{
    public class AdferoFeedsClient
    {
        private string baseUri;
        private AdferoCredentials credentials;

        /// <summary>
        /// Initialises a new instance of the Feeds Client
        /// </summary>
        /// <param name="baseUri">Uri of the API provided by your account manager</param>
        /// <param name="credentials">Credentials object containing public key and secret key</param>
        public AdferoFeedsClient(string baseUri, AdferoCredentials credentials)
        {
            this.baseUri = baseUri;
            this.credentials = credentials;
        }

        public AdferoFeed Get(int id)
        {
            return this.GetFeed(id, null, null);
        }

        public string GetRaw(int id, string format)
        {
            if (string.IsNullOrEmpty(format))
                throw new ArgumentException("format is required");

            return this.GetFeedRaw(id, null, null, format);
        }

        public AdferoFeedList ListFeeds(int offset, int limit)
        {
            return this.ListFeedsForFeed(offset, limit, null, null);
        }

        public string ListFeedsRaw(int offset, int limit, string format)
        {
            if (string.IsNullOrEmpty(format))
                throw new ArgumentException("format is required");

            return this.ListFeedsForFeedRaw(offset, limit, null, null, format);
        }

        private AdferoFeed GetFeed(int id, string[] properties, string[] fields)
        {
            string uri = this.GetUri(id, null, "xml", properties, fields, null, null);
            uri = string.Format("http://{0}:{1}@{2}", this.credentials.PublicKey, this.credentials.SecretKey, uri.Replace("http://", string.Empty));
            string xml = AdferoHelpers.GetXmlFromUri(uri);

            return this.GetFeedFromXmlString(xml);
        }

        private string GetFeedRaw(int id, string[] properties, string[] fields, string format)
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

        private string ListFeedsForFeedRaw(int offset, int limit, string[] properties, string[] fields, string format)
        {
            string uri = string.Empty;
            switch (format)
            {
                case "xml":
                    uri = this.GetUri(null, null, "xml", properties, fields, offset, limit);
                    break;

                case "json":
                    uri = this.GetUri(null, null, "json", properties, fields, offset, limit);
                    break;

                default:
                    throw new ArgumentException(string.Format("{0} format not supported", format));
            }

            uri = string.Format("http://{0}:{1}@{2}", this.credentials.PublicKey, this.credentials.SecretKey, uri.Replace("http://", string.Empty));
            return AdferoHelpers.GetRawResponse(uri);
        }

        private AdferoFeed GetFeedFromXmlString(string xml)
        {
            AdferoFeed feed = new AdferoFeed();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            foreach (XmlNode n in doc.SelectNodes("//feed/node()"))
            {
                switch (n.Name)
                {
                    case "id":
                        feed.Id = int.Parse(n.InnerText);
                        break;

                    case "name":
                        feed.Name = n.InnerText;
                        break;

                    case "state":
                        feed.State = n.InnerText;
                        break;

                    case "timeZone":
                        feed.TimeZone = n.InnerText;
                        break;

                    default:
                        break;
                }
            }

            return feed;
        }

        private AdferoFeedList ListFeedsForFeed(int offset, int limit, string[] properties, string[] fields)
        {
            string uri = this.GetUri(null, null, "xml", properties, fields, offset, limit);
            uri = string.Format("http://{0}:{1}@{2}", this.credentials.PublicKey, this.credentials.SecretKey, uri.Replace("http://", string.Empty));
            string xml = AdferoHelpers.GetXmlFromUri(uri);

            AdferoFeedList feeds = this.ListFeedsFromXmlString(xml);
            feeds.Limit = limit;
            feeds.Offset = offset;

            return feeds;
        }

        private AdferoFeedList ListFeedsFromXmlString(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            int totalCount = int.Parse(doc.SelectSingleNode("//feeds").Attributes["totalCount"].Value);
            AdferoFeedList feedsList = new AdferoFeedList();
            feedsList.TotalCount = totalCount;

            foreach (XmlNode n in doc.SelectNodes("//feeds/feed"))
            {
                foreach (XmlNode na in n.SelectNodes("id"))
                {
                    AdferoFeedListItem feed = new AdferoFeedListItem();
                    feed.Id = int.Parse(na.InnerText);
                    feedsList.Items.Add(feed);
                }
            }

            return feedsList;
        }

        private string GetUri(int? id, string identifier, string format, string[] properties, string[] fields, int? offset, int? limit)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            if (properties != null && properties.Length > 0)
                data["properties"] = string.Join(",", properties);

            if (fields != null && fields.Length > 0)
                data["fields"] = string.Join(",", fields);

            if ((id == null && identifier == null) || offset != null || limit != null)
            {
                data["offset"] = offset.ToString();
                data["limit"] = limit.ToString();

                List<string> parts = new List<string>();
                foreach (KeyValuePair<string, string> kv in data)
                    parts.Add(string.Format("{0}={1}", kv.Key, kv.Value));
                string queryString = HttpUtility.UrlDecode(string.Join("&", parts.ToArray()));

                return string.Format("{0}feeds.{1}?{2}", this.baseUri, format, queryString);
            }
            else
            {
                List<string> parts = new List<string>();
                foreach (KeyValuePair<string, string> kv in data)
                    parts.Add(string.Format("{0}={1}", kv.Key, kv.Value));
                string queryString = HttpUtility.UrlDecode(string.Join("&", parts.ToArray()));

                return string.Format("{0}feeds/{1}.{2}?{3}", this.baseUri, id, format, queryString);
            }
        }
    }
}