using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdferoVideoDotNet.AdferoArticles
{
    public abstract class AdferoListBase
    {
        public int TotalCount { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
    }
}