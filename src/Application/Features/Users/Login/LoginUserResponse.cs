namespace Application.Features.Users.Login;

public sealed record LoginUserResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    Guid UserId,
    string Email);