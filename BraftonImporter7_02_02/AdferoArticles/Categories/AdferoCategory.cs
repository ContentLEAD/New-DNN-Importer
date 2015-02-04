using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdferoVideoDotNet.AdferoArticles.Categories
{
    public class AdferoCategory : AdferoEntityBase
    {
        public string Name { get; set; }
        public int ParentId { get; set; }
    }
}