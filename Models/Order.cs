using System.ComponentModel.DataAnnotations;

namespace MvcMusicStoreModernized.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public string Username { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        [Required]
        public string PostalCode { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public decimal Total { get; set; }

        public List<OrderDetail> OrderDetails { get; set; } = new();
    }
}