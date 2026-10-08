using System;
using System.Collections.Generic;

namespace AdminDashbord.Models;

public partial class TrackAssignmentRule
{
    public int RuleId { get; set; }

    public int CourseId { get; set; }

    public decimal MinPercentage { get; set; }

    public decimal MaxPercentage { get; set; }

    public int AssignedTrackId { get; set; }

    public virtual CourseTrack AssignedTrack { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;
}
