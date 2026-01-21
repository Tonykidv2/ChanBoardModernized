using System.Text.Json.Serialization;

namespace ChanBoardModernized.Shared.Components.DTOs;

public record AuthResponseDto(string Token, string? ErrorMessage)
{
    [JsonIgnore]
    public bool hasError => !string.IsNullOrEmpty(ErrorMessage);
    public string RefreshToken {  get; set; } = string.Empty;
}
