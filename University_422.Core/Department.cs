using System.Collections.Generic;

namespace University_422.Core;

public partial class Department
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int FacultyId { get; set; }

    public bool IsActive { get; set; }

    public virtual Faculty Faculty { get; set; } = null!;

    public virtual ICollection<DepartmentsSubject> TableDepartmentsSubjects { get; set; } = new List<DepartmentsSubject>();
}
