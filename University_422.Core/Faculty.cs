using System.Collections.Generic;

namespace University_422.Core;

public partial class Faculty
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<Department> TableDepartments { get; set; } = new List<Department>();

    public virtual ICollection<Group> TableGroups { get; set; } = new List<Group>();
}
