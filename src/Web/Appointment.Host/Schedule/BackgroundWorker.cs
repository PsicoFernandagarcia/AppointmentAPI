using Appointment.Application.AppointmentUseCases.AddAppointmentByHost;
using Appointment.Application.AvailabilityUseCases.CreateAvailability;
using Appointment.Application.SendEmailUseCase.EndOfMonthMissingPayments;
using Appointment.Application.SendEmailUseCase.Reminder;
using Appointment.Application.UsersUseCase.GetUserByRole;
using Appointment.Domain;
using Appointment.Domain.Entities;
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
        private readonly IServiceAccountSingleton _serviceAccountSingleton;
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
                                ILogger<BackgroundWorker> logger,
                                IServiceAccountSingleton serviceAccountSingleton)
        {
            _mediator = mediator;
            _lifetime = lifetime;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _serviceAccountSingleton = serviceAccountSingleton;
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
            await AssignAppointmentsFromCalendar();
            await SendReminderEmail();
            if(DateTime.UtcNow.Day == DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month))
            {
                await SendPaymentEndOfMonthEmail();
            }
        }

        private async Task SendReminderEmail()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var app = dbContext.Appointments
                .Include(ap => ap.Patient)
                .Include(ap => ap.Host)
                .Where(ap =>
                            ap.DateFrom.Year == DateTime.Now.Year
                            && ap.DateFrom.Month == DateTime.Now.Month
                            && ap.DateFrom.Day == DateTime.Now.Day
                            && ap.Status != Domain.AppointmentStatus.CANCELED
                        )
                .OrderBy(ap => ap.DateFrom)
                .ToList();
            _logger.LogInformation($"EMAIL: Hour - {DateTime.UtcNow.ToLongDateString()} - Sending email to {app.Count} users");
            if (app is null || !app.Any()) return;
            await _mediator.Send(new SendReminderEmailCommand
            {
                HostEmail = app[0].Host.Email,
                HostName = app[0].Host.Name,
                Appointments = app
            });
            _logger.LogInformation($"Reminder email sent");

        }

        private async Task SendPaymentEndOfMonthEmail()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var missingPayments = dbContext.Appointments
                .Include(ap => ap.Patient)
                .Where(ap =>
                             ap.DateFrom.Year == DateTime.Now.Year
                             && ap.DateFrom.Month == DateTime.Now.Month
                             && ap.Status != Domain.AppointmentStatus.CANCELED
                            && ap.PaymentId == null
                        )
                .Select(ap => new
                MissingPayment(
                    $"{ap.Patient.Name} {ap.Patient.LastName}",
                    ap.DateFrom
                ))
                .ToList();
            _logger.LogInformation($"LAST MONTH EMAIL: Hour - {DateTime.UtcNow.ToLongDateString()} - Sending email with {missingPayments.Count} missing payments");
            if (missingPayments is null || !missingPayments.Any()) return;
            var host = dbContext.Users.Where(u => u.Roles.Any(r => r.Name == RolesEnum.HOST.ToString())).First();
            await _mediator.Send(new SendMissingPaymentsEmailCommand
            {
                Host = host,
                MissingPayments = missingPayments,
            });
            _logger.LogInformation($"Missing Payments Email sent");
        }

        private async Task AssignAppointmentsFromCalendar()
        {
            using var scope = _scopeFactory.CreateScope();
            var _mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var events = await _serviceAccountSingleton.GetNext30DaysEvents();
            var host = await _mediator.Send(new GetUserByRoleQuery
            {
                Role = Domain.RolesEnum.HOST
            });
            foreach (var e in events)
            {
                try
                {
                    if (!e.Summary.Contains("no en sistema", StringComparison.InvariantCultureIgnoreCase)) continue;
                    e.Summary = e.Summary.Replace("no en sistema", "", StringComparison.InvariantCultureIgnoreCase).Trim();
                    var names = e.Summary.Split(' ');
                    var user = (await GetUser(names[0], names[1])) ?? (await GetUser(names[1], names[0]));
                    if (user is null) continue;
                    var date = DateTime.Parse(e.Start.DateTimeRaw);
                    var appointmentDate = date.AddMinutes(date.Minute * -1);
                    var createAvailabilityResult = await _mediator.Send(new CreateAvailabilityCommand
                    {
                        HostId = host.Value.First().Id,
                        AmountOfTime = 60,
                        DateOfAvailability = appointmentDate
                    });
                    _logger.LogInformation($"Trying to create availability at {appointmentDate}");
                    Availability availability;
                    if (createAvailabilityResult.IsFailure)
                    {
                        _logger.LogInformation($"Error creating availability:  {createAvailabilityResult.Error.Message}. Trying to get existing");
                        availability = await GetAvailability(appointmentDate);
                        if (availability is null || !availability.IsEmpty)
                        {
                            var message = availability is null ? "There is no availability present..." : "Availability empty status: " + availability.IsEmpty;
                            _logger.LogInformation(message);
                            continue;
                        }
                    }
                    else
                    {
                        availability = createAvailabilityResult.Value;
                    }


                    _logger.LogInformation($"Trying to create appointment for user {user.Email}");
                    var appointmentCreated = await _mediator.Send(new CreateAppointmentByHostCommand
                    {
                        AvailabilityId = availability.Id,
                        DateFrom = appointmentDate,
                        DateTo = appointmentDate.AddMinutes(60),
                        HostId = host.Value.First().Id,
                        PatientId = user.Id,
                        PatientEmail = user.Email,
                        PatientName = user.Name,
                        Title = "Nuevo turno con " + user.Name,
                        TimezoneOffset = user.TimezoneOffset
                    });
                    if (appointmentCreated.IsSuccess)
                    {
                        _logger.LogInformation($"Appointment created");
                        e.Summary = $"{user.Name} {user.LastName} {user.Email}";
                        await _serviceAccountSingleton.UpdateEvent(e);
                    }
                    else
                    {
                        _logger.LogInformation($"Could not create appointment. Error: {appointmentCreated.Error.Message}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error trying to update No En sistema events. Description {Description}", ex.Message);
                }

            }

        }

        private async Task<Availability?> GetAvailability(DateTime date)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var availabilities = await dbContext.Availabilities
                            .Where(a => a.DateOfAvailability >= date.AddMinutes(-5)
                                        && a.DateOfAvailability <= date.AddMinutes(5)
                            )
                            .ToListAsync();
                return availabilities.Count() == 1 ? availabilities.First() : null;
            }
        }
        private async Task<User?> GetUser(string name, string lastName)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var users = await dbContext.Users.Where(u =>
                            (EF.Functions.Like(u.Name, $"%{name}%") || EF.Functions.Like(u.Name, $"%{lastName}%"))
                            && (EF.Functions.Like(u.LastName, $"%{name}%") || EF.Functions.Like(u.LastName, $"%{lastName}%"))
                            )
                            .ToListAsync();
                return users.Count() == 1 ? users.First() : null;
            }
        }



        override
        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        override
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
