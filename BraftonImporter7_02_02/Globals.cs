using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq;
using System.Data;
using System.Data.SqlClient;

using System.Configuration;
using Brafton.Modules.BraftonImporter7_02_02.dbDataLayer;


namespace Brafton.Modules.Globals
{
    

    public static class MyGlobals
    {
        //public const string Prefix = "ID_"; // cannot change
        public static string MyGlobalError = "Don't Panic"; // can change because not const
        public static string ArtOrBlog = "unk";
        public static string ArchiveLink = "xxxx";
        public static int IncludeVideo = 0;
        public static int IncludeUpdatedFeedContent = 0;
        public static string VideoBaseURL = "api.video.brafton.com";
        public static string VideoPhotoURL = "pictures.video.brafton.com";
        public static string VideoPublicKey = "xxxxxx";
        public static string VideoSecretKey = "xxxxxx";
        public static int? VideoFeedNumber = 0;
        public static string VideoFeedText = "xxxxxx";
        public static string api = "xxxxxx";
        public static string baseUrl = "xxxxxx";
        public static string DomainName = "xxxxxx";

        //These are temp holders for values to entered into the db
        public static string tempID = "0";
        public static string tempTitle = "xxxxxx";
        public static string tempExtract = "xxxxxx";
        public static string tempcontent = "xxxxxx";
        public static DateTime tempDate = DateTime.Today;
        public static string tempPaths = "xxxxxx";

        public static string imageInfo = "xxxxxx";
        public static string imageID = "xxx";

        public static string CompleteContent = "";
        public static string CompleteExtract = "";
        public static int brafID = 0;


            public static void PopGlobals()

                 {
                     using (DNNDataLayerDataContext dnnContext = new DNNDataLayerDataContext())
                     {

                         #region Fill Globals
                         BraftonTable pk = dnnContext.BraftonTables.FirstOrDefault(x => x.Content == "1");

                         if (pk != null)
                         {
                             if (pk.Api != null)
                             {
                                 api = pk.Api;
                             }
                             if (pk.BaseUrl != null)
                            {
                                 baseUrl = pk.BaseUrl;
                            }
                             if (pk.DomainName != null)
                            {
                                 DomainName = pk.DomainName;
                            }
                             
                             
                             
                             if (pk.VideoPublicKey != null)
                                {
                                 VideoPublicKey = pk.VideoPublicKey;
                                 }
                             if (pk.VideoSecretKey != null)
                                {
                                 VideoSecretKey = pk.VideoSecretKey;
                                }
                             if (pk.VideoFeedNumber != null)
                                 {
                                 VideoFeedNumber = pk.VideoFeedNumber;
                                 VideoFeedText = pk.VideoFeedNumber.ToString();
                                  }
                             if (pk.VideoBaseUrl != null)
                             {
                                 VideoBaseURL = pk.VideoBaseUrl;
                             }
                             if (pk.VideoPhotoURL != null)
                             {
                                 VideoPhotoURL = pk.VideoPhotoURL;
                             }

                             //Decide whether photo or video
                             if (pk.VideoSecretKey != null)
                             {
                                 ArtOrBlog = "video";
                             }

                             if (pk.Api != null)
                             {
                                 ArtOrBlog = "articles";
                             }

                             if (pk.Api != null && pk.VideoSecretKey != null)
                             {
                                 ArtOrBlog = "both";
                             }

                         #endregion
                         }
                     
                     
                     }

                 }


    }

    public class XmlBase
    {




        public void WriteToXml()
        {

        }

        public void ReadToXml()
        {

        }

    }




}