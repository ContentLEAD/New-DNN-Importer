using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdferoVideoDotNet.AdferoArticles.Articles
{
    public class AdferoArticleList : AdferoListBase
    {
        private List<AdferoArticleListItem> _items;

        public List<AdferoArticleListItem> Items { get { return _items; } }

        public AdferoArticleList()
        {
            _items = new List<AdferoArticleListItem>();
        }
    }
}