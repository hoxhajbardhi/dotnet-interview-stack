using Application.Common.Exceptions;
using MediatR;

namespace Application.Features.Health;

public sealed record ThrowTestQuery(string Kind) : IRequest<string>;

public sealed class ThrowTestQueryHandler : IRequestHandler<ThrowTestQuery, string>
{
    public Task<string> Handle(ThrowTestQuery request, CancellationToken cancellationToken)
    {
        return request.Kind switch
        {
            "notfound" => throw new NotFoundException("User", Guid.NewGuid()),
            "conflict" => throw new ConflictException("Email already exists."),
            "boom" => throw new InvalidOperationException("Something internal broke."),
            _ => Task.FromResult("no exception")
        };
    }
}