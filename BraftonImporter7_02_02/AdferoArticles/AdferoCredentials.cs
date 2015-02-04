using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdferoVideoDotNet.AdferoArticles
{
    public struct AdferoCredentials
    {
        public string PublicKey { get; set; }
        public string SecretKey { get; set; }

        public AdferoCredentials(string publicKey, string secretKey)
            :this()
        {
            PublicKey = publicKey;
            SecretKey = secretKey;
        }
    }
}