using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly ApplicationDbContext _context;
    public MovieRepository(ApplicationDbContext context) => _context = context;

    public async Task<IEnumerable<Movie>> GetAllAsync(int pageNumber, int pageSize, string? title)
    {
        var query = _context.Movies.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(m => m.Title.Contains(title));

        return await query.OrderBy(m => m.MovieId)
                          .Skip((pageNumber - 1) * pageSize)
                          .Take(pageSize)
                          .ToListAsync();
    }

    public async Task<Movie?> GetByIdAsync(int id) =>
        await _context.Movies.FindAsync(id);

    public async Task<Movie> CreateAsync(Movie movie)
    {
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();
        return movie;
    }

    public async Task UpdateAsync(Movie movie)
    {
        _context.Movies.Update(movie);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Movie movie)
    {
        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasShowsAsync(int movieId) =>
        await _context.Shows.AnyAsync(s => s.MovieId == movieId);
}