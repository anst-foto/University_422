using System;
using System.Collections.Generic;

namespace University_422.Core;

public partial class Person
{
    public int Id { get; set; }

    public int FirstNameId { get; set; }

    public int LastNameId { get; set; }

    public int PatronymicId { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public virtual FirstName FirstName { get; set; } = null!;

    public virtual LastName LastName { get; set; } = null!;

    public virtual Patronymic Patronymic { get; set; } = null!;

    public virtual ICollection<Student> TableStudents { get; set; } = new List<Student>();
}
