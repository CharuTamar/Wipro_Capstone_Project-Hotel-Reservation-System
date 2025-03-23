using HotelBookingAPI.Data;
using HotelBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

public class BookingService : IBookingService
{
    private readonly HotelBookingDbContext _context;

    public BookingService(HotelBookingDbContext context)
    {
        _context = context;
    }

    //  Create Booking
    public async Task<Booking> CreateBookingAsync(Booking booking)
{
    var room = await _context.Rooms.FindAsync(booking.RoomId);
    if (room == null || !room.IsAvailable)
        throw new InvalidOperationException("Room is not available or does not exist.");

    var numberOfNights = (booking.CheckOutDate - booking.CheckInDate).Days;
    if (numberOfNights <= 0)
        throw new InvalidOperationException("Check-out date must be after check-in date.");

    booking.IsPaid = false; //  Initialize payment status
    _context.Bookings.Add(booking);
    await _context.SaveChangesAsync();

    //  Add TotalAmount dynamically to the Booking model (optional)
    booking.Room = room; // Include room details for calculation

    return booking;
}


    //  Get Booking by ID
    public async Task<Booking?> GetBookingByIdAsync(int id)
    {
        return await _context.Bookings
            .Include(b => b.Room)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    //  Get All Bookings (Admin)
    public async Task<List<Booking>> GetAllBookingsAsync()
    {
        return await _context.Bookings
            .Include(b => b.Room)
            .Include(b => b.User)
            .ToListAsync();
    }

    //  Get Bookings by User (Registered User)
    public async Task<List<Booking>> GetBookingsByUserIdAsync(string userId)
    {
        return await _context.Bookings
            .Where(b => b.UserId == userId)
            .Include(b => b.Room)
            .Include(b => b.User)
            .ToListAsync();
    }

    //  Update Booking (Registered User can only update check-in/out dates)
    public async Task<Booking?> UpdateBookingAsync(int id, Booking updatedBooking, string userId, bool isAdmin)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
            return null;

        // 🔒 Only admin or owner of the booking can update
        if (!isAdmin && booking.UserId != userId)
            throw new UnauthorizedAccessException("You are not allowed to modify this booking.");

        // ❗ Registered user can only modify CheckInDate and CheckOutDate
        if (!isAdmin)
        {
            booking.CheckInDate = updatedBooking.CheckInDate;
            booking.CheckOutDate = updatedBooking.CheckOutDate;
        }
        else
        {
            //  Admin can update everything
            booking.CheckInDate = updatedBooking.CheckInDate;
            booking.CheckOutDate = updatedBooking.CheckOutDate;
            booking.IsPaid = updatedBooking.IsPaid;
            booking.RoomId = updatedBooking.RoomId;
        }

        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();

        return booking;
    }

    //  Cancel Booking (Registered User or Admin)
    public async Task<bool> CancelBookingAsync(int id, string userId, bool isAdmin)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
            return false;

        // 🔒 Only admin or owner of the booking can cancel
        if (!isAdmin && booking.UserId != userId)
            throw new UnauthorizedAccessException("You are not allowed to cancel this booking.");

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();

        return true;
    }
}
