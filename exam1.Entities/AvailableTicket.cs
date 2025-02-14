using System;
using System.Collections.Generic;

namespace exam1.Entities;

public partial class AvailableTicket
{
    public string TicketCode { get; set; } = null!;

    public int CategoryId { get; set; }

    public string TicketName { get; set; } = null!;

    public DateTime EventDate { get; set; }

    public int Quota { get; set; }

    public int Price { get; set; }

    public virtual CategoryTicket Category { get; set; } = null!;
}
