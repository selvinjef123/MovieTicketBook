using MovieTicketBooking.DTOs.Theatres;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class TheatreService : ITheatreService
{
    private readonly ITheatreRepository _repository;
    public TheatreService(ITheatreRepository repository) => _repository = repository;

    public async Task<IEnumerable<TheatreResponse>> GetAllAsync()
    {
        var theatres = await _repository.GetAllAsync();
        return theatres.Select(MapToResponse);
    }

    public async Task<TheatreResponse?> GetByIdAsync(int id)
    {
        var theatre = await _repository.GetByIdAsync(id);
        return theatre == null ? null : MapToResponse(theatre);
    }

    public async Task<TheatreResponse> CreateAsync(CreateTheatreRequest request)
    {
        var theatre = new Theatre
        {
            Name = request.Name,
            Location = request.Location
        };
        var created = await _repository.CreateAsync(theatre);
        return MapToResponse(created);
    }

    public async Task<bool> UpdateAsync(int id, UpdateTheatreRequest request)
    {
        var theatre = await _repository.GetByIdAsync(id);
        if (theatre == null) return false;

        theatre.Name = request.Name;
        theatre.Location = request.Location;
        await _repository.UpdateAsync(theatre);
        return true;
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
    {
        var theatre = await _repository.GetByIdAsync(id);
        if (theatre == null) return (false, null);

        if (await _repository.HasScreensAsync(id))
            return (false, "Cannot delete theatre containing active screens.");

        await _repository.DeleteAsync(theatre);
        return (true, null);
    }

    private static TheatreResponse MapToResponse(Theatre t) =>
        new(t.TheatreId, t.Name, t.Location);
}