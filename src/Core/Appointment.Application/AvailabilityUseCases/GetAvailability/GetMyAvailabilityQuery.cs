using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Collections.Generic;

namespace Appointment.Application.AvailabilityUseCases.GetAvailability
{
    public class GetMyAvailabilityQuery : IRequest<Result<IEnumerable<Availability>, ResultError>>
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}