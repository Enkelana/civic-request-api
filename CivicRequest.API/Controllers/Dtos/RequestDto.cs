namespace CivicRequest.API.DTOs
{
    public class CreateRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CitizenId { get; set; }
        public int CategoryId { get; set; }
    }

    public class UpdateRequestDto
    {
        public string? OfficerNotes { get; set; }
        public string? Status { get; set; }
    }
}