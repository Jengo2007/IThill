
using System.ComponentModel.DataAnnotations;

namespace IThill_academy.Models;

public class Course
{   
    [Required]
    public int Id { get; set; }
    [Required(ErrorMessage = "Имя курса обязателен")]
    public string Title { get; set; }
    
    [Required(ErrorMessage = "Описание курса обязателен")]
    public string Description { get; set; }
    
    [Required(ErrorMessage = "Цена курса обязателен")]
    public decimal Price { get; set; }
    
    [Required(ErrorMessage = "Длительность курса обязателен")]
    public int Duration { get; set; }
    
    [Required(ErrorMessage = "Фото  курса обязателен")]

    public string? ImagePath { get; set; }


}