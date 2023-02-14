using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace LacunaGenetics.Models
{
    public class UserModel
    {
        [Required(ErrorMessage = "Username should not be empty")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Only alpha numeric characters are allowed")]
        [JsonProperty("username")]
        public string Username { get; set; }

        [EmailAddress]
        [JsonProperty("email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password should not be empty")]
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
