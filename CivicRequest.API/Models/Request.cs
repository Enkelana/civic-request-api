using System.ComponentModel.DataAnnotations;

namespace CivicRequest.API.Models
{
    public enum RequestStatus
    {
        Pending,
        InProgress,
        Resolved,
        Rejected
    }

    public class Request
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? OfficerNotes { get; set; }

        public int CitizenId { get; set; }
        public int CategoryId { get; set; }

        public Citizen Citizen { get; set; } = null!;
        public Category Category { get; set; } = null!;
    }
}