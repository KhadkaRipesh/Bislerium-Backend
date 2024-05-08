using Application.Bislerium;
using Domain.Bislerium.DTOs.Blog;
using Domain.Bislerium.Exceptions;
using Domain.Bislerium.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace Presentation.Bislerium.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : Controller
    {
        private readonly IBlogService blogService;

        public BlogController(IBlogService blogService)
        {
            this.blogService = blogService;
        }

        // Add new blog
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Blog>> AddNewBlog([FromForm] CreateBlog blogDto)
        {
            try
            {
                Blog blog = await blogService.AddNewBlog(blogDto);
                return Ok(blog);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Update blog
        [Authorize]
        [HttpPut("{id:Guid}")]
        public async Task<ActionResult<Blog>> UpdateBlog([FromRoute] Guid id, [FromForm] UpdateBlog updateBlogDto)
        {
            try
            {
                Blog blog = await blogService.UpdateBlog(id, updateBlogDto);
                return Ok(blog);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Delete Blog
        [Authorize]
        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult<Blog>> DeleteBlog([FromRoute] Guid id)
        {
            try
            {
                Blog blog = await blogService.DeleteBlog(id);
                return Ok(blog);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get all blog
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Blog>>> GetAllBlogs()
        {
            IEnumerable<Blog> blogs = await blogService.GetAllBlogs();
            return Ok(blogs);
        }

        // Get all blogs 
        [HttpGet("get-blogs-by-user/{id:Guid}")]
        public async Task<ActionResult<IEnumerable<Blog>>> GetAllBlogsByBlogger([FromRoute] Guid id)
        {
            IEnumerable<Blog> blogs = await blogService.GetAllBlogsByBlogger(id);
            return Ok(blogs);
        }

        // Get Blogs by month
        [Authorize(Roles = "ADMIN")]
        [HttpGet("get-blogs-by-month/{month:int}")]
        public async Task<ActionResult<IEnumerable<Blog>>> GetAllBlogsByMonth([FromRoute] int month)
        {
            try
            {
                IEnumerable<Blog> blogs = await blogService.GetAllBlogsByMonth(month);
                return Ok(blogs);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get all blog by popularity
        [HttpGet("get-all-blogs-by-popularity/{month:int}")]
        public async Task<ActionResult<IEnumerable<Blog>>> GetAllBlogsByPopularityByMonth([FromRoute] int month)
        {
            IEnumerable<Blog> blogs = await blogService.GetAllBlogsByPopularityByMonth(month);
            return Ok(blogs);
        }


        // Get Blog by Id
        [AllowAnonymous]
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<Blog>> GetBlogById([FromRoute] Guid id)
        {
            try
            {
                Blog blog = await blogService.GetBlogById(id);
                return Ok(blog);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Get blogs by slug
        [AllowAnonymous]
        [HttpGet("{slug}")]
        public async Task<ActionResult<Blog>> GetBlogBySlug([FromRoute] string slug)
        {
            try
            {
                Blog blog = await blogService.GetBlogBySlug(slug);
                return Ok(blog);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get My Blogs
        [Authorize]
        [HttpGet("get-my-blogs")]
        public async Task<ActionResult<IEnumerable<Blog>>> GetMyBlogs()
        {
            IEnumerable<Blog> blogs = await blogService.GetMyBlogs();
            return Ok(blogs);
        }
    }
}
