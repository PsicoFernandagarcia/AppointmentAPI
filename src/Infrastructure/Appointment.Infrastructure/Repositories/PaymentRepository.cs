using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using Appointment.Infrastructure.Configuration;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

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
        => await _context.Payments
            .Include(p => p.Patient)
            .Include(p => p.AppointmentsPaid)
            .Where(p => p.HostId == hostId
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

        public async Task<IEnumerable<LastPaymentDto>> GetLatest(int hostId)
            => (await _context.Payments.Include(p => p.Patient)
                                      .Where(p => p.HostId == hostId)
                                      .GroupBy(p => new { p.HostId, p.PatientId })
                                      .Select(p => p.OrderByDescending(x => x.PaidAt).FirstOrDefault())
                                      .ToListAsync())
                                      .Select(payment => LastPaymentDto.FromPaymnet(payment))  ;

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
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (payment.AppointmentsPaid != null) {
                    await _context.Appointments.Where(a => a.PaymentId == payment.Id)
                        .ExecuteUpdateAsync(setter => setter.SetProperty(a => a.PaymentId, a => null));
                    await _context.SaveChangesAsync();

                    await _context.Appointments
                        .Where(a => a.PaymentId == null
                                && payment.AppointmentsPaid.Select(ap => ap.Id).Contains(a.Id)
                        )
                        .ExecuteUpdateAsync(setter => setter.SetProperty(a => a.PaymentId, a => payment.Id));
                    await _context.SaveChangesAsync();
                }
                _context.Payments.Update(payment);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return payment;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
   
        }
        public async Task<Payment> Insert(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Result<Payment, ResultError>> Insert(AddPaymentDto paymentDto)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var appointments = _context.Appointments.Where(a => paymentDto.Appointments.Contains(a.Id)).ToList();
                var paymentResult = Payment.Create(0,
                                                   paymentDto.PaidAt,
                                                   paymentDto.PatientId,
                                                   paymentDto.HostId,
                                                   paymentDto.Amount,
                                                   paymentDto.Currency,
                                                   paymentDto.SessionsPaid,
                                                   0,
                                                   paymentDto.Observations,
                                                   appointments);

                if (!paymentResult.IsSuccess)
                    return Result.Failure<Payment, ResultError>(new CreationError(paymentResult.Error));

                await _context.Payments.AddAsync(paymentResult.Value);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return paymentResult.Value;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}