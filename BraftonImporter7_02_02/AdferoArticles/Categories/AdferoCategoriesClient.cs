using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace AdferoVideoDotNet.AdferoArticles.Categories
{
    public class AdferoCategoriesClient
    {
        private string baseUri;
        private AdferoCredentials credentials;

        /// <summary>
        /// Initialises a new instance of the Categories Client
        /// </summary>
        /// <param name="baseUri">Uri of the API provided by your account manager</param>
        /// <param name="credentials">Credentials object containing public key and secret key</param>
        public AdferoCategoriesClient(string baseUri, AdferoCredentials credentials)
        {
            this.baseUri = baseUri;
            this.credentials = credentials;
        }

        public AdferoCategory Get(int id)
        {
            return this.GetCategory(id, null, null);
        }

        public string GetRaw(int id, string format)
        {
            if (string.IsNullOrEmpty(format))
                throw new ArgumentException("format is required");

            return this.GetCategoryRaw(id, null, null, format);
        }

        private AdferoCategoryList ListForFeed(int feedId, int offset, int limit)
        {
            return this.ListCategoriesForFeed(feedId, offset, limit, null, null);
        }

        public string ListForFeedRaw(int feedId, int offset, int limit, string format)
        {
            if (string.IsNullOrEmpty(format))
                throw new ArgumentException("format is required");

            return this.ListCategoriesForFeedRaw(feedId, offset, limit, null, null, format);
        }

        public AdferoCategoryList ListForArticle(int articleId, int offset, int limit)
        {
            return this.ListCategoriesForArticle(articleId, offset, limit, null, null);
        }

        public string ListForArticleRaw(int feedId, int offset, int limit, string format)
        {
            if (string.IsNullOrEmpty(format))
                throw new ArgumentException("format is required");

            return this.ListCategoriesForArticleRaw(feedId, offset, limit, null, null, format);
        }

        private AdferoCategory GetCategory(int id, string[] properties, string[] fields)
        {
            string uri = this.GetUri(id, null, "xml", properties, fields, null, null);
            uri = string.Format("http://{0}:{1}@{2}", this.credentials.PublicKey, this.credentials.SecretKey, uri.Replace("http://", string.Empty));
            string xml = AdferoHelpers.GetXmlFromUri(uri);

            return this.GetCategoryFromXmlString(xml);
        }

        private string GetCategoryRaw(int id, string[] properties, string[] fields, string format)
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

        private string ListCategoriesForFeedRaw(int feedId, int offset, int limit, string[] properties, string[] fields, string format)
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

        private string ListCategoriesForArticleRaw(int articleId, int offset, int limit, string[] properties, string[] fields, string format)
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

        private AdferoCategory GetCategoryFromXmlString(string xml)
        {
            AdferoCategory category = new AdferoCategory();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            foreach (XmlNode n in doc.SelectNodes("//category/node()"))
            {
                switch (n.Name)
                {
                    case "id":
                        category.Id = int.Parse(n.InnerText);
                        break;

                    case "name":
                        category.Name = n.InnerText;
                        break;

                    case "parentId":
                        category.ParentId = int.Parse(n.InnerText);
                        break;

                    default:
                        break;
                }
            }

            return category;
        }

        private AdferoCategoryList ListCategoriesForFeed(int feedId, int offset, int limit, string[] properties, string[] fields)
        {
            string uri = this.GetUri(feedId, "feedId", "xml", properties, fields, offset, limit);
            uri = string.Format("http://{0}:{1}@{2}", this.credentials.PublicKey, this.credentials.SecretKey, uri.Replace("http://", string.Empty));
            string xml = AdferoHelpers.GetXmlFromUri(uri);

            AdferoCategoryList Categories = this.ListCategoriesFromXmlString(xml);
            Categories.Limit = limit;
            Categories.Offset = offset;

            return Categories;
        }

        private AdferoCategoryList ListCategoriesForArticle(int articleId, int offset, int limit, string[] properties, string[] fields)
        {
            string uri = this.GetUri(articleId, "articleId", "xml", properties, fields, offset, limit);
            uri = string.Format("http://{0}:{1}@{2}", this.credentials.PublicKey, this.credentials.SecretKey, uri.Replace("http://", string.Empty));
            string xml = AdferoHelpers.GetXmlFromUri(uri);

            AdferoCategoryList categories = this.ListCategoriesFromXmlString(xml);
            categories.Limit = limit;
            categories.Offset = offset;

            return categories;
        }

        private AdferoCategoryList ListCategoriesFromXmlString(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            int totalCount = int.Parse(doc.SelectSingleNode("//categories").Attributes["totalCount"].Value);
            AdferoCategoryList categoryItems = new AdferoCategoryList();
            categoryItems.TotalCount = totalCount;

            foreach (XmlNode n in doc.SelectNodes("//categories/category"))
            {
                foreach (XmlNode na in n.SelectNodes("id"))
                {
                    AdferoCategoryListItem category = new AdferoCategoryListItem();
                    category.Id = int.Parse(na.InnerText);
                    categoryItems.Items.Add(category);
                }
            }

            return categoryItems;
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

                return string.Format("{0}categories.{1}?{2}", this.baseUri, format, queryString);
            }
            else
            {
                List<string> parts = new List<string>();
                foreach (KeyValuePair<string, string> kv in data)
                    parts.Add(string.Format("{0}={1}", kv.Key, kv.Value));
                string queryString = HttpUtility.UrlDecode(string.Join("&", parts.ToArray()));

                return string.Format("{0}categories/{1}.{2}?{3}", this.baseUri, id, format, queryString);
            }
        }
    }
}