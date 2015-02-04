using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdferoVideoDotNet.AdferoArticles.Feeds
{
    public class AdferoFeed : AdferoEntityBase
    {
        public string Name { get; set; }
        public string State { get; set; }
        public string TimeZone { get; set; }
    }
}