using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class ShowRepository : IShowRepository
{
    private readonly ApplicationDbContext _context;
    public ShowRepository(ApplicationDbContext context) => _context = context;

    public async Task<IEnumerable<Show>> GetAllAsync(DateTime? date, int? movieId, int? theatreId)
    {
        var query = _context.Shows
            .Include(s => s.Movie)
            .Include(s => s.Screen)
            .ThenInclude(sc => sc.Theatre)
            .AsNoTracking();

        if (date.HasValue)
            query = query.Where(s => s.StartTime.Date == date.Value.Date);

        if (movieId.HasValue)
            query = query.Where(s => s.MovieId == movieId.Value);

        if (theatreId.HasValue)
            query = query.Where(s => s.Screen.TheatreId == theatreId.Value);

        return await query.ToListAsync();
    }

    public async Task<Show?> GetByIdAsync(int id) =>
        await _context.Shows
            .Include(s => s.Movie)
            .Include(s => s.Screen)
            .ThenInclude(sc => sc.Theatre)
            .FirstOrDefaultAsync(s => s.ShowId == id);

    public async Task<Show> CreateAsync(Show show)
    {
        _context.Shows.Add(show);
        await _context.SaveChangesAsync();
        return show;
    }

    public async Task UpdateAsync(Show show)
    {
        _context.Shows.Update(show);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Show show)
    {
        _context.Shows.Remove(show);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasOverlappingShowAsync(int screenId, DateTime startTime, DateTime endTime, int? excludeShowId = null)
    {
        var query = _context.Shows.Where(s => s.ScreenId == screenId && s.StartTime < endTime && s.EndTime > startTime);
        if (excludeShowId.HasValue)
            query = query.Where(s => s.ShowId != excludeShowId.Value);

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<ShowSeat>> GetShowSeatsAsync(int showId, SeatStatus? status = null)
    {
        var query = _context.ShowSeats
            .Include(ss => ss.Seat)
            .Where(ss => ss.ShowId == showId)
            .AsNoTracking();

        if (status.HasValue)
            query = query.Where(ss => ss.Status == status.Value);

        return await query.ToListAsync();
    }

    public async Task<bool> HasBookingsAsync(int showId) =>
        await _context.Bookings.AnyAsync(b => b.ShowId == showId);
}