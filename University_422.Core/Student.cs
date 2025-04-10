using System;

namespace University_422.Core;

public partial class Student
{
    public int Id { get; set; }

    public int PersonId { get; set; }

    public int GroupId { get; set; }
    
    public TypeStatus Status { get; set; }

    public DateOnly EnrollmentDate { get; set; }

    public DateOnly? DegreeDate { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual Person Person { get; set; } = null!;
}
