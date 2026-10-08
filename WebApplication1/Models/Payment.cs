using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int StudentId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string PaymentType { get; set; } = null!;

    public string? ReferenceNumber { get; set; }

    public DateOnly? PaymentDate { get; set; }

    public string PaymentStatus { get; set; } = "Paid";

    public int? BranchId { get; set; }

    public virtual Branch? Branch { get; set; }

    public virtual Student Student { get; set; } = null!;
}
