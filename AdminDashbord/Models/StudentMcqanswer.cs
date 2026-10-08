using System;
using System.Collections.Generic;

namespace AdminDashbord.Models;

public partial class StudentMcqanswer
{
    public int AnswerId { get; set; }

    public int StudentId { get; set; }

    public int EntranceExamId { get; set; }

    public int McqId { get; set; }

    public string SelectedOption { get; set; } = null!;

    public int Marks { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public virtual EntranceExam EntranceExam { get; set; } = null!;

    public virtual EntranceMcq Mcq { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
