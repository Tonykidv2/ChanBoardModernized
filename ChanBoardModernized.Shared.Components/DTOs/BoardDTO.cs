using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChanBoardModernized.Shared.Components.DTOs;

public class BoardDTO
{
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string ShortName { get; set; }
    public string Description { get; set; } = string.Empty;
}
