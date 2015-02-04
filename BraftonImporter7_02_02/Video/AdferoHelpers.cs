using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.IO;
using System.Net;

namespace AdferoVideoDotNet.AdferoArticles
{
    public class AdferoHelpers
    {
        public static string GetXmlFromUri(string uri)
        {
            Uri u = new Uri(uri);
            HttpWebRequest req = WebRequest.Create(uri) as HttpWebRequest;
            if (!string.IsNullOrEmpty(u.UserInfo))
            {
                string userInfo = u.UserInfo;
                string userName = userInfo;
                string password = "";
                int length = userInfo.IndexOf(':');
                if (length != -1)
                {
                    userName = Uri.UnescapeDataString(userInfo.Substring(0, length));
                    int startIndex = length + 1;
                    password = Uri.UnescapeDataString(userInfo.Substring(startIndex, userInfo.Length - startIndex));
                }

                NetworkCredential networkCredential = new NetworkCredential(userName, password);
                req.Credentials = networkCredential;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(req.GetResponse().GetResponseStream());

            return doc.OuterXml;
        }

        public static string GetRawResponse(string uri)
        {
            string result = string.Empty;
            WebRequest request = WebRequest.Create(uri);
            request.Timeout = 30 * 60 * 1000;
            request.UseDefaultCredentials = true;
            request.Proxy.Credentials = request.Credentials;
            WebResponse response = (WebResponse)request.GetResponse();

            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                result = sr.ReadToEnd();
            }

            return result;
        }
    }
}