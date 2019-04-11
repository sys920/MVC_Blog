using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_Blog.Models.Domain
{
    public class Comment
    {
        public int Id { get; set; }
        public string CommentMessage { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime? DateUpdate { get; set; }
        public string CommentUpdatedReason { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Post Post { get; set; }       
    }
}