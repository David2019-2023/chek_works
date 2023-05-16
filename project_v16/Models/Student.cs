using project_v16.Enums;
using System.ComponentModel.DataAnnotations;

namespace project_v16.Models
{
    public class Student : User
    {
        public string? TopicThesis { get; set; }

        public int? TeacherId { get; set; }  
    }
}
