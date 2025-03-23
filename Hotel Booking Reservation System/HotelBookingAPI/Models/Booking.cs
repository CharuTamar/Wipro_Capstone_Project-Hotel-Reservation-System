using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HotelBookingAPI.Models;

public class Booking
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime CheckInDate { get; set; }

    [Required]
    public DateTime CheckOutDate { get; set; }

    [Required]
    public bool IsPaid { get; set; }

    // Foreign Key linking to Room
    [ForeignKey("Room")]
    public int RoomId { get; set; }
    public Room? Room { get; set; }

    // Foreign Key linking to User
    [ForeignKey("User")]
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
}
