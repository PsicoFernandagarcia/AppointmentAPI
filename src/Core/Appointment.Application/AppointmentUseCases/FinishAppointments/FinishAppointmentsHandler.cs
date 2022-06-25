using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.AppointmentUseCases.FinishAppointments
{
    public class FinishAppointmentsHandler : IRequestHandler<FinishAppointmentsCommand, Result<bool, ResultError>>
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public FinishAppointmentsHandler(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<Result<bool, ResultError>> Handle(FinishAppointmentsCommand request, CancellationToken cancellationToken)
        {
            await _appointmentRepository.FinalizeAppointments();
            return true;
        }


    }
}