using System;
using System.Collections.Generic;

namespace AdminDashbord.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? FullName { get; set; } = null!;

    public string? Email { get; set; } = null!;

    public string? Password { get; set; } = null!;

    public string? Phone { get; set; }

    public string UserImg { get; set; } = null!;

    public string Role { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
