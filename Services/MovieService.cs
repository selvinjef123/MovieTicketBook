using MovieTicketBooking.DTOs.Movies;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _repository;
    public MovieService(IMovieRepository repository) => _repository = repository;

    public async Task<IEnumerable<MovieResponse>> GetAllAsync(int pageNumber, int pageSize, string? title)
    {
        var movies = await _repository.GetAllAsync(pageNumber, pageSize, title);
        return movies.Select(MapToResponse);
    }

    public async Task<MovieResponse?> GetByIdAsync(int id)
    {
        var movie = await _repository.GetByIdAsync(id);
        return movie == null ? null : MapToResponse(movie);
    }

    public async Task<MovieResponse> CreateAsync(CreateMovieRequest request)
    {
        var movie = new Movie
        {
            Title = request.Title,
            Description = request.Description,
            DurationMinutes = request.DurationMinutes,
            ReleaseDate = request.ReleaseDate,
            Language = request.Language,
            Genre = request.Genre
        };

        var created = await _repository.CreateAsync(movie);
        return MapToResponse(created);
    }

    public async Task<bool> UpdateAsync(int id, UpdateMovieRequest request)
    {
        var movie = await _repository.GetByIdAsync(id);
        if (movie == null) return false;

        movie.Title = request.Title;
        movie.Description = request.Description;
        movie.DurationMinutes = request.DurationMinutes;
        movie.ReleaseDate = request.ReleaseDate;
        movie.Language = request.Language;
        movie.Genre = request.Genre;

        await _repository.UpdateAsync(movie);
        return true;
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
    {
        var movie = await _repository.GetByIdAsync(id);
        if (movie == null) return (false, null);

        if (await _repository.HasShowsAsync(id))
            return (false, "Cannot delete movie because it has existing show schedules.");

        await _repository.DeleteAsync(movie);
        return (true, null);
    }

    private static MovieResponse MapToResponse(Movie m) =>
        new(m.MovieId, m.Title, m.Description, m.DurationMinutes, m.ReleaseDate, m.Language, m.Genre);
}