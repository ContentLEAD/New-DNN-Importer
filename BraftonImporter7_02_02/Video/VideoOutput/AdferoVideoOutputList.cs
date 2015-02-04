using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdferoVideoDotNet.AdferoArticlesVideoExtensions.VideoOutputs
{
    public class AdferoVideoOutputList : AdferoVideoDotNet.AdferoArticles.AdferoListBase
    {
        private List<AdferoVideoOutputListItem> _items;

        public List<AdferoVideoOutputListItem> Items { get { return _items; } }

        public AdferoVideoOutputList()
        {
            _items = new List<AdferoVideoOutputListItem>();
        }
    }
}