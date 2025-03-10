using GraduationProject.models;
using System.Text;
using System.Text.Json;
namespace GraduationProject.Services
{
    public class RecommendationService
    {
        private readonly HttpClient _httpClient;

        public RecommendationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<string> GetRecommendationsAsync(User user, int topN = 10)
        {
            var request = new
            {
                user_id = user.Id,
                preferred_category = user.PreferredCategory,
                skill_level = user.SkillLevel,
                top_n = topN
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://localhost:8000/recommend", content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return null;
        }
    }
}
