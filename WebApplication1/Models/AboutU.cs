using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class AboutU
{
    public int AboutId { get; set; }

    public string? SectionTitle { get; set; }

    public string? SectionsImg { get; set; }

    public string Description { get; set; } = null!;
}
