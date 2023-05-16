using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_v16.Enums;
using project_v16.Models;
using project_v16.Services.UserService;

namespace project_v16.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
        private ApplicationContext db;
        private readonly IUserService _userService;
        public StudentController(ApplicationContext context, IUserService userService)
        {
            _userService = userService;
            db = context;
        }

        [Authorize]
        [HttpPut("EditStudent")]
        public async Task<JsonResult> EditStudent(Student updatedStudent)
        {
            var currentUserRole = await _userService.GetCurrentUserRole();
            if (currentUserRole != Role.Administrator) 
            {
                throw new Exception("Нет прав доступа");
            }

            if (updatedStudent == null)
            {
                return new JsonResult(new { success = false, message = "User not found" });
            }

            db.Students.Update(updatedStudent);


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

        [Authorize]
        [HttpGet("GetStudents")]
        public async Task<JsonResult> GetStudents()
        {
            var currentUserRole = await _userService.GetCurrentUserRole();
            var currentUserId = await _userService.GetCurrentUserId();
            var students = new List<Student>();
            switch (currentUserRole)
            {
                case Role.Administrator:
                    students = await db.Students.ToListAsync();
                    break;
                case Role.Student:
                    students = await db.Students.Where(x => x.Id == currentUserId).ToListAsync();
                    break;
                case Role.Teacher:
                    students = await db.Students.Where(x => x.TeacherId == currentUserId).Select(x => x as Student).ToListAsync();
                    break;
            }


            return new JsonResult(new { students });
        }
    }
}
