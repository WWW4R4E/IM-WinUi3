namespace ChatRoomASP.Models;

public class Password
{
    internal static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    internal static bool VerifyPassword(string hash,string password)
    {
        Console.WriteLine($"Verifying password with hash: {hash}");

        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}