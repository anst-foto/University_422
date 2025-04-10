using System;

namespace University_422.Core;

public partial class VPerson
{
    public int? Id { get; set; }

    public string? LastName { get; set; }

    public string? FirstName { get; set; }

    public string? Patronymic { get; set; }

    public DateOnly? DateOfBirth { get; set; }
}
