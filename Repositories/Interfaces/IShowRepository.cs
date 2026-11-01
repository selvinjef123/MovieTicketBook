using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces;

public interface IShowRepository
{
    Task<IEnumerable<Show>> GetAllAsync(DateTime? date, int? movieId, int? theatreId);
    Task<Show?> GetByIdAsync(int id);
    Task<Show> CreateAsync(Show show);
    Task UpdateAsync(Show show);
    Task DeleteAsync(Show show);
    Task<bool> HasOverlappingShowAsync(int screenId, DateTime startTime, DateTime endTime, int? excludeShowId = null);
    Task<IEnumerable<ShowSeat>> GetShowSeatsAsync(int showId, SeatStatus? status = null);
    Task<bool> HasBookingsAsync(int showId);
}