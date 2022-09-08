using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.AvailabilityUseCases.AppointmentConfigured
{
    public class AppointmentConfiguredHandler : IRequestHandler<AppointmentConfiguredCommand, Result<Availability, ResultError>>
    {
        private readonly IAvailabilityRepository _availabilityRepository;

        public AppointmentConfiguredHandler(IAvailabilityRepository availabilityRepository)
        {
            _availabilityRepository = availabilityRepository;
        }

        public async Task<Result<Availability, ResultError>> Handle(AppointmentConfiguredCommand request,
            CancellationToken cancellationToken)
        {
            var availability = await _availabilityRepository.GetById(request.AvailabilityId);
            if (availability is null || !availability.IsEmpty)
                return Result.Failure<Availability, ResultError>("there is no availability at this time");
            availability.IsEmpty = request.IsEmpty;
            await _availabilityRepository.Update(availability);
            return Result.Success<Availability, ResultError>(availability);

        }


    }
}