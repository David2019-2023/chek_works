using project_v16.Enums;

namespace project_v16.Services.UserService
{
    public interface IUserService
    {
        public ContextUser GetContextUser();
        public Task<int> GetCurrentUserId();
        public Task<Role?> GetCurrentUserRole();
    }
}
