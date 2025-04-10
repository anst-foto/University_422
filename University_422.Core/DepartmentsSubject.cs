namespace University_422.Core;

public partial class DepartmentsSubject
{
    public int Id { get; set; }

    public int DepartmentId { get; set; }

    public int SubjectId { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;
}
