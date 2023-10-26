using Appointment.Domain;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.AvailabilityUseCases.CreateAvailability
{
    public class CreateAvailabilityHandler : IRequestHandler<CreateAvailabilityCommand, Result<Availability, ResultError>>
                                            , IRequestHandler<CreateAvailabilitiesCommand, Result<IEnumerable<Availability>, ResultError>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAvailabilityRepository _availabilityRepository;
        private readonly IOutputCacheStore _cachingStore;


        public CreateAvailabilityHandler(IUserRepository userRepository, IAvailabilityRepository availabilityRepository, IOutputCacheStore cachingStore)
        {
            _userRepository = userRepository;
            _availabilityRepository = availabilityRepository;
            _cachingStore = cachingStore;
        }

        public async Task<Result<Availability, ResultError>> Handle(CreateAvailabilityCommand request,
            CancellationToken cancellationToken)
        {
            var u = await _userRepository.GetUserById(request.HostId);
            if (u is null) return Result.Failure<Availability, ResultError>("host does not exists");

            var availabilities = await _availabilityRepository.GetOverlapped(request.HostId, request.DateOfAvailability,
                                                                request.DateOfAvailability.AddMinutes(request.AmountOfTime));

            if (availabilities.Any()) return Result.Failure<Availability, ResultError>(new CreationError("there is already an appointment at this time"));
            
            await _cachingStore.EvictByTagAsync(CacheKeys.Availabilities, cancellationToken);
            return await _availabilityRepository.Insert(Availability.Create(0, request.HostId, request.DateOfAvailability,
                request.AmountOfTime, true).Value);
        }

        public async Task<Result<IEnumerable<Availability>, ResultError>> Handle(CreateAvailabilitiesCommand request,
            CancellationToken cancellationToken)
        {
            var u = await _userRepository.GetUserById(request.HostId);
            if (u is null) return Result.Failure<IEnumerable<Availability>, ResultError>("host does not exists");

            if (await HasOverlap(request.Availabilities))
                return Result.Failure<IEnumerable<Availability>, ResultError>(new CreationError("there is already an appointment at this time"));

            if (request.AvailabilitiesToRemove.Any())
                await _availabilityRepository.Delete(request.AvailabilitiesToRemove);

            await _cachingStore.EvictByTagAsync(CacheKeys.Availabilities, cancellationToken);

            return Result.Success<IEnumerable<Availability>, ResultError>(await _availabilityRepository.Insert(MapAvailabilitiesDto(request.Availabilities)));
        }

        private async Task<bool> HasOverlap(IEnumerable<CreateAvailabilityCommand> commands)
        {
            foreach (var c in commands)
            {
                var overlapped = await _availabilityRepository.GetOverlapped(c.HostId, c.DateOfAvailability.ToLocalTime(),
                    c.DateOfAvailability.ToLocalTime().AddMinutes(c.AmountOfTime - 5));
                if (overlapped.Any())
                    return true;
            }

            return false;
        }

        private IEnumerable<Availability> MapAvailabilitiesDto(IEnumerable<CreateAvailabilityCommand> command)
        {
            var modelList = new List<Availability>();
            foreach (var c in command)
            {
                modelList.Add(Availability.Create(0, c.HostId, c.DateOfAvailability, c.AmountOfTime, true).Value);
            }

            return modelList;
        }
    }
}