using Microsoft.AspNetCore.Identity;

namespace HotelBookingAPI.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } 
}
