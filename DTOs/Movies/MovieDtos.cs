using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Movies;

public record CreateMovieRequest(
    [Required, StringLength(150)] string Title,
    [StringLength(500)] string? Description,
    [Range(1, 1000)] int DurationMinutes,
    [Required] DateTime ReleaseDate,
    [Required, StringLength(50)] string Language,
    [Required, StringLength(50)] string Genre
);

public record UpdateMovieRequest(
    [Required, StringLength(150)] string Title,
    [StringLength(500)] string? Description,
    [Range(1, 1000)] int DurationMinutes,
    [Required] DateTime ReleaseDate,
    [Required, StringLength(50)] string Language,
    [Required, StringLength(50)] string Genre
);

public record MovieResponse(
    int MovieId,
    string Title,
    string? Description,
    int DurationMinutes,
    DateTime ReleaseDate,
    string Language,
    string Genre
);