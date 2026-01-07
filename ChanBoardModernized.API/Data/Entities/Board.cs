using System.ComponentModel.DataAnnotations;

namespace ChanBoardModernized.API.Data.Entities;

public class Board
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string ShortName { get; set; } = null!;
    public string Description { get; set; } = null!;
}
