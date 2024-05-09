using Domain.Bislerium.DTOs.Notification;
using Domain.Bislerium.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Bislerium
{
    public interface IFirebaseService
    {
        public Task<FirebaseToken> CreateNewToken(CreateToken payload);
    }
}
