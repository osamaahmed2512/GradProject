using System.Text.Json.Serialization;

namespace GraduationProject.Dto
{
    public class RecommendationResult
    {
        [JsonPropertyName("CourseId")]
        public int CourseId { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("Describtion")]
        public string Description { get; set; }

        [JsonPropertyName("Keywords")]
        public string Keywords { get; set; }

        [JsonPropertyName("CourseCategory")]
        public string CourseCategory { get; set; }

        [JsonPropertyName("LevelOfCourse")]
        public string LevelOfCourse { get; set; }

        [JsonPropertyName("Average Rating")]
        public double AverageRating { get; set; }
    }
}
