using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TicketBooking.Services;

namespace TicketBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet("events")]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _ticketService.GetEventsAsync();
            return Ok(events);
        }

        [HttpPost("events")]
        public async Task<IActionResult> CreateEvent([FromBody] Event newEvent)
        {
            if (newEvent == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid event data.");
            }

            var createdEvent = await _ticketService.CreateEventAsync(newEvent);
            return CreatedAtAction(nameof(GetEvents), new { id = createdEvent.Id }, createdEvent);
        }

        [HttpDelete("events/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var result = await _ticketService.DeleteEventAsync(id);
            return result ? NoContent() : NotFound();
        }

        [HttpPost("book")]
        public async Task<IActionResult> BookTicket([FromBody] Booking booking)
        {
            var bookedTicket = await _ticketService.BookTicketAsync(booking);
            if (bookedTicket == null)
            {
                return BadRequest("Invalid event or not enough available seats.");
            }

            return Ok(bookedTicket);
        }

        [HttpPut("cancel/{reference}")]
        public async Task<IActionResult> CancelBooking(string reference)
        {
            var result = await _ticketService.CancelBookingAsync(reference);
            return result ? Ok() : NotFound();
        }

        [HttpGet("booking/{reference}")]
        public async Task<IActionResult> GetBookingDetails(string reference)
        {
            var booking = await _ticketService.GetBookingDetailsAsync(reference);
            return booking == null ? NotFound() : Ok(booking);
        }
    }
}
