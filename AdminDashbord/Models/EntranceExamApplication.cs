using System;
using System.Collections.Generic;

namespace AdminDashbord.Models;

public partial class EntranceExamApplication
{
    public int ApplicationId { get; set; }

    public int StudentId { get; set; }

    public int EntranceExamId { get; set; }

    public int BranchId { get; set; }

    public int? PaymentId { get; set; }

    public DateOnly ApplyDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Branch Branch { get; set; } = null!;

    public virtual EntranceExam EntranceExam { get; set; } = null!;

    public virtual Payment? Payment { get; set; }

    public virtual Student Student { get; set; } = null!;
}
