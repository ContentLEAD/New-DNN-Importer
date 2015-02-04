using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace AdferoVideoDotNet.AdferoArticlesVideoExtensions.VideoPlayers
{
    public class AdferoVideoPlayersClient
    {
        private string baseUri { get; set; }
        private AdferoArticles.AdferoCredentials credentials { get; set; }

        public AdferoVideoPlayersClient(string baseUri, AdferoArticles.AdferoCredentials credentials)
        {
            this.baseUri = baseUri;
            this.credentials = credentials;
        }

        public AdferoVideoPlayer Get(int articleId, string playerName, AdferoVersion playerVersion)
        {
            if (string.IsNullOrEmpty(playerName))
                throw new ArgumentException("playerName is required");

            if (playerVersion == null)
                throw new ArgumentNullException("playerVersion", "playerVersion is required");

            return this.GetVideoPlayer(articleId, playerName, playerVersion, null, null);
        }

        public AdferoVideoPlayer GetWithFallback(int articleId, string playerName, AdferoVersion playerVersion, string fallbackPlayerName, AdferoVersion fallbackPlayerVersion)
        {
            if (string.IsNullOrEmpty(playerName))
                throw new ArgumentException("playerName is required");

            if (string.IsNullOrEmpty(fallbackPlayerName))
                throw new ArgumentException("fallbackPlayerName is required");

            if (playerVersion == null)
                throw new ArgumentNullException("playerVersion", "playerVersion is required");

            if (fallbackPlayerVersion == null)
                throw new ArgumentNullException("fallbackPlayerVersion", "fallbackPlayerVersion is required");

            return this.GetVideoPlayerWithFallback(articleId, playerName, playerVersion, fallbackPlayerName, fallbackPlayerVersion, null, null);
        }

        public string GetRaw(int articleId, string playerName, AdferoVersion playerVersion, string format)
        {
            if (string.IsNullOrEmpty(playerName))
                throw new ArgumentException("playerName is required");

            if (string.IsNullOrEmpty(format))
                throw new ArgumentException("format is required");

            if (playerVersion == null)
                throw new ArgumentNullException("playerVersion", "playerVersion is required");

            return this.GetVideoPlayerRaw(articleId, playerName, playerVersion, null, null, null, null, format);
        }

        public string GetRawWithFallback(int articleId, string playerName, AdferoVersion playerVersion, string fallbackPlayerName, AdferoVersion fallbackPlayerVersion, string format)
        {
            if (string.IsNullOrEmpty(playerName))
                throw new ArgumentException("playerName is required");

            if (string.IsNullOrEmpty(fallbackPlayerName))
                throw new ArgumentException("fallbackPlayerName is required");

            if (string.IsNullOrEmpty(format))
                throw new ArgumentException("format is required");

            if (playerVersion == null)
                throw new ArgumentNullException("playerVersion", "playerVersion is required");

            if (fallbackPlayerVersion == null)
                throw new ArgumentNullException("fallbackPlayerVersion", "fallbackPlayerVersion is required");

            return this.GetVideoPlayerRaw(articleId, playerName, playerVersion, fallbackPlayerName, fallbackPlayerVersion, null, null, format);
        }

        private AdferoVideoPlayer GetVideoPlayer(int articleId, string playerName, AdferoVersion playerVersion, string[] properties, string[] fields)
        {
            string uri = this.GetUri(articleId, playerName, playerVersion, null, null, "xml", properties, fields);
            uri = string.Format("http://{0}:{1}@{2}", this.credentials.PublicKey, this.credentials.SecretKey, uri.Replace("http://", string.Empty));
            string xml = AdferoArticles.AdferoHelpers.GetXmlFromUri(uri);

            return this.GetVideoPlayerFromXmlString(xml);
        }

        private AdferoVideoPlayer GetVideoPlayerWithFallback(int articleId, string playerName, AdferoVersion playerVersion, string fallbackPlayerName, AdferoVersion fallbackPlayerVersion, string[] properties, string[] fields)
        {
            string uri = this.GetUri(articleId, playerName, playerVersion, fallbackPlayerName, fallbackPlayerVersion, "xml", properties, fields);
            uri = string.Format("http://{0}:{1}@{2}", this.credentials.PublicKey, this.credentials.SecretKey, uri.Replace("http://", string.Empty));
            string xml = AdferoArticles.AdferoHelpers.GetXmlFromUri(uri);

            return this.GetVideoPlayerFromXmlString(xml);
        }

        private string GetVideoPlayerRaw(int articleId, string playerName, AdferoVersion playerVersion, string fallbackPlayerName, AdferoVersion fallbackPlayerVersion, string[] properties, string[] fields, string format)
        {
            string uri = string.Empty;

            switch (format)
            {
                case "xml":
                    uri = this.GetUri(articleId, playerName, playerVersion, fallbackPlayerName, fallbackPlayerVersion, "xml", properties, fields);
                    break;

                case "json":
                    uri = this.GetUri(articleId, playerName, playerVersion, fallbackPlayerName, fallbackPlayerVersion, "json", properties, fields);
                    break;

                default:
                    throw new ArgumentException(string.Format("{0} format not supported", format));
            }

            uri = string.Format("http://{0}:{1}@{2}", this.credentials.PublicKey, this.credentials.SecretKey, uri.Replace("http://", string.Empty));
            return AdferoArticles.AdferoHelpers.GetRawResponse(uri);
        }

        private AdferoVideoPlayer GetVideoPlayerFromXmlString(string xml)
        {
            AdferoVideoPlayer videoPlayer = new AdferoVideoPlayer();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            foreach (XmlNode n in doc.SelectNodes("//player/node()"))
            {
                switch (n.Name)
                {
                    case "embedCode":
                        videoPlayer.EmbedCode = n.InnerText;
                        break;

                    default:
                        break;
                }
            }

            return videoPlayer;
        }

        private string GetUri(int articleId, string playerName, AdferoVersion playerVersion, string fallbackPlayerName, AdferoVersion fallbackPlayerVersion, string format, string[] properties, string[] fields)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            if (properties != null && properties.Length > 0)
                data["properties"] = string.Join(",", properties);

            if (fields != null && fields.Length > 0)
                data["fields"] = string.Join(",", fields);

            data["articleId"] = articleId.ToString();
            data["playerVersion"] = playerVersion.ToString();

            if (!string.IsNullOrEmpty(fallbackPlayerName))
                data["fallbackPlayerName"] = fallbackPlayerName;

            if (fallbackPlayerVersion != null)
                data["fallbackPlayerVersion"] = fallbackPlayerVersion.ToString();

            List<string> parts = new List<string>();
            foreach (KeyValuePair<string, string> kv in data)
                parts.Add(string.Format("{0}={1}", kv.Key, kv.Value));
            string queryString = HttpUtility.UrlDecode(string.Join("&", parts.ToArray()));

            return string.Format("{0}players/{1}.{2}?{3}", this.baseUri, playerName, format, queryString);
        }
    }
}