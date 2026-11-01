using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScreensController : ControllerBase
{
    private readonly IScreenService _service;
    public ScreensController(IScreenService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? theatreId) =>
        Ok(await _service.GetAllAsync(theatreId));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var screen = await _service.GetByIdAsync(id);
        return screen == null ? NotFound() : Ok(screen);
    }

    [HttpGet("theatre/{theatreId:int}")]
    public async Task<IActionResult> GetByTheatre(int theatreId) =>
        Ok(await _service.GetAllAsync(theatreId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScreenRequest request)
    {
        var (response, error) = await _service.CreateAsync(request);
        if (error != null) return BadRequest(new { message = error });
        return CreatedAtAction(nameof(GetById), new { id = response!.ScreenId }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateScreenRequest request)
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