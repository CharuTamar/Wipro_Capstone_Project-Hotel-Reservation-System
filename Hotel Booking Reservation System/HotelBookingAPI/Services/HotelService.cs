using HotelBookingAPI.Interfaces;
using HotelBookingAPI.Data;
using HotelBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingAPI.Services;

public class HotelService : IHotelService
{
    private readonly HotelBookingDbContext _context;

    public HotelService(HotelBookingDbContext context)
    {
        _context = context;
    }

    //  Get all hotels
    public async Task<IEnumerable<Hotel>> GetAllHotelsAsync()
{
    return await _context.Hotels
                         .Include(h => h.Rooms) 
                         .ToListAsync(); //  Works directly with EF Core
}


    //  Get hotel by ID
    public async Task<Hotel> GetHotelByIdAsync(int id)
    {
        return await _context.Hotels.Include(h => h.Rooms)
                                    .FirstOrDefaultAsync(h => h.Id == id);
    }

    //  Add new hotel
    public async Task<Hotel> AddHotelAsync(Hotel hotel)
    {
        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();
        return hotel;
    }

    //  Update existing hotel
    public async Task<Hotel> UpdateHotelAsync(int id, Hotel hotel)
    {
        var existingHotel = await _context.Hotels.FindAsync(id);
        if (existingHotel == null) return null;

        existingHotel.Name = hotel.Name;
        existingHotel.Location = hotel.Location;
        existingHotel.Description = hotel.Description;

        await _context.SaveChangesAsync();
        return existingHotel;
    }

    //  Delete hotel
    public async Task<bool> DeleteHotelAsync(int id)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel == null) return false;

        _context.Hotels.Remove(hotel);
        await _context.SaveChangesAsync();
        return true;
    }
}
