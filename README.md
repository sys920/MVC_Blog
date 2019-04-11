# MVC Blog 
- This blog was made by Bootstrap template for a professional appearance 
- Admin only Posting new post and Managing,  Other login user can see the post and comment all posts 
- This blog support local and External(facebook) login 

# code 
ASP.NET Web Application (.NET Framework 4.6) 
- This source code doesn't have Web.config, Startup.Auth.cs because of personal informaiton.
- Controller/HomeController : All function Create, Edit, Delete post and commnet  
- Models/Domain : Commnet.cs, Post.cs  object 
- Models/ViewModels : All view models Create(post, commnet), Edit, search, Detail, index 
- Views/Home : All views Create(post, commnet), Edit, search, Detail, index
- Migrations/Configuration.cs : First Seed data 
- Css/ : flexslider.css : main slider
- js/ : fjquery.flexslider-min.js : main slider
- Scripts/tinymce : Files upload javascript

# Requiremnet :
- Create/Edit Posts : Page to input information to create or edit posts. TinyMCE and File upload must be included in the page.
- List Posts : Page to list all posts. This page can display additionally functionality/buttons based on user role. This page will not display the full details of the post.
- Delete Posts : Page or button to delete posts 

- View Post : Page used to display the full details of the post
- Admin users should be able to see all posts. Other users or users that are not logged-in should only be able to see posts that are published (published flag = true) 
- Add a Slug auto-generation functionality to the Post. Slug is a user friendly and URL valid name of a post. It should be unique, based on the title of the post and generated automatically on post creation. Special characters should be removed from it. If the generated slug already exists, a random number should be appended at the end. Slugs must be saved to the database

- Update all the links to show the details of the post to use the slug instead of the ID. You don’t need to update the links to edit or delete posts. The format of the link should be /blog/slug-of-the-post.
- Add a Comment class to the application. Comments should have body, date created, date updated and updated reason. 
- Comments should have a one-to-many relationship with Post (One Post has many Comments and one Comment belongs to a Post)
- Comments should have a one-to-many relationship with User (One User has many Comments and a Comment belongs to a User) 
- Any logged-in user should be able to add comments to any post. The page that displays the details of the post should have a section at the bottom to allow the user to add and see the comments of the post.
- Admins or Moderators can edit comments of any user. When doing so, they should provide the reason why the comment is being updated and the system should automatically update the date updated property of the comment.
- Admins or Moderators can delete any comments of any user.
- At least two Third party authentication should be added to the project.
- Users should be able to search posts. The search should be performed on the following fields: Title, Slug and Body.

