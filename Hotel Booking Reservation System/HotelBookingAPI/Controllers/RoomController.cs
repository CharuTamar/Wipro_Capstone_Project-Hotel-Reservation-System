using Microsoft.AspNetCore.Mvc;
using HotelBookingAPI.Interfaces;
using HotelBookingAPI.Models;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    //  Add Room to Hotel (Admin only)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddRoom([FromBody] Room room)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var createdRoom = await _roomService.AddRoomAsync(room);
        return CreatedAtAction(nameof(GetRoomsByHotelId), new { hotelId = room.HotelId }, createdRoom);
    }


    //  Get Rooms by Hotel ID (Publicly Accessible)
    [HttpGet("{hotelId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRoomsByHotelId(int hotelId)
    {
        Console.WriteLine($"Fetching rooms for hotelId: {hotelId}");
        var rooms = await _roomService.GetRoomsByHotelIdAsync(hotelId);
        if (rooms == null || !rooms.Any())
        {
            return NotFound($"No rooms found for hotel with ID {hotelId}");
        }
        return Ok(rooms);
    }



    //  Update Room (Admin only)
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRoom(int id, [FromBody] Room room)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var updatedRoom = await _roomService.UpdateRoomAsync(id, room);
            return Ok(updatedRoom);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message }); // 👈 More descriptive 404 message
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }


    //  Delete Room (Admin only)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        try
        {
            var isDeleted = await _roomService.DeleteRoomAsync(id);
            if (isDeleted)
            {
                return Ok(new { message = $"Room with ID {id} deleted successfully." });
            }
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }

        return BadRequest(new { message = "Failed to delete the room." });
    }

}
