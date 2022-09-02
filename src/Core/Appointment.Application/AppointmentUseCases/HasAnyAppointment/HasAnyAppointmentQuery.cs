using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;

namespace Appointment.Application.AppointmentUseCases.HasAnyAppointment
{
    public class HasAnyAppointmentQuery : IRequest<Result<bool, ResultError>>
    {
        public int PatientId { get; set; }
        public int HostId { get; set; }
    }
}
