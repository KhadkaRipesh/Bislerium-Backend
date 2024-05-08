using Application.Bislerium;
using Domain.Bislerium.DTOs.Comment;
using Domain.Bislerium.Exceptions;
using Domain.Bislerium.Models;
using Infrastructure.Bislerium.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Bislerium.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            this._commentService = commentService;
        }

        // Add new comment

        [HttpPost]
        public async Task<ActionResult<Comment>> AddNewComment([FromForm] CreateComment payload)
        {
            try
            {
                Comment comment = await _commentService.AddNewComment(payload);
                return Ok(comment);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //  Get All Comment
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetAllComments()
        {
            try
            {
                IEnumerable<Comment> comments = await _commentService.GetAllComments();
                return Ok(comments);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get Comment by slug
        [AllowAnonymous]
        [HttpGet("get-comment-by-blog/{slug}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsBySlug([FromRoute] string slug)
        {
            try
            {
                IEnumerable<Comment> comments = await _commentService.GetCommentsBySlug(slug);
                return Ok(comments);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get Comment By Id
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetCommentById(Guid id)
        {
            try
            {
                Comment comment = await _commentService.GetCommentById(id);
                return Ok(comment);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Delete comment
        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Comment>> DeleteComment([FromRoute] Guid id)
        {
            try
            {
                Comment comment = await _commentService.DeleteComment(id);
                return Ok(comment);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);

            }

        }
    }
}
