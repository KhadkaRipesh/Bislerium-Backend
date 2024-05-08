using Application.Bislerium;
using Domain.Bislerium.DTOs.Reaction;
using Domain.Bislerium.Exceptions;
using Domain.Bislerium.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Bislerium.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReactionController : Controller
    {
        private readonly IReactionService reactionService;


        public ReactionController(IReactionService reactionService)
        {
            this.reactionService = reactionService;
        }

        // Add reaction
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Reaction>> AddNewReaction([FromForm] CreateReaction payload)
        {
            try
            {
                Reaction reaction = await reactionService.AddNewReaction(payload);
                return Ok(reaction);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // Get All reaction
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reaction>>> GetAllReactions()
        {
            try
            {
                IEnumerable<Reaction> reaction = await reactionService.GetAllReactions();
                return Ok(reaction);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
