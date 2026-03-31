using System.ComponentModel.DataAnnotations;

namespace IThill_academy.DTOs;

public class RegisterStudentDto
{
    [Required(ErrorMessage = "Имя обязателен")]
    public string FirstName { get; set; }=string.Empty;
    [Required(ErrorMessage = "Фамилия обязателен")]
    public string LastName { get; set; }=string.Empty;
    [Required(ErrorMessage = "Номер телефона обязателен")]
    [RegularExpression(@"^\+992\d{9}$", ErrorMessage = "Телефон должен быть в формате +992XXXXXXXXX")]
    public string PhoneNumber { get; set; }=string.Empty;
    [Required(ErrorMessage = "Подтверждение кода обязателен")]
    public bool IsPhoneConfirmed { get; set; }
    public string CreatedAt { get; set; }=string.Empty;

}