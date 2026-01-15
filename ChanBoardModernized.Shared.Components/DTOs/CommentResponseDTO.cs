

using System.Text.Json.Serialization;

namespace ChanBoardModernized.Shared.Components.DTOs;

public record CommentResponseDTO(CommentDTO? Comment, string? ErrorMessage)
{
    [JsonIgnore]
    public bool hasError => !string.IsNullOrEmpty(ErrorMessage);
}
