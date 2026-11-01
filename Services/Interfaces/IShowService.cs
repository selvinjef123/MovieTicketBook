using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Services.Interfaces;

public interface IShowService
{
    Task<IEnumerable<ShowResponse>> GetAllAsync(DateTime? date, int? movieId, int? theatreId);
    Task<ShowResponse?> GetByIdAsync(int id);
    Task<(ShowResponse? Response, string? ErrorMessage)> CreateAsync(CreateShowRequest request);
    Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, UpdateShowRequest request);
    Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    Task<IEnumerable<ShowSeatResponse>?> GetShowSeatsAsync(int showId, SeatStatus? status = null);
}