using Appointment.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Appointment.Domain.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment> Update(Payment payment);
        Task<Payment> Insert(Payment payment);
        Task<IEnumerable<Payment>> GetLatestPayments(int hostId);
        Task<Payment> GetLastPayment(int patientId, int hostId);
        Task<IEnumerable<Payment>> GetPayments(int patientId, int hostId, int count);

    }
}