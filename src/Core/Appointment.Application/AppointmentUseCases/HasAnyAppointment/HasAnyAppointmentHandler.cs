using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.AppointmentUseCases.HasAnyAppointment
{
    public class HasAnyAppointmentHandler : IRequestHandler<HasAnyAppointmentQuery, Result<bool, ResultError>>
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public HasAnyAppointmentHandler(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<Result<bool, ResultError>> Handle(HasAnyAppointmentQuery request, CancellationToken cancellationToken)
            => await _appointmentRepository.HasAnyAppointment(request.PatientId,request.HostId);
    }
}