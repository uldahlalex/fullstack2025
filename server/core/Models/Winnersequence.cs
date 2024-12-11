using System;
using System.Collections.Generic;

namespace infrastructure;

public partial class Winnersequence
{
    public Guid Id { get; set; }

    public Guid Gameid { get; set; }

    public DateTime? CreatedAt { get; set; }

    public List<int> Sequence { get; set; } = null!;

    public virtual Game Game { get; set; } = null!;
}
