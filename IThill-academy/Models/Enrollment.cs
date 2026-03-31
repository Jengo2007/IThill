namespace IThill_academy.Models;

public class Enrollment
{
    public int Id { get; set; }
    public Guid StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Student Student { get; set; }
    public Course Course { get; set; }
}