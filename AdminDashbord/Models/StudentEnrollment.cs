using System;
using System.Collections.Generic;

namespace AdminDashbord.Models;

public partial class StudentEnrollment
{
    public int EnrollmentId { get; set; }

    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public int TrackId { get; set; }

    public DateOnly? EnrollmentDate { get; set; }

    public int? PaymentId { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Payment? Payment { get; set; }

    public virtual Student Student { get; set; } = null!;

    public virtual CourseTrack Track { get; set; } = null!;
}
