using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Bookings;

public record CreateBookingRequest(
    [Required, StringLength(100)] string CustomerName,
    [Required, EmailAddress, StringLength(150)] string CustomerEmail,
    [Required, Phone, StringLength(15)] string CustomerPhone,
    [Range(1, int.MaxValue)] int ShowId,
    [Required, MinLength(1)] List<int> SeatIds
);

public record BookingResponse(
    int BookingId,
    string BookingReference,
    int ShowId,
    string MovieTitle,
    string TheatreName,
    string ScreenName,
    DateTime ShowStartTime,
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    DateTime BookingDate,
    decimal TotalAmount,
    string Status,
    List<string> Seats
);