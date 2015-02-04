using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace AdferoVideoDotNet.AdferoArticles.Briefs
{
    public class AdferoBriefsClient
    {
        private string baseUri;
        private AdferoCredentials credentials;

        /// <summary>
        /// Initialises a new instance of the Briefs Client
        /// </summary>
        /// <param name="baseUri">Uri of the API provided by your account manager</param>
        /// <param name="credentials">Credentials object containing public key and secret key</param>
        public AdferoBriefsClient(string baseUri, AdferoCredentials credentials)
        {
            this.baseUri = baseUri;
            this.credentials = credentials;
        }

        public AdferoBrief Get(int id)
        {
            return this.GetBrief(id, null, null);
        }

        public string GetRaw(int id, string format)
        {
            if (string.IsNullOrEmpty(format))
                throw new ArgumentException("format is required");

            return this.GetBriefRaw(id, null, null, format);
        }

        private AdferoBriefList ListForFeed(int feedId, int offset, int limit)
        {
            return this.ListBriefsForFeed(feedId, offset, limit, null, null);
        }

        private string ListForFeedRaw(int feedId, int offset, int limit, string format)
        {
            if (string.IsNullOrEmpty(format))
                throw new ArgumentException("format is required");

            return this.ListBriefsForFeedRaw(feedId, offset, limit, null, null, format);
        }

        private AdferoBrief GetBrief(int id, string[] properties, string[] fields)
        {
            string uri = this.GetUri(id, null, "xml", properties, fields, null, null);
            uri = string.Format("http://{0}:{1}@{2}", this.credentials.PublicKey, this.credentials.SecretKey, uri.Replace("http://", string.Empty));
            string xml = AdferoHelpers.GetXmlFromUri(uri);

            return this.GetBriefFromXmlString(xml);
        }

        private string GetBriefRaw(int id, string[] properties, string[] fields, string format)
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

        private string ListBriefsForFeedRaw(int feedId, int offset, int limit, string[] properties, string[] fields, string format)
        {
            string uri = string.Empty;
            switch (format)
            {
                case "xml":
                    uri = this.GetUri(feedId, "feedId", "xml", properties, fields, offset, limit);
                    break;

                case "json":
                    uri = this.GetUri(feedId, "feedId", "json", properties, fields, offset, limit);
                    break;

                default:
                    throw new ArgumentException(string.Format("{0} format not supported", format));
            }

            uri = string.Format("http://{0}:{1}@{2}", this.credentials.PublicKey, this.credentials.SecretKey, uri.Replace("http://", string.Empty));
            return AdferoHelpers.GetRawResponse(uri);
        }

        private AdferoBrief GetBriefFromXmlString(string xml)
        {
            AdferoBrief brief = new AdferoBrief();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            foreach (XmlNode n in doc.SelectNodes("//brief/node()"))
            {
                switch (n.Name)
                {
                    case "id":
                        brief.Id = int.Parse(n.InnerText);
                        break;

                    case "name":
                        brief.Name = n.InnerText;
                        break;

                    case "feedId":
                        brief.FeedId = int.Parse(n.InnerText);
                        break;

                    default:
                        break;
                }
            }

            return brief;
        }

        private AdferoBriefList ListBriefsForFeed(int feedId, int offset, int limit, string[] properties, string[] fields)
        {
            string uri = this.GetUri(feedId, "feedId", "xml", properties, fields, offset, limit);
            uri = string.Format("http://{0}:{1}@{2}", this.credentials.PublicKey, this.credentials.SecretKey, uri.Replace("http://", string.Empty));
            string xml = AdferoHelpers.GetXmlFromUri(uri);

            AdferoBriefList briefs = this.ListBriefsFromXmlString(xml);
            briefs.Limit = limit;
            briefs.Offset = offset;

            return briefs;
        }

        private AdferoBriefList ListBriefsFromXmlString(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            int totalCount = int.Parse(doc.SelectSingleNode("//briefs").Attributes["totalCount"].Value);
            AdferoBriefList briefItems = new AdferoBriefList();
            briefItems.TotalCount = totalCount;

            foreach (XmlNode n in doc.SelectNodes("//briefs/brief"))
            {
                foreach (XmlNode na in n.SelectNodes("id"))
                {
                    AdferoBriefListItem brief = new AdferoBriefListItem();
                    brief.Id = int.Parse(na.InnerText);
                    briefItems.Items.Add(brief);
                }
            }

            return briefItems;
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

                return string.Format("{0}briefs.{1}?{2}", this.baseUri, format, queryString);
            }
            else
            {
                List<string> parts = new List<string>();
                foreach (KeyValuePair<string, string> kv in data)
                    parts.Add(string.Format("{0}={1}", kv.Key, kv.Value));
                string queryString = HttpUtility.UrlDecode(string.Join("&", parts.ToArray()));

                return string.Format("{0}briefs/{1}.{2}?{3}", this.baseUri, id, format, queryString);
            }
        }
    }
}