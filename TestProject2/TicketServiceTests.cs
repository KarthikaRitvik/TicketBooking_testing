using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Internal.Execution;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketBooking.Services;

namespace TicketBooking.Tests
{
    public class TicketServiceTests
    {
        private ApplicationDbContext _context;
        private TicketService _ticketService;
        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TicketBookingTestDb")
                .Options;

            _context = new ApplicationDbContext(options);
            _ticketService = new TicketService(_context);
        }

        [Test]
        public async Task GetEvents_ShouldReturnEvents()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database for each test
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                // Create and add a single Event
                var testEvent = new Event
                {
                    EventName = "Test Event",
                    EventDate = DateTime.UtcNow.AddDays(7),
                    Venue = "Test Venue",
                    TotalSeats = 100,
                    AvailableSeats = 100
                };
                context.Events.Add(testEvent);
                await context.SaveChangesAsync();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var ticketService = new TicketService(context);

                // Act
                var events = await ticketService.GetEventsAsync();

                // Assert
                Assert.IsNotNull(events);
                Assert.AreEqual(1, events.Count); // Expecting 1 event
                Assert.AreEqual("Test Event", events[0].EventName); // Verify the event name
            }
        }



        [Test]
        public async Task CreateEvent_ShouldAddEvent()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                var ticketService = new TicketService(context);

                var newEvent = new Event
                {
                    EventName = "New Test Event",
                    EventDate = DateTime.UtcNow.AddDays(7),
                    Venue = "New Test Venue", // Ensure Venue is set
                    TotalSeats = 150,
                    AvailableSeats = 150 // Set AvailableSeats initially to TotalSeats
                };

                // Act
                await ticketService.CreateEventAsync(newEvent);
            }

            using (var context = new ApplicationDbContext(options))
            {
                // Assert
                var addedEvent = await context.Events.FirstOrDefaultAsync(e => e.EventName == "New Test Event");
                Assert.IsNotNull(addedEvent);
                Assert.AreEqual("New Test Venue", addedEvent.Venue); // Verify Venue is correctly set
                Assert.AreEqual(150, addedEvent.TotalSeats);
                Assert.AreEqual(150, addedEvent.AvailableSeats);
            }
        }


        [Test]
        public async Task BookTicket_ShouldBookTicket()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                // Create and add an Event with all required properties
                var testEvent = new Event
                {
                    EventName = "Test Event",
                    EventDate = DateTime.UtcNow.AddDays(7),
                    Venue = "Test Venue", // Ensure Venue is set
                    TotalSeats = 100,
                    AvailableSeats = 100
                };
                context.Events.Add(testEvent);
                await context.SaveChangesAsync();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var ticketService = new TicketService(context);

                // Set up a booking request
                var booking = new Booking
                {
                    EventId = 1, // Assuming this is the ID of the testEvent you added
                    UserEmail = "test@example.com",
                    NumberOfTickets = 2
                };

                // Act
                var result = await ticketService.BookTicketAsync(booking);

                // Assert
                Assert.IsNotNull(result);
                var bookedEvent = await context.Events.FindAsync(1);
                Assert.AreEqual(98, bookedEvent.AvailableSeats); // Verify available seats are updated
            }
        }

        // More tests can be added for CancelBooking, DeleteEvent, and GetBookingDetails...
    }
}
