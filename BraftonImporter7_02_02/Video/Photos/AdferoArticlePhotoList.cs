using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdferoVideoDotNet.AdferoArticles.ArticlePhotos
{
    public class AdferoArticlePhotoList : AdferoListBase
    {
        private List<AdferoArticlePhotoListItem> _items;

        public List<AdferoArticlePhotoListItem> Items { get { return _items; } }

        public AdferoArticlePhotoList()
        {
            _items = new List<AdferoArticlePhotoListItem>();
        }
    }
}