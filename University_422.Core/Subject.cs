using System.Collections.Generic;

namespace University_422.Core;

public partial class Subject
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<DepartmentsSubject> TableDepartmentsSubjects { get; set; } = new List<DepartmentsSubject>();
}
