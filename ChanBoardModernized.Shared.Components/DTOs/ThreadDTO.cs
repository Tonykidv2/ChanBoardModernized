using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChanBoardModernized.Shared.Components.DTOs;

public class ThreadDTO
{
    public Guid Id { get; set; }
    public Guid BoardId { get; set; }
    public string Author { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    [Required]
    public string Content { get; set; } = string.Empty; //Used for the initial post content
    public DateTime CreatedDate { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
    public List<CommentDTO> Comments { get; set; } = new List<CommentDTO>();
}
