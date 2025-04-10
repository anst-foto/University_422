using System;

namespace University_422.Core;

public partial class VGroup
{
    public int? Id { get; set; }

    public string? Title { get; set; }

    public DateOnly? DateOpening { get; set; }

    public DateOnly? DateClosing { get; set; }

    public string? Faculty { get; set; }

    public int? Course { get; set; }

    public bool? IsActive { get; set; }
}
