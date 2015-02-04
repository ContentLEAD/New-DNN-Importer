using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdferoVideoDotNet.AdferoArticles.Feeds
{
    public class AdferoFeedList : AdferoListBase
    {
        private List<AdferoFeedListItem> _items;

        public List<AdferoFeedListItem> Items { get { return _items; } }

        public AdferoFeedList()
        {
            _items = new List<AdferoFeedListItem>();
        }
    }
}