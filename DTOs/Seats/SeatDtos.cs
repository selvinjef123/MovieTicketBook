using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Seats;

public record CreateSeatRequest(
    [Range(1, int.MaxValue)] int ScreenId,
    [Required, StringLength(5)] string RowName,
    [Range(1, 500)] int SeatNumber,
    [Required, StringLength(30)] string SeatType
);

public record BulkCreateSeatsRequest(
    [Range(1, int.MaxValue)] int ScreenId,
    [Required] List<string> RowNames,
    [Range(1, 100)] int SeatsPerRow,
    [Required, StringLength(30)] string SeatType
);

public record UpdateSeatRequest(
    [Required, StringLength(5)] string RowName,
    [Range(1, 500)] int SeatNumber,
    [Required, StringLength(30)] string SeatType
);

public record SeatResponse(
    int SeatId,
    int ScreenId,
    string RowName,
    int SeatNumber,
    string SeatType,
    string SeatLabel
);