using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;

namespace Appointment.Application.AppointmentUseCases.CancelAppointment
{
    public class CancelAppointmentsCommand : IRequest<Result<bool,ResultError>>
    {
        [Required]
        public int UserId { get; set; }
        public int AppointmentId { get; set; }
    }
}
