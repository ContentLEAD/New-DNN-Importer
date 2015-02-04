using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace AdferoVideoDotNet.AdferoPhotos
{
    public class AdferoPhotoClient
    {
        protected string baseUri;

        public AdferoPhotoClient(string baseUri)
        {
            Regex reg = new Regex("^http://[a-z0-9-]+(.[a-z0-9-]+)*(:[0-9]+)?(/.*)?$", RegexOptions.IgnoreCase);
            if (!reg.IsMatch(baseUri))
                throw new ArgumentException("Not a valid uri");

            if (!baseUri.EndsWith("/"))
                baseUri += "/";

            this.baseUri = baseUri;
        }

        public AdferoPhotos.Photos.AdferoPhotosClient Photos()
        {
            return new Photos.AdferoPhotosClient(this.baseUri);
        }
    }
}