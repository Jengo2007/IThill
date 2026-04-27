using System.ComponentModel.DataAnnotations;

namespace IThill_academy.DTOs;

public class CreateCourseDto
{
    [Required(ErrorMessage = "Имя курса обязателен")]
    public string Title { get; set; }
    [Required(ErrorMessage = "Напишите описание  к курсу обязательно")]

    public string Description { get; set; }
    [Required(ErrorMessage = "Укажите цену курса")]
    [Range(0, 1000000, ErrorMessage = "Цена должна быть положительной")]

    public decimal Price { get; set; }
    [Required(ErrorMessage = "Продливание курса обязателен")]

    public int Duration { get; set; }
    public IFormFile? Image { get; set; }
}