using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdferoVideoDotNet.AdferoArticles;

namespace AdferoVideoDotNet.AdferoArticlesVideoExtensions.VideoOutputs
{
    public class AdferoVideoOutput:AdferoEntityBase
    {
        public string Type { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Path { get; set; }
    }
}