using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;
    public BookingRepository(ApplicationDbContext context) => _context = context;

    public async Task<IEnumerable<Booking>> GetAllAsync(int? showId, BookingStatus? status)
    {
        var query = _context.Bookings
            .Include(b => b.Show).ThenInclude(s => s.Movie)
            .Include(b => b.Show).ThenInclude(s => s.Screen).ThenInclude(sc => sc.Theatre)
            .Include(b => b.BookingSeats).ThenInclude(bs => bs.Seat)
            .AsNoTracking();

        if (showId.HasValue)
            query = query.Where(b => b.ShowId == showId.Value);

        if (status.HasValue)
            query = query.Where(b => b.Status == status.Value);

        return await query.ToListAsync();
    }

    public async Task<Booking?> GetByIdAsync(int id) =>
        await _context.Bookings
            .Include(b => b.Show).ThenInclude(s => s.Movie)
            .Include(b => b.Show).ThenInclude(s => s.Screen).ThenInclude(sc => sc.Theatre)
            .Include(b => b.BookingSeats).ThenInclude(bs => bs.Seat)
            .FirstOrDefaultAsync(b => b.BookingId == id);

    public async Task<Booking> CreateBookingInTransactionAsync(Booking booking, List<ShowSeat> showSeatsToUpdate)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            foreach (var showSeat in showSeatsToUpdate)
            {
                showSeat.Status = SeatStatus.Booked;
                _context.ShowSeats.Update(showSeat);
            }
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return booking;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task CancelBookingInTransactionAsync(Booking booking, List<ShowSeat> showSeatsToRelease)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            booking.Status = BookingStatus.Cancelled;
            _context.Bookings.Update(booking);

            foreach (var showSeat in showSeatsToRelease)
            {
                showSeat.Status = SeatStatus.Available;
                _context.ShowSeats.Update(showSeat);
            }
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}