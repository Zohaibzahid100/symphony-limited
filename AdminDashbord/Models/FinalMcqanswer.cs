using System;
using System.Collections.Generic;

namespace AdminDashbord.Models;

public partial class FinalMcqanswer
{
    public int AnswerId { get; set; }

    public int StudentId { get; set; }

    public int FinalExamId { get; set; }

    public int McqId { get; set; }

    public string SelectedOption { get; set; } = null!;

    public int Marks { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public virtual FinalExam FinalExam { get; set; } = null!;

    public virtual FinalMcq Mcq { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
