using MovieTicketBooking.DTOs.Theatres;

namespace MovieTicketBooking.Services.Interfaces;

public interface ITheatreService
{
    Task<IEnumerable<TheatreResponse>> GetAllAsync();
    Task<TheatreResponse?> GetByIdAsync(int id);
    Task<TheatreResponse> CreateAsync(CreateTheatreRequest request);
    Task<bool> UpdateAsync(int id, UpdateTheatreRequest request);
    Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
}