namespace IThill_academy.Models;

public class PendingRegistration
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Code { get; set; }
    public DateTime ExpiresAt { get; set; }
}