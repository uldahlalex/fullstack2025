﻿namespace service.Models;

public class Game
{
    public Guid Id { get; set; }

    public int Weeknumber { get; set; }

    public int Yearnumber { get; set; }

    public virtual ICollection<Board> Boards { get; set; } = new List<Board>();

    public virtual Winnersequence? Winnersequence { get; set; }
}