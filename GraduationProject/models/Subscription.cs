namespace GraduationProject.models
{
    public class Subscription
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public User Student { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public DateTime SubscriptionDate { get; set; } = DateTime.UtcNow;
    }
}
