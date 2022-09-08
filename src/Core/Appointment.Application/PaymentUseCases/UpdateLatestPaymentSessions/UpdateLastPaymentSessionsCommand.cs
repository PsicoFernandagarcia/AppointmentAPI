using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;

namespace Appointment.Application.PaymentUseCases.UpdateLatestPaymentSessions
{
    public class UpdateLastPaymentSessionsCommand : IRequest<Result<Domain.Entities.Payment, ResultError>>
    {
        public int PatientId { get; set; }
        public int HostId { get; set; }
        public bool NewAppointmentAdded { get; set; }
        public string Currency { get; set; }

        public UpdateLastPaymentSessionsCommand()
        {

        }

        public UpdateLastPaymentSessionsCommand(int patientId, int hostId, bool newAppointmentAdded, string currency)
        {
            PatientId = patientId;
            HostId = hostId;
            NewAppointmentAdded = newAppointmentAdded;
            Currency = currency;
        }
    }
}
