namespace GraduationProject.Dto
{
    public class RegisterDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string? Introducton { get; set; }
        public string? PreferredCategory { get; set; }
        public string? SkillLevel { get; set; }
       
        public IFormFile CV { get; set; }
       
    }
}
