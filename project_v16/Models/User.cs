using project_v16.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_v16.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите email пользователя")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Введите пароль пользователя")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Укажите имя пользователя")]
        public string Name { get; set; } // имя пользователя

        [Required(ErrorMessage = "Укажите имя пользователя")]
        public Role Role { get; set; }       
    }
}
