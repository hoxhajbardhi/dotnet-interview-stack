using Application.Features.Users.Register;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Features.Users.Register;

public sealed class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommandValidator _validator = new();

    private static RegisterUserCommand ValidCommand() => new(
        Email: "bardh@bardh.dev",
        Password: "Test123!",
        FirstName: "Bardh",
        LastName: "Hoxhaj");

    [Fact]
    public void ShouldPassWhenCommandIsValid()
    {
        var result = _validator.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("notanemail")]
    [InlineData("missing@")]
    [InlineData("@nodomain.com")]
    public void ShouldFailWhenEmailIsInvalid(string email)
    {
        var command = ValidCommand() with { Email = email };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void ShouldFailWhenEmailExceedsMaxLength()
    {
        var longEmail = $"{new string('a', 250)}@b.com";
        var command = ValidCommand() with { Email = longEmail };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("short")]
    [InlineData("nouppercase1")]
    [InlineData("NOLOWERCASE1")]
    [InlineData("NoNumbers!")]
    public void ShouldFailWhenPasswordIsInvalid(string password)
    {
        var command = ValidCommand() with { Password = password };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("")]
    public void ShouldFailWhenFirstNameIsEmpty(string firstName)
    {
        var command = ValidCommand() with { FirstName = firstName };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Theory]
    [InlineData("")]
    public void ShouldFailWhenLastNameIsEmpty(string lastName)
    {
        var command = ValidCommand() with { LastName = lastName };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }
}