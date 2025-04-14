using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Dto
{
    public class RegisterDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
        public string? Introducton { get; set; }
        public string? PreferredCategory { get; set; }
        public string? SkillLevel { get; set; }
        public string? BIO { get; set; }
        public IFormFile? CV { get; set; }
       
    }
}
