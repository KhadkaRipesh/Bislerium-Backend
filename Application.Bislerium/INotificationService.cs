using Domain.Bislerium.DTOs.Notification;
using Domain.Bislerium.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Bislerium
{
    public interface INotificationService
    {
        public Task<Notification> AddNewNotification(CreateNotification payload);

        public Task<IEnumerable<Notification>> GetAllNotifications();

        public Task<Notification> DeleteNotification(Guid id);
    }
}
