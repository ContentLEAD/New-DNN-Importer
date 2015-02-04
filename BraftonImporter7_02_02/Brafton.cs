
using System;
using System.IO;
using System.Web;

using DotNetNuke.Services.Scheduling;


using DotNetNuke.Services.Exceptions;
using DotNetNuke.Common.Utilities;

using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

using System.Data.SqlClient;
using System.Xml;

using DotNetNuke.Entities.Portals;
using DotNetNuke;
using DotNetNuke.Security;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Common;
using System.Text.RegularExpressions;

using System.Text;
using System.Web.Hosting;

using System.Net;
using System.Drawing;
using System.Reflection;
using Brafton.Modules.Globals;
using Brafton.Modules.BraftonImporter7_02_02.dbDataLayer;

using AdferoVideoDotNet.AdferoArticlesVideoExtensions;
using Brafton.Modules.VideoImporter;
using AdferoVideoDotNet.AdferoArticles;
using AdferoVideoDotNet.AdferoPhotos.Photos;
using AdferoVideoDotNet.AdferoPhotos;
using AdferoVideoDotNet.AdferoArticlesVideoExtensions.VideoOutputs;


namespace Brafton.DotNetNuke
{

    public class BraftonSchedule : SchedulerClient
    {
        //Connection properties



        public SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SiteSqlServer"].ToString());
        public SqlConnection con2;
        public SqlCommand cmd = new SqlCommand();
        public SqlCommand cmd5;
        public BraftonSchedule(ScheduleHistoryItem objScheduleHistoryItem)
            : base()
        {
            ScheduleHistoryItem = objScheduleHistoryItem;

        }

        // Note we need to give a default constructor when override it
        public BraftonSchedule() : base() { }

        #region Debugging


    //    public string DoWorkDebug()
    //    {

    //        try
    //        {

    //            string description = updateScriptDebug();
    //            return description; //left for quick error reporting
    //            // then report success to the scheduler framework
    //           // ScheduleHistoryItem.Succeeded = true;
    //        }

    //// handle any exceptions
    //        catch (Exception exc)
    //        {
    //            // report a failure
    //            //   ScheduleHistoryItem.Succeeded = false;

    //            // log the exception into
    //            // the scheduler framework
    //               ScheduleHistoryItem.AddLogNote("EXCEPTION: " + exc.ToString());

    //            // call the Errored method
    //            Errored(ref exc);

    //            //log the exception into the DNN core
    //            Exceptions.LogException(exc);
    //            string ErrorMessage = "EXCEPTION";
    //            return ErrorMessage;
    //        }


    //    }
        public override void DoWork()
        {
            try
            {
                // start the process
                //string ups = updateScript();
                //updateScript();
                string description = updateScriptDebug();
                //return description; //left for quick error reporting
                // then report success to the scheduler framework
                ScheduleHistoryItem.Succeeded = true;

            }

        //     handle any exceptions
            catch (Exception exc)
            {
                // report a failure
                ScheduleHistoryItem.Succeeded = false;

                // log the exception into
                // the scheduler framework
                ScheduleHistoryItem.AddLogNote("EXCEPTION: " + exc.ToString());

                // call the Errored method
                Errored(ref exc);

                //log the exception into the DNN core
                Exceptions.LogException(exc);
                //string ErrorMessage = "EXCEPTION";
                //return ErrorMessage;
                MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Exception";
            }


        }
        #endregion

        #region GetBraftonSettings
        public string getNewsURL()
        {

            cmd.CommandText = "SELECT Api FROM Brafton WHERE content='1'";
            string feedURL = cmd.ExecuteScalar().ToString();
            return feedURL;
        }

        public string getBaseURL()
        {
            cmd.CommandText = "SELECT BaseUrl FROM Brafton WHERE content='1'";
            string baseURL = cmd.ExecuteScalar().ToString();
            return baseURL;
        }

        int getBlogID()
        {
            cmd.CommandText = "Select BlogId from Brafton Where Content = '1'";
            int blogID = (int)cmd.ExecuteScalar();
            return blogID;
        }
        public int getUpdatedContent()
        {
            int? UpdatedContent = 0;
            //cmd5 = new SqlCommand();

            using (DNNDataLayerDataContext dnnContext = new DNNDataLayerDataContext())
            {
                Brafton.Modules.BraftonImporter7_02_02.dbDataLayer.BraftonTable uf = dnnContext.BraftonTables.FirstOrDefault(x => x.Content == "1");

                //Insert into category
                if (uf != null)

                {
                    if (uf.IncUpdatedFeedContentValue != null)
                    {
                        //int? v1 =  uf.IncUpdatedFeedContentValue;
                        //UpdatedContent = v1 ?? default(int);
                        UpdatedContent = uf.IncUpdatedFeedContentValue;
                        

                    }
                    else
                    {
                        UpdatedContent = 0;
                        uf.IncUpdatedFeedContentValue = 0;

                        dnnContext.SubmitChanges();
                        
                    }
                   
                }
                
            }
            int? v1 = UpdatedContent;
            int v2 = v1 ?? default(int);
            return v2;
        }

        public string getUpdatedContent2()
        {
            var xx = "Select IncUpdatedFeedContentValue from Brafton Where Content = '1'";


            return xx;
        }

        int getPortalID()
        {
            cmd.CommandText = "Select PortalId from Brafton Where Content = '1'";
            int intPortalID = (int)cmd.ExecuteScalar();
            return intPortalID;
        }

        int getTabID()
        {
            cmd.CommandText = "Select TabId from Brafton Where Content = '1'";
            int PageTabID = (int)cmd.ExecuteScalar();
            return PageTabID;
        }



        int getLimit()
        {
            cmd.CommandText = "Select Limit from Brafton Where Content = '1'";
            if (!DBNull.Value.Equals(cmd.ExecuteScalar()))
            {
                return (int)cmd.ExecuteScalar();
            }
            else
            {

                return 5;
            }
        }

        string getDomainName()
        {
            cmd.CommandText = "Select DomainName from Brafton Where Content = '1'";
            string domainName = (string)cmd.ExecuteScalar();
            connection.Close();
            cmd.Dispose();
            return domainName;
        }

        #endregion

        #region SupportFuncs
        public string strip(string alias)
        {
            // invalid chars, make into spaces
            alias = Regex.Replace(alias, @"[^a-zA-Z0-9\s-]", "");
            // convert multiple spaces/hyphens into one space       
            alias = Regex.Replace(alias, @"[\s-]+", " ").Trim();
            // hyphens
            alias = Regex.Replace(alias, @"\s", "-");

            return alias;
        }

        //////////////SET PERMALINKS///////////////////////
        void setPermalinks(int tabID, int entryID, string slug, string braftonID)
        {
            //Permalink variables
            string directory;
            string permalink;
            slug = strip(slug);
            //Create Permalink
                                          
            //directory = HttpRuntime.AppDomainAppVirtualPath == "/" ? directory = "" : directory = HttpRuntime.AppDomainAppVirtualPath;
            directory = "/";
            permalink = "/blog/EntryId/" + entryID + "/" + slug;


            //Update the permalink in the database
            cmd.CommandText = "UPDATE Blog_Entries SET PermaLink = '" + permalink + "' WHERE BraftonID = '" + braftonID + "'";
            cmd.ExecuteNonQuery();
        }
        #endregion

 

        #region Debugging update script
        public string updateScriptDebug()
        {
            if (MyGlobals.ArtOrBlog == "articles")
            {

                MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Start of articles<br>";
            connection.Open();
            cmd.Connection = connection;

            //Get current directory for style sheets and images
            //was getting errors from the following so simplified it to AppDomainAppPath.ToString();
            string appPath = HttpRuntime.AppDomainAppVirtualPath == "/" ? appPath = "" : appPath = HttpRuntime.AppDomainAppVirtualPath;
            //string appPath = HttpRuntime.AppDomainAppVirtualPath.ToString();//AppDomainAppPath

            //Base api URL
            string newsURL = getNewsURL();
            string baseUrl = getBaseURL();
          //  MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "<br>************************************************<br>";
           // MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "newsURL " + newsURL + "<br>";
          //  MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "baseUrl " + baseUrl + "<br>";
           // MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "<br>************************************************<br>";

            ApiContext ac;

            if (MyGlobals.ArtOrBlog == "archive" )
            {
                ac = new ApiContext(MyGlobals.ArchiveLink);
            }
            else
            {
                ac = new ApiContext(newsURL, baseUrl);
            }

            //Since this column is an identity column in the table this value does not actually get inserted.
            int entryID = 0;

            //Used to compare the current BraftonID from the xml feed to the Brafton IDs in the Blog_Entries table
            int compareIDs;

            //Blog_Entries table variables
            string artBlogID;
            string title;
            string entry;
            DateTime addedDate;
            string description = string.Empty;
            string description_debug = string.Empty;
            bool published = true;
            bool allowComments = false;
            bool displayCopyright = false;
            string photoURL;
            string byline;
            string caption;
            int PageTabId = getTabID();
            ////////////////////////////

            //Blog_Categories table variables
            string slug;
            string category;
            int parentID = 0;
            int intPortalID = getPortalID();
            ////////////////////////////

            //This is for storing all of the category xml urls during the iteration
            ArrayList xmlArtCatURLs = new ArrayList();

            //Blog_Entry_Categories table Arrays, these are populated while populating
            //The Blog_Entry DataTable and Blog_Categories DataTable
            ArrayList entryIDArray = new ArrayList();
            ArrayList categoryArray = new ArrayList();
            /////////////////////////////////////////////////////////////////////////

            DataTable articleTable = DataTables.GetTable("Blog_Entries");

            //For Limit
            int l = 0;

            //Set the limit of the amount of articles that can be imported at a time
            int limit = getLimit();


            #region Article Loop DEBUG
            //Fill Blog_Entries DataTable
            foreach (newsItem ni in ac.News)
            {
                if (l < limit)
                {

                    artBlogID = ni.id.ToString();
                    title = ni.headline;
                    entry = ni.text;
                    description = ni.extract;
                    addedDate = ni.publishDate;
                    photoURL = ni.PhotosHref;
                    byline = ni.byLine;
                    DateTime today = DateTime.Today;
                    //for operations after update or insert
                    int entryId;
                    string tempSlug;



                    using (DNNDataLayerDataContext dnnContext = new DNNDataLayerDataContext())
                    {
                        Blog_Entry be = dnnContext.Blog_Entries.FirstOrDefault(x => x.BraftonID == artBlogID);

                        if (be != null)
                        {
                         #region Update Article
                            MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Start of update<br>";
                          
                            //If update feed content is on we do some stuff
                            int updateCheck = MyGlobals.IncludeUpdatedFeedContent;
                            int lastDayUpd = 0;
                            int todayDay = 0;

                            lastDayUpd = be.LastUpdatedOn.Value.DayOfYear;
                            //todayDay = DateTime.Today.DayOfYear;
                            //TODO set this check back in place FJD
                            

                              //if the article has not been updated today
                            if (lastDayUpd != todayDay)
                             {
                                //if they have updated feed content checked
                                if (updateCheck == 1)
                                {
                                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                //Update the article
                                 ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                 //Lets get the entryID that matches the BraftonID
                                 //But if there isn't one it means this is a new article and we should skip the update

                            //cmd.CommandText = "IF (SELECT EntryID FROM Blog_Entries WHERE BraftonID='" + artBlogID + "') IS NULL BEGIN SELECT 0 END ELSE (SELECT EntryID FROM Blog_Entries WHERE BraftonID=" + artBlogID + ")";

                            //int getDNNID = (int)cmd.ExecuteScalar();

                            //if (getDNNID > 0)
                            //{
                               // DateTime today = DateTime.Today;
                               //string displayDate = today.ToString("dd/MM/yyyy");
                                
                                #region IMAGE HANDLER 

                                    //photo img = ni.photos.First();
                                    //photo.Instance photoInstance = img.Instances.Where(x => x.type == enumeratedTypes.enumPhotoInstanceType.Medium).FirstOrDefault();
                                    //string photoTest = photoInstance.type.ToString();

                                    photo img = ni.photos.First();
                                    photo.Instance photoInstanceLarge = img.Instances.Where(x => x.type == enumeratedTypes.enumPhotoInstanceType.Large).FirstOrDefault();
                                    photo.Instance photoInstanceMedium = img.Instances.Where(x => x.type == enumeratedTypes.enumPhotoInstanceType.Medium).FirstOrDefault();
                                    photo.Instance photoInstanceSmall = img.Instances.Where(x => x.type == enumeratedTypes.enumPhotoInstanceType.Small).FirstOrDefault();
                                    photo.Instance photoInstance;

                                    if (photoInstanceLarge != null)
                                    {
                                        photoInstance = photoInstanceLarge;
                                        //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Using Large<br>";
                                    }
                                    else if (photoInstanceMedium != null)
                                    {
                                        photoInstance = photoInstanceMedium;
                                       // MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Using Medium<br>";
                                    }
                                    else if (photoInstanceSmall != null)
                                    {
                                        photoInstance = photoInstanceSmall;
                                       // MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Using Small<br>";
                                    }
                                    else
                                    {
                                        photoInstance = null;
                                    }
                                    //Otherwise leave it as null and move on


                                    //Checks to see if large images are enabled on the feed 
                                    if (photoInstance != null)
                                    {



                                        photoURL = photoInstance.url.ToString();
                                        caption = img.caption.ToString();


                                        //Checks to see if the feed has photos enabled.
                                        if (!string.IsNullOrEmpty(photoURL))
                                        {
                                            GetImages retrieveImage2 = new GetImages(photoURL, entry, description, appPath, caption);
                                            retrieveImage2.DownloadImageDebug();

                                            //The images is placed into the description and the entry here


                                            entry = retrieveImage2._entry;
                                            description = retrieveImage2._description;
                                            MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "**********************************************************<br>";
                                            MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "brafton ID: "+artBlogID+"<br>";
                                            MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Description: " + description  + "<br>";
                                            MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "**********************************************************<br>";
                                            //string testUrl = HttpContext.Current.Request.Url.Host;

                                        }
                                    }



                                #endregion IMAGE HANDLER 

                                    //Checks to see if the feed has the byline enabled.
                                    if (!string.IsNullOrEmpty(byline))
                                    {
                                        entry = entry.Insert(entry.Length, "<br /><br /><span class='byline'> By " + byline + "</span>");
                                    }


                                            #region Update Article
                                            
                                   
                                            be.Title = title;
                                            be.Entry = entry;
                                            be.Description = description;
                                            //be.Copyright = null;
                                            be.LastUpdatedOn = today;
                                            be.AllowComments = false;
                                            //dnnContext.
                                            dnnContext.SubmitChanges();
                                             #endregion

                                            entryId = be.EntryID;
                                            tempSlug = title;

                                            //Update Permalinks
                                               setPermalinks(PageTabId, entryId, tempSlug, artBlogID);


                                 }//end of update check

                             }//end of check for update today
                            #endregion Update Article
                        }
                    
                        else
                        
                        {
                           #region New Article
                           photo img = ni.photos.First();
                            photo.Instance photoInstanceLarge = img.Instances.Where(x => x.type == enumeratedTypes.enumPhotoInstanceType.Large).FirstOrDefault();
                            photo.Instance photoInstanceMedium = img.Instances.Where(x => x.type == enumeratedTypes.enumPhotoInstanceType.Medium).FirstOrDefault();
                            photo.Instance photoInstanceSmall = img.Instances.Where(x => x.type == enumeratedTypes.enumPhotoInstanceType.Small).FirstOrDefault();
                            photo.Instance photoInstance;

                            if (photoInstanceLarge != null)
                            {
                                photoInstance = photoInstanceLarge;
                               // MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Using Large<br>";
                            }
                            else if (photoInstanceMedium != null)
                            {
                                photoInstance = photoInstanceMedium;
                              //  MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Using Medium<br>";
                            }
                            else if (photoInstanceSmall != null)
                            {
                                photoInstance = photoInstanceSmall;
                               // MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Using Small<br>";
                            }
                            //Otherwise leave it as null and move on
                            else
                            {
                                photoInstance = null;
                            }
                           


                           #region IMAGE HANDLER
                            //Checks to see if medium images are enabled on the feed 
                            if (photoInstance != null)
                            {
                                photoURL = photoInstance.url.ToString();
                                caption = img.caption.ToString();

                                //Checks to see if the feed has photos enabled.
                                if (!string.IsNullOrEmpty(photoURL))
                                {
                                    
                                    GetImages retrieveImage2 = new GetImages(photoURL, entry, description, appPath, caption);
                                    retrieveImage2.DownloadImageDebug();

                                    //The images is placed into the description and the entry here

                                    description_debug = retrieveImage2._description;
                                    entry = retrieveImage2._entry;

                                }
                            }
                           #endregion IMAGE HANDLER
                            //Checks to see if the feed has the byline enabled.
                            if (!string.IsNullOrEmpty(byline))
                            {
                                entry = entry.Insert(entry.Length, "<br /><br /><span class='byline'> By " + byline + "</span>");
                            }

                            Blog_Entry newBlogEntry = new Blog_Entry();
                            
                            newBlogEntry.BlogID = getBlogID();
                            newBlogEntry.Title = title;
                            newBlogEntry.Entry = entry;
                            newBlogEntry.AddedDate = addedDate;
                            newBlogEntry.Published = published;
                            newBlogEntry.Description = description_debug;
                            newBlogEntry.AllowComments = allowComments;
                            newBlogEntry.DisplayCopyright = displayCopyright;
                            newBlogEntry.BraftonID = artBlogID;
                            newBlogEntry.LastUpdatedOn = today;

                            dnnContext.Blog_Entries.InsertOnSubmit(newBlogEntry);
                            dnnContext.SubmitChanges();



                            entryId = newBlogEntry.EntryID;
                           tempSlug = title;

                        
                           //For Permalinks
                           setPermalinks(PageTabId, entryId, tempSlug, artBlogID);

                            ///////////////////////////////////////////////////

                           //Place the category URL into the array for future use                
                           xmlArtCatURLs.Add(ni.CategoriesHref);

                           //For Future Use with Blog_Entry_Categories
                           entryIDArray.Add(artBlogID);

                           //increment limit
                           l++;
                            /////////////////////////////////////////////////////
                             #endregion New Article
                        }
                       
                    }

                    //////Passes all Brafton posts from the Database into this method and checks to see if this posts already exists
                    ////cmd.CommandText = "IF (SELECT BraftonID FROM Blog_Entries WHERE BraftonID='" + artBlogID + "') IS NOT NULL BEGIN SELECT 0 END ELSE SELECT 1";
                    //////cmd.CommandText = "SELECT 1";
                    ////compareIDs = (int)cmd.ExecuteScalar();

                    ////if it doesn't exist
                    //if (compareIDs == 1)
                    //#region Write Article DEBUG
                    
                    //{

                        


                    //   // articleTable.Rows.Add(getBlogID(), entryID, title, entry, addedDate, published, description, allowComments, displayCopyright, null, "", artBlogID);



                    //}
                   
                    //else

                    //    //***************************************************************************************
                    //    //***************************************************************************************
                    //    //***************************************************************************************


                    //}
                    //#endregion Write Article DEBUG

                }
            }
           
            #endregion Article Loop DEBUG
            //Check to see if the article table is empty, if it is, cancel the whole operation.
            if (articleTable != null && articleTable.Rows.Count > 0)
            {
                ///////////////////////////////////////////////////////////////////////////////////
                //TODO: category stuff
                ///////////////////////////////////////////////////////////////////////////////////
                #region Category stuff


                using (DNNDataLayerDataContext dnnContext = new DNNDataLayerDataContext())
                {
                    //Blog_Category bc = dnnContext.Blog_Categories.FirstOrDefault(x => x.Category == artBlogID);




                }


                //Used to copy the DataTables to the MSSQL Database
                //using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                //{
                //    bulkCopy.DestinationTableName = "Blog_Entries";
                //    bulkCopy.WriteToServer(articleTable);
                //}

                DataTable categoryTable = DataTables.GetTable("Blog_Categories");
                /////////////////////////////////////////////////////////////

                //Fill Blog_Categories DataTable
                for (int i = 0; i < articleTable.Rows.Count; i++)
                {
                    XmlDocument tmpXMLDoc = new XmlDocument();
                    tmpXMLDoc.Load(xmlArtCatURLs[i].ToString());
                    XmlNode name = tmpXMLDoc.GetElementsByTagName("name")[0];

                    if (name != null)
                    {
                        category = name.InnerText;
                    }
                    else
                    {
                        category = "Uncategorized";
                    }

                    slug = strip(category) + ".aspx";

                    cmd.CommandText = "IF (SELECT Category FROM Blog_Categories WHERE Category='" + category + "') IS NOT NULL BEGIN SELECT 0 END ELSE SELECT 1";
                    compareIDs = (int)cmd.ExecuteScalar();

                    if (compareIDs == 1)
                    {
                        //Compares the rows in the categoryTable to the category currently being processed.
                        categoryTable.DefaultView.Sort = "Category";
                        int findRow = categoryTable.DefaultView.Find(category);

                        if (findRow == -1)
                        {
                            //Create the row in the datatable
                            categoryTable.Rows.Add(entryID, category, slug, parentID, intPortalID);
                        }
                    }

                    //For Future Use with Blog_Entry_Categories
                    categoryArray.Add(category);

                }

                //Used to copy the DataTables to the MSSQL Database
                if (categoryTable != null && categoryTable.Rows.Count > 0)
                {
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = "Blog_Categories";
                        //bulkCopy.WriteToServer(categoryTable);
                    }
                }

                //Temporary variables that change with each iteration
                int tempCatID;
                int tempEntryID;
                string tempSlug;
                //int PageTabId = getTabID();

                //Match the CategoryIDs with the EntryIDs for the Blog_Entry_Categories Table
                DataTable catEntryTable = DataTables.GetTable("Blog_Entry_Categories");
                for (int i = 0; i < articleTable.Rows.Count; i++)
                {
                    category = categoryArray[i].ToString();
                    artBlogID = entryIDArray[i].ToString();

                    cmd.CommandText = "Select EntryID From Blog_Entries Where BraftonID = '" + artBlogID + "'";
                    tempEntryID = (int)cmd.ExecuteScalar();

                    cmd.CommandText = "Select Title From Blog_Entries Where BraftonID = '" + artBlogID + "'";
                    tempSlug = (string)cmd.ExecuteScalar();

                    cmd.CommandText = "Select CatID From Blog_Categories Where Category ='" + category + "'";
                    tempCatID = (int)cmd.ExecuteScalar();

                    catEntryTable.Rows.Add(entryID, tempEntryID, tempCatID);

                    //For Permalinks
                    //setPermalinks(PageTabId, tempEntryID, tempSlug, artBlogID);
                }

                //Used to copy the DataTables to the MSSQL Database
                if (catEntryTable != null && catEntryTable.Rows.Count > 0)
                {
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = "Blog_Entry_Categories";
                        bulkCopy.WriteToServer(catEntryTable);
                    }
                }

                //Dispose of all of the tables, datasets and commands
                catEntryTable.Dispose();
                categoryTable.Dispose();
            }

            articleTable.Dispose();
            cmd.Dispose();

            connection.Close();
                #endregion Category Stuff

            return description_debug;
        }//end of articles
            MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Video Include Global =" + MyGlobals.IncludeVideo;
            //If the feed includes videos
            if (MyGlobals.IncludeVideo == 1)
            {
                    ImportVideos();
             }
            MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "ARTORBLOG" + MyGlobals.ArtOrBlog;
            string returnVal = "include video";
            return returnVal;
            
            
        }
        #endregion

        #region Video Import

        public void ImportVideos()
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Import articles with video enabled
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


            string publicKey = MyGlobals.VideoPublicKey;
            string secretKey = MyGlobals.VideoSecretKey;
            int feedNumber;

            int tempCatID;
            int tempEntryID;
            string tempSlug;
            int PageTabId = 1;


                    

            //Validation below. I added these validation methods beneath the ImportVideos() method

             if (!int.TryParse(MyGlobals.VideoFeedText, out feedNumber))
            {
               
                MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Invalid video feed number. Stopping";
                return;
            }

            if (!BraftonVideoClass.ValidateVideoPublicKey(publicKey))
            {
               
                MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Invalid video public key. Stopping.";
                return;
            }

            if (!BraftonVideoClass.ValidateGuid(secretKey))
            {
                MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Invalid video public key. Stopping.";
                return;
            }

           

            //This is establishing the URLs for the video api,creating a new videoClient object, and then using the client libraries to get the video articles from the feed - Ly
            //string baseUrl = "http://api.video.brafton.com";
            //string basePhotoUrl = "http://pictures.directnews.co.uk/v2/"; 

            string baseUrl = "http://" + MyGlobals.VideoBaseURL +"/v2/";
            string basePhotoUrl = "http://" + MyGlobals.VideoPhotoURL + "/v2/";

            //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "<br>***********************************************<br>";
            //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "<br>***********Global base URL- brafton.cs***********************<br>";
            //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + baseUrl;
            //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "<br>***********************************************<br>";


            AdferoVideoClient videoClient = new AdferoVideoClient(baseUrl, publicKey, secretKey);
            AdferoClient client = new AdferoClient(baseUrl, publicKey, secretKey);
            AdferoPhotoClient photoClient = new AdferoPhotoClient(basePhotoUrl);

            AdferoVideoOutputsClient xc = new AdferoVideoClient(baseUrl, publicKey, secretKey).VideoOutputs();
             
            AdferoVideoDotNet.AdferoArticles.ArticlePhotos.AdferoArticlePhotosClient photos = client.ArticlePhotos();
            string scaleAxis = AdferoVideoDotNet.AdferoPhotos.Photos.AdferoScaleAxis.X;

            AdferoVideoDotNet.AdferoArticles.Feeds.AdferoFeedsClient feeds = client.Feeds();
            AdferoVideoDotNet.AdferoArticles.Feeds.AdferoFeedList feedList = feeds.ListFeeds(0, 10);

            AdferoVideoDotNet.AdferoArticles.Articles.AdferoArticlesClient articles = client.Articles();
            AdferoVideoDotNet.AdferoArticles.Articles.AdferoArticleList articleList = articles.ListForFeed(feedList.Items[feedNumber].Id, "live", 0, 100);

            int articleCount = articleList.Items.Count;
            AdferoVideoDotNet.AdferoArticles.Categories.AdferoCategoriesClient categories = client.Categories();

            foreach (AdferoVideoDotNet.AdferoArticles.Articles.AdferoArticleListItem item in articleList.Items)
            {
                int brafId = item.Id;
                AdferoVideoDotNet.AdferoArticles.Articles.AdferoArticle article = articles.Get(brafId);
                MyGlobals.brafID = brafId;

                string brafIDForInsert = brafId.ToString();


                string presplashLink;

                if (article.Fields.ContainsKey("preSplash"))
                {
                    presplashLink = article.Fields["preSplash"];
                    
                }
                else
                {
                    presplashLink = "";
                }
                

                #region Build Embed
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Build the embed to be added to the content section of the blog entry
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                string display = "<video id='video-"+brafIDForInsert+"' class='ajs-default-skin atlantis-js' controls preload='auto' width='512' height='288' poster='"+presplashLink+"'>";

                // For each Video Output 
                
                foreach (AdferoVideoDotNet.AdferoArticlesVideoExtensions.VideoOutputs.AdferoVideoOutputListItem vidOut in videoClient.VideoOutputs().ListForArticle(brafId, 0, 20).Items)
                {
                    
                    int vidid = vidOut.Id;

                    string displayType = "";

                    var z = xc.Get(vidid);

                    displayType = z.Path.Substring(z.Path.Length - 3);



                    if (displayType == "flv")
                    {
                        displayType = "flash";
                    }

                    if (displayType == "ebm")
                    {

                        displayType = "webm";
                    }

                    string displayPath = z.Path;

                    string displayHeight = z.Height.ToString();

                    display = display + "<source src='" + displayPath + "' type='video/" + displayType + "' data-resolution='" + displayHeight + "' />";

                }


                // Add the closing tag and the atlantis script
                display = display + "</video><script type='text/javascript'>var atlantisVideo = AtlantisJS.Init({videos: [{id: 'video-" + brafIDForInsert + "'}]});</script>";

                #endregion Build embed



                #region set globals
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Set Global Variables in Globals.cs
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //This area adds all the fields to the Global variables class so that they are accessible from the BraftonVideoClass
                //I know there is a better way to do this but I did it this was for simplicity and in case I needed them somewhere else
                string addDate = article.Fields["date"];

                MyGlobals.tempID = brafIDForInsert;

                if (article.Fields.ContainsKey("title"))
                {
                    MyGlobals.tempTitle = article.Fields["title"];

                }
                else
                {
                    MyGlobals.tempTitle = "";
                }
                

                if (article.Fields.ContainsKey("extract"))
                {
                    MyGlobals.tempExtract = article.Fields["extract"];

                }
                else
                {
                    MyGlobals.tempExtract = "";
                }

                if (article.Fields.ContainsKey("content"))
                {
                    MyGlobals.tempcontent = article.Fields["content"];

                }
                else
                {
                    MyGlobals.tempcontent = "";
                }
                
                MyGlobals.tempDate = DateTime.Parse(addDate);
                MyGlobals.tempPaths = display;
                #endregion


                tempEntryID = BraftonVideoClass.InsertVideoPost();
                tempSlug = article.Fields["title"];
               

                #region Update Permalink
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Update permalink 
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                
                string permalink;
                string slug = strip(tempSlug);
                    //Create Permalink
                    
                   
                using (DNNDataLayerDataContext dnnContext = new DNNDataLayerDataContext())
                {
                    Brafton.Modules.BraftonImporter7_02_02.dbDataLayer.Blog_Entry pk = dnnContext.Blog_Entries.FirstOrDefault(x => x.BraftonID == brafIDForInsert);

                                    //Update the permalink
                                    if (pk != null)
                                    {

                                        permalink = "/blog/EntryId/" + pk.EntryID + "/" + slug;

                                        pk.PermaLink = permalink;
                                        dnnContext.SubmitChanges();
                                      
                                    }

                }

                #endregion Update Permalink

                #region Categories
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Add categories to Blog_Entry_Categories table 
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                foreach (AdferoVideoDotNet.AdferoArticles.Categories.AdferoCategoryListItem cats in client.Categories().ListForArticle(brafId,0,20).Items)
                {
                    int categoryId;
                    int catTest = cats.Id;
                    string catName = categories.Get(catTest).Name;
                    if (catName == null)
                    {
                        catName = "Uncategorized";
                    }
                    int pID = categories.Get(catTest).ParentId;
                    string catslug = strip(catName) + ".aspx";
                   
                    



                    using (DNNDataLayerDataContext dnnContext = new DNNDataLayerDataContext())
                    {
                        Brafton.Modules.BraftonImporter7_02_02.dbDataLayer.Blog_Category ca = dnnContext.Blog_Categories.FirstOrDefault(x => x.Category == catName);

                        //Insert into category
                        if (ca != null)
                        {

                            categoryId = ca.CatID;
                        }

                        else
                        {
                          
                            Blog_Category newBlogCat = new Blog_Category();
                            newBlogCat.Category = catName;
                            newBlogCat.Slug = catslug;
                            newBlogCat.ParentID = pID;
                            newBlogCat.PortalID = 0;

                            dnnContext.Blog_Categories.InsertOnSubmit(newBlogCat);
                            dnnContext.SubmitChanges();

                            categoryId = newBlogCat.CatID;
                                                        
                        }

                        Brafton.Modules.BraftonImporter7_02_02.dbDataLayer.Blog_Entry_Category bec = dnnContext.Blog_Entry_Categories.FirstOrDefault(x => x.EntryID == tempEntryID && x.CatID == categoryId);

                        if (bec == null)
                        {
                            Blog_Entry_Category newBlogEntryCat = new Blog_Entry_Category();
                            newBlogEntryCat.EntryID = tempEntryID;
                            newBlogEntryCat.CatID = categoryId;

                            dnnContext.Blog_Entry_Categories.InsertOnSubmit(newBlogEntryCat);
                            dnnContext.SubmitChanges();
                        }

                    }

                }

                #endregion Categories


        }


        #endregion Video Import


    }
    }
}





