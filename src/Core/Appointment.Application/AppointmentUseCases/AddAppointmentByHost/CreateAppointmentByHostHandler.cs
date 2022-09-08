using Appointment.Application.AppointmentUseCases.AddAppointment;
using Appointment.Application.AuthUseCases.CreateUser;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.AppointmentUseCases.AddAppointmentByHost
{
    public class CreateAppointmentByHostHandler : IRequestHandler<CreateAppointmentByHostCommand, Result<Domain.Entities.Appointment, ResultError>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        public CreateAppointmentByHostHandler(IMediator mediator, IUserRepository userRepository)
        {
            _mediator = mediator;
            _userRepository = userRepository;
        }

        public async Task<Result<Domain.Entities.Appointment, ResultError>> Handle(CreateAppointmentByHostCommand request, CancellationToken cancellationToken)
        {
            if (request.PatientId == 0)
            {
                var patientIdResult = await CreatePatientIfNoExistsAndGetId(request, cancellationToken);
                if (!patientIdResult.IsSuccess) return Result.Failure<Domain.Entities.Appointment, ResultError>(patientIdResult.Error);
                request.PatientId = patientIdResult.Value;
            }
            return await _mediator.Send(new CreateAppointmentCommand
            {
                CreatedById = request.HostId,
                HostId = request.HostId,
                AvailabilityId = request.AvailabilityId,
                Color = request.Color,
                DateFrom = request.DateFrom,
                DateTo = request.DateTo,
                LocalDateTime = request.LocalDateTime,
                PatientId = request.PatientId,
                Title = request.Title,
                With = request.PatientEmail
            }, cancellationToken);
        }

        private async Task<Result<int>> CreatePatientIfNoExistsAndGetId(CreateAppointmentByHostCommand request, CancellationToken cancellationToken)
        {
            var defaultEmail = $"{request.PatientEmail}_";
            var patient = await _userRepository.GetUserByEmail(request.PatientEmail);
            if (patient is null)
            {
                var patientResult = await _mediator.Send(new CreateUserCommand
                {
                    Email = request.PatientEmail,
                    Name = request.PatientName,
                    LastName = ".",
                    Password = "_",
                    TimezoneOffset = request.TimezoneOffset,
                    UserName = defaultEmail
                }, cancellationToken);
                if (!patientResult.IsSuccess) return Result.Failure<int>(patientResult.Error.Message);
                patient = patientResult.Value;
            }
            return patient.Id;
        }
    }
}