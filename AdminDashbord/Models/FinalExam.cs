using System;
using System.Collections.Generic;

namespace AdminDashbord.Models;

public partial class FinalExam
{
    public int FinalExamId { get; set; }

    public int CourseId { get; set; }

    public string ExamTitle { get; set; } = null!;

    public DateOnly? ExamDate { get; set; }

    public int TotalMarks { get; set; }

    public DateOnly? ResultAnnouncementDate { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<FinalMcqanswer> FinalMcqanswers { get; set; } = new List<FinalMcqanswer>();

    public virtual ICollection<FinalMcq> FinalMcqs { get; set; } = new List<FinalMcq>();

    public virtual ICollection<FinalResult> FinalResults { get; set; } = new List<FinalResult>();
}
