using IThill_academy.Controllers;
using IThill_academy.Data;
using IThill_academy.DTOs;
using IThill_academy.Models;
using Microsoft.EntityFrameworkCore;

namespace IThill_academy.Services;

public class CourseService
{
    private readonly ApplicationDbContext _context;

    public CourseService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Course>> GetAllCourses(int page ,int pageSize)
    {
        var totalCount=await _context.Courses.CountAsync();
        var courses=await _context.Courses.OrderBy(c=>c.Title)
            .Skip((page-1)*pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<Course>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            Items = courses

        };
    }

    public async Task<Course?> GetCoursesById(int id)
    {       
        var course= await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
        if (course == null)
        {
            throw new InvalidOperationException("Course not found");
        }

        return course;
    }

    public async Task<Course> CreateCourse(CreateCourseDto dto)
    {
        string? imagePath = null;

        if (dto.Image != null && dto.Image.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(dto.Image.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                throw new Exception("Разрешены только JPG и PNG");
            var folder = Path.Combine("wwwroot", "images", "courses");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid() + Path.GetExtension(dto.Image.FileName);
            var savePath = Path.Combine(folder, fileName);

            await using var stream = new FileStream(savePath, FileMode.Create);
            await dto.Image.CopyToAsync(stream);

            imagePath = $"/images/courses/{fileName}";
        }

        var course = new Course
        {
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            Duration = dto.Duration,
            ImagePath = imagePath,
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
        return course;
    }


    public async Task<Course> UpdateCourseById(int id,UpdateCourseDto dto)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null)
        {
            throw new InvalidOperationException("Course not found");
        }
        course.Title = dto.Title;
        course.Description = dto.Description;
        course.Price = dto.Price;
        course.Duration = dto.Duration;
        await _context.SaveChangesAsync();
        return course;
    }

    public async Task<Course> DeleteCourseById(int? id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null)
        {
            throw new InvalidOperationException("Course not found");
        }
        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
        return course;
    }
    public async Task<Course> UpdateCourse(UpdateCourseDto dto)
    {
        var course = await _context.Courses.FindAsync(dto.Id);
        if (course == null)
        {
            throw new InvalidOperationException("Курс не найден");
        }

        course.Title = dto.Title;
        course.Description = dto.Description;
        course.Price = dto.Price;
        course.Duration = dto.Duration;

        if (dto.Image != null)
        {
            // допустим, сохраняем путь к файлу
            course.ImagePath = await SaveImageAsync(dto.Image);
        }

        await _context.SaveChangesAsync();
        return course;
    }
    public async Task<Course> DeleteCourse(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null)
        {
            throw new InvalidOperationException("Курс не найден");
        }

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();

        return course;
    }
    
    private async Task<string?> SaveImageAsync(IFormFile image)
    {
        if (image == null || image.Length == 0)
            return null;

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(image.FileName).ToLower();

        if (!allowedExtensions.Contains(extension))
            throw new Exception("Разрешены только JPG и PNG");

        var folder = Path.Combine("wwwroot", "images", "courses");
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
        var savePath = Path.Combine(folder, fileName);

        await using var stream = new FileStream(savePath, FileMode.Create);
        await image.CopyToAsync(stream);

        return $"/images/courses/{fileName}";
    }

    

    public async Task<List<Course>> Getcourses(string? search, string? sort)
    {
        var query = _context.Courses.AsQueryable();
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(c => c.Title.ToLower().Contains(search) 
                                     || c.Description.ToLower().Contains(search));
        }

        if (!string.IsNullOrEmpty(sort))
        {
            switch (sort.ToLower())
            {
                case "title_asc":
                    query = query.OrderBy(c => c.Title);
                    break;
                case "title_desc":
                    query = query.OrderByDescending(c => c.Title);
                    break;
                default:
                    break;
            }
        }
        return await query.ToListAsync();

    }
}