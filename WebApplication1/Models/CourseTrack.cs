using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class CourseTrack
{
    public int TrackId { get; set; }

    public int CourseId { get; set; }

    public string TrackName { get; set; } = null!;

    public int DurationMonths { get; set; }

    public decimal CourseFee { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<CourseTopic> CourseTopics { get; set; } = new List<CourseTopic>();

    public virtual ICollection<EntranceResult> EntranceResults { get; set; } = new List<EntranceResult>();

    public virtual ICollection<StudentEnrollment> StudentEnrollments { get; set; } = new List<StudentEnrollment>();

    public virtual ICollection<TrackAssignmentRule> TrackAssignmentRules { get; set; } = new List<TrackAssignmentRule>();
}
