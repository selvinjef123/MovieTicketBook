using MovieTicketBooking.DTOs.Seats;

namespace MovieTicketBooking.Services.Interfaces;

public interface ISeatService
{
    Task<IEnumerable<SeatResponse>> GetByScreenIdAsync(int screenId);
    Task<SeatResponse?> GetByIdAsync(int id);
    Task<(SeatResponse? Response, string? ErrorMessage)> CreateAsync(CreateSeatRequest request);
    Task<(IEnumerable<SeatResponse>? Response, string? ErrorMessage)> BulkCreateAsync(BulkCreateSeatsRequest request);
    Task<bool> UpdateAsync(int id, UpdateSeatRequest request);
    Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
}