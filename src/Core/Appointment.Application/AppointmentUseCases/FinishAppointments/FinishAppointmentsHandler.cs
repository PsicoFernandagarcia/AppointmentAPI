using Appointment.Domain;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.AppointmentUseCases.FinishAppointments
{
    public class FinishAppointmentsHandler : IRequestHandler<FinishAppointmentsCommand, Result<bool, ResultError>>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IOutputCacheStore _cachingStore;


        public FinishAppointmentsHandler(IAppointmentRepository appointmentRepository, IOutputCacheStore cachingStore)
        {
            _appointmentRepository = appointmentRepository;
            _cachingStore = cachingStore;
        }

        public async Task<Result<bool, ResultError>> Handle(FinishAppointmentsCommand request, CancellationToken cancellationToken)
        {
            await _appointmentRepository.FinalizeAppointments();
            await _cachingStore.EvictByTagAsync(CacheKeys.Appointments, cancellationToken);

            return true;
        }


    }
}