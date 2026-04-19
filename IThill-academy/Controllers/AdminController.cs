using IThill_academy.Models;
using IThill_academy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IThill_academy.Controllers;

[Authorize(Roles = "Admin")] 
[ApiController]
[Route("api/[controller]")]
public class AdminController : Controller
{
    private readonly AuthService _authService;
    private readonly CourseService _courseService;
    private readonly EnrollmentService _enrollmentService;

    public AdminController(AuthService authService, CourseService courseService, EnrollmentService enrollmentService)
    {
        _authService = authService;
        _courseService = courseService;
        _enrollmentService = enrollmentService;
    }


    [HttpGet("AllStudents")]
    public async Task<IActionResult> GetAllStudents(int page=1, int pageSize=4)
    {
        var result = await _authService.GetAllStudent(page, pageSize);
        return Ok(new
        {
            result.Page,
            result.PageSize,
            result.TotalCount,
            result.TotalPages,
            Students=result.Items.Select(s=>new
            {
                s.Id,
                s.FirstName,
                s.LastName,
                s.PhoneNumber,
                s.Role
            })
        });
    }


    [HttpGet("Enrollments")]
    public async Task<ActionResult> GetAllEnrollments(int page=1, int pageSize=4)
    {
        var enrollments = await _enrollmentService.GetAllEnrollments(page , pageSize);
        return Ok(new
        {
            enrollments.Page,
            enrollments.PageSize,
            enrollments.TotalCount,
            enrollments.TotalPages,
            Enrollments=enrollments.Items.Select(e=>new
            {
                e.Id,
                e.StudentId,
                FirstName=e.Student.FirstName,
                LastName=e.Student.LastName,
                PhoneNumber=e.Student.PhoneNumber,
                e.CourseId,
                Title=e.Course.Title,
                Description=e.Course.Description,
                e.CreatedAt
            })
        });
    }

    [HttpGet("AllCourses")]
    public async Task<IActionResult> GetAllCourses(int page=1,int pageSize=5)
    {
        var courses = await _courseService.GetAllCourses(page, pageSize);
        return Ok(new
        {
            courses.Page,
            courses.PageSize,
            courses.TotalPages,
            courses.TotalCount,
            Courses=courses.Items.Select(c=>new
            {
                c.Id,
                c.Title,
                c.Description,
            })
        });
    }

    [HttpDelete("DeleteEnrollment")]
    public async Task<IActionResult> DeleteStudentFromCourse(Guid studentId, int courseId)
    {
        try
        {
            var enrollment = await _enrollmentService.DeletEnrollment(studentId, courseId);
            if (enrollment == null) return NotFound("Студент не найден в этом курсе.");
            return Ok(new
            {
                enrollment.Id,
                enrollment.StudentId,
                StudentName = enrollment.Student.FirstName,
                StudentLastName=enrollment.Student.LastName,
                enrollment.CourseId,
                CourseName = enrollment.Course.Title,
                enrollment.CreatedAt
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("DeleteCourse")]
    public async Task<IActionResult> DeleteCourseFromCourses(int courseId)
    {
        try
        {
            var result = await _courseService.DeleteCourseById(courseId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("DeleteStudent")]
    public async Task<IActionResult> DeleteStudent(Guid studentId)
    {
        try
        {
            var student = await _authService.DeleteStudent(studentId);
            return Ok(student);
        }
        catch(InvalidOperationException ex)
        {
            return BadRequest (ex.Message);
        }
    }
}