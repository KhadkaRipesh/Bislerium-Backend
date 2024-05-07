using Domain.Bislerium.DTOs.User;
using Domain.Bislerium.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Bislerium
{
    public interface IUserService
    {
        public Task<User> Register(RegisterUser payload);
        public Task<User> Login(LoginUser payload);
        public Task<User> ChangePassword(ChangePassword payload);
        public Task<User> AddAdmin(RegisterUser payload);
        public Task<User> UpdateProfile(UpdateProfile payload);
        public Task<bool> DeleteUser();
        public Task<bool> Logout();
        public Task<User?> GetCurrentUser();
        public Task<IEnumerable<User>> GetAllAdmin();
        public Task<IEnumerable<User>> GetAllBloggers();
        public Task<IEnumerable<User>> GetAllBloggersByPopularity(int month);
        public Task<User> GetUserById(Guid id);
        public Task<object> GetStats(int month);
    }
}
