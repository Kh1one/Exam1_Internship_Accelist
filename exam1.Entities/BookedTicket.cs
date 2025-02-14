using System;
using System.Collections.Generic;

namespace exam1.Entities;

public partial class BookedTicket
{
    public int BookedTicketId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
