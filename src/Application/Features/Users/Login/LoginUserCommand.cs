using Application.Common.Entities;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Settings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.Features.Users.Login;

public sealed record LoginUserCommand(
    string Email,
    string Password) : IRequest<LoginUserResponse>;

public sealed class LoginUserCommandHandler
    : IRequestHandler<LoginUserCommand, LoginUserResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly JwtSettings _jwtSettings;

    public LoginUserCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginUserResponse> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Gjej user-in
        var user = await _context.Users
            .FirstOrDefaultAsync(
                u => EF.Functions.ILike(u.Email, request.Email),
                cancellationToken);

        // 2. Verifiko user + password (same error — timing attack prevention)
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure(
                    "Credentials",
                    "Invalid email or password.")
            });
        }

        // 3. Gjenero tokens
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
        var refreshTokenValue = _jwtTokenGenerator.GenerateRefreshToken();

        // 4. Ruaj refresh token
        var refreshToken = RefreshToken.Create(
            user.Id,
            refreshTokenValue,
            _jwtSettings.RefreshTokenExpirationDays);

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new LoginUserResponse(
            AccessToken: accessToken,
            RefreshToken: refreshTokenValue,
            AccessTokenExpiresAt: DateTime.UtcNow.AddMinutes(
                _jwtSettings.AccessTokenExpirationMinutes),
            UserId: user.Id,
            Email: user.Email);
    }
}