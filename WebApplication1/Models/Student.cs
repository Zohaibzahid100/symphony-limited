using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public int UserId { get; set; }

    public int? BranchId { get; set; }

    public string? RollNumber { get; set; }

    public DateOnly? RegistrationDate { get; set; }

    public virtual Branch? Branch { get; set; }

    public virtual ICollection<EntranceResult> EntranceResults { get; set; } = new List<EntranceResult>();

    public virtual ICollection<FinalMcqanswer> FinalMcqanswers { get; set; } = new List<FinalMcqanswer>();

    public virtual ICollection<FinalResult> FinalResults { get; set; } = new List<FinalResult>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<StudentEnrollment> StudentEnrollments { get; set; } = new List<StudentEnrollment>();

    public virtual ICollection<StudentLabRegistration> StudentLabRegistrations { get; set; } = new List<StudentLabRegistration>();

    public virtual ICollection<StudentMcqanswer> StudentMcqanswers { get; set; } = new List<StudentMcqanswer>();

    public virtual User User { get; set; } = null!;
}
