namespace Application.Models.Entities;

public class Player
{
    public Guid Id { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool Activated { get; set; }

    public virtual ICollection<Board> Boards { get; set; } = new List<Board>();
    public string Salt { get; set; } = null!;
    public string Hash { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Role { get; set; } = null!;
}