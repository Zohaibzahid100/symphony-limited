using System;
using System.Collections.Generic;

namespace AdminDashbord.Models;

public partial class LabSession
{
    public int LabSessionId { get; set; }

    public int CourseId { get; set; }

    public string SessionName { get; set; } = null!;

    public decimal? SessionFee { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<StudentLabRegistration> StudentLabRegistrations { get; set; } = new List<StudentLabRegistration>();
}
