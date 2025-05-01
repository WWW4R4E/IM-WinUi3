using System.Text.Json.Serialization;

namespace IMWinUi.Models;

public class ResultInformation
{
    [JsonPropertyName("UserId")] public int Id { get; set; }

    [JsonPropertyName("UserName")] public string Name { get; set; }

    public byte[] ProfilePicture { get; set; } = new byte[0];
    public string? Description { get; set; }
}