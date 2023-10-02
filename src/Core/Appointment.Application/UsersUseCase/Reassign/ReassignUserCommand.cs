using Appointment.Application.PaymentUseCases.AddPayment;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using Appointment.Infrastructure.Configuration;
using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.UsersUseCase.Reassign
{
    public class ReassignUserCommand : IRequest<Result<bool, ResultError>>
    {
        public int UserFrom { get; set; }
        public int UserTo { get; set; }
    }

    public class ReassignUserHandler : IRequestHandler<ReassignUserCommand, Result<bool, ResultError>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly AppDbContext _context;

        public ReassignUserHandler(IPaymentRepository paymentRepository, IAppointmentRepository appointmentRepository, AppDbContext context)
        {
            _paymentRepository = paymentRepository;
            _appointmentRepository = appointmentRepository;
            _context = context;
        }

        public async Task<Result<bool, ResultError>> Handle(ReassignUserCommand request, CancellationToken cancellationToken)
        {
            using var scope = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await _paymentRepository.ReassignPayments(request.UserFrom, request.UserTo);
                await _appointmentRepository.ReassignAppointments(request.UserFrom, request.UserTo);
                await scope.CommitAsync(cancellationToken);
            }
            catch (Exception)
            {
                await scope.RollbackAsync(cancellationToken);
                throw;
            }
            return true;
        }
    }
}
