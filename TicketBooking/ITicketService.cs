using System.Collections.Generic;
using System.Threading.Tasks;

namespace TicketBooking.Services
{
    public interface ITicketService
    {
        Task<List<Event>> GetEventsAsync();
        Task<Event> CreateEventAsync(Event newEvent);
        Task<bool> DeleteEventAsync(int id);
        Task<Booking> BookTicketAsync(Booking booking);
        Task<bool> CancelBookingAsync(string reference);
        Task<Booking> GetBookingDetailsAsync(string reference);
    }
}
