using Domain.Bislerium.DTOs.Blog;
using Domain.Bislerium.DTOs.Comment;
using Domain.Bislerium.DTOs.Notification;
using Domain.Bislerium.DTOs.Reaction;
using Domain.Bislerium.DTOs.User;
using Domain.Bislerium.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Bislerium.Utils
{
    public static class Extension
    {
        public static User ToUser(this RegisterUser registerDto)
        {
            return new User
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                Password = registerDto.Password,
                Role = registerDto.Role,
            };
        }
        public static Blog ToBlog(this CreateBlog blogDto)
        {
            return new Blog
            {
                Title = blogDto.Title,
                Summary = blogDto.Summary,
                Body = blogDto.Body,
                Slug = blogDto.Slug,
                UserID = blogDto.UserID,
            };
        }
        public static Comment ToComment(this CreateComment commentDto)
        {
            return new Comment
            {
                Body = commentDto.Body,
                BlogID = commentDto.BlogID,
                UserID = commentDto.UserID,
            };
        }

        public static Reaction ToReaction(this CreateReaction reactionDto)
        {
            return new Reaction
            {
                BlogID = reactionDto.BlogID,
                UserID = reactionDto.UserID,
                Type = reactionDto.Type,
            };
        }

        public static Notification ToNotification(this CreateNotification notificationDto)
        {
            return new Notification
            {
                Body = notificationDto.Body,
                UserID = notificationDto.UserID,
            };
        }


    }
}