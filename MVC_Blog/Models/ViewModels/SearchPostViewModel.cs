using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_Blog.Models.ViewModels
{
    public class SearchPostViewModel
    {
        public int Id { get; set; }
        public string Title { set; get; }
        public string Body { set; get; }
        public Boolean Published { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime? DateUpdate { get; set; }
        public string MediaUrl { get; set; }
        public Boolean MainSlide { get; set; }
        public string Slug { get; set; }
        public string keyword { get; set; }
    }
}