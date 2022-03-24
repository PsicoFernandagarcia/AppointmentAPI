using System;
using System.Collections.Generic;
using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;

namespace Appointment.Application.AvailabilityUseCases.ChangeAvailabilityStatus
{
    public class ChangeAvailabilityStatusCommand : IRequest<Result<bool, ResultError>>
    {
        public int HostId { get; set; }
        public DateTime DateFromUtc { get; set; }
        public DateTime DateToUtc { get; set; }
        public bool IsEmpty { get; set; }
    }
}