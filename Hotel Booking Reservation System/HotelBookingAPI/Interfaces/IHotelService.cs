using HotelBookingAPI.Models;

namespace HotelBookingAPI.Interfaces;

public interface IHotelService
{
    Task<IEnumerable<Hotel>> GetAllHotelsAsync();
    Task<Hotel> GetHotelByIdAsync(int id);
    Task<Hotel> AddHotelAsync(Hotel hotel);
    Task<Hotel> UpdateHotelAsync(int id, Hotel hotel);
    Task<bool> DeleteHotelAsync(int id);
}
