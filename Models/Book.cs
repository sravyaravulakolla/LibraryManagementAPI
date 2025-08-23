using System;
using System.Collections.Generic;

namespace LibraryManagementAPI.Models;

public partial class Book
{
    public int BookId { get; set; }

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public int CategoryId { get; set; }

    public decimal Price { get; set; }

    public int AvailableCopies { get; set; }

    public int LibraryId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Library Library { get; set; } = null!;
}
