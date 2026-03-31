using System.ComponentModel.DataAnnotations;

namespace IThill_academy.DTOs;

public class EnrollCourseDto
{
    [Required(ErrorMessage = "CourseId обязателен")]
    public int CourseId { get; set; }
}