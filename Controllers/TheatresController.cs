using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Theatres;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TheatresController : ControllerBase
{
    private readonly ITheatreService _service;
    public TheatresController(ITheatreService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var theatre = await _service.GetByIdAsync(id);
        return theatre == null ? NotFound() : Ok(theatre);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTheatreRequest request)
    {
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.TheatreId }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTheatreRequest request)
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