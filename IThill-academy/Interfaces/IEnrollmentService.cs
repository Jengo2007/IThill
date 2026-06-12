using IThill_academy.Models;

namespace IThill_academy.Interfaces;

public interface IEnrollmentService
{
    Task<Enrollment> Enroll(Guid studentId, int courseId);
    Task<List<Course>> GetMyCourses(Guid studentId);
    Task<PagedResult<Enrollment>> GetAllEnrollments(int page, int pageSize, string sortOrder);
    Task<Enrollment> DeletEnrollment(Guid studentId, int courseId);
}