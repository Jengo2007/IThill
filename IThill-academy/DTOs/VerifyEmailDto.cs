using System.ComponentModel.DataAnnotations;

namespace IThill_academy.DTOs;

public class VerifyEmailDto
{



    [Required(ErrorMessage = "Номер тедефона обязателен")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Подтверждение кода обязателен")]

    public string Code { get; set; }
}   