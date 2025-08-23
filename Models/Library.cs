using System;
using System.Collections.Generic;

namespace LibraryManagementAPI.Models;

public partial class Library
{
    public int LibraryId { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public int MaximumCapacity { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
