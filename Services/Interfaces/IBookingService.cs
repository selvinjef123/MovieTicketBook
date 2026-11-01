using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Services.Interfaces;

public interface IBookingService
{
    Task<IEnumerable<BookingResponse>> GetAllAsync(int? showId, BookingStatus? status);
    Task<BookingResponse?> GetByIdAsync(int id);
    Task<(BookingResponse? Response, string? ErrorMessage, int StatusCode)> CreateBookingAsync(CreateBookingRequest request);
    Task<(bool Success, string? ErrorMessage, int StatusCode)> CancelBookingAsync(int id);
}