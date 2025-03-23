using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Data
{
    public class HotelBookingDbContext : IdentityDbContext<ApplicationUser>
    {
        public HotelBookingDbContext(DbContextOptions<HotelBookingDbContext> options)
            : base(options) { }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }
}
