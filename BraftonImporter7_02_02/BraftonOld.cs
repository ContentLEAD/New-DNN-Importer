
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
using System.Text.RegularExpressions;

using System.Text;
using System.Web.Hosting;

using System.Net;
using System.Drawing;
using System.Reflection;

namespace Brafton.DotNetNuke
{

    public class BraftonSchedule : SchedulerClient
    {
        //Connection properties

        public SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SiteSqlServer"].ToString());
        public SqlCommand cmd = new SqlCommand();

        public BraftonSchedule(ScheduleHistoryItem objScheduleHistoryItem)  : base()
        {
            ScheduleHistoryItem = objScheduleHistoryItem;
        }

        // Note we need to give a default constructor when override it
        public BraftonSchedule() : base() { }

        #region Debugging
        public override void DoWork()
        {
            try
            {
                // start the process
                updateScript();
                // then report success to the scheduler framework
                ScheduleHistoryItem.Succeeded = true;
            }

            // handle any exceptions
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
            directory = HttpRuntime.AppDomainAppVirtualPath == "/" ? directory = "" : directory = HttpRuntime.AppDomainAppVirtualPath;
            permalink = directory + "/blog/EntryId/" + entryID + "/" + slug;
            //Update the permalink in the database
            cmd.CommandText = "UPDATE Blog_Entries SET PermaLink = '" + permalink + "' WHERE BraftonID = '" + braftonID + "' and PermaLink = ''";
            cmd.ExecuteNonQuery();
        }
        #endregion

        public void updateScript()
        {
            
            connection.Open();
            cmd.Connection = connection;

            //Get current directory for style sheets and images
            string appPath = HttpRuntime.AppDomainAppVirtualPath == "/" ? appPath = "" : appPath = HttpRuntime.AppDomainAppVirtualPath;

            //Base api URL
            string newsURL = getNewsURL();
            string baseUrl = getBaseURL();

            ApiContext ac = new ApiContext(newsURL, baseUrl);

            //Since this column is an identity column in the table this value does not actually get inserted.
            int entryID = 0;

            //Used to compare the current BraftonID from the xml feed to the Brafton IDs in the Blog_Entries table
            int compareIDs;

            //Blog_Entries table variables
            string artBlogID;
            string title;
            string entry;
            DateTime addedDate;
            string description;
            string published = "True";
            string allowComments = "False";
            string displayCopyright = "False";
            string photoURL;
            string byline;
            string caption;
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
                    //////////////////////////////////////////////////////////////////////////

                    //Passes all Brafton posts from the Database into this method and checks to see if this posts already exists
                    cmd.CommandText = "IF (SELECT BraftonID FROM Blog_Entries WHERE BraftonID='" + artBlogID + "') IS NOT NULL BEGIN SELECT 0 END ELSE SELECT 1";
                    compareIDs = (int)cmd.ExecuteScalar();

                    if (compareIDs == 1)
                    {
                        photo img = ni.photos.First();
                        photo.Instance photoInstance = img.Instances.Where(x => x.type == enumeratedTypes.enumPhotoInstanceType.Large).FirstOrDefault();

                        //Checks to see if large images are enabled on the feed 
                        if (photoInstance != null)
                        {
                            photoURL = photoInstance.url.ToString();
                            caption = img.caption.ToString();

                            //Checks to see if the feed has photos enabled.
                            if (!string.IsNullOrEmpty(photoURL))
                            {
                                GetImages retrieveImage = new GetImages(photoURL, entry, description, appPath, caption);
                                retrieveImage.DownloadImage();

                                

                                //The images is placed into the description and the entry here
                                description = retrieveImage._description;
                                entry = retrieveImage._entry;

                                //errorCheckingLabel.text = description;
                            }
                        }

                        //Checks to see if the feed has the byline enabled.
                        if (!string.IsNullOrEmpty(byline))
                        {
                            entry = entry.Insert(entry.Length, "<br /><br /><span class='byline'> By " + byline + "</span>");
                        }

                        articleTable.Rows.Add(getBlogID(), entryID, title, entry, addedDate, published, description, allowComments, displayCopyright, null, "", artBlogID);

                        //Place the category URL into the array for future use                
                        xmlArtCatURLs.Add(ni.CategoriesHref);

                        //For Future Use with Blog_Entry_Categories
                        entryIDArray.Add(artBlogID);

                        //increment limit
                        l++;
                    }

                }
            }

            //Check to see if the article table is empty, if it is, cancel the whole operation.
            if (articleTable != null && articleTable.Rows.Count > 0)
            {
                //Used to copy the DataTables to the MSSQL Database
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "Blog_Entries";
                    bulkCopy.WriteToServer(articleTable);
                }

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
                        bulkCopy.WriteToServer(categoryTable);
                    }
                }

                //Temporary variables that change with each iteration
                int tempCatID;
                int tempEntryID;
                string tempSlug;
                int PageTabId = getTabID();

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
                    setPermalinks(PageTabId, tempEntryID, tempSlug, artBlogID);
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
        }

    }
}



