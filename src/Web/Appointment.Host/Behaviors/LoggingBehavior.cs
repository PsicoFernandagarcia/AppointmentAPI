using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Host.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling {typeof(TRequest).Name}");
            var response = await next();
            if (response is Result result && result.IsFailed)
                _logger.LogError($"Handling {typeof(TRequest).Name} Error!", result);

            _logger.LogInformation($"Handled {typeof(TResponse).Name}");

            return response;
        }
    }
}