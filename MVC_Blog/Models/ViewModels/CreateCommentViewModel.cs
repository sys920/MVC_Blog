using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_Blog.Models.ViewModels
{
    public class CreateUpadteCommentViewModel
    {

        public string CommentMessage { get; set; }
         
        public string CommentUpdatedReason { get; set; }

        public int? CommentUpdateId { get; set; }

        public int? CommentDeleteId { get; set; }


    }
}