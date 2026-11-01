using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class SeatService : ISeatService
{
    private readonly ISeatRepository _seatRepository;
    private readonly IScreenRepository _screenRepository;

    public SeatService(ISeatRepository seatRepository, IScreenRepository screenRepository)
    {
        _seatRepository = seatRepository;
        _screenRepository = screenRepository;
    }

    public async Task<IEnumerable<SeatResponse>> GetByScreenIdAsync(int screenId)
    {
        var seats = await _seatRepository.GetByScreenIdAsync(screenId);
        return seats.Select(MapToResponse);
    }

    public async Task<SeatResponse?> GetByIdAsync(int id)
    {
        var seat = await _seatRepository.GetByIdAsync(id);
        return seat == null ? null : MapToResponse(seat);
    }

    public async Task<(SeatResponse? Response, string? ErrorMessage)> CreateAsync(CreateSeatRequest request)
    {
        var screen = await _screenRepository.GetByIdAsync(request.ScreenId);
        if (screen == null)
            return (null, $"Screen with ID {request.ScreenId} does not exist.");

        if (await _seatRepository.ExistsAsync(request.ScreenId, request.RowName, request.SeatNumber))
            return (null, $"Seat {request.RowName}{request.SeatNumber} already exists on this screen.");

        var seat = new Seat
        {
            ScreenId = request.ScreenId,
            RowName = request.RowName.ToUpper(),
            SeatNumber = request.SeatNumber,
            SeatType = request.SeatType
        };

        var created = await _seatRepository.CreateAsync(seat);
        return (MapToResponse(created), null);
    }

    public async Task<(IEnumerable<SeatResponse>? Response, string? ErrorMessage)> BulkCreateAsync(BulkCreateSeatsRequest request)
    {
        var screen = await _screenRepository.GetByIdAsync(request.ScreenId);
        if (screen == null)
            return (null, $"Screen with ID {request.ScreenId} does not exist.");

        var seatsToCreate = new List<Seat>();
        foreach (var row in request.RowNames)
        {
            var cleanRow = row.Trim().ToUpper();
            for (int number = 1; number <= request.SeatsPerRow; number++)
            {
                if (!await _seatRepository.ExistsAsync(request.ScreenId, cleanRow, number))
                {
                    seatsToCreate.Add(new Seat
                    {
                        ScreenId = request.ScreenId,
                        RowName = cleanRow,
                        SeatNumber = number,
                        SeatType = request.SeatType
                    });
                }
            }
        }

        if (seatsToCreate.Count == 0)
            return (null, "No new seats were created because all requested seat combinations already exist.");

        await _seatRepository.CreateRangeAsync(seatsToCreate);
        return (seatsToCreate.Select(MapToResponse), null);
    }

    public async Task<bool> UpdateAsync(int id, UpdateSeatRequest request)
    {
        var seat = await _seatRepository.GetByIdAsync(id);
        if (seat == null) return false;

        seat.RowName = request.RowName.ToUpper();
        seat.SeatNumber = request.SeatNumber;
        seat.SeatType = request.SeatType;

        await _seatRepository.UpdateAsync(seat);
        return true;
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
    {
        var seat = await _seatRepository.GetByIdAsync(id);
        if (seat == null) return (false, null);

        await _seatRepository.DeleteAsync(seat);
        return (true, null);
    }

    private static SeatResponse MapToResponse(Seat s) =>
        new(s.SeatId, s.ScreenId, s.RowName, s.SeatNumber, s.SeatType, $"{s.RowName}{s.SeatNumber}");
}