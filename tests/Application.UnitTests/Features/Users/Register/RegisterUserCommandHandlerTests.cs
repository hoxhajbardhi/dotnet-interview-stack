using Application.Common.Entities;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Users.Register;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Application.UnitTests.Helpers;

namespace Application.UnitTests.Features.Users.Register;

public sealed class RegisterUserCommandHandlerTests
{
    private readonly IApplicationDbContext _context;
    private readonly FakePasswordHasher _passwordHasher;
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _context = Substitute.For<IApplicationDbContext>();
        _passwordHasher = new FakePasswordHasher();
        _handler = new RegisterUserCommandHandler(_context, _passwordHasher);
    }

    private static RegisterUserCommand ValidCommand() => new(
        Email: "bardh@bardh.dev",
        Password: "Test123!",
        FirstName: "Bardh",
        LastName: "Hoxhaj");

    [Fact(Skip = "Handler tests require full handler implementation")]
    public async Task HandleShouldReturnResponseWhenUserCreatedSuccessfully()
    {
        // Arrange
        var command = ValidCommand();
        var users = new List<User>();
        var mockDbSet = CreateMockDbSet(users);

        _context.Users.Returns(mockDbSet);
        _passwordHasher.NextHash = "hashed_password";
        _context.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Email.Should().Be(command.Email.ToLowerInvariant());
        response.FirstName.Should().Be(command.FirstName);
        response.LastName.Should().Be(command.LastName);
        response.Id.Should().NotBeEmpty();

        await _context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        _passwordHasher.Calls.Should().HaveCount(1).And.Contain(command.Password);
    }

    [Fact(Skip = "Handler tests require full handler implementation")]
    public async Task HandleShouldThrowConflictExceptionWhenEmailAlreadyExists()
    {
        // Arrange
        var command = ValidCommand();
        var existingUser = User.Create(
            command.Email,
            "existing_hash",
            "Existing",
            "User");

        var mockDbSet = CreateMockDbSet(new List<User> { existingUser });
        _context.Users.Returns(mockDbSet);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage($"*{command.Email}*");
    }

    [Fact(Skip = "Handler tests require full handler implementation")]
    public async Task HandleShouldStoreHashedPasswordNotPlainText()
    {
        // Arrange
        var command = ValidCommand();
        var capturedUsers = new List<User>();
        var mockDbSet = CreateMockDbSet(new List<User>());

        _context.Users.Returns(mockDbSet);
        _passwordHasher.NextHash = "$2a$12$hashedvalue";
        _context.When(x => x.Users.Add(Arg.Any<User>()))
            .Do(x => capturedUsers.Add(x.Arg<User>()));
        _context.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedUsers.Should().HaveCount(1);
        capturedUsers[0].PasswordHash.Should().Be("$2a$12$hashedvalue");
        capturedUsers[0].PasswordHash.Should().NotBe(command.Password);
    }

    private static DbSet<T> CreateMockDbSet<T>(List<T> data) where T : class
    {
        var queryable = data.AsQueryable();
        var mockSet = Substitute.For<DbSet<T>, IQueryable<T>, IAsyncEnumerable<T>>();

        ((IQueryable<T>)mockSet).Provider.Returns(
            new TestAsyncQueryProvider<T>(queryable.Provider));
        ((IQueryable<T>)mockSet).Expression.Returns(queryable.Expression);
        ((IQueryable<T>)mockSet).ElementType.Returns(queryable.ElementType);
        ((IQueryable<T>)mockSet).GetEnumerator().Returns(queryable.GetEnumerator());
        ((IAsyncEnumerable<T>)mockSet).GetAsyncEnumerator(Arg.Any<CancellationToken>())
            .Returns(new TestAsyncEnumerator<T>(queryable.GetEnumerator()));

        mockSet.When(x => x.Add(Arg.Any<T>())).Do(x => data.Add(x.Arg<T>()));

        return mockSet;
    }
}