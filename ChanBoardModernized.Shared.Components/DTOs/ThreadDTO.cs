using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChanBoardModernized.Shared.Components.DTOs;

public class ThreadDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
    public List<CommentDTO> Comments { get; set; } = new List<CommentDTO>();
}
