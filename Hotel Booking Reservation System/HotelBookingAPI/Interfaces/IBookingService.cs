using HotelBookingAPI.Models;

public interface IBookingService
{
    Task<Booking> CreateBookingAsync(Booking booking);
    Task<Booking?> GetBookingByIdAsync(int id);
    Task<List<Booking>> GetBookingsByUserIdAsync(string userId);
    Task<List<Booking>> GetAllBookingsAsync();
    Task<Booking?> UpdateBookingAsync(int id, Booking updatedBooking, string userId, bool isAdmin);
    Task<bool> CancelBookingAsync(int id, string userId, bool isAdmin);
}
