using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdferoVideoDotNet.AdferoArticles.Briefs
{
    public class AdferoBrief : AdferoEntityBase
    {
        public string Name { get; set; }
        public int FeedId { get; set; }
    }
}