using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class StudentLabRegistration
{
    public int LabRegistrationId { get; set; }

    public int StudentId { get; set; }

    public int LabSessionId { get; set; }

    public DateOnly? RegisterDate { get; set; }

    public int? PaymentId { get; set; }

    public virtual LabSession LabSession { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;

    public virtual Payment? Payment { get; set; }
}
