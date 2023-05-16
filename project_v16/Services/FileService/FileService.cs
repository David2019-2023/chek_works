using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using project_v16.Models;

namespace project_v16.Services.FileService
{
    public class FileService : IFileService
    {
        
        
        public async Task<IEnumerable<Models.File>> GetAll(ApplicationContext context)
        {
            return await (from c in context.Files
                          select c).ToListAsync();
        }
        public async Task<Models.File> GetFile(int Id, ApplicationContext context)
        {
            return await (from c in context.Files
                          where c.Id == Id
                          select c).FirstOrDefaultAsync();
        }
        public async Task<bool> AddFile(Models.File file, ApplicationContext context)
        {
            try
            {
                var oldFile = await context.Files.Where(x => x.FileName == file.FileName && x.StudentId == file.StudentId).FirstOrDefaultAsync();
                if (oldFile != null && oldFile.Id > 0) 
                { 
                    oldFile.Content = file.Content;
                    oldFile.MimeType = file.MimeType;   
                    context.Files.Update(oldFile);
                    await context.SaveChangesAsync();
                    return true;
                }
                context.Files.Add(file);
                await context.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
