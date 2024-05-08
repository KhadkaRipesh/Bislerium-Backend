using Domain.Bislerium.DTOs.Comment;
using Domain.Bislerium.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Bislerium
{
    public interface ICommentService
    {
        public Task<Comment> AddNewComment(CreateComment payload);

        public Task<Comment> GetCommentById(Guid? id);

        public Task<IEnumerable<Comment>> GetCommentsBySlug(string slug);

        public Task<Comment> DeleteComment(Guid id);

        public Task<IEnumerable<Comment>> GetAllComments();
    }
}
