namespace MVC_Blog.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using MVC_Blog.Models;
    using MVC_Blog.Models.Domain;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MVC_Blog.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;

        }

        protected override void Seed(MVC_Blog.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.


            if (!context.Posts.Any())
            {
                var post = new Post();
                post.Title = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
                post.Body = "Nulla a lectus ac dolor finibus rhoncus in sed felis. Suspendisse vel suscipit urna. Vestibulum lorem dolor, accumsan id faucibus non, hendrerit vel eros. Praesent tincidunt nisl at purus eleifend,  Interdum et malesuada fames ac ante ipsum primis in faucibus. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Cras eu ligula pharetra, aliquet felis sit amet, pellentesque orci. Maecenas a magna in tellus malesuada maximus et ac orci. Maecenas at facilisis nulla. Suspendisse et nisl sit amet ex egestas mattis ut a lacus. Integer commodo consectetur sapien quis laoreet. Nulla pulvinar vitae odio sed bibendum. Nunc fermentum, augue at consequat faucibus, nisi dui convallis nunc, sed euismod felis ligula at massa. Nulla vitae vestibulum leo, vel volutpat sem.";

                post.MediaUrl = "~/images/blog-13.jpg";
                post.MainSlide =true;
                post.Published = true;
                post.DateCreate = DateTime.Now;
                post.DateUpdate = DateTime.Now;
                post.Slug = "lorem-ipsum-dolor-sit";

                context.Posts.Add(post);
                context.SaveChanges();
            }

            //RoleManager, used to manage roles
            var roleManager = new RoleManager<IdentityRole>( new RoleStore<IdentityRole>(context));


            //UserManager, used to manage users
            var userManager = new UserManager<ApplicationUser>( new UserStore<ApplicationUser>(context));

            //Adding admin role if it doesn't exist.
            if (!context.Roles.Any(p =>p.Name =="Admin"))
            {
                var adminRole = new IdentityRole("Admin");
                roleManager.Create(adminRole);
            }

            //Adding Moderato role if it doesn't exist.
            if (!context.Roles.Any(p => p.Name == "Moderator"))
            {
                var moderatoRole = new IdentityRole("Moderator");
                roleManager.Create(moderatoRole);
            }



            //Creating the adminUser
            ApplicationUser adminUser, moderatoUser;

            if (!context.Users.Any(p => p.UserName == "admin@blog.com"))
            {
                adminUser = new ApplicationUser();
                adminUser.UserName = "admin@blog.com";
                adminUser.Email = "admin@blog.com";

                userManager.Create(adminUser, "Password-1");
            }
            else
            {
                adminUser = context.Users.First(p => p.UserName == "admin@blog.com");
            }


            //Creating the moderatoUser
            if (!context.Users.Any(p => p.UserName == "moderator@blog.com"))
            {
                moderatoUser = new ApplicationUser();
                moderatoUser.UserName = "moderator@blog.com";
                moderatoUser.Email = "moderator@blog.com";

                userManager.Create(moderatoUser, "Password-1");
            }
            else
            {
                moderatoUser = context.Users.First(p => p.UserName == "moderator@blog.com");
            }

            //Make sure the user is on the admin role
            if (!userManager.IsInRole(adminUser.Id, "Admin"))
            {
                userManager.AddToRole(adminUser.Id, "Admin");
            }

            if (!userManager.IsInRole(moderatoUser.Id, "Moderator"))
            {
                userManager.AddToRole(moderatoUser.Id, "Moderator");
            }
        }
    }
}
