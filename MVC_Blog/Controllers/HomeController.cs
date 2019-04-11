using Microsoft.AspNet.Identity;
using MVC_Blog.Models;
using MVC_Blog.Models.Domain;
using MVC_Blog.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Data.Entity;

namespace MVC_Blog.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext DbContext;
        private List<IndexPostViewModel> model;

        public HomeController() 
        {
            DbContext = new ApplicationDbContext();
        }
        
        [HttpGet]
        //Display All Posts or Result searched 
        public ActionResult Index(string searchKeyword)
        {
            ViewBag.SearchKeyword = searchKeyword;
            var isAdmin = User.IsInRole("Admin");

            if (searchKeyword == null )
            {
                model = DbContext.Posts.Where(p => isAdmin || p.Published)
                .Select(p => new IndexPostViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Body = p.Body,
                    DateCreate = p.DateCreate,
                    DateUpdate = p.DateUpdate,
                    MediaUrl = p.MediaUrl,
                    Published = p.Published,
                    MainSlide = p.MainSlide,
                    Slug = p.Slug
                }).ToList();
            }
            else
            {
                model = DbContext.Posts.Where(p => (isAdmin || p.Published) && (p.Title.Contains(searchKeyword) || p.Slug.Contains(searchKeyword) || p.Body.Contains(searchKeyword)))
                .Select(p => new IndexPostViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Body = p.Body,
                    DateCreate = p.DateCreate,
                    DateUpdate = p.DateUpdate,
                    MediaUrl = p.MediaUrl,
                    Published = p.Published,
                    MainSlide = p.MainSlide,
                    Slug = p.Slug
                }).ToList();    
            }
            return View(model);
        }
       
        [HttpGet]
        //[Route("Blog/{SlugName}")]
        //Display post Detail and all Comments 
        public ActionResult DetailBySlugName(string slugName, int? CommentUpdateId)
        {            
            if (string.IsNullOrWhiteSpace(slugName))
            {
                return RedirectToAction(nameof(HomeController.Index));
            }

            var post = DbContext.Posts.FirstOrDefault(p => p.Slug == slugName);

            if (post == null)
            {
                return RedirectToAction(nameof(HomeController.Index));
            }            

            var model = new DetailPostViewModel();
            model.Id = post.Id;
            model.Title = post.Title;
            model.Body = post.Body;
            model.Published = post.Published;
            model.DateCreate = post.DateCreate;
            model.DateUpdate = post.DateUpdate;
            model.MediaUrl = post.MediaUrl;
            model.Slug = slugName;
            model.CommentUpdateId = CommentUpdateId;

            //Display user's commnet  in the textBox when admin need to be updated  
            if (CommentUpdateId != null)
            {
                var updatedComment = DbContext.Comments.FirstOrDefault(p => p.Id == CommentUpdateId);

                model.CommentUpdateId = CommentUpdateId;
                model.CommentMessage = updatedComment.CommentMessage;
                model.CommentUpdatedReason = updatedComment.CommentUpdatedReason;
            }

            // Get comment on this post
            var comments = DbContext.Comments.Where(p => p.Post.Id == post.Id).ToList(); 
            model.Comments = comments;
            return View("Detail", model);
        }
        
        //Manage Comment : Create, delete, update and add reason 
        [HttpPost]
        [Authorize]
        public ActionResult DetailBySlugName(string SlugName, CreateUpadteCommentViewModel formData)
        {
            var post = DbContext.Posts.FirstOrDefault(p => p.Slug == SlugName);  
            var userId = User.Identity.GetUserId();
            var userName = User.Identity.GetUserName();

            //User Create new comment 
            if (formData.CommentMessage != null && formData.CommentUpdateId == null) 
            { 
                var comment = new Comment();
                comment.CommentMessage = formData.CommentMessage;               
                comment.DateCreate = DateTime.Now;
                comment.UserId = userId;
                
                post.Comments.Add(comment);

                DbContext.SaveChanges();
                
                ModelState.Clear();
            }
            //Update comment when message and Reason are in the formData 
            else if (formData.CommentMessage != null && formData.CommentUpdatedReason != null &&  formData.CommentUpdateId != null)
            {
                var updateComment = DbContext.Comments.FirstOrDefault(p => p.Id == formData.CommentUpdateId);
                updateComment.CommentMessage = formData.CommentMessage;
                updateComment.CommentUpdatedReason = formData.CommentUpdatedReason;

                updateComment.DateUpdate = DateTime.Now;
              
                DbContext.SaveChanges();
                ModelState.Clear();
                formData.CommentUpdateId = null;
            }

            //Delete comment 
            if (formData.CommentDeleteId != null && (User.IsInRole("Admin") || User.IsInRole("Moderator")))
            {
                var comment = DbContext.Comments.FirstOrDefault(p => p.Id == formData.CommentDeleteId);
                if (comment != null)
                {
                    DbContext.Comments.Remove(comment);
                    DbContext.SaveChanges();
                }
              
            }

            //Create new model for detail view with comment
            var model = new DetailPostViewModel();
            model.Id = post.Id;
            model.Title = post.Title;
            model.Body = post.Body;
            model.Published = post.Published;
            model.DateCreate = post.DateCreate;
            model.DateUpdate = post.DateUpdate;
            model.MediaUrl = post.MediaUrl;
            model.Slug = SlugName;
            
           
            //var comments = DbContext.Comments.Where(p => p.Post.Id == post.Id).Include(p => p.User).ToList();
            var comments = DbContext.Comments.Where(p => p.Post.Id == post.Id).ToList();

            model.Comments = comments;           
          
            return RedirectToAction("DetailBySlugName", "Home", new { SlugName, formData.CommentUpdateId } );
        }        

        [HttpGet]
        [Authorize(Roles = "Admin")]
        //Create new post view
        public ActionResult Create()
        {          
            return View();
        }

        [HttpPost]
        //Redirecting Save Action for creating  
        [Authorize(Roles = "Admin")]
        public ActionResult Create(CreatePostViewModel inputFormData)
        {
            return SavePost(null, inputFormData);
        }

        //Post save and update
        public ActionResult SavePost(int? id, CreatePostViewModel inputFormData)
        {
            Post post;

            if (!ModelState.IsValid)
            {
                return View();
            }

            var userId = User.Identity.GetUserId();

            string fileExtension;
            if (inputFormData.Media != null)
            {
                fileExtension = Path.GetExtension(inputFormData.Media.FileName);

                if (!Constants.AllowedFileExtensions.Contains(fileExtension)) 
                {
                    ModelState.AddModelError("", "Only jpg, jpeg, png extensions are allowed");
                    return View();
                }
            }                   

            //New post
            if (!id.HasValue)
            {
                var slug = GenerateSlug(inputFormData.Title);
                var uniqueSlugLoop = false;


                while (!uniqueSlugLoop == true)
                {
                    //check Slug name whether duplicated 
                    var slugInPosts = DbContext.Posts.FirstOrDefault(
                      p => p.Slug == slug);
                    if (slugInPosts != null)
                    {
                        var randomNumber = new Random().Next(1, 99);
                        slug = slug + "-" + randomNumber;
                    }
                    else
                    {
                        uniqueSlugLoop = true;
                    }
                   
                }

                post = new Post();
                post.UserId = userId;
                post.DateCreate = DateTime.Now;
                post.Slug = slug;
                DbContext.Posts.Add(post);
            }
            else
            //Update post
            {
                post = DbContext.Posts.FirstOrDefault(
                    p => p.Id == id);
                if (post == null)
                {
                    return RedirectToAction(nameof(HomeController.Index));
                }
                post.DateUpdate = DateTime.Now;
            }

            post.Title = inputFormData.Title;
            post.Body = inputFormData.Body;     

            post.Published = inputFormData.Published;
            post.MainSlide = inputFormData.MainSlide;

            //Image file check 
            if (inputFormData.Media != null)
            {
                if (!Directory.Exists(Constants.MappedUploadFolder))
                {
                    Directory.CreateDirectory(Constants.MappedUploadFolder);
                }

                var fileName = inputFormData.Media.FileName;
                var fullPathFileName = Constants.MappedUploadFolder + fileName;
                
                inputFormData.Media.SaveAs(fullPathFileName);
                post.MediaUrl = Constants.UploadFolder + fileName;
            }

            DbContext.SaveChanges();

            return RedirectToAction(nameof(HomeController.Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        //Display Post in the input box for editing
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(HomeController.Index));
            }

            var post = DbContext.Posts.FirstOrDefault(
                p => p.Id == id);

            if (post == null)
            {
                return RedirectToAction(nameof(HomeController.Index));
            }

            var model = new CreatePostViewModel();
            model.Title = post.Title;
            model.Body = post.Body;
            model.Published = post.Published;
            model.MainSlide = post.MainSlide;
            model.MediaUrl = post.MediaUrl;

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        //Redirecting Save Action for editing  
        public ActionResult Edit(int id, CreatePostViewModel inputFormData)
        {
            return SavePost(id, inputFormData);
        }

        [Authorize(Roles = "Admin")]
        //Delete Post
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(HomeController.Index));
            }

            var post = DbContext.Posts.FirstOrDefault(p => p.Id == id);

            if (post != null)
            {
                //Check post has comments 
                var comments = DbContext.Comments.Where(p => p.Post.Id == id);
                if (comments.Count() > 0 )
                {
                    //Firt comment need to be removed 
                    foreach (var ele in comments)
                    {
                        DbContext.Comments.Remove(ele); 
                    }

                    DbContext.SaveChanges();
                }               

                DbContext.Posts.Remove(post);
                DbContext.SaveChanges();
            }
            
            return RedirectToAction(nameof(HomeController.Index));
        }

        //Create slug name from post title
        public string GenerateSlug(string Title)
        {
            string str = Title.ToLower();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", " ").Trim();
            str = Regex.Replace(str, @"\s", "-");
            str = str.Substring(0, str.Length <= 70 ? str.Length : 70).Trim();
            if (str == "")
            {
                str = "default-slugname";
            }
            return str;
        }

       
    }
}