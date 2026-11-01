using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces;

public interface IScreenRepository
{
    Task<IEnumerable<Screen>> GetAllAsync(int? theatreId);
    Task<Screen?> GetByIdAsync(int id);
    Task<Screen> CreateAsync(Screen screen);
    Task UpdateAsync(Screen screen);
    Task DeleteAsync(Screen screen);
    Task<bool> HasSeatsOrShowsAsync(int screenId);
}