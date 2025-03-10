namespace GraduationProject.models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation property for the many-to-many relationship
        public List<CourseTag> CourseTags { get; set; }
        //public List<LessonTag> lessonTags { get; set; }
    }
}
