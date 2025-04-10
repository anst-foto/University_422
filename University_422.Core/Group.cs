using System;
using System.Collections.Generic;

namespace University_422.Core;

public partial class Group
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public DateOnly DateOpening { get; set; }

    public DateOnly? DateClosing { get; set; }

    public int Course { get; set; }

    public int FacultyId { get; set; }

    public bool IsActive { get; set; }

    public virtual Faculty Faculty { get; set; } = null!;

    public virtual ICollection<Student> TableStudents { get; set; } = new List<Student>();
}
