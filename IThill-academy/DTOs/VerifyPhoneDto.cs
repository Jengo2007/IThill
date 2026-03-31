using System.ComponentModel.DataAnnotations;

namespace IThill_academy.DTOs;

public class VerifyPhoneDto
{



    [Required(ErrorMessage = "Номер тедефона обязателен")]
    [RegularExpression(@"^\+992\d{9}$", ErrorMessage = "Телефон должен быть в формате +992XXXXXXXXX")]
    public string PhoneNumber { get; set; }
    [Required(ErrorMessage = "Подтверждение кода обязателен")]

    public string Code { get; set; }
}