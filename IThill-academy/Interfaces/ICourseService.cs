using IThill_academy.DTOs;
using IThill_academy.Models;

namespace IThill_academy.Interfaces;

public interface ICourseService
{
    Task<PagedResult<Course>> GetAllCourses(int page, int pageSize);
    Task<Course?> GetCoursesById(int id);
    Task<Course> CreateCourse(CreateCourseDto dto);
    Task<Course> UpdateCourseById(int id, UpdateCourseDto dto);
    Task<Course> DeleteCourseById(int? id);
    Task<Course> UpdateCourse(UpdateCourseDto dto);
    Task<Course> DeleteCourse(int id);
    Task<List<Course>> GetCourses(string? search, string? sort);
}