using Application.Common.Interfaces;

namespace Application.UnitTests.Helpers;

/// <summary>
/// Fake password hasher for testing that allows configuring return values
/// and capturing calls without NSubstitute complications.
/// </summary>
internal sealed class FakePasswordHasher : IPasswordHasher
{
    public string NextHash { get; set; } = "fake_hash";
    public List<string> Calls { get; } = new();

    public string Hash(string password)
    {
        Calls.Add(password);
        return NextHash;
    }

    public bool Verify(string password, string passwordHash)
    {
        throw new NotImplementedException("Not needed for RegisterUserCommand tests");
    }
}
