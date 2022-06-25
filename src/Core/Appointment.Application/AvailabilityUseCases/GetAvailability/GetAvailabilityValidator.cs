using FluentValidation;
using System;

namespace Appointment.Application.AvailabilityUseCases.GetAvailability
{
    public class GetAvailabilityValidator : AbstractValidator<GetAvailabilityQuery>
    {
        public GetAvailabilityValidator()
        {
            RuleFor(x => x.HostId).GreaterThan(0).WithMessage("Host id not valid");
            RuleFor(x => x.DateTo).GreaterThan(DateTime.Now).WithMessage("Date should be higher than now");
        }
    }
}
