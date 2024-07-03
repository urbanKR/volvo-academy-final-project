using System.ComponentModel.DataAnnotations;

namespace NewsAggregationPlatform.Models.DTOs.Account
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string? Username { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
