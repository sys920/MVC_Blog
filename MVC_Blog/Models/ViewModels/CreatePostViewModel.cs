using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MVC_Blog.Models.ViewModels

{
    public class CreatePostViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required, AllowHtml]
        public string Body { get; set; }     
        
        public Boolean Published { get; set; }

        public Boolean MainSlide { get; set; }

        public HttpPostedFileBase Media { get; set; }

        public string MediaUrl { get; set; }

        public string Slug { get; set; }
    }
}