using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HotelBookingAPI.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Required]
        public string RoomType { get; set; } // Example: Single, Double, Suite

        [Required]
        public decimal PricePerNight { get; set; }

        [Required]
        public bool IsAvailable { get; set; }

        // Foreign key to link with Hotel
        public int HotelId { get; set; }

        [ForeignKey("HotelId")]
        [JsonIgnore]
        public Hotel? Hotel { get; set; }
    }
}
