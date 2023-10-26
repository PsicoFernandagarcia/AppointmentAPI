using Appointment.Domain;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.AvailabilityUseCases.AppointmentConfigured
{
    public class AppointmentConfiguredHandler : IRequestHandler<AppointmentConfiguredCommand, Result<Availability, ResultError>>
    {
        private readonly IAvailabilityRepository _availabilityRepository;
        private readonly IOutputCacheStore _cachingStore;


        public AppointmentConfiguredHandler(IAvailabilityRepository availabilityRepository, IOutputCacheStore cachingStore)
        {
            _availabilityRepository = availabilityRepository;
            _cachingStore = cachingStore;
        }

        public async Task<Result<Availability, ResultError>> Handle(AppointmentConfiguredCommand request,
            CancellationToken cancellationToken)
        {
            var availability = await _availabilityRepository.GetById(request.AvailabilityId);
            if (availability is null || !availability.IsEmpty)
                return Result.Failure<Availability, ResultError>("there is no availability at this time");
            
            availability.IsEmpty = request.IsEmpty;
            availability.AppointmentId = request.AppointmentId;
            availability.AppointmentWith = request.AppointmentWith;

            await _availabilityRepository.Update(availability);
            await _cachingStore.EvictByTagAsync(CacheKeys.Availabilities,cancellationToken);
            return Result.Success<Availability, ResultError>(availability);

        }


    }
}