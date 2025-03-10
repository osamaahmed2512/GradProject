using GraduationProject.data;
using GraduationProject.models;
using GraduationProject.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationsController : ControllerBase
    {
        private readonly RecommendationService _recommendationService;
        private readonly AppDBContext _context;
       public RecommendationsController(RecommendationService recommendationService, AppDBContext context)
        {
            _recommendationService = recommendationService;
            _context= context;
        }

        [HttpPost]
        public async Task<IActionResult> GetRecommendations()
        {
            // Extract user ID from the token
            var userIdClaim = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid token: User ID not found.");
            }

            // Fetch user details from the database
            var user = await _context.users
                .Where(u => u.Id == userId)
                .Select(u => new User
                {
                    Id = u.Id,
                    PreferredCategory = u.PreferredCategory,
                    SkillLevel = u.SkillLevel
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Set TopN to 10
            const int topN = 10;

            // Get recommendations
            var recommendations = await _recommendationService.GetRecommendationsAsync(user, topN);
            if (recommendations == null)
            {
                return BadRequest("Failed to get recommendations.");
            }

            return Ok(recommendations);
        }
    }
}
