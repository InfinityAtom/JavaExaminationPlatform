using System;
using System.Collections.Generic;

namespace JavaExam;

public partial class Specialization
{
    public int SpecializationId { get; set; }

    public string SpecializationName { get; set; } = null!;

    public virtual ICollection<Studenti> Studentis { get; set; } = new List<Studenti>();
}
