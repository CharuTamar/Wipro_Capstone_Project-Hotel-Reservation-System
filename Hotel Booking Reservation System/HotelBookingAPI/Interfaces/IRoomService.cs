using HotelBookingAPI.Models;

namespace HotelBookingAPI.Interfaces
{
    public interface IRoomService
    {
        Task<Room> AddRoomAsync(Room room);
        Task<IEnumerable<Room>> GetRoomsByHotelIdAsync(int hotelId);
        Task<Room> UpdateRoomAsync(int id, Room room);
        Task<bool> DeleteRoomAsync(int id);
    }
}
