<%@ Control Language="C#" AutoEventWireup="true" CodeFile="View.ascx.cs" Inherits="BraftonView.Brafton_Importer_Clean.DesktopModules_Brafton_View2" %>

<script runat="server">



</script>



<link href="<%= appPath %>/DesktopModules/Brafton/css/style.css" rel="stylesheet"
    type="text/css" />
<div id="braftonView">
    <h1>
        Brafton DotNetNuke Module</h1>
    <p class="IDs">
       <%-- Current Portal ID:--%>
        <asp:Label ID="currentPortalID" runat="server" Visible="false" />
    </p>
    <p>
        ****The Brafton Module has to be installed on the same page as the DNN Blog Module
        in order to build the Permalinks****</p>

    <p class="IDs">
       <%-- Current Tab ID:--%>
        <asp:Label ID="currentTabID" runat="server" Visible="false" />

    </p>
    <asp:UpdatePanel ID="updateAPIKey" runat="server" UpdateMode="Always" EnableViewState="true">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="setBaseURL" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="setAPI" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="setBlogID" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="Import" EventName="Click" />
        </Triggers>
        <ContentTemplate>
           <div class="firstSelectorHeader"><h2>I have:</h2></div>
            <div class="firstSelectors">
                <asp:RadioButtonList ID="RadioButtonList1" runat="server">
                    <asp:ListItem Text="Articles" Value="articles"></asp:ListItem>
                    <asp:ListItem Text="Videos" Value="video"></asp:ListItem>
                    <asp:ListItem Text="Both" Value="both"></asp:ListItem>
                    <asp:ListItem Text="Archive" Value="archive"></asp:ListItem>
                </asp:RadioButtonList></div>
            <asp:Button ID="firstSelectorSubmit" runat="server" Text="Show The Settings" OnClick="showSections" />
                           <h3>
                    Set the Blog that you want to import the articles/videos to:</h3>
                
                <p class="IDs">
                    <h2>Blog Name:</h2>
                    <br />Current Blog:
                    <asp:Literal ID="currentBlogID" runat="server" /><br />
                    <asp:DropDownList runat="server" ID="blogIdDrpDwn" ClientIDMode="Static" AutoPostBack="true"
                        ViewStateMode="Enabled" />
                </p>
                <asp:Button ID="setBlogID" AutoPostBack="true" ClientIDMode="Static" ViewStateMode="Enabled"
                    runat="server" Text="Choose the blog" OnClick="setBlogID_Click" />
            <asp:Label CssClass="error" ID="labelError" runat="server" />

            <asp:PlaceHolder runat="server" ID="ArticlePlaceHolder" Visible="false">
            <h2>Article Settings</h2>
            <p>
                Before articles can be imported from our feed all of these statements have to be
                <span class="boolTrue">True</span>.</p>
            <div id="checks">
                <p>
                    1.) Is the DotNetNuke Blog Module Installed?
                    <asp:Label ID="boolBlogModule"  runat="server" /></p>
                <p>
                    2.) Have you created a blog?
                    <asp:Label ID="boolBlogCreated" runat="server" /></p>
                <p>
                    3.) Are friendly URLs turned on?
                    <asp:Label ID="boolFriendURL" runat="server" /></p>
                <p>
                    4.) Have you set the specific blog that you want to import the articles into?
                    <asp:Label ID="boolCheckBlogID" runat="server" /></p>
                <p>
                    5.) Have you set the Base Url?
                    <asp:Label ID="boolCheckUrl" runat="server" /></p>
                <p>
                    6.) Have you set the Brafton API Key?
                    <asp:Label ID="boolCheckAPI" runat="server" /></p>
                
                   
            </div>
            <asp:PlaceHolder ID="setAPIPH" runat="server" Visible="false">
                
                    
                <p class="IDs">
                    <h2>BASE URL</h2>
                    Set The Base URL Here:
                    <br /> <%--Current Base URL:--%>
                    Default http://api.brafton.com/ or http://api.contentlead.com/
                    <asp:Literal ID="baseURLLabel" runat="server" /><br />
                    <asp:TextBox ID="baseURL" runat="server" Width="350px"></asp:TextBox></p>
                <asp:Button ID="setBaseURL" runat="server" Text="Set Base URL" OnClick="setBaseURL_Click" />

                <p class="IDs">
                    <h2>API Key</h2>
                    Set The API Key Here:
                    <br /><%--Current API:--%>
                    <asp:Literal ID="apiURLLabel" runat="server" /><br />
                    <asp:TextBox ID="apiURL" runat="server" Width="350px"></asp:TextBox></p>
                <asp:Button ID="setAPI" runat="server" Text="Set API" OnClick="setAPI_Click" />
            </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="nextStep" Visible="true">


                </asp:PlaceHolder>
                </asp:PlaceHolder><%-- end of article placeholder --%>

            <asp:PlaceHolder ID="setARCHIVES" runat="server" Visible="false">
                
                    
                <p class="IDs">
                    <h2>ARCHIVE URL</h2>
                    Set The Archive URL Here:
                    <br /> <%--Archive URL:--%>
                    <asp:Literal ID="archiveURLLabel" runat="server" /><br />
                    <asp:TextBox ID="archiveURL" runat="server" Width="350px"></asp:TextBox></p>
                </asp:PlaceHolder><%-- end of article placeholder --%>

             <asp:PlaceHolder runat="server" ID="VideoPlaceHolder" Visible="false">
                <h3>Video Section</h3>
                <em>*if you do not have video included with your account please do not adjust anything below as it will have adverse effects on your blog.</em>
                <br /><br />
                <%-- <asp:CheckBox ID="InclVideo" runat="server" AutoPostBack="true" OnCheckedChanged="showVideoSettings"  />--%>
                 <asp:PlaceHolder runat="server" ID="VideoSettings" Visible="false" ViewStateMode="Enabled">
                <br /><br />
                 <panel id="VideoSettingsDiv" runat="server">   
                 <div class="vidInputsContainer">
                    <asp:PlaceHolder runat="server" ID="CurrentVidSetting1" Visible="false">
                    <br />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="CurrentVidSetting2" Visible="true">
                       Please provide the following (These values can be obtained from your Account Manager):<br />
                      </asp:PlaceHolder>
                <div class="vidSettingLabel">Video Base URI(Default: api.video.brafton.com):</div><div class="vidSettingTextField"><asp:TextBox ID="VideoBaseURL" runat="server" Width="350px"></asp:TextBox></div><br />

                <div class="vidSettingLabel">Video Base URI(Default: pictures.video.brafton.com):</div><div class="vidSettingTextField"><asp:TextBox ID="VideoPhotoURL" runat="server" Width="350px"></asp:TextBox></div><br />

                <div class="vidSettingLabel">Video Public Key:</div><div class="vidSettingTextField"><asp:TextBox ID="VideoPublicKey" runat="server" Width="350px"></asp:TextBox></div><br />

                <div class="vidSettingLabel">Video Secret Key:</div><div class="vidSettingTextField"><asp:TextBox ID="VideoSecretKey" runat="server" Width="350px"></asp:TextBox></div><br />

                <div class="vidSettingLabel">Video Feed Number:</div><div class="vidSettingTextField"><asp:TextBox ID="VideoFeedNumber" runat="server" Width="350px"></asp:TextBox></div><br />

                <asp:Button ID="setVidSettings" runat="server" Text="Set Video Settings" OnClick="checkForVideo" />
                <asp:Button ID="updateVidSettings" runat="server" Text="Update Video Settings" Visible="false" OnClick="checkForVideo" />
                     </div>
                     </panel> 
                </asp:PlaceHolder>


                 </asp:PlaceHolder><%-- End of video placeholder--%>

                <h3>Article Feed Updates</h3>
                Include updated feed content when syncing?
                <asp:CheckBox ID="InclUpdatedFeedContent" runat="server"  />
                <br />
                <em>*articles will only update at most 1 time per day regardless of the frequency of running the importer</em> 
                <br /><br /><br />
                

                <placeholder id="errorMessages" runat="server" Visible="false">
                    <p class="IDs">
                    Debuggin Message:
                    <asp:Literal  ID="errorCheckingLabel" runat="server" /><br />
                    Checked Status:
                    <asp:Literal  ID="checkedStatusLabel" runat="server" /><br />
                    Global Error:
                    <asp:Literal  ID="globalErrorMessage" runat="server" /><br />
                    </p>
                 </placeholder>
            
            <br />
            <br />
            <asp:Button ID="Import" AutoPostBack="true" ClientIDMode="Static" ViewStateMode="Enabled"
                Visible="false"  runat="server" Text="Import Articles" OnClick="Import_Click" />
          <asp:Button ID="ShowGlobals" AutoPostBack="true" ClientIDMode="Static" ViewStateMode="Enabled"
                Visible="false"  runat="server" Text="Show Messages" OnClick="show_globals" />
           <asp:Button ID="SaveSettings" AutoPostBack="true" ClientIDMode="Static" ViewStateMode="Enabled"
                Visible="false"  runat="server" Text="Save Settings" OnClick="saveSettings" />
            <asp:UpdateProgress ID="UpdateProgress1" runat="server" DynamicLayout="false">
                <ProgressTemplate>
                    <div id="updateBack">
                        
                        <h1>Loading ...</h1>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>













