using Application.Bislerium;
using Domain.Bislerium.DTOs.Comment;
using Domain.Bislerium.DTOs.Notification;
using Domain.Bislerium.Exceptions;
using Domain.Bislerium.Models;
using Infrastructure.Bislerium.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Bislerium.Services
{
    public class CommentService: ICommentService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IUserService userServices;
        private readonly IBlogService blogService;
        private readonly INotificationService notificationService;

        public CommentService(ApplicationDBContext dbContext, IUserService userServices, IBlogService blogService, INotificationService notificationService)
        {
            this._dbContext = dbContext;
            this.userServices = userServices;
            this.blogService = blogService;
            this.notificationService = notificationService;
        }

        // Add Comment
        public async Task<Comment> AddNewComment(CreateComment payload)
        {
            User? user = await userServices.GetCurrentUser();

            if (user == null)
                throw new ProgramException("Please Login Before Adding Comment.");

            Blog blog = await blogService.GetBlogById(payload.BlogID) ?? throw new ProgramException("Blog Not Found.");

            Comment comment = payload.ToComment();
            comment.UserID = user.ID;
            comment.BlogID = blog.ID;
            comment.Body = payload.Body;

            await _dbContext.Comments.AddAsync(comment);
            await _dbContext.SaveChangesAsync();

            if (blog.UserID != user.ID)
            {
                CreateNotification notificationDto = new CreateNotification
                {
                    Body = $"{user.UserName} comments on your post",
                    UserID = user.ID
                };
                await notificationService.AddNewNotification(notificationDto);
            }

            return comment;
        }

        // Get Comment By Id
        public async Task<Comment> GetCommentById(Guid? id)
        {
            Comment? comment = await _dbContext.Comments.FindAsync(id);

            if (comment == null)
                throw new ProgramException("Comment Not Found.");

            if (comment.IsDeleted)
                throw new ProgramException("Comment Not Found.");

            return comment;
        }

        // Get All Comments
        public async Task<IEnumerable<Comment>> GetAllComments()
        {
            IEnumerable<Comment> comments = await _dbContext.Comments
                .Include(c => c.User)
                .Include(c => c.Blog)
                .Where(c => c.IsActive && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return comments;
        }


        // Get Comments By Slug
        public async Task<IEnumerable<Comment>> GetCommentsBySlug(string slug)
        {
            Blog? blog = await _dbContext.Blogs
                                     .FirstOrDefaultAsync(b => b.Slug == slug);
            if (blog == null)
                throw new ProgramException("Blog Not Found.");

            IEnumerable<Comment> comments = await _dbContext.Comments.Where(c => c.BlogID == blog.ID).Where(c => c.IsActive && !c.IsDeleted).Include(c => c.User)
                .Include(c => c.Blog).OrderByDescending(c => c.CreatedAt).ToListAsync(); ;
            return comments;
        }

        // Delete Comment
        public async Task<Comment> DeleteComment(Guid id)
        {
            User? user = await userServices.GetCurrentUser();
            if (user == null)
                throw new ProgramException("Please Login Before Deleting Comment.");

            Comment? comment = await _dbContext.Comments.FirstOrDefaultAsync(c => c.ID == id);

            if (comment == null)
                throw new ProgramException("Comment Not Found.");

            comment.IsDeleted = true;
            comment.IsActive = false;

            _dbContext.Comments.Update(comment);
            await _dbContext.SaveChangesAsync();

            return comment;
        }


    }
}
