using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace AdferoVideoDotNet.AdferoArticles
{
    public class AdferoClient
    {
        protected string baseUri;
        protected AdferoCredentials credentials;

        public AdferoClient(string baseUri, string publicKey, string secretKey)
        {
            Regex regBase = new Regex("^http://[a-z0-9-]+(.[a-z0-9-]+)*(:[0-9]+)?(/.*)?$", RegexOptions.IgnoreCase);
            if (!regBase.IsMatch(baseUri))
                throw new ArgumentException("Not a valid uri");

            if (!baseUri.EndsWith("/"))
                baseUri += "/";

            this.baseUri = baseUri;
            this.credentials = new AdferoCredentials(publicKey, secretKey);
        }

        public AdferoArticles.Articles.AdferoArticlesClient Articles()
        {
            return new Articles.AdferoArticlesClient(this.baseUri, this.credentials);
        }

        public AdferoArticles.ArticlePhotos.AdferoArticlePhotosClient ArticlePhotos()
        {
            return new ArticlePhotos.AdferoArticlePhotosClient(this.baseUri, this.credentials);
        }

        public AdferoArticles.Briefs.AdferoBriefsClient Briefs()
        {
            return new Briefs.AdferoBriefsClient(this.baseUri, this.credentials);
        }

        public AdferoArticles.Categories.AdferoCategoriesClient Categories()
        {
            return new Categories.AdferoCategoriesClient(this.baseUri, this.credentials);
        }

        public AdferoArticles.Feeds.AdferoFeedsClient Feeds()
        {
            return new Feeds.AdferoFeedsClient(this.baseUri, this.credentials);
        }
    }
}