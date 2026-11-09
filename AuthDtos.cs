using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.DTOs;

public record RegisterRequest(
    [Required, MaxLength(100)] string FullName,
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password,
    [Required] string Role);

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);

public record AuthResponse(
    int UserId,
    string FullName,
    string Email,
    string Role);