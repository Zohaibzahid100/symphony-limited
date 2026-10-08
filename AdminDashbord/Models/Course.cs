using System;
using System.Collections.Generic;

namespace AdminDashbord.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public string CourseName { get; set; } = null!;

    public string CourseImg { get; set; } = null!;

    public string? Description { get; set; }

    public string CourseStatus { get; set; } = null!;

    public virtual ICollection<CourseTrack> CourseTracks { get; set; } = new List<CourseTrack>();

    public virtual ICollection<EntranceExam> EntranceExams { get; set; } = new List<EntranceExam>();

    public virtual ICollection<FinalExam> FinalExams { get; set; } = new List<FinalExam>();

    public virtual ICollection<LabSession> LabSessions { get; set; } = new List<LabSession>();

    public virtual ICollection<StudentEnrollment> StudentEnrollments { get; set; } = new List<StudentEnrollment>();

    public virtual ICollection<TrackAssignmentRule> TrackAssignmentRules { get; set; } = new List<TrackAssignmentRule>();
}
