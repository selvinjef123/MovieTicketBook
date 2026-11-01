using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class ScreenRepository : IScreenRepository
{
    private readonly ApplicationDbContext _context;
    public ScreenRepository(ApplicationDbContext context) => _context = context;

    public async Task<IEnumerable<Screen>> GetAllAsync(int? theatreId)
    {
        var query = _context.Screens.Include(s => s.Theatre).AsNoTracking();
        if (theatreId.HasValue)
            query = query.Where(s => s.TheatreId == theatreId.Value);

        return await query.ToListAsync();
    }

    public async Task<Screen?> GetByIdAsync(int id) =>
        await _context.Screens.Include(s => s.Theatre).FirstOrDefaultAsync(s => s.ScreenId == id);

    public async Task<Screen> CreateAsync(Screen screen)
    {
        _context.Screens.Add(screen);
        await _context.SaveChangesAsync();
        return screen;
    }

    public async Task UpdateAsync(Screen screen)
    {
        _context.Screens.Update(screen);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Screen screen)
    {
        _context.Screens.Remove(screen);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasSeatsOrShowsAsync(int screenId) =>
        await _context.Seats.AnyAsync(s => s.ScreenId == screenId) ||
        await _context.Shows.AnyAsync(s => s.ScreenId == screenId);
}