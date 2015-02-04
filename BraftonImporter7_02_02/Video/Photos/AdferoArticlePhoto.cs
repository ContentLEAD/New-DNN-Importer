using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdferoVideoDotNet.AdferoArticles.ArticlePhotos
{
    public class AdferoArticlePhoto : AdferoEntityBase
    {
        private Dictionary<string, string> _fields;

        public int SourcePhotoId { get; set; }
        public Dictionary<string, string> Fields { get { return _fields; } }

        public AdferoArticlePhoto()
        {
            _fields = new Dictionary<string, string>();
        }
    }
}