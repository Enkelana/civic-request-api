using System.ComponentModel.DataAnnotations;

namespace CivicRequest.API.Models
{
    public class Citizen
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Request> Requests { get; set; } = new List<Request>();
    }
}