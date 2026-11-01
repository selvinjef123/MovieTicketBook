using MovieTicketBooking.DTOs.Movies;

namespace MovieTicketBooking.Services.Interfaces;

public interface IMovieService
{
    Task<IEnumerable<MovieResponse>> GetAllAsync(int pageNumber, int pageSize, string? title);
    Task<MovieResponse?> GetByIdAsync(int id);
    Task<MovieResponse> CreateAsync(CreateMovieRequest request);
    Task<bool> UpdateAsync(int id, UpdateMovieRequest request);
    Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
}