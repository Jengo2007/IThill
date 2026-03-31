using IThill_academy.Data;
using IThill_academy.DTOs;
using IThill_academy.Models;
using IThill_academy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IThill_academy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CourseController : Controller
{
    private readonly CourseService _courseService;
    private readonly ApplicationDbContext _context;
    public CourseController(CourseService courseService, ApplicationDbContext context)
    {
        _courseService = courseService;
        _context = context;
    }

    [HttpGet("Getall")]
    public async Task<ActionResult<List<Course>>> GetAllCourses(int page = 1, int pageSize = 5)
    {
        var courses=await _courseService.GetAllCourses(page, pageSize);
        return Ok(courses);
    }
    [Authorize(Roles = "Admin")]
    [HttpGet("GetcourseByid")]
    public async Task<ActionResult<Course>> GetCourseById(int id)
    {
        try
        {
            var course = await _courseService.GetCoursesById(id);

            return Ok(course);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
    [Authorize(Roles = "Admin")]
    [HttpPost("create")]
    public async Task<ActionResult<Course>> AddCourse(CreateCourseDto dto)
    {
        var course = await _courseService.CreateCourse(dto);
        return course;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut("update")]
    public async Task<ActionResult<Course>> UpdateCourse(int id, UpdateCourseDto dto)
    {
        try
        {
            var course = await _courseService.UpdateCourseById(id, dto);

            return Ok(course);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("delete")]
    public async Task<ActionResult<Course>> DeleteCourse(int id)
    {
        try
        {
            var course = await _courseService.DeleteCourseById(id);
            return Ok(course);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("getcourses")]
    public async Task<ActionResult> Getcourses(string? search,string? sort)
    { 
        var query= _context.Courses.AsQueryable();
        if (!string.IsNullOrEmpty(search)) 
        { 
            query=query.Where(c=>c.Title.ToLower().Contains(search)||c.Description.ToLower().Contains(search)); 
        }

        if (!string.IsNullOrEmpty(sort))
        {
            switch (sort.ToLower())
            {
                case "title_asc":
                    query=query.OrderBy(c=>c.Title);
                    break;
                case "title_desc":
                    query=query.OrderByDescending(c => c.Title);
                    break;
                default:
                    break;
            }
        }
        var courses = await query.ToListAsync(); 
        return Ok(courses);
    }


}