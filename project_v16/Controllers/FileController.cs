using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_v16.Enums;
using project_v16.Models;
using project_v16.Services.FileService;
using project_v16.Services.UserService;
using project_v16.ViewModels;

namespace project_v16.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IUserService _userService;
        private readonly ApplicationContext _context;
        public FileController(IFileService fileService, IUserService userService, ApplicationContext context)
        {
            _fileService = fileService;
            _userService = userService; 
            _context = context;
        }


        [Authorize]
        [HttpPost("UploadFile")]
        public async Task<JsonResult> UploadFile(IFormFile file)
        {
            var currentUserRole = await _userService.GetCurrentUserRole();
            if (currentUserRole != Role.Student)
            {
                return new JsonResult(new { succes = false });
            }

            if (file.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    var newFile = new Models.File();
                    newFile.MimeType = file.ContentType;
                    newFile.FileName = Path.GetFileName(file.FileName);
                    newFile.StudentId = await _userService.GetCurrentUserId();
                    newFile.Content = ms.ToArray();
                    var result = await _fileService.AddFile(newFile, _context);
                    return new JsonResult(new {succes = result});
                }
            }
            return new JsonResult(new { succes = false });
        }

        [Authorize]
        [HttpGet("DownloadFile")]
        public async Task<FileResult> DownloadFile(int fileId)
        {
            var studentId = await _context.Files.Where(x => x.Id == fileId).Select(x => x.StudentId).FirstOrDefaultAsync();
            var currentUserId = await _userService.GetCurrentUserId();
            var userHaveAccess = await _context.Students.Where(x => x.TeacherId == currentUserId && x.Id == studentId).AnyAsync();
            if (currentUserId != studentId && !userHaveAccess)
            {
                throw new Exception("Нет прав доступа");
            }
            var file = await _fileService.GetFile(fileId, _context);
            return File(file.Content, file.MimeType);
        }

        [Authorize]
        [HttpPost("AddFileComment")]
        public async Task<JsonResult> AddFileComment(AddFileModel commentModel)
        {
            var currentUserRole = await _userService.GetCurrentUserRole();
            if (currentUserRole != Enums.Role.Teacher)
            {
                return new JsonResult(new { succes = false });
            }

            var fileComment = new FileComment
            {
                Comment = commentModel.Comment,
                FileId = commentModel.FileId             
            };
            _context.FileComments.Add(fileComment);
            await _context.SaveChangesAsync();
            return new JsonResult(new { succes = true });
        }

        [Authorize]
        [HttpGet("GetStudetFiles")]
        public async Task<JsonResult> GetStudetFiles(int studentId)
        {
            var currentUserId = await _userService.GetCurrentUserId();
            var userHaveAccess = await _context.Students.Where(x => x.TeacherId == currentUserId && x.Id == studentId).AnyAsync();
            if (currentUserId != studentId && !userHaveAccess)
            {
                throw new Exception("Нет прав доступа");
            }
            var files = await _context.Files.Where(x => x.StudentId == studentId).Select(x => new FileModel { Id = x.Id, FileName = x.FileName}).ToListAsync();
            return new JsonResult(new { files });
        }

        [Authorize]
        [HttpGet("GetFileComments")]
        public async Task<JsonResult> GetFileComments(int fileId)
        {
            var studentId = await _context.Files.Where(x => x.Id == fileId).Select(x => x.StudentId).FirstOrDefaultAsync();
            var currentUserId = await _userService.GetCurrentUserId();
            var userHaveAccess = await _context.Students.Where(x => x.TeacherId == currentUserId && x.Id == studentId).AnyAsync();
            if (currentUserId != studentId && !userHaveAccess)
            {
                throw new Exception("Нет прав доступа");
            }
            var fileComments = await _context.FileComments.Where(x => x.FileId == fileId).ToListAsync();
            return new JsonResult(new { fileComments });
        }
    }
}
