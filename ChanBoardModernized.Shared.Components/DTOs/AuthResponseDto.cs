using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChanBoardModernized.Shared.Components.DTOs;

public record AuthResponseDto(string Token, string? ErrorMessage)
{
    [JsonIgnore]
    public bool hasError => !string.IsNullOrEmpty(ErrorMessage);
}
