using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.Collections.Generic;

namespace Appointment.Application.PaymentUseCases.GetPaymentsFromPatientByHost
{
    public class GetPaymentsFromPatientByHostQuery : IRequest<Result<IEnumerable<Payment>, ResultError>>
    {
        public int HostId { get; set; }
        public int Count { get; set; }
        public int PatientId { get; set; }
        public GetPaymentsFromPatientByHostQuery()
        {

        }

        public GetPaymentsFromPatientByHostQuery(int hostId, int count, int patientId)
        {
            HostId = hostId;
            Count = count;
            PatientId = patientId;
        }
    }
}
