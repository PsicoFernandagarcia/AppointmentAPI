using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;

namespace Appointment.Application.AppointmentUseCases.FinishAppointments
{
    public class FinishAppointmentsCommand : IRequest<Result<bool,ResultError>>
    {
    }
}
