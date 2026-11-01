using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Screens;

public record CreateScreenRequest(
    [Range(1, int.MaxValue)] int TheatreId,
    [Required, StringLength(50)] string ScreenName
);

public record UpdateScreenRequest(
    [Required, StringLength(50)] string ScreenName
);

public record ScreenResponse(
    int ScreenId,
    int TheatreId,
    string ScreenName,
    string TheatreName
);