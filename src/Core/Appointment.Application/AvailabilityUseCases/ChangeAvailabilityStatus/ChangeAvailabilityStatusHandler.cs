using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.AvailabilityUseCases.ChangeAvailabilityStatus
{
    public class ChangeAvailabilityStatusHandler : IRequestHandler<ChangeAvailabilityStatusCommand, Result<bool, ResultError>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAvailabilityRepository _availabilityRepository;

        public ChangeAvailabilityStatusHandler(IUserRepository userRepository, IAvailabilityRepository availabilityRepository)
        {
            _userRepository = userRepository;
            _availabilityRepository = availabilityRepository;
        }

        public async Task<Result<bool, ResultError>> Handle(ChangeAvailabilityStatusCommand request,
            CancellationToken cancellationToken)
        {
            var u = await _userRepository.GetUserById(request.HostId);
            if (u is null) return Result.Failure<bool, ResultError>("host does not exists");
            var availability = (await _availabilityRepository.GetByFilter(request.HostId, request.DateFromUtc, request.DateToUtc, false)).FirstOrDefault();
            if (availability is null) return true;

            var availabilityEntity = await _availabilityRepository.GetById(availability.Id);
            availabilityEntity.IsEmpty = request.IsEmpty;
            await _availabilityRepository.Update(availabilityEntity);
            return true;

        }


    }
}