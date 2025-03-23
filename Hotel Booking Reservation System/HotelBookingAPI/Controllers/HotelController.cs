using HotelBookingAPI.Interfaces;
using HotelBookingAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        //  Get all hotels
        // Publically accessible
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetHotels()
        {
            var hotels = await _hotelService.GetAllHotelsAsync();
            return Ok(hotels);
        }

        //  Get hotel by ID
        // Publically accessible
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHotelById(int id)
        {
            var hotel = await _hotelService.GetHotelByIdAsync(id);
            if (hotel == null) return NotFound(new { Message = "Hotel not found!" });
            return Ok(hotel);
        }

        //  Add new hotel (Admin only)
        // [Authorize]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        // [AllowAnonymous]
        public async Task<IActionResult> AddHotel([FromBody] Hotel hotel)
        {
            var createdHotel = await _hotelService.AddHotelAsync(hotel);
            return CreatedAtAction(nameof(GetHotelById), new { id = createdHotel.Id }, createdHotel);
        }

        //  Update existing hotel (Admin only)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateHotel(int id, [FromBody] Hotel hotel)
        {
            var updatedHotel = await _hotelService.UpdateHotelAsync(id, hotel);
            if (updatedHotel == null) return NotFound(new { Message = "Hotel not found!" });
            return Ok(updatedHotel);
        }

        //  Delete hotel (Admin only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            var result = await _hotelService.DeleteHotelAsync(id);
            if (!result) return NotFound(new { Message = "Hotel not found!" });
            return NoContent(); // 204 status code
        }
    }
}
