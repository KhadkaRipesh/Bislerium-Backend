using Application.Bislerium;
using Domain.Bislerium.DTOs.Blog;
using Domain.Bislerium.Enums;
using Domain.Bislerium.Exceptions;
using Domain.Bislerium.Models;
using Infrastructure.Bislerium.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Slugify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Infrastructure.Bislerium.Services
{
    public class BlogService: IBlogService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IUserService userService;
        private readonly IFirebaseService _firebaseService;
        private readonly string _environment;

        public BlogService(ApplicationDBContext dbContext, IUserService userService, IFirebaseService firebaseService)
        {
            _dbContext = dbContext;
            this.userService = userService;
            this._firebaseService = firebaseService;
            _environment = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"); ;
        }

        public async Task<Blog> AddNewBlog(CreateBlog blogDto)
        {

            User? user = await userService.GetCurrentUser();
            if (user == null)
                throw new ProgramException("Please Login Before Adding Blog.");

            if (user.Role != UserRole.BLOGGER)
                throw new ProgramException("Bloggers Are Only Allowed To Add Blog");

            Blog blog = blogDto.ToBlog();


            if (!IsImageValid(blogDto.Image))
            {
                throw new ProgramException("Blog Image Must Be In The Format Of Eihter .png, .jpg, or .jpeg");
            }

            string userDirectory = Path.Combine(_environment, "Blog");
            if (!Directory.Exists(userDirectory))
            {
                Directory.CreateDirectory(userDirectory);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(blogDto.Image.FileName);
            string imagePath = Path.Combine(userDirectory, uniqueFileName);

            using (FileStream fileStream = System.IO.File.Create(imagePath))
            {
                await blogDto.Image.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
            }

            string relativeImagePath = Path.Combine("Blog", uniqueFileName);

            blog.Image = relativeImagePath;
            blog.UserID = user.ID;

            SlugHelper slugHelper = new SlugHelper();
            blog.Slug = slugHelper.GenerateSlug(blog.Title);

            _dbContext.Blogs.Update(blog);
            await _dbContext.SaveChangesAsync();

            // Send push notification to Admin

            // await _firebaseService.SendPushNotifications(new List<string> { "A072D4AF-6D46-42B0-9BFB-E9DD9AB9BAC0" }, "Test", "This is test notification");

            return blog;
        }

        // To check if the email is valid or not
        private bool IsImageValid(IFormFile image)
        {
            // Check the file format
            string[] allowedFormats = { ".jpg", ".png", ".jpeg" };
            string fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!allowedFormats.Contains(fileExtension))
            {
                return false;
            }

            // Check the file size
            long fileSizeLimit = 3_000_000; // 3 MB
            if (image.Length > fileSizeLimit)
            {
                return false;
            }

            return true;
        }

        // Update blog
        public async Task<Blog> UpdateBlog(Guid id, UpdateBlog updateBlogDto)
        {
            User? user = await userService.GetCurrentUser();
            if (user == null)
                throw new ProgramException("Please Login Before Updating Blog.");


            Blog? blog = await _dbContext.Blogs.FirstOrDefaultAsync(b => b.ID == id);
            if (blog == null)
                throw new ProgramException("Blog Not Found");

            if (blog.IsDeleted)
                throw new ProgramException("Blog Not Found");

            if (blog.UserID != user.ID)
                throw new ProgramException("You Are Not Authorized To Update This Blog");

            blog.Title = updateBlogDto.Title == null || updateBlogDto.Title.Equals("") ? blog.Title : updateBlogDto.Title;
            blog.Summary = updateBlogDto.Summary == null || updateBlogDto.Summary.Equals("") ? blog.Summary : updateBlogDto.Summary;
            blog.Body = updateBlogDto.Body == null || updateBlogDto.Body.Equals("") ? blog.Body : updateBlogDto.Body;

            if (updateBlogDto.Image != null)
            {
                if (!IsImageValid(updateBlogDto.Image))
                {
                    throw new ProgramException("Blog Image Must Be In The Format Of Eihter .png, .jpg, or .jpeg and less than 3 MB");
                }

                string userDirectory = Path.Combine(_environment, "Blog");
                if (!Directory.Exists(userDirectory))
                {
                    Directory.CreateDirectory(userDirectory);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(updateBlogDto.Image.FileName);
                string imagePath = Path.Combine(userDirectory, uniqueFileName);

                using (FileStream fileStream = System.IO.File.Create(imagePath))
                {
                    await updateBlogDto.Image.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }

                string relativeImagePath = Path.Combine("Blog", uniqueFileName);

                blog.Image = relativeImagePath;
            }

            SlugHelper slugHelper = new SlugHelper();
            blog.Slug = slugHelper.GenerateSlug(blog.Title);

            _dbContext.Blogs.Update(blog);
            await _dbContext.SaveChangesAsync();

            return blog;
        }

        // Delete blog
        public async Task<Blog> DeleteBlog(Guid id)
        {
            User? user = await userService.GetCurrentUser();
            if (user == null)
                throw new ProgramException("Please Login Before Deleting Blog.");

            Blog? blog = await _dbContext.Blogs.FirstOrDefaultAsync(b => b.ID == id);
            if (blog == null)
                throw new ProgramException("Blog Not Found");

            if (blog.UserID != user.ID)
                throw new ProgramException("You Are Not Authorized To Delete This Blog");


            blog.IsActive = false;
            blog.IsDeleted = true;

            _dbContext.Blogs.Update(blog);
            await _dbContext.SaveChangesAsync();

            return blog;
        }

        // Get All Blog by Blogger id
        public async Task<IEnumerable<Blog>> GetAllBlogsByBlogger(Guid id)
        {
            IEnumerable<Blog> blogs = await _dbContext.Blogs
                                       .OrderByDescending(b => b.CreatedAt)
                                       .Where(b => b.IsActive && !b.IsDeleted)
                                       .Where(b => b.UserID == id)
                                       .ToListAsync();
            return blogs;
        }

        // Calculate Blog Popularity
        private double CalculatePopularityScore(Blog blog)
        {
            double score = 0;

            var commentCount = _dbContext.Comments.Where(c => c.BlogID == blog.ID).Count();

            var likeCount = _dbContext.Reactions.Where(r => r.BlogID == blog.ID && r.Type == ReactionType.LIKE).Count();

            var dislikeCount = _dbContext.Reactions.Where(r => r.BlogID == blog.ID && r.Type == ReactionType.DISLIKE).Count();


            score = (commentCount * 1) + (likeCount * 2) + (dislikeCount * (-1));
            return score;
        }

        // Get All Blogs
        public async Task<IEnumerable<Blog>> GetAllBlogs()
        {
            IEnumerable<Blog> blogs = await _dbContext.Blogs
                                       .Where(b => b.IsActive && !b.IsDeleted)
                                       .ToListAsync();
            blogs = blogs.OrderByDescending(blog => CalculatePopularityScore(blog));

            return blogs;
        }

        // Get Blog by popularity on specfic month
        public async Task<IEnumerable<Blog>> GetAllBlogsByPopularityByMonth(int month)
        {
            IEnumerable<Blog> blogs = await _dbContext.Blogs
                                                    .ToListAsync();

            if (month == 0)
            {
                blogs = blogs.OrderByDescending(blog => CalculatePopularityScore(blog)).Take(10);
            }
            else
            {
                var startDate = new DateTime(DateTime.Now.Year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                blogs = blogs.Where(b => b.CreatedAt >= startDate && b.CreatedAt <= endDate)
                            .Where(b => !b.IsDeleted && b.IsActive);

                blogs = blogs.OrderByDescending(blog => CalculatePopularityScore(blog)).Take(10);
            }

            return blogs;
        }

        // Get Blog By Id
        public async Task<Blog> GetBlogById(Guid id)
        {
            Blog? existingBlog = await _dbContext.Blogs.FirstOrDefaultAsync(b => b.ID == id);


            if (existingBlog == null)
            {
                throw new ProgramException("Blog not found");
            }

            if (existingBlog.IsDeleted)
                throw new ProgramException("Blog Not Found");

            if (existingBlog.IsActive == false)
                throw new ProgramException("Blog Not Found");

            return existingBlog;
        }

        // Get Blog by month
        public async Task<IEnumerable<Blog>> GetAllBlogsByMonth(int month)
        {
            if (month == 0)
            {
                return await _dbContext.Blogs
                                       .OrderByDescending(b => b.CreatedAt)
                                       .Where(b => b.IsActive && !b.IsDeleted)
                                       .ToListAsync();
            }

            var startDate = new DateTime(DateTime.Now.Year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var blogs = await _dbContext.Blogs
                .Where(b => b.CreatedAt >= startDate && b.CreatedAt <= endDate)
                .Where(b => !b.IsDeleted && b.IsActive)
                .ToListAsync();

            return blogs;
        }

        // Get Blog by Slug
        public async Task<Blog> GetBlogBySlug(string slug)
        {
            var existingBlog = await _dbContext.Blogs.Include(b => b.User)
                                     .FirstOrDefaultAsync(b => b.Slug == slug);

            if (existingBlog == null)
            {
                throw new ProgramException("Blog Not Found");
            }

            if (existingBlog.IsDeleted)
                throw new ProgramException("Blog Not Found");

            if (existingBlog.IsActive == false)
                throw new ProgramException("Blog Not Found");

            return existingBlog;

        }

        // Get my all blogs
        public async Task<IEnumerable<Blog>> GetMyBlogs()
        {
            User? user = await userService.GetCurrentUser();
            if (user == null)
                throw new ProgramException("Please Login Before Getting Blog.");

            IEnumerable<Blog> blogs = await _dbContext.Blogs.Where(b => b.IsActive && !b.IsDeleted).Where(b => b.UserID == user.ID).OrderByDescending(b => b.CreatedAt).ToListAsync();

            if (blogs == null)
                throw new ProgramException("Blog Not Found");

            return blogs;
        }



    }
}
