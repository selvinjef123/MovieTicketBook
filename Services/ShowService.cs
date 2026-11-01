using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class ShowService : IShowService
{
    private readonly IShowRepository _showRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly IScreenRepository _screenRepository;
    private readonly ISeatRepository _seatRepository;

    public ShowService(
        IShowRepository showRepository,
        IMovieRepository movieRepository,
        IScreenRepository screenRepository,
        ISeatRepository seatRepository)
    {
        _showRepository = showRepository;
        _movieRepository = movieRepository;
        _screenRepository = screenRepository;
        _seatRepository = seatRepository;
    }

    public async Task<IEnumerable<ShowResponse>> GetAllAsync(DateTime? date, int? movieId, int? theatreId)
    {
        var shows = await _showRepository.GetAllAsync(date, movieId, theatreId);
        return shows.Select(MapToResponse);
    }

    public async Task<ShowResponse?> GetByIdAsync(int id)
    {
        var show = await _showRepository.GetByIdAsync(id);
        return show == null ? null : MapToResponse(show);
    }

    public async Task<(ShowResponse? Response, string? ErrorMessage)> CreateAsync(CreateShowRequest request)
    {
        if (request.EndTime <= request.StartTime)
            return (null, "EndTime must be later than StartTime.");

        var movie = await _movieRepository.GetByIdAsync(request.MovieId);
        if (movie == null)
            return (null, $"Movie with ID {request.MovieId} does not exist.");

        var screen = await _screenRepository.GetByIdAsync(request.ScreenId);
        if (screen == null)
            return (null, $"Screen with ID {request.ScreenId} does not exist.");

        if (await _showRepository.HasOverlappingShowAsync(request.ScreenId, request.StartTime, request.EndTime))
            return (null, "Screen is already scheduled for an overlapping show during this timeframe.");

        var physicalSeats = (await _seatRepository.GetByScreenIdAsync(request.ScreenId)).ToList();
        if (physicalSeats.Count == 0)
            return (null, "Cannot schedule show: Screen has no configured physical seats.");

        var show = new Show
        {
            MovieId = request.MovieId,
            ScreenId = request.ScreenId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            TicketPrice = request.TicketPrice,
            ShowSeats = physicalSeats.Select(s => new ShowSeat
            {
                SeatId = s.SeatId,
                Status = SeatStatus.Available
            }).ToList()
        };

        var created = await _showRepository.CreateAsync(show);
        created.Movie = movie;
        created.Screen = screen;
        return (MapToResponse(created), null);
    }

    public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, UpdateShowRequest request)
    {
        if (request.EndTime <= request.StartTime)
            return (false, "EndTime must be later than StartTime.");

        var show = await _showRepository.GetByIdAsync(id);
        if (show == null) return (false, null);

        if (await _showRepository.HasOverlappingShowAsync(show.ScreenId, request.StartTime, request.EndTime, id))
            return (false, "Screen is already scheduled for an overlapping show during this timeframe.");

        show.StartTime = request.StartTime;
        show.EndTime = request.EndTime;
        show.TicketPrice = request.TicketPrice;

        await _showRepository.UpdateAsync(show);
        return (true, null);
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
    {
        var show = await _showRepository.GetByIdAsync(id);
        if (show == null) return (false, null);

        if (await _showRepository.HasBookingsAsync(id))
            return (false, "Cannot delete show that has existing customer bookings.");

        await _showRepository.DeleteAsync(show);
        return (true, null);
    }

    public async Task<IEnumerable<ShowSeatResponse>?> GetShowSeatsAsync(int showId, SeatStatus? status = null)
    {
        var show = await _showRepository.GetByIdAsync(showId);
        if (show == null) return null;

        var showSeats = await _showRepository.GetShowSeatsAsync(showId, status);
        return showSeats.Select(ss => new ShowSeatResponse(
            ss.SeatId,
            ss.Seat.RowName,
            ss.Seat.SeatNumber,
            $"{ss.Seat.RowName}{ss.Seat.SeatNumber}",
            ss.Seat.SeatType,
            show.TicketPrice,
            ss.Status.ToString()
        ));
    }

    private static ShowResponse MapToResponse(Show s) =>
        new(s.ShowId, s.MovieId, s.Movie?.Title ?? string.Empty, s.ScreenId, s.Screen?.ScreenName ?? string.Empty,
            s.Screen?.TheatreId ?? 0, s.Screen?.Theatre?.Name ?? string.Empty, s.StartTime, s.EndTime, s.TicketPrice);
}