using MovieTicketBooking.DTOs.Screens;

namespace MovieTicketBooking.Services.Interfaces;

public interface IScreenService
{
    Task<IEnumerable<ScreenResponse>> GetAllAsync(int? theatreId);
    Task<ScreenResponse?> GetByIdAsync(int id);
    Task<(ScreenResponse? Response, string? ErrorMessage)> CreateAsync(CreateScreenRequest request);
    Task<bool> UpdateAsync(int id, UpdateScreenRequest request);
    Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
}