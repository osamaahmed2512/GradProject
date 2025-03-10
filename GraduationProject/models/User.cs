﻿namespace GraduationProject.models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string? ImageUrl { get; set; }
        public string? Introduction { get; set; } 
        public string? CVUrl { get; set; }
        public string? PreferredCategory { get; set; }
        public string? SkillLevel { get; set; }
        public bool IsApproved { get; set; } = false;
        public List<Course> Courses { get; set; }

        public ICollection<Rating> Rating { get; set; }
    }
}
