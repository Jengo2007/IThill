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

        public async Task<PagedResult<Enrollment>> GetAllEnrollments(int page ,int pagesize)
    {
        var totalCount = await _context.Enrollments.CountAsync();
        var enrollments=await _context.Enrollments
            .Include(e=>e.Student)
            .Include(e=>e.Course)
            .OrderBy(e=>e.CreatedAt)
            .Skip((page - 1) * pagesize)
            .Take(pagesize)
            .ToListAsync();

        return new PagedResult<Enrollment>
        {
            Page = page,
            PageSize = pagesize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pagesize),
            Items = enrollments
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