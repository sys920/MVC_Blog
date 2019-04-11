using MVC_Blog.Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_Blog.Models.ViewModels
{
    public class DetailPostViewModel
    {
        //Dispaly for Post detail view
        public int Id { get; set; }
        public string Title { set; get; }
        public string Body { set; get; }
        public Boolean Published { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime? DateUpdate { get; set; }
        public string MediaUrl { get; set; }
        public Boolean MainSlide { get; set; }
        public string Slug { get; set; }

        //Display for Comment on this post
        public List<Comment> Comments { get; set; }        

        //For Create or update comment 
        public int? CommentUpdateId { get; set; }
        [Required]
        public string CommentMessage { get; set; }

        [Required]
        public string CommentUpdatedReason { get; set; }      
    }
}