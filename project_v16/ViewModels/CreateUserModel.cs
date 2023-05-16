using project_v16.Enums;
using System.ComponentModel.DataAnnotations;

namespace project_v16.ViewModels
{
    public class CreateUserModel
    {  
        public string Email { get; set; }    
        public string Password { get; set; }
        public string Name { get; set; } // имя пользователя
        public Role Role { get; set; }
    }
}
