using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;

using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;

using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Web.Hosting;
using System.Diagnostics;

using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

using DotNetNuke;
using DotNetNuke.Security;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Scheduling;


using System.Reflection;
using System.Security;
using System.Security.Permissions;
using Brafton;
using Brafton.Modules.Globals;

using System.Net;
using Brafton.Modules.VideoImporter;




namespace BraftonView.Brafton_Importer_Clean
{
    [SecurityCritical]
    public partial class DesktopModules_Brafton_View2 : DotNetNuke.Entities.Modules.PortalModuleBase
    {
      
        //Connection properties
        public SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SiteSqlServer"].ToString());
        public SqlConnection connection2;
        public SqlCommand cmd = new SqlCommand();
        public SqlCommand cmd2 = new SqlCommand();

        //Application path variable
        public string appPath;

        //DNN Variables
        int intPortalID;
        int PageTabId;
        string checkDomain;

        //Local Variables
        public int checkBlogModule;
        public int checkFriendURLS;
        public int checkBlogCreated;
        public int checkNewsAPI;
        public int checkBaseUrl;
        public int checkLimit;
        public int checkBlogID;
        public int checkVidID;
        public Dictionary<string, int> checkAll = new Dictionary<string, int>();

        //Global Variables
        public int IncludeUpdatedFeedContent;

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

        public Dictionary<string, int> checks()
        {
            cmd.CommandText = "IF OBJECT_ID('Blog_Settings') IS NOT NULL select 1 else select 0";
            checkFriendURLS = ((int)cmd.ExecuteScalar());
            checkAll.Add("Friendly Urls", checkFriendURLS);

            cmd.CommandText = "IF OBJECT_ID('Blog_Blogs') IS NOT NULL SELECT 1 Else Select 0";
            checkBlogCreated = (int)cmd.ExecuteScalar();
            checkAll.Add("Blog", checkBlogCreated);

            cmd.CommandText = "IF (SELECT BaseUrl FROM Brafton WHERE content='1') IS NOT NULL SELECT 1 ELSE SELECT 0";
            checkBaseUrl = (int)cmd.ExecuteScalar();
            checkAll.Add("Base Url", checkBaseUrl);

            cmd.CommandText = "IF (SELECT Api FROM Brafton WHERE content='1') IS NOT NULL SELECT 1 ELSE SELECT 0";
            checkNewsAPI = (int)cmd.ExecuteScalar();
            checkAll.Add("News API", checkNewsAPI);

            cmd.CommandText = "IF (SELECT Limit FROM Brafton WHERE content='1') IS NOT NULL SELECT 1 ELSE SELECT 0";
            checkLimit = (int)cmd.ExecuteScalar();
            checkAll.Add("Limit", checkLimit);

            cmd.CommandText = "IF (SELECT count(*) as total_record FROM DesktopModules WHERE FriendlyName = 'blog') > 0 Select 1 else select 0";
            checkBlogModule = ((int)cmd.ExecuteScalar());
            checkAll.Add("Blog Module", checkBlogModule);

            cmd.CommandText = "If (SELECT count(*) as total_record FROM Brafton WHERE BlogId >= 1) > 0 Select 1 else select 0";
            checkBlogID = ((int)cmd.ExecuteScalar());
            checkAll.Add("Blog ID", checkBlogID);

            cmd.CommandText = "IF (SELECT VideoPublicKey FROM Brafton WHERE content='1') IS NOT NULL SELECT 1 ELSE SELECT 0";
            checkVidID = ((int)cmd.ExecuteScalar());
            checkAll.Add("Vid ID", checkVidID);

            return checkAll;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            //connection.Open();
            //cmd.Connection = connection;
            //cmd2.Connection = connection;

            //cmd2.CommandText = "Select Title From Blog_Entries where EntryID = 2";

            //string apiTest = cmd2.ExecuteScalar().ToString();

            // apiURLLabel.Text = apiTest;

            //connection.Close();


            //********************************************************************
            //TODO check for Brafton Table


           //cmd2.CommandText = "CREATE TABLE [dbo].Brafton(Id int IDENTITY(1,1) NOT NULL,Content nvarchar(MAX) NULL,Api nvarchar(MAX) NULL,BaseUrl nvarchar(MAX) NULL, BlogId int NULL,PortalId int NULL,TabId int NULL,DomainName nvarchar(MAX) NULL, Limit int NULL)";
            //cmd2.ExecuteNonQuery();

            //********************************************************************



            try
            {

                //Get current directory for style sheets and images
                appPath = HttpRuntime.AppDomainAppVirtualPath == "/" ? appPath = "" : appPath = HttpRuntime.AppDomainAppVirtualPath;

                connection.Open();
                cmd.Connection = connection;

                //Showing your current portal ID
                
                intPortalID = PortalSettings.PortalId;
                currentPortalID.Text = intPortalID.ToString();

                //Showing your current TabID
                PageTabId = PortalSettings.ActiveTab.TabID;
                currentTabID.Text = PortalSettings.ActiveTab.TabID.ToString();

                //Get your current domain to insert into database for error checking
                checkDomain = HttpContext.Current.Request.Url.Host;

                //MyGlobals.MyGlobalError = MyGlobals.MyGlobalError + "Check Domain:" + checkDomain;

                Dictionary<string, int> checkAll = checks();

                //Sets the current PortalID in the Brafton Table
                cmd.CommandText = "IF Exists (SELECT * FROM Brafton WHERE content='1') UPDATE Brafton SET PortalId = " + intPortalID + " WHERE Content = '1' else INSERT INTO Brafton (Content, PortalId) VALUES (1, '" + intPortalID + "')";
                cmd.ExecuteNonQuery();

                //Sets the current TabID in the Brafton Table
                cmd.CommandText = "IF Exists (SELECT * FROM Brafton WHERE content='1') UPDATE Brafton SET TabId = " + PageTabId + " WHERE Content = '1' else INSERT INTO Brafton (Content, TabId) VALUES (1, '" + PageTabId + "')";
                cmd.ExecuteNonQuery();

                //Sets the current domain in the Brafton Table
                cmd.CommandText = "IF Exists (SELECT * FROM Brafton WHERE content='1') UPDATE Brafton SET DomainName = 'http://" + checkDomain + "' WHERE Content = '1' else INSERT INTO Brafton (Content, DomainName) VALUES (1, 'http://" + checkDomain + "')";
                cmd.ExecuteNonQuery();

                //HERE WE SET THE GLOBAL VARIABLES 
                //This fills all the relevant fields with values from the database if they exist 
                //All the prior queries have been left intact for the time being but can eventually be phased out

                MyGlobals.PopGlobals(); 

                checkedStatusLabel.Text = MyGlobals.MyGlobalError; //For debugging it is a global text field you pass errors into

                if (checkAll["Friendly Urls"] == 1)
                {
                    boolFriendURL.Text = "True";
                    boolFriendURL.CssClass = "boolTrue";
                }
                else
                {
                    boolFriendURL.Text = "False";
                    boolFriendURL.CssClass = "boolFalse";
                }

                if (checkAll["Blog Module"] == 1)
                {
                    boolBlogModule.Text = "True";
                    boolBlogModule.CssClass = "boolTrue";
                    cmd.CommandText = "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Blog_Entries' AND column_name='BraftonID') BEGIN ALTER TABLE Blog_Entries ADD BraftonID nvarchar(255) END";
                    cmd.ExecuteNonQuery();

                }
                else
                {
                    boolBlogModule.Text = "False";
                    boolBlogModule.CssClass = "boolFalse";
                }


                if (checkAll["Blog"] == 1)
                {
                    boolBlogCreated.Text = "True";
                    boolBlogCreated.CssClass = "boolTrue";

                    cmd.CommandText = "Select Title From Blog_Blogs where PortalID = '" + intPortalID + "'";
                    SqlDataReader blogOptions = cmd.ExecuteReader();

                    if (!IsPostBack && blogOptions.HasRows)
                    {
                        while (blogOptions.Read())
                        {
                            blogIdDrpDwn.Items.Add(new ListItem(blogOptions.GetString(0)));
                        }
                    }

                    blogOptions.Close();
                    setAPIPH.Visible = true;
                }
                else
                {
                    boolBlogCreated.Text = "False";
                    boolBlogCreated.CssClass = "boolFalse";
                }

                if (checkAll["News API"] == 0)
                {
                    boolCheckAPI.Text = "False";
                    boolCheckAPI.CssClass = "boolFalse";
                    nextStep.Visible = false;
                }
                else
                {
                    cmd.CommandText = "SELECT Api FROM Brafton WHERE content='1'";
                    string newsAPI = (string)cmd.ExecuteScalar();
                    boolCheckAPI.Text = "<span class='boolTrue'>True</span>";
                    //apiURLLabel.Text = newsAPI;
                    apiURL.Text = newsAPI;
                    if (checkAll["Base Url"] == 1)
                    {
                        nextStep.Visible = true;
                      
                    }
                }

                if (checkAll["Base Url"] == 0)
                {
                    boolCheckUrl.Text = "False";
                    boolCheckUrl.CssClass = "boolFalse";
                }
                else
                {
                    cmd.CommandText = "SELECT BaseUrl FROM Brafton WHERE content='1'";
                    string newsAPI = (string)cmd.ExecuteScalar();
                    boolCheckUrl.Text = "<span class='boolTrue'>True</span>";
                    //baseURLLabel.Text = newsAPI;
                    baseURL.Text = newsAPI;
                }

                //if (checkAll["Limit"] == 1)
                //{
                //    cmd.CommandText = "SELECT Limit FROM Brafton WHERE content='1'";
                //    limitLabel.Text = cmd.ExecuteScalar().ToString();
                //}

                if (checkAll["Blog ID"] == 0)
                {
                    boolCheckBlogID.Text = "<span class='boolFalse'>False</span>";
                }
                else
                {
                    boolCheckBlogID.Text = "<span class='boolTrue'>True</span>";
                    cmd.CommandText = "Select Title From Blog_Blogs Where BlogID = " + getBlogID();
                    string blogTitle = (string)cmd.ExecuteScalar();
                    currentBlogID.Text = blogTitle;
                }

                //check for video settings if there fill fields and make visible
                if (checkAll["Vid ID"] == 1)
                {
                    if (Page.IsPostBack == false)
                    {

                        VideoSettings.Visible = true;
                        //InclVideo.Checked = true;
                        updateVidSettings.Visible = true;
                        MyGlobals.IncludeVideo = 1;
                        setVidSettings.Visible = false;
                        CurrentVidSetting1.Visible = true;
                        CurrentVidSetting2.Visible = false;
                        //CurrentVidSetting3.Visible = true;
                        VideoBaseURL.Text = MyGlobals.VideoBaseURL;
                        VideoPhotoURL.Text = MyGlobals.VideoPhotoURL;
                        VideoPublicKey.Text = MyGlobals.VideoPublicKey;
                        VideoSecretKey.Text = MyGlobals.VideoSecretKey;
                        VideoFeedNumber.Text = MyGlobals.VideoFeedNumber.ToString();
                       
                        RadioButtonList1.SelectedValue = "video";
                        VideoSettings.Visible = true;
                        VideoPlaceHolder.Visible = true;

                        Import.Visible = true;
                    }
                }

                if (checkAll["Vid ID"] == 1 && checkAll["Base Url"] == 1)
                {
                    
                    
                    Import.Visible = true;
                    if (Page.IsPostBack == false)
                    {
                        RadioButtonList1.SelectedValue = "both";
                        ArticlePlaceHolder.Visible = true;
                        VideoSettings.Visible = true;
                        ArticlePlaceHolder.Visible = true;
                        VideoPlaceHolder.Visible = true;
                    }
                }

                if (checkAll.Values.Sum() == 6)
                {
                    Import.Visible = true;
                    if (Page.IsPostBack == false)
                    {
                    ArticlePlaceHolder.Visible = true;

                    
                        RadioButtonList1.SelectedValue = "articles";
                        MyGlobals.MyGlobalError = "Should be show Articles";
                    }
                        
                }
                else
                {
                    int sumnum = checkAll.Values.Sum();
                    MyGlobals.MyGlobalError = "Here is the problem" + sumnum;
                }

                //Check what the value of IncUpdatedFeedContentValue is in the db 1 is checked 0 is not checked
                Brafton.DotNetNuke.BraftonSchedule B1 = new Brafton.DotNetNuke.BraftonSchedule();
                int checkForUpdatedCheck = B1.getUpdatedContent(); 
                //int checkForUpdatedCheck = 1;
                if (Page.IsPostBack == false) { 
                        if (checkForUpdatedCheck == 1)
                        {
                            InclUpdatedFeedContent.Checked = true;
                            MyGlobals.IncludeUpdatedFeedContent = 1;
                            cmd.CommandText = "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Blog_Entries' AND column_name='LastUpdatedOn') BEGIN ALTER TABLE Blog_Entries ADD LastUpdatedOn DATETIME NULL END";
                            cmd.ExecuteNonQuery();
                            //checkedStatusLabel.Text = "The value in the db =" + checkForUpdatedCheck + " and the value in globals is " + MyGlobals.IncludeUpdatedFeedContent +"Means it should be checked";
                            
                        }
                        else
                        {
                            InclUpdatedFeedContent.Checked = false;
                            MyGlobals.IncludeUpdatedFeedContent = 0;
                            //checkedStatusLabel.Text = "The value in the db =" + checkForUpdatedCheck + " and the value in globals is " + MyGlobals.IncludeUpdatedFeedContent+" Means it should not be checked";
                           
                        }
                }
                connection.Close();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                labelError.Text = "Generic exception: " + ex.ToString();
                connection.Close();
                cmd.Dispose();
            }

        }

        #region GetAndSetBlock

        #region showSections

        protected void showSections(object sender, EventArgs e)
        {
            if (RadioButtonList1.Text == "articles")
            {
                if (Page.IsPostBack == true)
                {
                    ArticlePlaceHolder.Visible = true;
                    VideoPlaceHolder.Visible = false;
                    MyGlobals.ArtOrBlog = "articles";
                    VideoSettings.Visible = false;
                }
            }

            if (RadioButtonList1.Text == "video")
            {
                if (Page.IsPostBack == true)
                {
                    ArticlePlaceHolder.Visible = false;
                    VideoPlaceHolder.Visible = true;
                    MyGlobals.ArtOrBlog = "videos";
                    VideoSettings.Visible = true;
                }
            }

            if (RadioButtonList1.Text == "both")
            {
                if (Page.IsPostBack == true)
                {
                    ArticlePlaceHolder.Visible = true;
                    VideoPlaceHolder.Visible = true;
                    VideoSettings.Visible = true;
                    MyGlobals.ArtOrBlog = "both";
                }
            }

            if (RadioButtonList1.Text == "archive")
            {
                if (Page.IsPostBack == true)
                {
                    ArticlePlaceHolder.Visible = true;
                    VideoPlaceHolder.Visible = false;
                    MyGlobals.ArtOrBlog = "archive";
                    MyGlobals.ArchiveLink = archiveURL.Text;
                    VideoSettings.Visible = false;
                }
            }
        }

        #endregion

        #region check settings

        protected string checkSettings()
        {
            if (MyGlobals.ArtOrBlog == "articles" && MyGlobals.VideoSecretKey != "xxxxxx")
            {
               
               return "break";
            }

            if (MyGlobals.ArtOrBlog == "videos" && MyGlobals.api != "xxxxxx")
            {
                return "break";
            }

            if (MyGlobals.ArtOrBlog == "both" && MyGlobals.api != "xxxxxx" && MyGlobals.VideoSecretKey != "xxxxxx")
            {
                return "break";
             }
            return "GTG";
        }

        #endregion


        #region BLOG ID
        ///////////////////GET AND SET BLOG ID//////////////////////////
        protected void setBlogID_Click(object sender, EventArgs e)
        {
          	
			
            connection.Open();
            int findBlogID;
            string test = blogIdDrpDwn.Text;
            cmd.CommandText = "IF OBJECT_ID('Blog_Blogs') IS NOT NULL(Select BlogID FROM Blog_Blogs Where title = '" + blogIdDrpDwn.Text + "') Else select 0";
            findBlogID = (int)cmd.ExecuteScalar();

            if (findBlogID != 0)
            {
                cmd.CommandText = "UPDATE Brafton SET BlogId = " + findBlogID + " WHERE Content = '1'";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "Select Title From Blog_Blogs Where BlogID = " + getBlogID();
                string blogTitle = (string)cmd.ExecuteScalar();
                currentBlogID.Text = blogTitle;

                //Set Check Blog ID Label = "TRUE"
                boolCheckBlogID.Text = "<span class='boolTrue'>True</span>";

                //Make import button visible
                Import.Visible = true;
            }
            connection.Close();

        }

        protected void showVideoSettings(object sender, EventArgs e)
        {
            //if (InclVideo.Checked == true)
            //{
            //    VideoSettings.Visible = true;
            //}
            //else
            //{
            //    VideoSettings.Visible = false;
            //}
        }

        int getBlogID()
        {
            cmd.CommandText = "Select BlogId from Brafton Where Content = '1'";
            int blogID = (int)cmd.ExecuteScalar();
            return blogID;
        }
        #endregion BLOG ID

        #region Import Limits
        ///////////////////GET AND SET Limit//////////////////////////
        protected void setLimit_Click(object sender, EventArgs e)
        {
            string localVariable = "setLimit_Click";

        }
        #endregion Import Limits

        #region API Key
        ///////////////////GET AND SET API KEY//////////////////////////
        protected void setAPI_Click(object sender, EventArgs e)
        {
            string newsURL = apiURL.Text;
            connection.Open();
            cmd.Connection = connection;

            try
            {

                cmd.CommandText = "IF OBJECT_ID('Brafton') IS NOT NULL(SELECT count(*) as total_record FROM Brafton WHERE content='1') ELSE SELECT 0";
                int apiAvail = (int)cmd.ExecuteScalar();

                if (apiAvail == 1 && apiURL.Text != "")
                {
                    cmd.CommandText = "UPDATE Brafton SET Api = '" + newsURL + "' WHERE Content = '1'";
                    cmd.ExecuteNonQuery();
                    //apiURLLabel.Text = newsURL;
                    apiURL.Text = newsURL;
                    boolCheckAPI.Text = "<span class='boolTrue'>True</span>";
                }
                else if (apiAvail == 0 && apiURL.Text != "")
                {
                    cmd.CommandText = "INSERT INTO Brafton (Content, Api) VALUES (1, '" + newsURL + "' )";
                    cmd.ExecuteNonQuery();
                    //apiURLLabel.Text = newsURL;
                    apiURL.Text = newsURL;
                    boolCheckAPI.Text = "<span class='boolTrue'>True</span>";
                }

                if (checkAll.Values.Sum() == 6)
                {
                    Import.Visible = true;

                }
            }
            catch (Exception ex)
            {
                labelError.Text = "Generic exception: " + ex.ToString();
            }
            connection.Close();


        }
        #endregion API Key



        #region Base URL
        ///////////////////GET AND SET Base URL//////////////////////////
        protected void setBaseURL_Click(object sender, EventArgs e)
        {
            string newBaseURL = baseURL.Text;
            connection.Open();
            cmd.Connection = connection;

            try
            {
                cmd.CommandText = "IF OBJECT_ID('Brafton') IS NOT NULL(SELECT count(*) as total_record FROM Brafton WHERE content='1') ELSE SELECT 0";
                int apiAvail = (int)cmd.ExecuteScalar();

                if (apiAvail == 1 && baseURL.Text != "")
                {
                    cmd.CommandText = "UPDATE Brafton SET BaseUrl = '" + newBaseURL + "' WHERE Content = '1'";
                    cmd.ExecuteNonQuery();
                    //baseURLLabel.Text = newBaseURL;
                    baseURL.Text = newBaseURL;
                    boolCheckUrl.Text = "<span class='boolTrue'>True</span>";
                }
                else if (apiAvail == 0 && baseURL.Text != "")
                {
                    cmd.CommandText = "INSERT INTO Brafton (Content, BaseUrl) VALUES (1, '" + newBaseURL + "' )";
                    cmd.ExecuteNonQuery();
                   // baseURLLabel.Text = newBaseURL;
                    baseURL.Text = newBaseURL;
                    boolCheckUrl.Text = "<span class='boolTrue'>True</span>";
                }
                if (checkAll.Values.Sum() == 6)
                {
                    Import.Visible = true;
                    
                }

            }
            catch (Exception ex)
            {
                labelError.Text = "Generic exception: " + ex.ToString();
            }
            connection.Close();

        }

       // public string getBaseURL()
        //{
        //    cmd.CommandText = "SELECT BaseUrl FROM Brafton WHERE content='1'";
        //    string baseURL = cmd.ExecuteScalar().ToString();
        //    return baseURL;
        //}
        #endregion Base URL

        #region Updated Feed Content
        ///////////////////GET AND SET Checkbox for Updated Content//////////////////////////
        protected void setUpdateContent_Click()
        {
            connection2 = new SqlConnection(ConfigurationManager.ConnectionStrings["SiteSqlServer"].ToString());

            connection2.Open();
            cmd.Connection = connection2;

            cmd.CommandText = "IF OBJECT_ID('Brafton') IS NOT NULL(SELECT count(*) as total_record FROM Brafton WHERE content='1') ELSE SELECT 0";
            int updateContent = (int)cmd.ExecuteScalar();


            Brafton.DotNetNuke.BraftonSchedule B1 = new Brafton.DotNetNuke.BraftonSchedule();
            int checkForUpdatedCheck = B1.getUpdatedContent();



            //Check what is set in the db
            Brafton.DotNetNuke.BraftonSchedule B2 = new Brafton.DotNetNuke.BraftonSchedule();
            int checkForUpdatedCheckDB = B2.getUpdatedContent();

            if (InclUpdatedFeedContent.Checked == true)
            {
                MyGlobals.IncludeUpdatedFeedContent = 1;
                cmd.CommandText = "UPDATE Brafton SET IncUpdatedFeedContentValue = 1 WHERE Content = '1'";
                cmd.ExecuteNonQuery();
                //checkedStatusLabel.Text = "The after click value in the db =" + checkForUpdatedCheckDB + " and the value in globals is " + MyGlobals.IncludeUpdatedFeedContent+"And should be checked";
                //checkedStatusLabel.Text = "CLICKED";
            }
            else if (InclUpdatedFeedContent.Checked == false)
            {
                MyGlobals.IncludeUpdatedFeedContent = 0;
                cmd.CommandText = "UPDATE Brafton SET IncUpdatedFeedContentValue = 0 WHERE Content = '1'";
                cmd.ExecuteNonQuery();
                //checkedStatusLabel.Text = "The afet click value in the db =" + checkForUpdatedCheckDB + " and the value in globals is " + MyGlobals.IncludeUpdatedFeedContent+"And should not be checked";
                //checkedStatusLabel.Text = "NOT CLICKED";
            }

            else
            {
                checkedStatusLabel.Text = "SOMETHING ELSE HAPPENED";
            }
            //connection2.Close();
            try
            {




            }
            catch (Exception ex)
            {
                labelError.Text = "Generic exception: " + ex.ToString();
            }
            

        }


        #endregion Updated Feed Content


        #region Check For Video

        public void checkForVideo(object sender, EventArgs e)
        {
            if (VideoBaseURL.Text != "")
            {
                MyGlobals.VideoBaseURL = VideoBaseURL.Text;
                BraftonVideoClass.AddUpdateVideoBaseURL(VideoBaseURL.Text);
            }
            if (VideoPhotoURL.Text != "")
            {
                MyGlobals.VideoPhotoURL = VideoPhotoURL.Text;
                BraftonVideoClass.AddUpdateVideoPhotoURL(VideoPhotoURL.Text);
            }
            if (VideoPublicKey.Text != "")
            {
                MyGlobals.VideoPublicKey = VideoPublicKey.Text;
                BraftonVideoClass.AddUpdatePublicKey(VideoPublicKey.Text);
                MyGlobals.IncludeVideo = 1;
               // currentVideoPublicKey.Text = VideoPublicKey.Text;
                //CurrentVidSetting1.Visible = true;
                //cmd.CommandText = "UPDATE Brafton SET IncUpdatedFeedContentValue = 1 WHERE Content = '1'"; 
                //cmd.ExecuteNonQuery();
                //checkedStatusLabel.Text = "The after click value in the db =" + checkForUpdatedCheckDB + " and the value in globals is " + MyGlobals.IncludeUpdatedFeedContent+"And should be checked";
                //checkedStatusLabel.Text = "CLICKED";
            }

            if (VideoSecretKey.Text != "")
            {
                MyGlobals.VideoSecretKey = VideoSecretKey.Text;
                BraftonVideoClass.AddUpdateSecretKey(VideoSecretKey.Text);
               // currentVideoSecretKey.Text = VideoSecretKey.Text;
                //CurrentVidSetting2.Visible = true;
                //cmd.CommandText = "UPDATE Brafton SET IncUpdatedFeedContentValue = 1 WHERE Content = '1'";
                //cmd.ExecuteNonQuery();
                //checkedStatusLabel.Text = "The after click value in the db =" + checkForUpdatedCheckDB + " and the value in globals is " + MyGlobals.IncludeUpdatedFeedContent+"And should be checked";
                //checkedStatusLabel.Text = "CLICKED";
            }
            if (VideoFeedNumber.Text != "")
            {
                int VidFeedNum = Convert.ToInt32(VideoFeedNumber.Text);
                MyGlobals.VideoFeedNumber = VidFeedNum;
                MyGlobals.VideoFeedText = VideoFeedNumber.Text;
                BraftonVideoClass.AddUpdateFeedNum(VidFeedNum);
              //  currentVideoFeedNumber.Text = VideoFeedNumber.Text;
                //CurrentVidSetting3.Visible = true;
                //cmd.CommandText = "UPDATE Brafton SET IncUpdatedFeedContentValue = 1 WHERE Content = '1'";
                //cmd.ExecuteNonQuery();
                //checkedStatusLabel.Text = "The after click value in the db =" + checkForUpdatedCheckDB + " and the value in globals is " + MyGlobals.IncludeUpdatedFeedContent+"And should be checked";
                //checkedStatusLabel.Text = MyGlobals.VideoFeedNumber;
            }
            updateVidSettings.Visible = true;
            setVidSettings.Visible = false;
            CurrentVidSetting1.Visible = true;
            CurrentVidSetting2.Visible = false;
           

        }
        #endregion Video Settings

        #region POSSIBLE DELETE
        public string getNewsURL()
        {
            cmd.CommandText = "SELECT Api FROM Brafton WHERE content='1'";
            string feedURL = cmd.ExecuteScalar().ToString();
            return feedURL;
        }
        #endregion POSSIBLE DELETE

        #endregion

        protected void Import_Click(object sender, EventArgs e)
        {
            //Double Check settings and confirm possible errors

            string cs = checkSettings();

            setUpdateContent_Click();
            Brafton.DotNetNuke.BraftonSchedule newSched = new Brafton.DotNetNuke.BraftonSchedule();
            newSched.DoWork();
            //Response.Redirect(Request.RawUrl);

           // Brafton.DotNetNuke.BraftonSchedule testSched = new Brafton.DotNetNuke.BraftonSchedule();
            //errorCheckingLabel.Text =  testSched.DoWork();
            //testSched.DoWork();

          
            globalErrorMessage.Text = MyGlobals.MyGlobalError;
        }

        protected void show_globals(object sender, EventArgs e)
        {

            globalErrorMessage.Text = MyGlobals.MyGlobalError + " imageInfo:" + MyGlobals.imageInfo;
        }

        protected void saveSettings(object sender, EventArgs e)
        {

            Response.Redirect(Request.RawUrl);
        }


    }

}



























