using Newtonsoft.Json;

namespace LibraryManagementAPI.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public int LibraryId { get; set; }
        [JsonIgnore]
        public Library Library { get; set; } = null!; // Navigation property
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
