using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdferoVideoDotNet.AdferoArticles.Briefs
{
    public class AdferoBriefList : AdferoVideoDotNet.AdferoArticles.AdferoListBase
    {
        private List<AdferoBriefListItem> _items;

        public List<AdferoBriefListItem> Items { get { return _items; } }

        public AdferoBriefList()
        {
            _items = new List<AdferoBriefListItem>();
        }
    }
}