using System;

namespace University_422.Core;

public partial class VStudent
{
    public int? Id { get; set; }

    public string? LastName { get; set; }

    public string? FirstName { get; set; }

    public string? Patronymic { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? GroupName { get; set; }

    public int? Course { get; set; }

    public string? FacultyName { get; set; }

    public TypeStatus Status { get; set; }
}
