using Application.Bislerium;
using Domain.Bislerium.DTOs.User;
using Domain.Bislerium.Enums;
using Domain.Bislerium.Exceptions;
using Domain.Bislerium.Models;
using Infrastructure.Bislerium.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Bislerium.Services
{
    public class UserService: IUserService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _environment;

        public UserService(ApplicationDBContext _dbContext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this._dbContext = _dbContext;
            this._configuration = configuration;
            this._httpContextAccessor = httpContextAccessor;
            _environment = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        }

        
        // To create user
        public async Task<User> Register(RegisterUser payload)
        {
            if (await EmailExists(payload.Email))
                throw new ProgramException("User with this email address already exists");

            payload.Password = BCrypt.Net.BCrypt.HashPassword(payload.Password);

            User user = payload.ToUser();

            await _dbContext.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            user = await UploadImage(user.ID, payload.Image);

            return user;
        }

        // To check if the email exists or not
        public async Task<bool> EmailExists(String email)
        {
            var contains = await _dbContext.Users.FirstOrDefaultAsync(user => user.Email == email);
            if (contains == null) return false;
            return true;
        }


        public async Task<User> UploadImage(Guid userId, IFormFile? image)
        {
            Console.WriteLine("reached here");
            User? user = await _dbContext.Users.Where(user => user.ID == userId).FirstOrDefaultAsync();
            if (user == null)
                throw new ProgramException("User not found");

            if (!IsImageValid(image))
            {
                throw new ProgramException("Image shouldn't exceed 3MB");
            }

            // Validate the document file
            if (image == null || image.Length == 0)
            {
                return user;
            }

            string userDirectory = Path.Combine(_environment, "User");
            if (!Directory.Exists(userDirectory))
            {
                Directory.CreateDirectory(userDirectory);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            string imagePath = Path.Combine(userDirectory, uniqueFileName);

            using (FileStream fileStream = System.IO.File.Create(imagePath))
            {
                await image.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
            }

            string relativeImagePath = Path.Combine("User", uniqueFileName);
            user.Image = relativeImagePath;
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }


        // Check Image validation
        public static bool IsImageValid(IFormFile image)
        {
            // Check the file format
            string[] allowedFormats = { ".jpg", ".png", ".jpeg" };
            string fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!allowedFormats.Contains(fileExtension))
            {
                return false;
            }

            // Check the file size
            long fileSizeLimit = 3_000_000; // 1.5 MB
            if (image.Length > fileSizeLimit)
            {
                return false;
            }

            return true;
        }

        // For user login
        public async Task<User> Login(LoginUser payload)
        {
            User? user = await AuthenticateUser(payload);

            if (user == null)
            {
                throw new ProgramException("Email and password didn't matched");
            }

            if (user.IsActive == false)
            {
                throw new ProgramException("User is not active. Please contact the admin.");
            }

            return user;
        }

        // Function for authenticating user login
        private async Task<User?> AuthenticateUser(LoginUser userLoginDto)
        {

            User? existingUser = await _dbContext.Users
                .Where(user => user.Email == userLoginDto.Email)
                .FirstOrDefaultAsync();

            if (existingUser == null) return null;

            if (existingUser.IsActive == false) return null;

            bool isPasswordMatched = BCrypt.Net.BCrypt.Verify(userLoginDto.Password, existingUser.Password);

            if (!isPasswordMatched) return null;


            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new (ClaimTypes.Email, userLoginDto.Email),
                    new (ClaimTypes.Role, existingUser.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha512Signature)
            };

            Console.WriteLine(tokenDescriptor.ToString());

            var token = tokenHandler.CreateToken(tokenDescriptor);

            User user = await _dbContext.Users.Where(user => user.Email == userLoginDto.Email).FirstAsync();
            user.AccessToken = tokenHandler.WriteToken(token);
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        // Change Password
        public async Task<User> ChangePassword(ChangePassword payload)
        {
            User? user = await GetCurrentUser();

            if (user == null) throw new ProgramException("Please logged in before changing password.");

            if (user.IsActive == false) throw new ProgramException("User is not active. Please contact the admin.");

            bool isPasswordMatched = BCrypt.Net.BCrypt.Verify(payload.CurrentPassword, user.Password);

            if (!isPasswordMatched)
            {
                throw new ProgramException("Incorrect current password.");
            }

            if (payload.NewPassword != payload.ConfirmPassword)
            {
                throw new ProgramException("New password and confirm password does not matched.");
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(payload.ConfirmPassword);
            user.Password = hashedPassword;
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        // Get current user
        public async Task<User?> GetCurrentUser()
        {
            var currentUser = _httpContextAccessor.HttpContext?.User;

            if (currentUser == null)
            {
                return null;
            }

            var userEmail = currentUser?.FindFirstValue(ClaimTypes.Email);

            User? user = await _dbContext.Users.Where(user => user.Email == userEmail).FirstOrDefaultAsync();

            return user;
        }


        // Add admin
        public async Task<User> AddAdmin(RegisterUser payload)
        {
            User? currentUser = await GetCurrentUser();

            if (currentUser == null)
                throw new ProgramException("Please login first!!!.");

            if (currentUser.Role != UserRole.ADMIN)
                throw new ProgramException("Only ADMIN can add admin.");

            payload.Role = UserRole.ADMIN;
            User user = await Register(payload);

            return user;
        }

        //Get all admin
        public async Task<IEnumerable<User>> GetAllAdmin()
        {
            User? currentUser = await GetCurrentUser();

            if (currentUser == null)
                throw new ProgramException("Please login first!!!.");

            if (currentUser.Role != UserRole.ADMIN)
                throw new ProgramException("Only ADMIN can view all admin.");

            IEnumerable<User> users = await _dbContext.Users.Where(user => user.Role == UserRole.ADMIN).ToListAsync();

            users = users.Where(user => user.ID != currentUser.ID).ToList();

            return users;
        }

        // Get all Blogger
        public async Task<IEnumerable<User>> GetAllBloggers()
        {
            User? currentUser = await GetCurrentUser();

            if (currentUser == null)
                throw new ProgramException("Please login first");

            if (currentUser.Role != UserRole.ADMIN)
                throw new ProgramException("Only ADMIN can view ALL BLOGGERS.");

            IEnumerable<User> users = await _dbContext.Users.Where(user => user.Role == UserRole.BLOGGER).ToListAsync();

            return users;
        }

        // Funtion to calculate blogger popularity score
        public double CalculateBloggerPopularityScore(User blogger, DateTime startDate, DateTime endDate)
        {
            double score = 0;

            // Get all blogs authored by the blogger within the specified month
            var bloggerBlogs = _dbContext.Blogs
                .Where(b => b.UserID == blogger.ID && b.CreatedAt >= startDate && b.CreatedAt <= endDate)
                .ToList();

            // Calculate total comments, likes, and dislikes for all blogs authored by the blogger within the specified month
            var totalComments = bloggerBlogs.Sum(blog => _dbContext.Comments.Where(c => c.BlogID == blog.ID).Count());
            var totalLikes = bloggerBlogs.Sum(blog => _dbContext.Reactions.Where(r => r.BlogID == blog.ID && r.Type == ReactionType.LIKE).Count());
            var totalDislikes = bloggerBlogs.Sum(blog => _dbContext.Reactions.Where(r => r.BlogID == blog.ID && r.Type == ReactionType.DISLIKE).Count());

            // Calculate the score based on the total engagement metrics
            score = (totalComments * 1) + (totalLikes * 2) + (totalDislikes * (-1));

            return score;
        }

        // Get all bloggers by popularity
        public async Task<IEnumerable<User>> GetAllBloggersByPopularity(int month)
        {
            var endDate = new DateTime();
            var startDate = new DateTime();

            if (month == 0)
            {
                startDate = new DateTime(DateTime.Now.Year, 1, 1);
                endDate = new DateTime(DateTime.Now.Year, 12, 30);
            }

            else
            {
                startDate = new DateTime(DateTime.Now.Year, month, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
            }

            // Retrieve all bloggers
            var users = await _dbContext.Users.Where(user => user.Role == UserRole.BLOGGER).ToListAsync();

            // Calculate popularity scores for bloggers within the specified month
            var bloggerPopularityScores = users.Select(blogger => new
            {
                Blogger = blogger,
                PopularityScore = CalculateBloggerPopularityScore(blogger, startDate, endDate)
            });

            // Order bloggers by their popularity score and take the top 10
            var topBloggers = bloggerPopularityScores.OrderByDescending(entry => entry.PopularityScore).Take(10).Select(entry => entry.Blogger);

            return topBloggers;
        }

        // Update Profile
        public async Task<User> UpdateProfile(UpdateProfile payload)
        {
            User currentUser = await GetCurrentUser() ?? throw new ProgramException("Login required");

            // Update UserName if provided
            if (!string.IsNullOrEmpty(payload.UserName))
            {
                currentUser.UserName = payload.UserName;
            }
            if (payload.Image != null)
            {
                if (!IsImageValid(payload.Image))
                {
                    throw new ProgramException("Profile Image Must Be In The Format Of Eihter .png, .jpg, or .jpeg");
                }

                // Create directory if it doesn't exist
                string userDirectory = Path.Combine(_environment, "User");
                if (!Directory.Exists(userDirectory))
                {
                    Directory.CreateDirectory(userDirectory);
                }

                // Generate unique file name
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(payload.Image.FileName);
                string imagePath = Path.Combine(userDirectory, uniqueFileName);

                // Save image to disk
                using (FileStream fileStream = System.IO.File.Create(imagePath))
                {
                    await payload.Image.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }

                // Update user's image path
                currentUser.Image = "/User/" + uniqueFileName;
            }

            // Save changes to the database
            _dbContext.Users.Update(currentUser);
            await _dbContext.SaveChangesAsync();

            return currentUser;
        }


        // Delete User
        public async Task<bool> DeleteUser()
        {
            User? currentUser = await GetCurrentUser() ??
                throw new ProgramException("Login Required");

            if (currentUser == null) throw new ProgramException("Please logged in before deleting account");


            User? userToDelete = await _dbContext.Users.FindAsync(currentUser.ID) ??
                throw new ProgramException("The user does not exists to be deleted.");

            userToDelete.IsActive = false;
            userToDelete.IsDeleted = true;

            // Update user in the database
            _dbContext.Users.Update(userToDelete);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Logout user
        public async Task<bool> Logout()
        {
            User? user = await GetCurrentUser();

            if (user == null) return false;

            user.AccessToken = "";
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        // Get User By Id
        public async Task<User> GetUserById(Guid id)
        {
            User? user = await _dbContext.Users.FindAsync(id);

            if (user == null)
                throw new ProgramException("User not found.");

            return user;
        }

        // Get Mertics for dashboard
        public async Task<object> GetStats(int month)
        {
            var stats = new Dictionary<string, object>
            {
                { "totalBlogs", _dbContext.Blogs.Count() },
                { "totalComments", _dbContext.Comments.Count() },
                { "totalLikes", _dbContext.Reactions.Where(reaction => reaction.Type == ReactionType.LIKE).Count() },
                { "totalDislikes", _dbContext.Reactions.Where(reaction => reaction.Type == ReactionType.DISLIKE).Count() },
                { "totalReactions", _dbContext.Reactions.Count() },
                { "totalUsers", _dbContext.Users.Count() },
                { "totalAdmins", _dbContext.Users.Where(user => user.Role == UserRole.ADMIN).Count() },
                { "totalBloggers", _dbContext.Users.Where(user => user.Role == UserRole.BLOGGER).Count() },
                { "totalActiveUsers", _dbContext.Users.Where(user => user.IsActive == true).Count() },
                { "totalInactiveUsers", _dbContext.Users.Where(user => user.IsActive == false).Count() },
             };

            if (month != 0)
            {
                stats["totalBlogs"] = _dbContext.Blogs.Where(blog => blog.CreatedAt.Month == month).Count();
                stats["totalComments"] = _dbContext.Comments.Where(comment => comment.CreatedAt.Month == month).Count();
                stats["totalLikes"] = _dbContext.Reactions.Where(reaction => reaction.Type == ReactionType.LIKE && reaction.CreatedAt.Month == month).Count();
                stats["totalDislikes"] = _dbContext.Reactions.Where(reaction => reaction.Type == ReactionType.DISLIKE && reaction.CreatedAt.Month == month).Count();
            }

            return await Task.FromResult<object>(stats);

        }

    }
}
