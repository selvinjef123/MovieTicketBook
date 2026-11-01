using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Theatres;

public record CreateTheatreRequest(
    [Required, StringLength(150)] string Name,
    [Required, StringLength(200)] string Location
);

public record UpdateTheatreRequest(
    [Required, StringLength(150)] string Name,
    [Required, StringLength(200)] string Location
);

public record TheatreResponse(
    int TheatreId,
    string Name,
    string Location
);