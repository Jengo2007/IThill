namespace IThill_academy.Models;

public class SmsCode
{
    public int Id { get; set; }               
    public string PhoneNumber { get; set; }   
    public string Code { get; set; }          
    public DateTime ExpiresAt { get; set; }   
    public bool IsUsed { get; set; } 
}