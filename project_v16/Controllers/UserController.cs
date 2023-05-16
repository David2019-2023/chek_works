using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using project_v16.Enums;
using project_v16.Models;
using project_v16.Services.UserService;
using project_v16.ViewModels;

namespace project_v16.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private ApplicationContext db;

        private readonly IUserService _userService;
        public UserController(ApplicationContext context, IUserService userService)
        {
            _userService = userService; 
            db = context;
        }

        [Authorize]
        [HttpPost("CreateUser")]
        public async Task<JsonResult> CreateUser(CreateUserModel userModel)
        {
            var currentUserRole = await _userService.GetCurrentUserRole();
            if (currentUserRole != Role.Administrator)
            {
                throw new Exception("Нет прав доступа");
            }
            if (await db.Users.AnyAsync(u => u.Email == userModel.Email))
            {
                //ModelState.AddModelError(nameof(user.Email), "Пользователь с таким email уже существует");
                return new JsonResult(new { success = false });  //errors = ModelState
            }
            var somth = await _userService.GetCurrentUserId();
            switch (userModel.Role)
            {
                case Role.Student:
                    var student = new Student
                    {
                        Email = userModel.Email,
                        Name = userModel.Name,
                        Role = userModel.Role,
                        Password = userModel.Password
                    };
                    db.Students.Add(student);
                    break;
                default:
                    var user = new User
                    {
                        Email = userModel.Email,
                        Name = userModel.Name,
                        Role = userModel.Role,
                        Password = userModel.Password
                    };
                    db.Users.Add(user);
                    break;
            }           
            await db.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }

        [Authorize]
        [HttpGet("GetUsers")]
        public async Task<JsonResult> GetUsers()
        {
            var currentUserRole = await _userService.GetCurrentUserRole();
            var currentUserId = await _userService.GetCurrentUserId();
            var users = new List<User>();
            switch (currentUserRole)
            {
                case Role.Administrator:
                    users = await db.Users.ToListAsync();
                    break;
                case Role.Student:
                    users = await db.Users.Where(x => x.Id == currentUserId).ToListAsync();
                    break;
                case Role.Teacher:
                    users = await db.Students.Where(x => x.TeacherId == currentUserId).Select(x => x as User).ToListAsync();
                    break;
            }

            
            return new JsonResult(new { users });
        }


        [Authorize]
        [HttpDelete("DeleteUser")]
        public async Task<JsonResult> DeleteUser(string username)
        {
            var currentUserRole = await _userService.GetCurrentUserRole();
            if (currentUserRole != Role.Administrator)
            {
                throw new Exception("Нет прав доступа");
            }
            var user = await db.Users.FirstOrDefaultAsync(u => u.Name == username);

            if (user != null)
            {
                db.Users.Remove(user);
                await db.SaveChangesAsync();
                return new JsonResult(new { success = true });
            }
            else
            {
                return new JsonResult(new { success = false, message = "User not found" });
            }
        }

        [Authorize]
        [HttpPut("EditUser")]
        public async Task<JsonResult> EditUser(User updatedUser)
        {
            var currentUserRole = await _userService.GetCurrentUserRole();
            if (currentUserRole != Role.Administrator)
            {
                throw new Exception("Нет прав доступа");
            }

            if (updatedUser == null)
            {
                return new JsonResult(new { success = false, message = "User not found" });
            }

            db.Users.Update(updatedUser);
            
            
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return new JsonResult(new { success = false, message = "Unable to save changes. The user was deleted by another user." });
            }

            return new JsonResult(new { success = true });
        }

       

    }

}
