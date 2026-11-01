using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeatsController : ControllerBase
{
    private readonly ISeatService _service;
    public SeatsController(ISeatService service) => _service = service;

    [HttpGet("screen/{screenId:int}")]
    public async Task<IActionResult> GetByScreen(int screenId) =>
        Ok(await _service.GetByScreenIdAsync(screenId));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var seat = await _service.GetByIdAsync(id);
        return seat == null ? NotFound() : Ok(seat);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSeatRequest request)
    {
        var (response, error) = await _service.CreateAsync(request);
        if (error != null) return BadRequest(new { message = error });
        return CreatedAtAction(nameof(GetById), new { id = response!.SeatId }, response);
    }

    [HttpPost("bulk")]
    public async Task<IActionResult> BulkCreate([FromBody] BulkCreateSeatsRequest request)
    {
        var (response, error) = await _service.BulkCreateAsync(request);
        if (error != null) return BadRequest(new { message = error });
        return Ok(response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSeatRequest request)
    {
        var updated = await _service.UpdateAsync(id, request);
        return updated ? Ok() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, errorMessage) = await _service.DeleteAsync(id);
        if (!success && errorMessage == null) return NotFound();
        if (!success) return Conflict(new { message = errorMessage });
        return NoContent();
    }
}