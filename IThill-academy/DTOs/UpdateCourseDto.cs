using System.ComponentModel.DataAnnotations;

namespace IThill_academy.DTOs;

public class UpdateCourseDto
{ 
        [Required(ErrorMessage = "Имя обязателен")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Описание курса обязателен")]

        public string Description { get; set; } 
        [Required(ErrorMessage = "Цуна курса обязателен")]

        public string Price { get; set; }
        [Required(ErrorMessage = "Цена курса обязателен")]

        public int Duration { get; set; }
        
}
