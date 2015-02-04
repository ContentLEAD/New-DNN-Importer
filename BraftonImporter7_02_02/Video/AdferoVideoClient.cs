using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdferoVideoDotNet.AdferoArticlesVideoExtensions.VideoOutputs;
using System.Text.RegularExpressions;
using AdferoVideoDotNet.AdferoArticlesVideoExtensions.VideoPlayers;

namespace AdferoVideoDotNet.AdferoArticlesVideoExtensions
{
    public class AdferoVideoClient : AdferoArticles.AdferoClient
    {
        public AdferoVideoClient(string baseUri, string publicKey, string secretKey)
            : base(baseUri, publicKey, secretKey)
        {
        }

        public AdferoVideoOutputsClient VideoOutputs()
        {
            return new AdferoVideoOutputsClient(this.baseUri, this.credentials);
        }

        public AdferoVideoPlayersClient VideoPlayers()
        {
            return new AdferoVideoPlayersClient(this.baseUri, this.credentials);
        }
    }
}