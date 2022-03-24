using System;
using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;

namespace Appointment.Application.AvailabilityUseCases.CreateAvailability
{
    public class CreateAvailabilityValidator : AbstractValidator<CreateAvailabilityCommand>
    {
        public CreateAvailabilityValidator()
        {
            RuleFor(x => x.DateOfAvailability).GreaterThan(DateTime.Now).WithMessage("Date should be higher than now");
            RuleFor(x => x.AmountOfTime).GreaterThan(10).WithMessage("Time should be higher than 10 minutes");
        }
    }
}
