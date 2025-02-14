using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace exam1.Entities;

public partial class BookedTicketDetail
{

    public int BookedTicketDetailId { get; set; }

    [Key]

    public string TicketCode { get; set; } = null!;


    public int Quantity { get; set; }

    public virtual BookedTicket BookedTicketDetailNavigation { get; set; } = null!;

    public virtual AvailableTicket TicketCodeNavigation { get; set; } = null!;
}
