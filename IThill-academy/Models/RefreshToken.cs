namespace IThill_academy.Models;

public class RefreshToken
{
    public int Id { get; set; } // 🔑 первичный ключ

    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public DateTime Created { get; set; }
    public string UserId { get; set; }

    public bool IsExpired => DateTime.UtcNow >= Expires;
}