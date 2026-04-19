using System.ComponentModel.DataAnnotations;

namespace IThill_academy.DTOs;

public class LoginDto
{   
    [Required(ErrorMessage = "Номер телефона обязателен")]
    [EmailAddress]
    public string Email { get; set; }
    [Required(ErrorMessage = "Пароль обязателен!")]
    public string Password { get; set; }
}