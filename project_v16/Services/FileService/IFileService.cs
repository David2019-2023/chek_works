using project_v16.Models;

namespace project_v16.Services.FileService
{
    public interface IFileService
    {
        public Task<bool> AddFile(Models.File file, ApplicationContext context);
        public Task<IEnumerable<Models.File>> GetAll(ApplicationContext context);
        public Task<Models.File> GetFile(int Id, ApplicationContext context);
    }
}
