﻿using GraduationProject.data;
using GraduationProject.Dto;
using GraduationProject.Migrations;
using GraduationProject.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorController : ControllerBase
    {

        private readonly AppDBContext _context;

        public InstructorController(AppDBContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("GetAllCourses")]
        
        public async Task<IActionResult> GetAllCourses()
        {
            // Retrieve all courses with their associated tags
            var courses = await _context.courses
                .Include(c => c.CourseTags) // Include the CourseTags
                .ThenInclude(ct => ct.Tag)
                .Include(x =>x.Instructor)// Include the Tag within CourseTags
                .Select(c => new
                { 
                    c.Id,
                    c.Name,
                    c.Describtion,
                    c.No_of_hours,
                    c.Instructor_Id,
                    InstructorName= c.Instructor.Name,
                    
                    c.no_of_students,
                    c.CreationDate,
                    c.LevelOfCourse,
                    c.ImgUrl,
                    c.AverageRating,
                    c.Price,
                    c.Discount,
                    c.DiscountedPrice,
                    Tags = c.CourseTags.Select(ct => ct.Tag.Name).ToList() // Extract tag names

                })
                .ToListAsync();

            return Ok(courses);
        }

        [HttpGet("GetInstructorCourses")]
        [Authorize(Policy = "InstructorPolicy")]
        public async Task<IActionResult> GetInstructorCourses()
        {
            try
            {
                var instructorId = int.Parse(User.FindFirst("Id")?.Value);

                var courses = await _context.courses
                    .Where(c => c.Instructor_Id == instructorId)
                    .Select(c => new
                    {
                         c.Id,
                         c.Name,
                         c.ImgUrl,
                         NoOfStudents = c.no_of_students,
                         c.CreationDate
                    })
                    .ToListAsync();

                return Ok(courses);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error fetching courses", error = ex.Message });
            }
        }

        [HttpGet("GetAllCourseOfInstructor")]
        public async Task<IActionResult> GetAllCourseOfInstructor(int instructorId)
        {
            try
            {
                // Get the instructor ID from the authenticated user's claims
                //var instructorId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
                

                // Retrieve all courses for the specified instructor, including their associated tags
                var courses = await _context.courses
                    .Where(c => c.Instructor_Id == instructorId) // Filter by instructor ID
                    .Include(c => c.CourseTags) // Include the CourseTags
                    .ThenInclude(ct => ct.Tag)  // Include the Tag within CourseTags
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Describtion,
                        c.No_of_hours,
                        c.Instructor_Id,
                        c.no_of_students,
                        c.CreationDate,
                        c.LevelOfCourse,
                        c.ImgUrl,
                        Tags = c.CourseTags.Select(ct => ct.Tag.Name).ToList() // Extract tag names
                    })
                    .ToListAsync();

                // If no courses are found, return a 404 Not Found response
                if (courses == null || !courses.Any())
                {
                    return NotFound($"No courses found for instructor with ID {instructorId}.");
                }

                // Return the list of courses with a 200 OK response
                return Ok(courses);
            }
            catch (Exception ex)
            {
                // Log the exception and return a 500 Internal Server Error response
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
       
        [HttpGet("GetInstructorCourseCount")]
        [Authorize(Policy = "InstructorPolicy")]
        public async Task<IActionResult> GetInstructorCourseCount()
        {
            // Get Instructor ID from the JWT token
            var instructorId = int.Parse(User.FindFirst("Id")?.Value);

            // Count courses where Instructor_Id matches
            var totalCourses = await _context.courses
                .Where(c => c.Instructor_Id == instructorId)
                .CountAsync();

            return Ok(new { TotalCourses = totalCourses });
        }

        [HttpPost]
        [Route("AddCourse")]
        [Authorize(Policy = "InstructorAndAdminPolicy")] 
        [ServiceFilter(typeof(CustomModelStateFilter))]
        public async Task<IActionResult> AddCourse([FromForm] CourseDto courseDto)
        {
           

            // Generate a unique file name
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(courseDto.Image.FileName)}";

            // Define the folder to save the image
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "courseImages");
            // Ensure the directory exists
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);
            // Save the image to the server
            var filePath = Path.Combine(uploadPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await courseDto.Image.CopyToAsync(stream);
            }

            var instructorId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
            var userRole = User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;

            if (userRole != "teacher")
                return Unauthorized(new {StatusCode= StatusCodes.Status401Unauthorized, Message = "Only instructors can add courses" });

            // Create a new Course object
            var course = new GraduationProject.models.Course
            {
                Name = courseDto.Name,
                Describtion = courseDto.Describtion,
                CourseCategory= courseDto.CourseCategory.ToLower(),
                Instructor_Id = instructorId,
                ImgUrl = $"/courseImages/{fileName}",
                CourseTags = new List<CourseTag>(),
                LevelOfCourse = courseDto.LevelOfCourse.ToLower(),
                Price= courseDto.Price,
                Discount= courseDto.Discount ??0,
                Sections = new List<Section>()
            };

            if (courseDto.Tag != null && courseDto.Tag.Any())
            {
                foreach (var tagName in courseDto.Tag)
                {
                    // Check if the tag already exists
                    var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                    if (tag == null)
                    {
                        // If not, create a new tag
                        tag = new Tag { Name = tagName };
                        _context.Tags.Add(tag);
                    }

                    // Add the tag to the course
                    course.CourseTags.Add(new CourseTag
                    {
                        Tag = tag
                    });
                }
            }
            // Save the course to the database
            _context.courses.Add(course);
            await _context.SaveChangesAsync();

            return Ok(new {id = course.Id, Message = "Course added successfully",statuscode=StatusCodes.Status200OK });
        }
        [HttpDelete]
        [Route("DeleteCourseById/{id}")]
        [Authorize(Policy = "InstructorAndAdminPolicy")]
        public async Task<IActionResult> DeleteCourseById(int id )
        {
            if (!int.TryParse(id.ToString(), out var parsedId))
            {
                return BadRequest("Invalid ID format. Please provide a valid integer.");
            }
            var course = await _context.courses.Include(c =>c.Sections).ThenInclude(x =>x.Lessons)
                .Include(c =>c.CourseTags).FirstOrDefaultAsync(c => c.Id == id);
           
            if(course == null) 
                return NotFound(new {message = "Course Not Found"});
            var userrole = User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;
            var userid = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
           
            if (userrole != "teacher" && userrole != "admin")
            {
                return Unauthorized(new { Message = "You are not authorized to delete this course" });
            }

            //// Delete all LessonTags associated with Lessons in this Course
            //var lessonTags = course.Lessons.SelectMany(l => l.LessonTags).ToList();
            //if (lessonTags.Any())
            //{
            //    _context.LessonTag.RemoveRange(lessonTags);
            //}
            //delete all lessons within each section
            foreach (var section in course.Sections)
            {
                if (section.Lessons.Any())
                {
                    _context.Lesson.RemoveRange(section.Lessons);
                }
            }
            // Delete all Lessons associated with this Course
            if (course.Sections.Any())
            {
                _context.Sections.RemoveRange(course.Sections);
            }

            // Delete the associated tags if any (optional, depends on cascading rules)
            if (course.CourseTags != null && course.CourseTags.Any())
            {
                _context.CourseTags.RemoveRange(course.CourseTags);
            }
            // Delete the associated image from the server
            if (!string.IsNullOrEmpty(course.ImgUrl))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", course.ImgUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            // Remove the course from the database
            _context.courses.Remove(course);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Course deleted successfully" });

        }
        [HttpGet]
        [Route("GetTotalEnrollments")]
        [Authorize(Policy = "InstructorPolicy")]
        public async Task<IActionResult> GetTotalEnrollments()
        {
            // Extract instructor ID from the JWT token
            var instructorIdClaim = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(instructorIdClaim))
            {
                return Unauthorized(new {status=StatusCodes.Status403Forbidden, Message = "Invalid token" });
            }

            int instructorId = int.Parse(instructorIdClaim);

            var totalEnrollments = await _context.Subscriptions
                .Where(s => _context.courses.Any(c => c.Id == s.CourseId && c.Instructor_Id == instructorId))
                .CountAsync();

            return Ok(new {statuscode=StatusCodes.Status200OK , TotalEnrollments = totalEnrollments });
        }


        [HttpGet]
        [Route("GetCourseById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetCourseById(int id)
        {
            // Retrieve the course by its ID
            var course = await _context.courses.
                Include(c =>c.Instructor).
                Include(c =>c.Sections)
                .Include(c =>c.Rating)
                .Include(c => c.CourseTags) // Include related tags if needed
                .ThenInclude(ct => ct.Tag)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return NotFound(new { Message = "Course not found" });
            //var userRole = User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;
            //if (userRole != "teacher" && userRole != "admin")
            //    return Unauthorized(new { Message = "Only instructors and admin can get course" , userRole });

            // Get the current user's ID from the token
            var userIdClaim = User.FindFirst("Id");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                // Find or create rating record for this user and course
                var rating = await _context.Rating
                    .FirstOrDefaultAsync(r => r.StudentId == userId && r.CourseId == course.Id);

                if (rating == null)
                {
                    // Create new rating record if it doesn't exist
                    rating = new Rating
                    {
                        StudentId = userId,
                        CourseId = course.Id,
                        ClickCount = 1,
                        CompletionStatus = "No",
                        TimeSpentHours = 0,
                        RatingDate = DateTime.UtcNow
                    };
                    _context.Rating.Add(rating);
                }
                else
                {
                    // Increment click count for existing rating
                    rating.ClickCount++;
                }

                await _context.SaveChangesAsync();
            }

            // Optionally map to a DTO for a cleaner response
            var courseDto = new
            {
                course.Id,
                course.Name,
                course.Describtion,
                course.No_of_hours,
                course.Instructor_Id,
                course.no_of_students,
                course.CreationDate,
                course.LevelOfCourse,
                course.ImgUrl,
                course.Price,
                course.Discount,
                course.DiscountedPrice,
                Tags = course.CourseTags?.Select(ct => ct.Tag.Name).ToList()
            };

            return Ok(courseDto);
        }
      
    }



}
