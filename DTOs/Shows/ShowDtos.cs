using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Shows;

public record CreateShowRequest(
    [Range(1, int.MaxValue)] int MovieId,
    [Range(1, int.MaxValue)] int ScreenId,
    [Required] DateTime StartTime,
    [Required] DateTime EndTime,
    [Range(0.0, 100000.0)] decimal TicketPrice
);

public record UpdateShowRequest(
    [Required] DateTime StartTime,
    [Required] DateTime EndTime,
    [Range(0.0, 100000.0)] decimal TicketPrice
);

public record ShowResponse(
    int ShowId,
    int MovieId,
    string MovieTitle,
    int ScreenId,
    string ScreenName,
    int TheatreId,
    string TheatreName,
    DateTime StartTime,
    DateTime EndTime,
    decimal TicketPrice
);

public record ShowSeatResponse(
    int SeatId,
    string RowName,
    int SeatNumber,
    string SeatLabel,
    string SeatType,
    decimal Price,
    string Status
);