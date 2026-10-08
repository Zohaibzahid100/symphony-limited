using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Branch
{
    public int BranchId { get; set; }

    public string BranchName { get; set; } = null!;

    public string BranchImg { get; set; } = null!;

    public string BranchCity { get; set; } = null!;

    public string BranchEmail { get; set; } = null!;

    public string? Address { get; set; }

    public string? BranchContact { get; set; }

    public string BranchStatus { get; set; } = null!;

    public virtual ICollection<EntranceExam> EntranceExams { get; set; } = new List<EntranceExam>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
