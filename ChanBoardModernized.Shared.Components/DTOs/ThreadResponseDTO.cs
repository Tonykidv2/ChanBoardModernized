using ChanBoardModernized.Shared.Components.DTOs;
using System.Text.Json.Serialization;

namespace ChanBoardModernized.Shared.Components.DTOsl;

public record ThreadResponseDTO(ThreadDTO? Thread, string ErrorMessage)
{
    [JsonIgnore]
    public bool hasError => !string.IsNullOrEmpty(ErrorMessage);
}
