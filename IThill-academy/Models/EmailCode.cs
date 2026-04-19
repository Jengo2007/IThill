namespace IThill_academy.Models;

public class EmailCode
{
    public int Id { get; set; }           
    public Guid StudentId { get; set; }
    public Student Student { get; set; }
    public string Code { get; set; }          
    public DateTime ExpiresAt { get; set; }   
    public bool IsUsed { get; set; } 
}