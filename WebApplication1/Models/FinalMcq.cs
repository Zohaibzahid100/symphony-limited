using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class FinalMcq
{
    public int McqId { get; set; }

    public int FinalExamId { get; set; }

    public int? TopicId { get; set; }

    public string QuestionText { get; set; } = null!;

    public string OptionA { get; set; } = null!;

    public string OptionB { get; set; } = null!;

    public string OptionC { get; set; } = null!;

    public string OptionD { get; set; } = null!;

    public string CorrectOption { get; set; } = null!;

    public int McqMarks { get; set; }

    public virtual FinalExam FinalExam { get; set; } = null!;

    public virtual ICollection<FinalMcqanswer> FinalMcqanswers { get; set; } = new List<FinalMcqanswer>();

    public virtual CourseTopic? Topic { get; set; }
}
