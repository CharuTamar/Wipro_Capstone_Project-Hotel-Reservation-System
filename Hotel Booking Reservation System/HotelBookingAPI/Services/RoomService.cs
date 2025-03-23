using HotelBookingAPI.Data;
using HotelBookingAPI.Models;
using HotelBookingAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingAPI.Services
{
    public class RoomService : IRoomService
    {
        private readonly HotelBookingDbContext _context;

        public RoomService(HotelBookingDbContext context)
        {
            _context = context;
        }

        //  Add Room to a Hotel
        public async Task<Room?> AddRoomAsync(Room room)
        {
            var hotelExists = await _context.Hotels.AnyAsync(h => h.Id == room.HotelId);
            if (!hotelExists)
            {
                throw new Exception($"Hotel with ID {room.HotelId} does not exist.");
            }

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return room;
        }


        //  Get Rooms by Hotel ID
        public async Task<IEnumerable<Room>> GetRoomsByHotelIdAsync(int hotelId)
        {
            var rooms = await _context.Rooms
            .AsNoTracking()
            .Where(r => r.HotelId == hotelId)
            .ToListAsync();
            Console.WriteLine($"Rooms fetched from DB: {rooms.Count}");
            return rooms;

        }


        //  Update Room
        public async Task<Room?> UpdateRoomAsync(int id, Room room)
        {
            var existingRoom = await _context.Rooms.FindAsync(id);
            if (existingRoom == null)
            {
                throw new KeyNotFoundException($"Room with ID {id} not found.");
            }

            //  Check if HotelId exists before updating
            var hotelExists = await _context.Hotels.AnyAsync(h => h.Id == room.HotelId);
            if (!hotelExists)
            {
                throw new KeyNotFoundException($"Hotel with ID {room.HotelId} not found.");
            }

            existingRoom.RoomType = room.RoomType;
            existingRoom.PricePerNight = room.PricePerNight;
            existingRoom.IsAvailable = room.IsAvailable;
            existingRoom.HotelId = room.HotelId;

            await _context.SaveChangesAsync();

            return existingRoom;
        }


        //  Delete Room
        public async Task<bool> DeleteRoomAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                throw new KeyNotFoundException($"Room with ID {id} not found.");
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
