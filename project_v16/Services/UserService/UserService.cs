using Microsoft.EntityFrameworkCore;
using project_v16.Enums;
using project_v16.Models;
using System.Security.Claims;

namespace project_v16.Services.UserService
{
    
    public class UserService : IUserService
    {

        private readonly ApplicationContext _context;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor, ApplicationContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context; 
        }

        public ContextUser GetContextUser()
        {
            var contextUser = new ContextUser();   
            contextUser.Email = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
            

            return contextUser;
        }

        public async Task<int> GetCurrentUserId()
        { 
            var contextUser = GetContextUser();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == contextUser.Email);
            return user != null ? user.Id : 0;
        }

        public async Task<Role?> GetCurrentUserRole()
        {
            var contextUser = GetContextUser();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == contextUser.Email);
            return user != null ? user.Role : null;
        }
    }
}
