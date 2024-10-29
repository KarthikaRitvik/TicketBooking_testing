public class Booking
{
    public int? Id { get; set; }
    public int EventId { get; set; }
    public string? UserEmail { get; set; }
    public int NumberOfTickets { get; set; }
    public string? BookingReference { get; set; }

    //public virtual Event Event { get; set; }
}
