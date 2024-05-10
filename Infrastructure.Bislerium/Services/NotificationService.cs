using Application.Bislerium;
using Domain.Bislerium.DTOs.Notification;
using Domain.Bislerium.Exceptions;
using Domain.Bislerium.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Bislerium.Services
{
    public class NotificationService: INotificationService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IUserService userServices;

        public NotificationService(ApplicationDBContext dbContext, IUserService userServices)
        {
            this._dbContext = dbContext;
            this.userServices = userServices;
        }


        // Create Notification
        public async Task<Notification> AddNewNotification(CreateNotification payload)
        {
            User? user = await userServices.GetCurrentUser();

            if (user == null)
                throw new ProgramException("Please Login Before Adding Notification.");

            Notification notification = new Notification
            {
                Body = payload.Body,
                UserID = payload.UserID
            };

            await _dbContext.Notifications.AddAsync(notification);
            await _dbContext.SaveChangesAsync();

            return notification;
        }

        // Get my All Notification
        public async Task<IEnumerable<Notification>> GetAllNotifications()
        {
            User? user = await userServices.GetCurrentUser();
            if (user == null)
                throw new ProgramException("Please Login Before Adding Notification.");


            IEnumerable<Notification> notifications = await _dbContext.Notifications.Where(c => c.UserID == user.ID && c.IsActive && !c.IsDeleted).ToListAsync();
            return notifications;
        }


        // Delete Notification by Id
        public async Task<Notification> DeleteNotification(Guid id)
        {
            User? user = await userServices.GetCurrentUser();
            if (user == null)
                throw new ProgramException("Please Login Before Deleting Notification.");

            Notification? notification = await _dbContext.Notifications.FirstOrDefaultAsync(n => n.ID == id);

            if (notification == null)
                throw new ProgramException("Notification Not Found.");


            notification.IsDeleted = true;
            notification.IsActive = false;

            _dbContext.Notifications.Update(notification);
            await _dbContext.SaveChangesAsync();

            return notification;
        }
    }
}
