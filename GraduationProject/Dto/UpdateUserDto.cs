namespace GraduationProject.Dto
{
    public class UpdateUserDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ImageUrl { get; set; }
        public string? Introduction { get; set; }
        public string? CVUrl { get; set; }
        public string? Role { get; set; }
        public bool? IsApproved { get; set; }
    }
}
