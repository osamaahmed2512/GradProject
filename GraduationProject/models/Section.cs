namespace GraduationProject.models
{
    public class Section
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public List<Lesson> Lessons { get; set; }
    }
}
