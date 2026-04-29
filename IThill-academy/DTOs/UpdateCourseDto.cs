using System.ComponentModel.DataAnnotations;

namespace IThill_academy.DTOs;

public class UpdateCourseDto
{ 
        [Required]
        public int Id { get; set; }   
        [Required(ErrorMessage = "Имя обязателен")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Описание курса обязателен")]

        public string Description { get; set; } 
        [Required(ErrorMessage = "Цуна курса обязателен")]

        public decimal Price { get; set; }
        [Required(ErrorMessage = "Цена курса обязателен")]

        public int Duration { get; set; }
        public IFormFile? Image { get; set; }
        
}
