using System.Security.Claims;
using HotelBookingAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requires authentication
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        //  Create Booking (Registered User Only)
        [HttpPost("{roomId}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateBooking(int roomId, [FromBody] Booking booking)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            booking.UserId = userId;
            booking.RoomId = roomId; //  Set RoomId from route parameter

            try
            {
                var createdBooking = await _bookingService.CreateBookingAsync(booking);
                return CreatedAtAction(nameof(GetBookingById), new { id = createdBooking.Id }, createdBooking);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //  Get Booking by ID (Registered User or Admin)
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            // 🔒 Only admin or owner of the booking can access it
            if (!isAdmin && booking.UserId != userId)
                return Forbid();

            return Ok(booking);
        }

        //  Get All Bookings (Admin Only)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(bookings);
        }

        //  Get Bookings by User (Registered User Only)
        [HttpGet("user")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetBookingsByUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);

            return Ok(bookings);
        }

        //  Update Booking (Registered User can only update check-in/out dates)
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] Booking updatedBooking)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            try
            {
                var booking = await _bookingService.UpdateBookingAsync(id, updatedBooking, userId, isAdmin);
                if (booking == null) return NotFound();

                return Ok(booking);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        //  Cancel Booking (Registered User or Admin)
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            try
            {
                var success = await _bookingService.CancelBookingAsync(id, userId, isAdmin);
                if (!success) return NotFound();

                return NoContent(); // 204 - Success with no content
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid(); // 403 - Forbidden
            }
        }
    }
}
