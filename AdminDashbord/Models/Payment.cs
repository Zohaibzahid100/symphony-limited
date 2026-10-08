using System;
using System.Collections.Generic;

namespace AdminDashbord.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int StudentId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string PaymentType { get; set; } = null!;

    public string? ReferenceNumber { get; set; }

    public DateOnly? PaymentDate { get; set; }

    public int? BranchId { get; set; }

    public string? PaymentStatus { get; set; }

    public virtual Branch? Branch { get; set; }

    public virtual ICollection<EntranceExamApplication> EntranceExamApplications { get; set; } = new List<EntranceExamApplication>();

    public virtual Student Student { get; set; } = null!;

    public virtual ICollection<StudentEnrollment> StudentEnrollments { get; set; } = new List<StudentEnrollment>();

    public virtual ICollection<StudentLabRegistration> StudentLabRegistrations { get; set; } = new List<StudentLabRegistration>();
}
