using System;
using System.Collections.Generic;

namespace infrastructure;

public partial class Player
{
    public Guid Id { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? Activated { get; set; }

    public virtual ICollection<Board> Boards { get; set; } = new List<Board>();
}
