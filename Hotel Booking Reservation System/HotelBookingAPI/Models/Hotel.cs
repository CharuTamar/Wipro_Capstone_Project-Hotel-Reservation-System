using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.Models;

public class Hotel
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    [MaxLength(200)]
    public string Location { get; set; }

    public string Description { get; set; }

    // Navigation property
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}
