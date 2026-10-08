using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class EntranceResult
{
    public int ResultId { get; set; }

    public int StudentId { get; set; }

    public int EntranceExamId { get; set; }

    public int MarksObtained { get; set; }

    public int TotalMarks { get; set; }

    public decimal? Percentage { get; set; }

    public string ResultStatus { get; set; } = null!;

    public int? AssignedTrackId { get; set; }

    public DateOnly? ResultDate { get; set; }

    public virtual CourseTrack? AssignedTrack { get; set; }

    public virtual EntranceExam EntranceExam { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
