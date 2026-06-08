namespace Application.Features.Users.Register;

public sealed record RegisterUserResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    DateTime CreatedAt);