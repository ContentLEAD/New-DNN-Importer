using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using Brafton.Modules.BraftonImporter7_02_02.dbDataLayer;
using Brafton.Modules.Globals;
using AdferoVideoDotNet.AdferoArticles;
using AdferoVideoDotNet.AdferoPhotos;

namespace Brafton.Modules.VideoImporter
{
    public class BraftonVideoClass
    {


            

        //validation methods
        public static bool ValidateVideoPublicKey(string publicKey)
        {
            Regex reg = new Regex("[a-f0-9]{8}", RegexOptions.IgnoreCase);
            return reg.IsMatch(publicKey);
        }

        public static bool ValidateGuid(string guid)
        {
            Regex reg = new Regex("[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}", RegexOptions.IgnoreCase);
            return reg.IsMatch(guid);
        }

        public static void AddUpdatePublicKey(string Key)
        {
            using (DNNDataLayerDataContext dnnContext = new DNNDataLayerDataContext())
            {
               
                #region Update
                BraftonTable pk = dnnContext.BraftonTables.FirstOrDefault(x => x.Content == "1");

                if (pk != null)
                {
                     //pk.VideoPublicKey = TEST;
                     pk.VideoPublicKey = Key;
                     MyGlobals.VideoPublicKey = Key;

                  
                     
                    dnnContext.SubmitChanges();
                   // MyGlobals.VideoFeedNumber = "pk is not NULL";


                #endregion
                }
                else
                {
                    //Blog_Entry newBlogEntry = new Blog_Entry();
                    BraftonTable addPublicKey = new BraftonTable();

                    addPublicKey.VideoPublicKey = Key;

                    dnnContext.BraftonTables.InsertOnSubmit(addPublicKey);
                    dnnContext.SubmitChanges();
                    MyGlobals.VideoFeedNumber = 0;

                }
            }
        }
        public static void AddUpdateSecretKey(string Key)
        {
            using (DNNDataLayerDataContext dnnContext = new DNNDataLayerDataContext())
            {

                #region Update
                BraftonTable pk = dnnContext.BraftonTables.FirstOrDefault(x => x.Content == "1");

                if (pk != null)
                {
                       
                     pk.VideoSecretKey = Key;
                     MyGlobals.VideoSecretKey = Key;
                     dnnContext.SubmitChanges();
                #endregion
                }
                else
                {


                }
            }
        }

        public static void AddUpdateVideoBaseURL(string Key)//TODO still need to validate URL to see if it matches either of the 2 apis
        {
            using (DNNDataLayerDataContext dnnContext = new DNNDataLayerDataContext())
            {

                #region Update
                BraftonTable pk = dnnContext.BraftonTables.FirstOrDefault(x => x.Content == "1");

                if (pk != null)
                {

                    pk.VideoBaseUrl = Key;
                    MyGlobals.VideoBaseURL = Key;
                    dnnContext.SubmitChanges();
                #endregion
                }
                else
                {


                }
            }
        }
        public static void AddUpdateVideoPhotoURL(string Key)//TODO still need to validate URL to see if it matches either of the 2 apis
        {
            using (DNNDataLayerDataContext dnnContext = new DNNDataLayerDataContext())
            {

                #region Update
                BraftonTable pk = dnnContext.BraftonTables.FirstOrDefault(x => x.Content == "1");

                if (pk != null)
                {

                    pk.VideoPhotoURL = Key;
                    MyGlobals.VideoPhotoURL = Key;
                    dnnContext.SubmitChanges();
                #endregion
                }
                else
                {


                }
            }
        }
        public static void AddUpdateFeedNum(int Key)
        {
            using (DNNDataLayerDataContext dnnContext = new DNNDataLayerDataContext())
            {

                #region Update
                Brafton.Modules.BraftonImporter7_02_02.dbDataLayer.BraftonTable pk = dnnContext.BraftonTables.FirstOrDefault(x => x.Content == "1");

                if (pk != null)
                {
                     pk.VideoFeedNumber = Key;
                     MyGlobals.VideoFeedNumber = Key;



                    dnnContext.SubmitChanges();
                #endregion
                }
                else
                {


                }
            }
        }

        public static int InsertVideoPost()
        {
            using (DNNDataLayerDataContext dnnContext = new DNNDataLayerDataContext())
            {
                #region Get all the values
                //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "ID:" + brafId + "Date:" + article.Fields["date"] + "CONTENT:" + article.Fields["content"] + "<br>";
                
                //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Paths   " + MyGlobals.tempPaths + "<br>";




                string BraftonID = MyGlobals.tempID;
                string Title = MyGlobals.tempTitle;
                string Extract = MyGlobals.tempExtract;
                DateTime AddedDate = MyGlobals.tempDate;
                DateTime LastUpdate = DateTime.Today;
                int brafID  = MyGlobals.brafID;

                //we still need to build permalinks
                string permalink = "";


                int returnId;
                //add the embed code to the content
                string embeddedEntry = createEmbedCode();



                //now lets add the photos
                addPhotos(brafID, embeddedEntry, Extract);

                string completeEntry = MyGlobals.CompleteContent;
                string completeExtract = MyGlobals.CompleteExtract;

                //MyGlobals.imageInfo = MyGlobals.imageInfo + completeExtract;

                #endregion

                Brafton.Modules.BraftonImporter7_02_02.dbDataLayer.Blog_Entry pk = dnnContext.Blog_Entries.FirstOrDefault(x => x.BraftonID == BraftonID);




                //IF there is an article then we update
                if (pk != null )
                {
                    //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "<br>***********************************************<br>";
                    //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "************Check DB**********************<br>";
                    //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + completeExtract;
                    //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "************Updating Extract**********************<br>";
                    //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + completeExtract;
                    //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "<br>***********************************************<br>";
                    //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "************Updating Entry/Content**********************<br>";
                    //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + completeEntry;
                    //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "<br>***********************************************<br>";

                    int lastDayUpd = pk.LastUpdatedOn.Value.DayOfYear;
                    int todayDay = 0;
                    //int todayDay = DateTime.Today.DayOfYear; TODO turn this back on
                    returnId = pk.EntryID; //Needed to check for updates to Categories
                    //Unless it was updated today
                    if (lastDayUpd != todayDay) { 
                #region Update
                    

                    pk.Title = Title;
                    pk.Published = true;//This is default unless the client wants to have articles go into draft status this functionality is not built at all as of yet
                    pk.Description = completeExtract;
                    pk.BraftonID = BraftonID;
                    pk.Entry = completeEntry;
                    pk.LastUpdatedOn = LastUpdate;
                    pk.AddedDate = AddedDate;
                    pk.BlogID = 1;
                    pk.PermaLink = permalink;
                    pk.AllowComments = false;

                    dnnContext.SubmitChanges();

                #endregion
                    }
                }
                else
                //Else we insert
                {
                    #region Insert


                    

                    Blog_Entry newBlogEntry = new Blog_Entry();

                    newBlogEntry.Title = Title;
                    newBlogEntry.Published = true;//This is default unless the client wants to have articles go into draft status this functionality is not built at all as of yet
                    newBlogEntry.Description = completeExtract;
                    newBlogEntry.BraftonID = BraftonID;
                    newBlogEntry.Entry = completeEntry;
                    newBlogEntry.LastUpdatedOn = LastUpdate;
                    newBlogEntry.AddedDate = AddedDate;
                    newBlogEntry.BlogID = 1;
                    newBlogEntry.PermaLink = permalink;
                    newBlogEntry.AllowComments = false;

                    dnnContext.Blog_Entries.InsertOnSubmit(newBlogEntry);
                    dnnContext.SubmitChanges();

                    returnId = newBlogEntry.EntryID;

                    #endregion
                }

                return returnId;
            }
        }

        public static string createEmbedCode()
        {
            string embedCode;

            string paths = MyGlobals.tempPaths;


            string startContent = MyGlobals.tempcontent;


            embedCode = paths + startContent;


            return embedCode;
        }
#region Photos
        public static void addPhotos(int brafId, string entry, string extract)
        {

       
                //AdferoVideoOutputsClient xc = new AdferoVideoClient(baseUrl, publicKey, secretKey).VideoOutputs();
            
            string publicKey = MyGlobals.VideoPublicKey;
            string secretKey = MyGlobals.VideoSecretKey;
            string baseUrl = "http://" + MyGlobals.VideoBaseURL + "/v2/";

            //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "************Just checking the BAseURL**********************<br>";
            //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + baseUrl;
            //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "<br>***********************************************<br>";

            //in case there are no photos
            MyGlobals.CompleteContent = entry;
            MyGlobals.CompleteExtract = extract;



             AdferoClient client = new AdferoClient(baseUrl, publicKey, secretKey);
             AdferoVideoDotNet.AdferoArticles.ArticlePhotos.AdferoArticlePhotosClient photos = client.ArticlePhotos();

                foreach (AdferoVideoDotNet.AdferoArticles.ArticlePhotos.AdferoArticlePhotoListItem phot in photos.ListForArticle(brafId, 0, 20).Items) 
                {
                    //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "////////////////////////Display entry Preimage///////////////////////////////<br>";
                    //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Here is a photo" + phot.Id;
                    //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "<br>/////////////////////////////////////////////////////////////////////////<br>";
                //string test = photos.Get(brafId).SourcePhotoId.ToString();
                    int imageID = phot.Id;
                    int photoID = photos.Get(imageID).SourcePhotoId;
                    string alttext = photos.Get(imageID).Fields["altText"];
                    string caption = photos.Get(imageID).Fields["caption"];

                    MyGlobals.imageID = photoID.ToString();

                    string photoURL = "http://" + MyGlobals.VideoPhotoURL +"/v2/photo/" + photoID + ".jpg";
                    //TODO still need to add global value here

                    //string entry = "TEST CONTENT";
                    //string description = "TEST EXTRACT";
                    string appPath = HttpRuntime.AppDomainAppVirtualPath == "/" ? appPath = "" : appPath = HttpRuntime.AppDomainAppVirtualPath;



                    GetImages retrieveImage2 = new GetImages(photoURL, entry, extract, appPath, caption);
                    retrieveImage2.DownloadImageDebug();



                    MyGlobals.CompleteContent = retrieveImage2._entry;
                    MyGlobals.CompleteExtract = retrieveImage2._description;
                }

        }
#endregion Photos
}
}