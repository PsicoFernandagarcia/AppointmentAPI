using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;

namespace Appointment.Application.AvailabilityUseCases.GetAvailability
{
    public class GetAvailabilityHandler: IRequestHandler<GetAvailabilityQuery, Result<IEnumerable<AvailabilityDto>, ResultError>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAvailabilityRepository _availabilityRepository;

        public GetAvailabilityHandler(IUserRepository userRepository, IAvailabilityRepository availabilityRepository)
        {
            _userRepository = userRepository;
            _availabilityRepository = availabilityRepository;
        }

        public async Task<Result<IEnumerable<AvailabilityDto>, ResultError>> Handle(GetAvailabilityQuery request,
            CancellationToken cancellationToken)
        {
            var u = await _userRepository.GetUserById(request.HostId);
            if (u is null) return Result.Failure<IEnumerable<AvailabilityDto>, ResultError>("host does not exists");
            return Result.Success<IEnumerable<AvailabilityDto>, ResultError>(
                await _availabilityRepository.GetByFilter(request.HostId,request.DateFrom,request.DateTo, request.ShowOnlyAvailable)
                ); 
        }

    }
}