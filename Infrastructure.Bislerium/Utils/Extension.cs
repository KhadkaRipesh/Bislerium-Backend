using Domain.Bislerium.DTOs.User;
using Domain.Bislerium.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Bislerium.Utils
{
    public static class Extension
    {
        public static User ToUser(this RegisterUser registerDto)
        {
            return new User
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                Password = registerDto.Password,
                Role = registerDto.Role,
            };
        }

       
    }
}