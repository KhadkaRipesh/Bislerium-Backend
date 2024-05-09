using Application.Bislerium;
using Domain.Bislerium.DTOs.User;
using Domain.Bislerium.Exceptions;
using Domain.Bislerium.Models;
using Infrastructure.Bislerium.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace Presentation.Bislerium.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService userService;

        public UserController(IUserService userServices)
        {
            this.userService = userServices;
        }

        // Register User
        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromForm] RegisterUser payload)
        {
            try
            {
                User user = await userService.Register(payload);
                return Ok(user);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        // Login User
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<User>> Login([FromBody] LoginUser payload)
        {
            try
            {
                User user = await userService.Login(payload);
                return Ok(user);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Change password
        [Authorize]
        [HttpPost]
        [Route("change-password")]
        public async Task<ActionResult<User>> ChangePassword([FromBody] ChangePassword payload)
        {
            try
            {
                User user = await userService.ChangePassword(payload);
                return user;
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Add Admin
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [Route("add/admin")]
        public async Task<ActionResult<User>> AddAdmin([FromForm] RegisterUser payload)
        {
            try
            {
                return await userService.AddAdmin(payload);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // get All Admin
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        [Route("get/allAdmin")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllAdmin()
        {
            try
            {
                IEnumerable<User> users = await userService.GetAllAdmin();
                return Ok(users);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get All Blogger
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        [Route("get/allBloggers")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllBloggers()
        {
            try
            {
                IEnumerable<User> users = await userService.GetAllBloggers();
                return Ok(users);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get Blogger by popularity
        [Authorize(Roles = "ADMIN")]
        [HttpGet("get/all-bloggers-by-popularity/{month:int}")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllBloggersByPopularity(int month)
        {
            try
            {
                IEnumerable<User> users = await userService.GetAllBloggersByPopularity(month);
                return Ok(users);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Update Profile
        [Authorize]
        [HttpPut]
        [Route("update/profile")]
        public async Task<ActionResult<User>> UpdateProfile([FromForm] UpdateProfile payload)
        {
            try
            {
                User user = await userService.UpdateProfile(payload);
                return Ok(user);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Delete User
        [Authorize]
        [HttpDelete]
        [Route("delete")]
        public async Task<ActionResult> DeleteUser()
        {
            try
            {
                await userService.DeleteUser();
                return Ok("User Has Been Deleted Successfully");
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Logout User
        [Authorize]
        [HttpGet]
        [Route("logout")]
        public async Task<ActionResult> Logout()
        {
            try
            {
                await userService.Logout();
                return Ok("Successfully logout");
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // get current user
        [Authorize]
        [HttpGet]
        [Route("get-current-user")]
        public async Task<ActionResult<User>> GetCurrentUser()
        {
            try
            {
                User? user = await userService.GetCurrentUser();
                return Ok(user);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get user by id
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        [Route("get/{id}")]
        public async Task<ActionResult<User>> GetUserById(Guid id)
        {
            try
            {
                User user = await userService.GetUserById(id);
                return Ok(user);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get stats for dashbiard
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        [Route("get-stats/{month:int}")]
        public async Task<object> GetStats(int month)
        {
            try
            {
                var stats = await userService.GetStats(month);
                return Ok(stats);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
