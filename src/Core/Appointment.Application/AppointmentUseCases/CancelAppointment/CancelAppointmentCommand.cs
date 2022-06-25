using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Appointment.Application.AppointmentUseCases.CancelAppointment
{
    public class CancelAppointmentsCommand : IRequest<Result<bool, ResultError>>
    {
        [Required]
        public int UserId { get; set; }
        public int AppointmentId { get; set; }
    }
}
