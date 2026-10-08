using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class FinalResult
{
    public int ResultId { get; set; }

    public int StudentId { get; set; }

    public int FinalExamId { get; set; }

    public int TotalQuestions { get; set; }

    public int CorrectAnswers { get; set; }

    public int WrongAnswers { get; set; }

    public int TotalMarks { get; set; }

    public int MarksObtained { get; set; }

    public decimal Percentage { get; set; }

    public string? Grade { get; set; }

    public string? Remarks { get; set; }

    public string? ResultStatus { get; set; }

    public DateOnly? ResultDate { get; set; }

    public virtual FinalExam FinalExam { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
