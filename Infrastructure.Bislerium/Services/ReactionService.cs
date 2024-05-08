using Application.Bislerium;
using Domain.Bislerium.DTOs.Notification;
using Domain.Bislerium.DTOs.Reaction;
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
    public class ReactionService: IReactionService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IUserService userServices;
        private readonly IBlogService blogService;
        private readonly ICommentService commentService;
        private readonly INotificationService notificationService;

        public ReactionService(ApplicationDBContext dbContext, IUserService userServices, IBlogService blogService, ICommentService commentService, INotificationService notificationService)
        {
            this._dbContext = dbContext;
            this.userServices = userServices;
            this.blogService = blogService;
            this.commentService = commentService;
            this.notificationService = notificationService;
        }


        // Add reaction
        public async Task<Reaction> AddNewReaction(CreateReaction reactionDto)
        {
            User? user = await userServices.GetCurrentUser();

            if (user == null)
                throw new ProgramException("Please Login Before Adding Reacting.");

            Blog blog = await blogService.GetBlogById(reactionDto.BlogID) ?? throw new ProgramException("Blog Not Found.");

            Reaction? existingReaction = await _dbContext.Reactions
                .FirstOrDefaultAsync(r =>
                    r.UserID == user.ID &&
                    r.BlogID == blog.ID &&
                    r.CommentID == reactionDto.CommentID);

            if (existingReaction != null)
            {
                // If an existing reaction with the same user, blog, and comment ID exists, update its type
                existingReaction.Type = reactionDto.Type;
                _dbContext.Reactions.Update(existingReaction);
                await _dbContext.SaveChangesAsync();

                if (blog.UserID != user.ID)
                {
                    CreateNotification notificationDto = new CreateNotification
                    {
                        Body = $"{user.UserName} reacted on your post",
                        UserID = user.ID
                    };
                    await notificationService.AddNewNotification(notificationDto);
                }

                return existingReaction;
            }

            else
            {
                // Create a new reaction if no existing reaction found for the specified criteria
                Reaction reaction = reactionDto.ToReaction();
                reaction.UserID = user.ID;
                reaction.BlogID = blog.ID;
                reaction.Type = reactionDto.Type;

                if (reactionDto.CommentID != null)
                {
                    Comment comment = await commentService.GetCommentById(reactionDto.CommentID);
                    if (comment == null)
                        throw new ProgramException("Comment Not Found.");

                    reaction.CommentID = comment.ID;
                }


                await _dbContext.Reactions.AddAsync(reaction);
                await _dbContext.SaveChangesAsync();

                if (blog.UserID != user.ID)
                {
                    CreateNotification notificationDto = new CreateNotification
                    {
                        Body = $"{user.UserName} reacted on your post",
                        UserID = user.ID
                    };
                    await notificationService.AddNewNotification(notificationDto);
                }
                return reaction;
            }
        }

        // Get All Reaction
        public async Task<IEnumerable<Reaction>> GetAllReactions()
        {
            IEnumerable<Reaction> reaction = await _dbContext.Reactions.ToListAsync();
            return reaction;
        }
    }
}
