using Appointment.Infrastructure.Configuration;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Host.Schedule
{
    public class BackgroundWorker : BackgroundService,IDisposable
    {
        private readonly IMediator _mediator;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer? _timer = null;
        public BackgroundWorker(IMediator mediator, IHostApplicationLifetime lifetime, IServiceScopeFactory scopeFactory)
        {
            _mediator = mediator;
            _lifetime = lifetime;
            _scopeFactory = scopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!await WaitForAppStartup(_lifetime, stoppingToken))
            {
                return;
            }
            var currentMinute = DateTime.Now.Minute;
            await Task.Delay(TimeSpan.FromSeconds((60-currentMinute) *60), stoppingToken);

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                                TimeSpan.FromMinutes(60)
                                );
        }
        static async Task<bool> WaitForAppStartup(IHostApplicationLifetime lifetime, CancellationToken stoppingToken)
        {
            var startedSource = new TaskCompletionSource();
            var cancelledSource = new TaskCompletionSource();

            using var reg1 = lifetime.ApplicationStarted.Register(() => startedSource.SetResult());
            using var reg2 = stoppingToken.Register(() => cancelledSource.SetResult());

            Task completedTask = await Task.WhenAny(
                startedSource.Task,
                cancelledSource.Task).ConfigureAwait(false);

            // If the completed tasks was the "app started" task, return true, otherwise false
            return completedTask == startedSource.Task;
        }

        private async void DoWork(object? state)
        {
            await AppointmentStatusTask();
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
