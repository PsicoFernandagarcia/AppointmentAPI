using Appointment.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Appointment.Domain.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment> Update(Payment payment);
        Task<Payment> Insert(Payment payment);
        Task<IEnumerable<Payment>> GetLatest(int hostId);
        Task<Payment> GetLast(int patientId, int hostId);
        Task<IEnumerable<Payment>> Get(int patientId, int hostId, int count);
        Task<Payment> Get(int paymentId);

    }
}