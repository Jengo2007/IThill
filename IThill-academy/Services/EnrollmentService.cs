using IThill_academy.Data;
using IThill_academy.Models;
using Microsoft.EntityFrameworkCore;

namespace IThill_academy.Services;

public class EnrollmentService
{
    private readonly ApplicationDbContext _context;

    public EnrollmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Enrollment> Enroll(Guid studentId, int courseId)
    {
        
        var course= await _context.Courses.FirstOrDefaultAsync(c=>c.Id==courseId);
        if (course == null) throw new InvalidOperationException("Course not found");
        
        var existing=await _context.Enrollments.FirstOrDefaultAsync(e => e.StudentId == studentId 
          && e.CourseId == courseId);
        
        if (existing != null)
            throw new InvalidOperationException("Студент уже записан на этот курс.");
        
        var enrollment = new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            CreatedAt = DateTime.UtcNow

        };
        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();
        return enrollment;

    }

    public async Task<List<Course>> GetMyCourses(Guid studentId)
    {
        return await _context.Enrollments.Where(e => e.StudentId == studentId).Include(e => e.Course)
            .Select(e => e.Course).ToListAsync();
    }

        public async Task<PagedResult<Enrollment>> GetAllEnrollments(int page ,int pageSize,string sortOrder)
    {
        var query = _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .AsQueryable();

        switch (sortOrder)
        {
            case "student_asc":
                query = query.OrderBy(e => e.Student.FirstName);
                break;
            case "student_desc":
                query = query.OrderByDescending(e => e.Student.FirstName);
                break;
            case "course_asc":
                query = query.OrderBy(e => e.Course.Title);
                break;
            case "course_desc":
                query = query.OrderByDescending(e => e.Course.Title);
                break;
            case "newest":
                query = query.OrderByDescending(e => e.CreatedAt);
                break;
            case "oldest":
                query = query.OrderBy(e => e.CreatedAt);
                break;
            default:
                query = query.OrderByDescending(e => e.CreatedAt); // новые сверху по умолчанию
                break;
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Enrollment>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            Items = items
        };
    }

    public async Task<Enrollment> DeletEnrollment(Guid studentId, int courseId)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course).FirstOrDefaultAsync(e => e.StudentId == studentId&& e.CourseId == courseId);
        if (enrollment == null)
            throw new InvalidOperationException("Запись не найдена");
        _context.Enrollments.Remove(enrollment);
        await _context.SaveChangesAsync();
        return enrollment;
    }    
}