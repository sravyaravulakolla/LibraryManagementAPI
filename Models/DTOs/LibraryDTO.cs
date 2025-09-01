namespace LibraryManagementAPI.Models.DTOs
{
    public class LibraryDTO
    {
        public int LibraryId { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public int MaximumCapacity { get; set; }

        public List<LibraryAvailabilityDto> Books { get; set; } = new();

    }
}
