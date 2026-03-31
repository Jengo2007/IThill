using System.ComponentModel.DataAnnotations;

namespace IThill_academy.DTOs;

public class LoginDto
{   
    [Required(ErrorMessage = "Номер телефона обязателен")]
    [RegularExpression(@"^\+992\d{9}$", ErrorMessage = "Телефон должен быть в формате +992XXXXXXXXX")]
    public string PhoneNumber { get; set; }
}