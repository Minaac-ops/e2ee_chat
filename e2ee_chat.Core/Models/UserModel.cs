namespace e2ee_chat.Core.Models;

public class UserModel
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Random { get; set; }
    public byte[] EncryptedRandom { get; set; }
    public byte[] IV { get; set; }
    public byte[] PasswordSalt { get; set; }

    public bool IsPasswordValid()
    {
        return Password.Length > 8 &&
               Password.Any(char.IsUpper) &&
               Password.Any(char.IsLower) &&
               Password.Any(char.IsDigit) &&
               Password.Any(ch => "!@#$%^&*()".Contains(ch));
    }
}