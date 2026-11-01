using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Models;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShowsController : ControllerBase
{
    private readonly IShowService _service;
    public ShowsController(IShowService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DateTime? date, [FromQuery] int? movieId, [FromQuery] int? theatreId) =>
        Ok(await _service.GetAllAsync(date, movieId, theatreId));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var show = await _service.GetByIdAsync(id);
        return show == null ? NotFound() : Ok(show);
    }

    [HttpGet("movie/{movieId:int}")]
    public async Task<IActionResult> GetByMovie(int movieId) =>
        Ok(await _service.GetAllAsync(null, movieId, null));

    [HttpGet("theatre/{theatreId:int}")]
    public async Task<IActionResult> GetByTheatre(int theatreId) =>
        Ok(await _service.GetAllAsync(null, null, theatreId));

    [HttpGet("search")]
    public async Task<IActionResult> SearchByDate([FromQuery] DateTime date) =>
        Ok(await _service.GetAllAsync(date, null, null));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateShowRequest request)
    {
        var (response, error) = await _service.CreateAsync(request);
        if (error != null) return BadRequest(new { message = error });
        return CreatedAtAction(nameof(GetById), new { id = response!.ShowId }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateShowRequest request)
    {
        var (success, error) = await _service.UpdateAsync(id, request);
        if (!success && error == null) return NotFound();
        if (!success) return BadRequest(new { message = error });
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, errorMessage) = await _service.DeleteAsync(id);
        if (!success && errorMessage == null) return NotFound();
        if (!success) return Conflict(new { message = errorMessage });
        return NoContent();
    }

    [HttpGet("{showId:int}/seats")]
    public async Task<IActionResult> GetSeatLayout(int showId)
    {
        var seats = await _service.GetShowSeatsAsync(showId);
        return seats == null ? NotFound() : Ok(seats);
    }

    [HttpGet("{showId:int}/seats/available")]
    public async Task<IActionResult> GetAvailableSeats(int showId)
    {
        var seats = await _service.GetShowSeatsAsync(showId, SeatStatus.Available);
        return seats == null ? NotFound() : Ok(seats);
    }

    [HttpGet("{showId:int}/seats/booked")]
    public async Task<IActionResult> GetBookedSeats(int showId)
    {
        var seats = await _service.GetShowSeatsAsync(showId, SeatStatus.Booked);
        return seats == null ? NotFound() : Ok(seats);
    }
}