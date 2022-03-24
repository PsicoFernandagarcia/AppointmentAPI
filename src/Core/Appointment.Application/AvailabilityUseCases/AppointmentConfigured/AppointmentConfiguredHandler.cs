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

namespace Appointment.Application.AvailabilityUseCases.AppointmentConfigured
{
    public class AppointmentConfiguredHandler : IRequestHandler<AppointmentConfiguredCommand, Result<string, ResultError>>
    {
        private readonly IAvailabilityRepository _availabilityRepository;

        public AppointmentConfiguredHandler( IAvailabilityRepository availabilityRepository)
        {
            _availabilityRepository = availabilityRepository;
        }

        public async Task<Result<string, ResultError>> Handle(AppointmentConfiguredCommand request,
            CancellationToken cancellationToken)
        {
            var availability = await _availabilityRepository.GetById(request.AvailabilityId);
            if (availability is null || !availability.IsEmpty ) return Result.Failure<string, ResultError>("there is no availability at this time");
            availability.IsEmpty = request.IsEmpty;
            await _availabilityRepository.Update(availability);
            return Result.Success<string, ResultError>(string.Empty);

        }

        
    }
}