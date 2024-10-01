using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Appointment.Domain.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment> Update(Payment payment);
        Task<Payment> Insert(Payment payment);
        Task<Result<Payment, ResultError>> Insert(AddPaymentDto payment);
        Task<IEnumerable<LastPaymentDto>> GetLatest(int hostId);
        Task<Payment> GetLast(int patientId, int hostId);
        Task<IEnumerable<Payment>> Get(int patientId, int hostId, int count);
        Task<Payment> Get(int paymentId);
        Task<IEnumerable<PaymentInformation>> GetYearInformation(int year, int hostId);
        Task<int> ReassignPayments(int userFrom, int userTo);

    }
}