using GraduationProject.data;
using GraduationProject.Dto;
using GraduationProject.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly AppDBContext _context;

        public SubscriptionController(AppDBContext context)
        {
            _context = context; 
        }

        
        [HttpPost("Subscribe")]
        [Authorize(Policy = "StudentPolicy")] 
        public async Task<IActionResult> SubscribeToCourse([FromBody] SubscriptionDto subscriptionDto)
        { // Extract student ID from token
            var studentIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(studentIdClaim))
            {
                return Unauthorized(new { Message = "Invalid token" });
            }

            int studentId = int.Parse(studentIdClaim); 
            var student = await _context.users.FindAsync(studentId);
            if (student == null)
            {
                return NotFound(new { Message = "Student not found" });
            }

            var course = await _context.courses.FindAsync(subscriptionDto.CourseId);
            if (course == null)
            {
                return NotFound(new { Message = "Course not found" });
            }

            // Check if already subscribed
            var existingSubscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.StudentId == studentId && s.CourseId == subscriptionDto.CourseId);
            if (existingSubscription != null)
            {
                return BadRequest(new { Message = "Student is already subscribed to this course" });
            }

            var subscription = new Subscription
            {
                StudentId = studentId,
                CourseId = subscriptionDto.CourseId,
                SubscriptionDate = DateTime.UtcNow
            };

            _context.Subscriptions.Add(subscription);
            course.no_of_students += 1;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Subscription successful" });
        }

      
        [HttpGet("GetStudentSubscriptions/{studentId}")]
        public async Task<IActionResult> GetStudentSubscriptions(int studentId)
        {
            var subscriptions = await _context.Subscriptions
                .Where(s => s.StudentId == studentId)
                .Include(s => s.Course)
                .Select(s => new
                {
                    s.Course.Id,
                    s.Course.Name,
                    s.SubscriptionDate
                })
                .ToListAsync();

            return Ok(subscriptions);
        }

        // 🔹 Unsubscribe a student from a course
        [HttpDelete("Unsubscribe/{studentId}/{courseId}")]
        public async Task<IActionResult> Unsubscribe( int courseId)
        {
            // Extract student ID from token
            var studentIdClaim = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(studentIdClaim))
            {
                return Unauthorized(new { Message = "Invalid token" });
            }
            int studentId = int.Parse(studentIdClaim);
            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.StudentId == studentId && s.CourseId == courseId);

            if (subscription == null)
            {
                return NotFound(new { Message = "Subscription not found" });
            }

            _context.Subscriptions.Remove(subscription);

            var course = await _context.courses.FindAsync(courseId);
            if (course != null && course.no_of_students > 0)
            {
                course.no_of_students -= 1;
            }
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Unsubscribed successfully" });
        }
       
        [HttpGet("GetEnrollments")]
        [Authorize(Policy = "InstructorPolicy")]
        public async Task<IActionResult> GetEnrollments([FromQuery] int? latest)
        {
            var instructorId = int.Parse(User.FindFirst("Id")?.Value);

            var query = _context.Subscriptions
                .Include(s => s.Student)
                .Include(s => s.Course)
                .Where(s => s.Course.Instructor_Id == instructorId)
                .OrderByDescending(s => s.SubscriptionDate)
                .Select(s => new
                {
                    StudentName = s.Student.Name,
                    CourseTitle = s.Course.Name,
                    s.SubscriptionDate
                });

            // If 'latest' is provided, take only the specified number of enrollments
            if (latest.HasValue && latest > 0)
            {
                query = query.Take(latest.Value);
            }

            var enrollments = await query.ToListAsync();
            return Ok(enrollments);
        }


    }

}
