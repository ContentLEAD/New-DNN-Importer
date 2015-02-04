using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdferoVideoDotNet.AdferoArticles.Articles
{
    public class AdferoArticle : AdferoEntityBase
    {
        private Dictionary<string, string> _fields;

        public int FeedId { get; set; }
        public int BriefId { get; set; }
        public string State { get; set; }
        public Dictionary<string, string> Fields { get { return _fields; } }

        public AdferoArticle()
        {
            _fields = new Dictionary<string, string>();
        }
    }
}