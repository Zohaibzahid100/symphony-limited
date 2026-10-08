using System;
using System.Collections.Generic;

namespace AdminDashbord.Models;

public partial class CourseTopic
{
    public int TopicId { get; set; }

    public int TrackId { get; set; }

    public string TopicName { get; set; } = null!;

    public virtual ICollection<EntranceMcq> EntranceMcqs { get; set; } = new List<EntranceMcq>();

    public virtual ICollection<FinalMcq> FinalMcqs { get; set; } = new List<FinalMcq>();

    public virtual CourseTrack Track { get; set; } = null!;
}
