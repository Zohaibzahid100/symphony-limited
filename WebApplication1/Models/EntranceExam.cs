using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class EntranceExam
{
    public int EntranceExamId { get; set; }

    public int CourseId { get; set; }

    public string ExamTitle { get; set; } = null!;

    public DateOnly ExamDate { get; set; }

    public DateOnly? LastDateToApply { get; set; }

    public decimal ExamFee { get; set; }

    public int TotalMarks { get; set; }

    public string ExamStatus { get; set; } = null!;

    public int BranchId { get; set; }

    public DateOnly? ResultAnnouncementDate { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<EntranceMcq> EntranceMcqs { get; set; } = new List<EntranceMcq>();

    public virtual ICollection<EntranceResult> EntranceResults { get; set; } = new List<EntranceResult>();

    public virtual ICollection<StudentMcqanswer> StudentMcqanswers { get; set; } = new List<StudentMcqanswer>();
}
