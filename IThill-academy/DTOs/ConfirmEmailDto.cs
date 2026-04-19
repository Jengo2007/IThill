using System.ComponentModel.DataAnnotations;

namespace IThill_academy.DTOs;

public class ConfirmEmailDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Code { get; set; }
}