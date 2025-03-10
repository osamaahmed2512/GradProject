﻿namespace GraduationProject.Dto
{
    public class UpdateLesson
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? SectionId { get; set; }
        public List<string>? Tags { get; set; }
        public IFormFile? video { get; set; }
    }
}
