using System.Collections.Generic;

namespace University_422.Core;

public partial class FirstName
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Person> TablePeople { get; set; } = new List<Person>();
}
