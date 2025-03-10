namespace GraduationProject.models
{
    public class RecommendationRequest
    {
        public string preferred_category { get; set; }
        public string SkillLevel { get; set; }
        public int TopN { get; set; } = 10;
    }
}
