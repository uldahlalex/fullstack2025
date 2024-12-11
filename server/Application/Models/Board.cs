namespace service.Models;

public class Board
{
    public Guid Id { get; set; }

    public Guid Userid { get; set; }

    public Guid Gameid { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<int> Sortednumbers { get; set; } = null!;

    public bool Afviklet { get; set; }

    public bool Won { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual Player User { get; set; } = null!;
}