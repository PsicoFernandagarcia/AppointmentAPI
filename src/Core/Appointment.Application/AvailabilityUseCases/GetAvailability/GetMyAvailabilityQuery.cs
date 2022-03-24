using System;
using System.Collections.Generic;
using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;

namespace Appointment.Application.AvailabilityUseCases.GetAvailability
{
    public class GetMyAvailabilityQuery : IRequest<Result<IEnumerable<Availability>, ResultError>>
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}