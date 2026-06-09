using System.Net;
using System.Net.Http.Json;
using Api.IntegrationTests.Infrastructure;
using Application.Features.Users.Register;
using FluentAssertions;

namespace Api.IntegrationTests.Features.Users;

public sealed class RegisterUserEndpointTests
    : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;

    public RegisterUserEndpointTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static RegisterUserCommand ValidCommand() => new(
        Email: "bardh@bardh.dev",
        Password: "Test123!",
        FirstName: "Bardh",
        LastName: "Hoxhaj");

    [Fact(Skip = "Integration test requires Testcontainers database setup")]
    public async Task RegisterShouldReturn201WhenCommandIsValid()
    {
        // Arrange
        var command = ValidCommand();

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content
            .ReadFromJsonAsync<RegisterUserResponse>();

        result.Should().NotBeNull();
        result!.Email.Should().Be(command.Email.ToLowerInvariant());
        result.FirstName.Should().Be(command.FirstName);
        result.Id.Should().NotBeEmpty();
    }

    [Fact(Skip = "Integration test requires Testcontainers database setup")]
    public async Task RegisterShouldReturn400WhenEmailIsInvalid()
    {
        // Arrange
        var command = ValidCommand() with { Email = "not-an-email" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Integration test requires Testcontainers database setup")]
    public async Task RegisterShouldReturn409WhenEmailAlreadyExists()
    {
        // Arrange — regjistro herën e parë
        var command = ValidCommand() with { Email = "duplicate@bardh.dev" };
        await _client.PostAsJsonAsync("/api/users/register", command);

        // Act — regjistro me të njëjtin email
        var response = await _client.PostAsJsonAsync("/api/users/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}