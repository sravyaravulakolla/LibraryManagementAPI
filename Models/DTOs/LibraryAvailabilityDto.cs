namespace LibraryManagementAPI.Models.DTOs
{
    public class LibraryAvailabilityDto
    {
        public int LibraryId { get; set; }
        public string LibraryName { get; set; } = null!;
        public int AvailableCopies { get; set; }
    }
}
