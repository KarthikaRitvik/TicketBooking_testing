using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TicketBooking.Services
{
    public class TicketService : ITicketService
    {
        private readonly ApplicationDbContext _context;

        public TicketService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Event>> GetEventsAsync()
        {
            return await _context.Events.ToListAsync();
        }

        public async Task<Event> CreateEventAsync(Event newEvent)
        {
            newEvent.AvailableSeats = newEvent.TotalSeats;
            await _context.Events.AddAsync(newEvent);
            await _context.SaveChangesAsync();
            return newEvent;
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            var eventToDelete = await _context.Events.FindAsync(id);
            if (eventToDelete == null)
            {
                return false;
            }

            _context.Events.Remove(eventToDelete);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Booking> BookTicketAsync(Booking booking)
        {
            var eventToBook = await _context.Events.FindAsync(booking.EventId);
            if (eventToBook == null || booking.NumberOfTickets > eventToBook.AvailableSeats)
            {
                return null; // Invalid booking
            }

            eventToBook.AvailableSeats -= booking.NumberOfTickets;
            Booking book = new Booking()
            {
                EventId = booking.EventId,
                UserEmail = booking.UserEmail,
                NumberOfTickets = booking.NumberOfTickets,
                BookingReference = Guid.NewGuid().ToString()
            };

            await _context.Bookings.AddAsync(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<bool> CancelBookingAsync(string reference)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingReference == reference);
            if (booking == null) return false;

            var eventToRestore = await _context.Events.FindAsync(booking.EventId);
            eventToRestore.AvailableSeats += booking.NumberOfTickets;

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();  
            return true;
        }

        public async Task<Booking> GetBookingDetailsAsync(string reference)
        {
            return await _context.Bookings.FirstOrDefaultAsync(b => b.BookingReference == reference);
        }
    }
}
