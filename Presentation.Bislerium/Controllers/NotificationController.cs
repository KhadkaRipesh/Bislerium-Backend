using Application.Bislerium;
using Domain.Bislerium.DTOs.Notification;
using Domain.Bislerium.Exceptions;
using Domain.Bislerium.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Bislerium.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : Controller
    {
        private readonly INotificationService notificationService;

        public NotificationController(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Notification>> AddNewNotification(CreateNotification payload)
        {
            try
            {
                Notification notification = await notificationService.AddNewNotification(payload);
                return Ok(notification);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get All Notification
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetAllNotifications()
        {
            try
            {
                IEnumerable<Notification> notification = await notificationService.GetAllNotifications();
                return Ok(notification);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Delete Notification
        [Authorize]
        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult<Notification>> DeleteNotification([FromRoute] Guid id)
        {
            try
            {
                Notification notification = await notificationService.DeleteNotification(id);
                return Ok(notification);
            }
            catch (ProgramException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
