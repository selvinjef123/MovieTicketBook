using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces;

public interface IBookingRepository
{
    Task<IEnumerable<Booking>> GetAllAsync(int? showId, BookingStatus? status);
    Task<Booking?> GetByIdAsync(int id);
    Task<Booking> CreateBookingInTransactionAsync(Booking booking, List<ShowSeat> showSeatsToUpdate);
    Task CancelBookingInTransactionAsync(Booking booking, List<ShowSeat> showSeatsToRelease);
}