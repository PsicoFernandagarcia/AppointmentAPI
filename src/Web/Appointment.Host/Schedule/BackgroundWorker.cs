using Appointment.Application.SendEmailUseCase.Reminder;
using Appointment.Infrastructure.Configuration;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Host.Schedule
{
    public class BackgroundWorker : BackgroundService, IDisposable
    {
        private readonly IMediator _mediator;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BackgroundWorker> _logger;
        private PeriodicTimer _periodic;
        private Timer? _timer = null;
        private const int HOUR_TO_SEND_REMINDER_UTC = 6;
        private static DateTime DATETIME_TO_SEND_REMINDER = (new DateTime(DateTime.UtcNow.Year,
                                                                          DateTime.UtcNow.Month,
                                                                          DateTime.UtcNow.Day,
                                                                          HOUR_TO_SEND_REMINDER_UTC,
                                                                          0,
                                                                          0,
                                                                          0,
                                                                          DateTimeKind.Utc)).AddDays(1);
        public BackgroundWorker(IMediator mediator,
                                IHostApplicationLifetime lifetime,
                                IServiceScopeFactory scopeFactory,
                                ILogger<BackgroundWorker> logger)
        {
            _mediator = mediator;
            _lifetime = lifetime;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!await WaitForAppStartup(_lifetime, stoppingToken))
                return;

            var timeToWait = DATETIME_TO_SEND_REMINDER - DateTime.UtcNow;
            await Task.Delay(timeToWait, stoppingToken);
            await DoWork();

            _periodic = new PeriodicTimer(TimeSpan.FromHours(24));
            while (
                await _periodic.WaitForNextTickAsync(stoppingToken)
                && !stoppingToken.IsCancellationRequested
                )
            {
                await DoWork();
            }


        }
        static async Task<bool> WaitForAppStartup(IHostApplicationLifetime lifetime, CancellationToken stoppingToken)
        {
            var startedSource = new TaskCompletionSource();
            var canceledSource = new TaskCompletionSource();

            using var reg1 = lifetime.ApplicationStarted.Register(() => startedSource.SetResult());
            using var reg2 = stoppingToken.Register(() => canceledSource.SetResult());

            Task completedTask = await Task.WhenAny(
                startedSource.Task,
                canceledSource.Task).ConfigureAwait(false);

            // If the completed tasks was the "app started" task, return true, otherwise false
            return completedTask == startedSource.Task;
        }

        private async Task DoWork()
        {
            await SendReminderEmail();
        }

        private async Task SendReminderEmail()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var app = dbContext.Appointments
                .Where(ap =>
                            ap.DateFrom.Year == DateTime.Now.Year
                            && ap.DateFrom.Month == DateTime.Now.Month
                            && ap.DateFrom.Day == DateTime.Now.Day
                            && ap.Status != Domain.AppointmentStatus.CANCELED
                        )
                .OrderBy(ap => ap.DateFrom)
                .Include(ap => ap.Patient)
                .ToList();
            _logger.LogInformation($"EMAIL: Hour - {DateTime.UtcNow.ToLongDateString()} - Sending email to {app.Count} users");
            if (app is null || !app.Any()) return;
            await _mediator.Send(new SendReminderEmailCommand
            {
                HostEmail = app[0].Host.Email,
                HostName = app[0].Host.Name,
                Appointments = app
            });
        }

        private async Task AppointmentStatusTask()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var sqlCommand = @" UPDATE AppointmentDb.Appointments 
                                SET Status = 3,
                                UpdatedAt = NOW()
                                WHERE   DateTo < NOW() 
                                        AND Status = 0
                            ";
                await dbContext.Database.ExecuteSqlRawAsync(sqlCommand);

            }
        }


        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
