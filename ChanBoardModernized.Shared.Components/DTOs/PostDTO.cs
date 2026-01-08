using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChanBoardModernized.Shared.Components.DTOs;

public class CommentDTO
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public int PostDigits { get; set; }
    public string Title { get; set; }
    public string? Author { get; set; }
    public Guid PhotoID { get; set; }
}
