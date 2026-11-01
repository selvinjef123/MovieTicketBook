using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IShowRepository _showRepository;

    public BookingService(IBookingRepository bookingRepository, IShowRepository showRepository)
    {
        _bookingRepository = bookingRepository;
        _showRepository = showRepository;
    }

    public async Task<IEnumerable<BookingResponse>> GetAllAsync(int? showId, BookingStatus? status)
    {
        var bookings = await _bookingRepository.GetAllAsync(showId, status);
        return bookings.Select(MapToResponse);
    }

    public async Task<BookingResponse?> GetByIdAsync(int id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        return booking == null ? null : MapToResponse(booking);
    }

    public async Task<(BookingResponse? Response, string? ErrorMessage, int StatusCode)> CreateBookingAsync(CreateBookingRequest request)
    {
        var show = await _showRepository.GetByIdAsync(request.ShowId);
        if (show == null)
            return (null, $"Show with ID {request.ShowId} does not exist.", 404);

        var requestedSeatIds = request.SeatIds.Distinct().ToList();
        var showSeats = (await _showRepository.GetShowSeatsAsync(request.ShowId)).ToList();

        var matchedShowSeats = showSeats.Where(ss => requestedSeatIds.Contains(ss.SeatId)).ToList();

        if (matchedShowSeats.Count != requestedSeatIds.Count)
            return (null, "One or more selected seats do not exist for this show's screen.", 400);

        var unavailableSeats = matchedShowSeats.Where(ss => ss.Status != SeatStatus.Available).ToList();
        if (unavailableSeats.Count > 0)
        {
            var seatLabels = string.Join(", ", unavailableSeats.Select(s => $"{s.Seat.RowName}{s.Seat.SeatNumber}"));
            return (null, $"Seats are no longer available: {seatLabels}", 409);
        }

        decimal totalAmount = requestedSeatIds.Count * show.TicketPrice;
        string reference = $"MOV{Random.Shared.Next(100000, 999999)}";

        var booking = new Booking
        {
            BookingReference = reference,
            ShowId = request.ShowId,
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            CustomerPhone = request.CustomerPhone,
            BookingDate = DateTime.UtcNow,
            TotalAmount = totalAmount,
            Status = BookingStatus.Confirmed,
            BookingSeats = requestedSeatIds.Select(seatId => new BookingSeat
            {
                SeatId = seatId,
                Price = show.TicketPrice
            }).ToList()
        };

        var createdBooking = await _bookingRepository.CreateBookingInTransactionAsync(booking, matchedShowSeats);
        var fullBooking = await _bookingRepository.GetByIdAsync(createdBooking.BookingId);

        return (MapToResponse(fullBooking!), null, 201);
    }

    public async Task<(bool Success, string? ErrorMessage, int StatusCode)> CancelBookingAsync(int id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
            return (false, $"Booking with ID {id} not found.", 404);

        if (booking.Status == BookingStatus.Cancelled)
            return (false, "Booking is already cancelled.", 400);

        var bookedSeatIds = booking.BookingSeats.Select(bs => bs.SeatId).ToList();
        var showSeatsToRelease = (await _showRepository.GetShowSeatsAsync(booking.ShowId))
            .Where(ss => bookedSeatIds.Contains(ss.SeatId))
            .ToList();

        await _bookingRepository.CancelBookingInTransactionAsync(booking, showSeatsToRelease);
        return (true, null, 200);
    }

    private static BookingResponse MapToResponse(Booking b) =>
        new(
            b.BookingId,
            b.BookingReference,
            b.ShowId,
            b.Show?.Movie?.Title ?? string.Empty,
            b.Show?.Screen?.Theatre?.Name ?? string.Empty,
            b.Show?.Screen?.ScreenName ?? string.Empty,
            b.Show?.StartTime ?? DateTime.MinValue,
            b.CustomerName,
            b.CustomerEmail,
            b.CustomerPhone,
            b.BookingDate,
            b.TotalAmount,
            b.Status.ToString(),
            b.BookingSeats.Select(bs => $"{bs.Seat.RowName}{bs.Seat.SeatNumber}").ToList()
        );
}