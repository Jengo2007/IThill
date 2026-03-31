using System.ComponentModel.DataAnnotations;

namespace IThill_academy.DTOs;

public class CreateCourseDto
{
    [Required(ErrorMessage = "Имя курса обязателен")]
    public string Title { get; set; }
    [Required(ErrorMessage = "Напишите описание  к курсу обязательно")]

    public string Description { get; set; }
    [Required(ErrorMessage = "Укажите Цену  курса обязательно")]

    public string Price { get; set; }
    [Required(ErrorMessage = "Продливание курса обязателен")]

    public int Duration { get; set; }
}