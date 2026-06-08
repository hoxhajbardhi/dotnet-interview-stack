using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace Application.Features.Users.Register;

public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName) : IRequest<RegisterUserResponse>;

public sealed class RegisterUserCommandHandler
    : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterUserResponse> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Check email uniqueness (use normalized lowercase comparison so EF can translate to SQL)
        // Use PostgreSQL ILIKE via EF.Functions for a translatable, case-insensitive comparison
        var normalizedEmail = request.Email;
        var emailExists = await _context.Users
            .AnyAsync(u => EF.Functions.ILike(u.Email, normalizedEmail),
                cancellationToken);

        if (emailExists)
        {
            throw new ConflictException($"Email '{request.Email}' is already registered.");
        }

        // 2. Hash password
        var passwordHash = _passwordHasher.Hash(request.Password);

        // 3. Create user via factory method
        var user = User.Create(
            request.Email,
            passwordHash,
            request.FirstName,
            request.LastName);

        // 4. Save
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        // 5. Return response
        return new RegisterUserResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.CreatedAt);
    }
}