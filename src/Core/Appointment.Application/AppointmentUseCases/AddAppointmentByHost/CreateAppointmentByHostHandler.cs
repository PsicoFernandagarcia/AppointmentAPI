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
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        public CreateAppointmentByHostHandler(IAppointmentRepository appointmentRepository, IMediator mediator, IUserRepository userRepository)
        {
            _appointmentRepository = appointmentRepository;
            _mediator = mediator;
            _userRepository = userRepository;
        }

        public async Task<Result<Domain.Entities.Appointment, ResultError>> Handle(CreateAppointmentByHostCommand request, CancellationToken cancellationToken)
        {
            if (request.PatientId == 0)
            {
                var defaultEmail = $"{request.PatientEmail}_";
                var patient = await _userRepository.GetUserByEmail(request.PatientEmail);
                if (patient == null)
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
                    if (!patientResult.IsSuccess) return Result.Failure<Domain.Entities.Appointment, ResultError>(patientResult.Error);
                    patient = patientResult.Value;
                }
                request.PatientId = patient.Id;
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
    }
}