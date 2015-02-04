using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdferoVideoDotNet.AdferoArticles.Categories
{
    public class AdferoCategoryList : AdferoListBase
    {
        private List<AdferoCategoryListItem> _items;

        public List<AdferoCategoryListItem> Items { get { return _items; } }

        public AdferoCategoryList()
        {
            _items = new List<AdferoCategoryListItem>();
        }
    }
}