

namespace IThill_academy.Models;

public enum UserRole
{
    Student,
    Admin
}
public class Student
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserRole Role { get; set; } = UserRole.Student;
    public ICollection<EmailCode> EmailCodes { get; set; }

}