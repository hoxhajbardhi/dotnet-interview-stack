using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private static readonly Action<ILogger, string, Exception?> _handlingRequest = LoggerMessage.Define<string>(
        LogLevel.Information,
        new EventId(0, "HandlingRequest"),
        "Handling {RequestName}");

    private static readonly Action<ILogger, string, long, Exception?> _handledRequest = LoggerMessage.Define<string, long>(
        LogLevel.Information,
        new EventId(1, "HandledRequest"),
        "Handled {RequestName} in {ElapsedMilliseconds}ms");

    private static readonly Action<ILogger, string, long, Exception?> _failedRequest = LoggerMessage.Define<string, long>(
        LogLevel.Error,
        new EventId(2, "FailedHandlingRequest"),
        "Failed handling {RequestName} after {ElapsedMilliseconds}ms");

    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        _handlingRequest(_logger, requestName, null);

        try
        {
            var response = await next();

            stopwatch.Stop();
            _handledRequest(_logger, requestName, stopwatch.ElapsedMilliseconds, null);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _failedRequest(_logger, requestName, stopwatch.ElapsedMilliseconds, ex);
            throw;
        }
    }
}