using Application.Bislerium;
using Domain.Bislerium.DTOs.Notification;
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
    public class FirebaseController : Controller
    {
        private readonly IFirebaseService firebaseService;

        public FirebaseController(IFirebaseService firebaseService)
        {
            this.firebaseService = firebaseService;
        }

        // Save firebase Token
        [Authorize]
        [HttpPost]
        [Route("save-token")]
        public async Task<ActionResult<FirebaseToken>> CreateFirebaseToken(CreateToken payload)
        {
            try
            {
                FirebaseToken token = await firebaseService.CreateNewToken(payload);
                return Ok(token);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
