using System;
using System.Collections.Generic;

namespace infrastructure;

public partial class Game
{
    public Guid Id { get; set; }

    public int Weeknumber { get; set; }

    public int Yearnumber { get; set; }

    public virtual ICollection<Board> Boards { get; set; } = new List<Board>();

    public virtual Winnersequence? Winnersequence { get; set; }
}
