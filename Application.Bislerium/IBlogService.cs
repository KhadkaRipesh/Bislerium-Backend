using Domain.Bislerium.DTOs.Blog;
using Domain.Bislerium.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Bislerium
{
    public interface IBlogService
    {
        public Task<Blog> AddNewBlog(CreateBlog payload);

        public Task<Blog> UpdateBlog(Guid id, UpdateBlog payload);

        public Task<Blog> DeleteBlog(Guid id);

        public Task<IEnumerable<Blog>> GetAllBlogs();

        public Task<Blog> GetBlogById(Guid id);

        public Task<Blog> GetBlogBySlug(string slug);

        public Task<IEnumerable<Blog>> GetMyBlogs();

        public Task<IEnumerable<Blog>> GetAllBlogsByBlogger(Guid id);

        public Task<IEnumerable<Blog>> GetAllBlogsByMonth(int month);

        public Task<IEnumerable<Blog>> GetAllBlogsByPopularityByMonth(int month);
    }
}
