using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdferoVideoDotNet.AdferoArticlesVideoExtensions.VideoPlayers
{
    public class AdferoVersion
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Build { get; set; }

        public AdferoVersion(int major, int minor, int build)
        {
            this.Major = major;
            this.Minor = minor;
            this.Build = build;
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}", this.Major, this.Minor, this.Build);
        }
    }
}