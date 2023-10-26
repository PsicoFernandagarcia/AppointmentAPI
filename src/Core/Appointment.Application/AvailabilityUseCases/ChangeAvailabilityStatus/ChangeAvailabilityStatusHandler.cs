using Appointment.Domain;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.AvailabilityUseCases.ChangeAvailabilityStatus
{
    public class ChangeAvailabilityStatusHandler : IRequestHandler<ChangeAvailabilityStatusCommand, Result<bool, ResultError>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAvailabilityRepository _availabilityRepository;
        private readonly IOutputCacheStore _cachingStore;


        public ChangeAvailabilityStatusHandler(IUserRepository userRepository, IAvailabilityRepository availabilityRepository, IOutputCacheStore cachingStore)
        {
            _userRepository = userRepository;
            _availabilityRepository = availabilityRepository;
            _cachingStore = cachingStore;
        }

        public async Task<Result<bool, ResultError>> Handle(ChangeAvailabilityStatusCommand request,
            CancellationToken cancellationToken)
        {
            var u = await _userRepository.GetUserById(request.HostId);
            if (u is null) return Result.Failure<bool, ResultError>("host does not exists");

            var availabilityEntity = await _availabilityRepository.GetByAppointmentId(request.AppointmentId);
            if (availabilityEntity is null)
                return false;

            availabilityEntity.IsEmpty = request.IsEmpty;
            if(request.IsEmpty)
            {
                availabilityEntity.AppointmentId = default;
                availabilityEntity.AppointmentWith = string.Empty;
            }
            await _availabilityRepository.Update(availabilityEntity);
            await _cachingStore.EvictByTagAsync(CacheKeys.Availabilities, cancellationToken);
            return true;

        }




    }
}