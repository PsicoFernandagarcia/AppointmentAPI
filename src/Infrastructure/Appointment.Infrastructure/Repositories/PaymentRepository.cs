using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appointment.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> GetLast(int patientId, int hostId)
        => await _context.Payments.Where(p => p.HostId == hostId
                                                && p.PatientId == patientId)
                                  .GroupBy(p => new { p.HostId, p.PatientId })
                                  .Select(p => p.OrderByDescending(x => x.PaidAt).FirstOrDefault())
                                  .FirstOrDefaultAsync();

        public async Task<IEnumerable<Payment>> Get(int patientId, int hostId, int count)
        => await _context.Payments.Where(p => p.HostId == hostId
                                                && p.PatientId == patientId
                                            )
                                  .OrderByDescending(x => x.PaidAt)
                                  .Take(count)
                                  .ToListAsync();

        public async Task<Payment> Get(int paymentId)
        => await _context.Payments.Where(p => p.Id == paymentId
                                            )
                                  .OrderByDescending(x => x.PaidAt)
                                  .FirstOrDefaultAsync();

        public async Task<IEnumerable<Payment>> GetLatest(int hostId)
            => await _context.Payments.Where(p => p.HostId == hostId)
                                      .GroupBy(p => new { p.HostId, p.PatientId })
                                      .Select(p => p.OrderByDescending(x => x.PaidAt).FirstOrDefault())
                                      .ToListAsync();

        public async Task<IEnumerable<PaymentInformation>> GetYearInformation(int year, int hostId)
        => (await _context.Payments.Where(p => p.PaidAt.Year == year && p.HostId == hostId && p.Amount > 0)
                                                    .GroupBy(p => new { p.Currency, p.PaidAt.Month, p.PatientId, p.HostId, p.Patient.LastName, p.Patient.Name })
                                                    .Select(g => new PaymentInformation(
                                                        g.Key.PatientId,
                                                        g.Key.HostId,
                                                        g.Sum(p => p.Amount),
                                                        g.Key.Currency,
                                                        g.Key.Month,
                                                        $"{g.Key.LastName} {g.Key.Name}"
                                                        ))
            .ToListAsync())
            .OrderBy(g => g.Month)
            .ThenBy(g => g.PatientFullName);


        public async Task<int> ReassignPayments(int userFrom, int userTo)
         => await _context.Payments
                .Where(p => p.PatientId == userFrom)
                .ExecuteUpdateAsync(p => p.SetProperty(payment => payment.PatientId, userTo));

        public async Task<Payment> Update(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
            return payment;
        }
        public async Task<Payment> Insert(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

    }
}