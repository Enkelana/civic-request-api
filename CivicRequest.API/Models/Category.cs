using System.ComponentModel.DataAnnotations;

namespace CivicRequest.API.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }

        public ICollection<Request> Requests { get; set; } = new List<Request>();
    }
}