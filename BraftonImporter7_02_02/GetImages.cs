using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using ImportImages;
using System.IO;
using System.Drawing;
using System.Net;
using System.Web;

using Brafton.Modules.Globals;

class GetImages
{

    public string _description;
    public string _entry;
    public string _photoURL;
    public string _appPath;
    public string _caption;
    public string _descript;

    public GetImages(string photoURL, string entry, string description, string appPath, string caption)
    {
        _description = description;
        _entry = entry;
        _photoURL = photoURL;
        _appPath = appPath;
        _caption = caption;
    }



    public void DownloadImageDebug()
    {
        //string RootURl = HttpContext.Current.Request.Url.Host;
        //RootURl = "http://" + RootURl;
        string imageName = "";

        // To grab the first instance of the <p> and insert the image in there
        int tmpDesPos = _description.IndexOf("<p>");
        int tmpEntPos = _entry.IndexOf("<p>");

        // Download the image from the feed
        int lastPlace = _photoURL.IndexOf("_", 0);
        int firstPlace = _photoURL.LastIndexOf("/");

        if (MyGlobals.imageID == "xxx")//if the global imageid is default it is article else it is video
        {
            imageName = _photoURL.Substring(firstPlace, lastPlace - firstPlace);
        }
        else
        {
            imageName ="/"+ MyGlobals.imageID;
        }
        

        //string checkImage = System.Environment.CurrentDirectory + "images\\" + imageName.Substring(1) + ".jpg";
        string checkImage = _appPath + "images\\" + imageName.Substring(1) + ".jpg";

        //Keep this before the image download in case the posts were deleted for reimport
        _entry = _entry.Insert(tmpEntPos + "<p>".Length, "<div class='newsThumbSingle'><img alt=\"" + _caption + "\" class='thumbnail' width='200px' height='200px' src='"  + "/images" + imageName + ".jpg' /><div class='caption'>" + _caption + "</div></div>");
        _entry = _entry.Insert(0, "<style type='text/css'> .newsThumbSingle {background:#ddd;margin:5px;padding:5px;position:relative;float:right;-moz-border-radius: 3px;border-radius: 3px;z-index:1;text-align:center;max-width:200px;} .caption{font-size:10px;line-height:normal;}</style>");

        _description = _description.Insert(0, "<img class='thumbnail' style='padding:10px' align='left' width='150px' height='150px' src='" + "/images" + imageName + ".jpg' />");

        //check to see if image exists in images folder if not download it
        if (!File.Exists(checkImage))
        {
            //MyGlobals.imageInfo = MyGlobals.imageInfo + "made it to call image download";
            ImageDownload imgObject = new ImageDownload();
            imgObject.saveImage(_photoURL, imageName);
        }
    }

}

//ORiginal DownloadImage Method replaced 7/2014 fjd
//public void DownloadImage()
//{

//    // To grab the first instance of the <p> and insert the image in there
//    int tmpDesPos = _description.IndexOf("<p>");
//    int tmpEntPos = _entry.IndexOf("<p>");

//    // Download the image from the feed
//    int lastPlace = _photoURL.IndexOf("_", 0);
//    int firstPlace = _photoURL.LastIndexOf("/");
//    string imageName = _photoURL.Substring(firstPlace, lastPlace - firstPlace);
//    string checkImage = HostingEnvironment.ApplicationPhysicalPath + "images\\" + imageName.Substring(1) + ".jpg";

//    //Keep this before the image download in case the posts were deleted for reimport
//    _entry = _entry.Insert(tmpEntPos + "<p>".Length, "<div class='newsThumbSingle'><img alt=\"" + _caption + "\" class='thumbnail' width='200px' height='200px' src='" + _appPath + "/images" + imageName + ".jpg' /><div class='caption'>" + _caption + "</div></div>");
//    _entry = _entry.Insert(0, "<style type='text/css'> .newsThumbSingle {background:#ddd;margin:5px;padding:5px;position:relative;float:right;-moz-border-radius: 3px;border-radius: 3px;z-index:1;text-align:center;max-width:200px;} .caption{font-size:10px;line-height:normal;}</style>");

//    _description = _description.Insert(0, "<img class='thumbnail' style='padding:10px' align='left' width='150px' height='150px' src='" + _appPath + "/images" + imageName + ".jpg' />");

//    if (!File.Exists(checkImage))
//    {
//        ImageDownload imgObject = new ImageDownload();
//        imgObject.saveImage(_photoURL, imageName);     
//    }
//}
