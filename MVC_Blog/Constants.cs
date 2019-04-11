using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_Blog
{
    public static class Constants
    {
        public static readonly List<string> AllowedFileExtensions =
                  new List<string> { ".jpg",".JPG", ".jpeg", ".JPEG",".png", ".PNG" };

        public static readonly string UploadFolder = "~/Upload/";

        public static readonly string MappedUploadFolder = HttpContext.Current.Server.MapPath(UploadFolder);

    }
}