using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_Blog.Models.Domain
{
    public class Post
    {
        public int Id { set; get; }

        public virtual ApplicationUser User { get; set; }
        public string UserId { get; set; }

        public string Title { set; get; }
        public string Body { set; get; }
        public Boolean Published { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime? DateUpdate { get; set; }
 
        public string MediaUrl { get; set; }
        public Boolean MainSlide { get; set; }
        public string Slug { get; set; }

        public virtual List<Comment> Comments { get; set; }

    }
}