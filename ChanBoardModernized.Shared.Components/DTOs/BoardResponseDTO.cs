using System.Text.Json.Serialization;

namespace ChanBoardModernized.Shared.Components.DTOs;

public record BoardResponseDTO(BoardDTO? BoardDTO, string? ErrorMessage)
{
    [JsonIgnore]
    public bool hasError => !string.IsNullOrEmpty(ErrorMessage);
}
