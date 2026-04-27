

using System.ComponentModel.DataAnnotations;

namespace IThill_academy.Models;

public enum UserRole
{
    Student,
    Admin
}
public class Student
{
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "Заполните поля")]
    public string FirstName { get; set; }
    [Required(ErrorMessage = "Заполните поля")]
    public string LastName { get; set; }
    [Required(ErrorMessage = "Заполните поля")]
    public string PhoneNumber { get; set; }
    [Required(ErrorMessage = "Заполните поля")]
    public string Password { get; set; }
    [Required(ErrorMessage = "Заполните поля")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Заполните поля")]
    public bool IsEmailConfirmed { get; set; }
    [Required(ErrorMessage = "Заполните поля")]
    public DateTime CreatedAt { get; set; }
    public UserRole Role { get; set; } = UserRole.Student;
    public ICollection<EmailCode> EmailCodes { get; set; }

}