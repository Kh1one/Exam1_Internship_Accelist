using System;
using System.Collections.Generic;

namespace exam1.Entities;

public partial class CategoryTicket
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public virtual ICollection<AvailableTicket> AvailableTickets { get; set; } = new List<AvailableTicket>();
}
