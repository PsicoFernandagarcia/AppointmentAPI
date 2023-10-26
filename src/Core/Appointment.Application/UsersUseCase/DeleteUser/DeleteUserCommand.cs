using Appointment.Application.PaymentUseCases.AddPayment;
using Appointment.Domain;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using Appointment.Infrastructure.Configuration;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.UsersUseCase.DeleteUser
{
    public class DeleteUserCommand : IRequest<Result<bool, ResultError>>
    {
        public int UserFrom { get; set; }
        public int UserTo { get; set; }
    }

    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Result<bool, ResultError>>
    {
        private readonly IOutputCacheStore _cachingStore;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _context;

        public DeleteUserHandler(IPaymentRepository paymentRepository,
                                 IAppointmentRepository appointmentRepository,
                                 AppDbContext context,
                                 IUserRepository userRepository,
                                 IOutputCacheStore cachingStore)
        {
            _paymentRepository = paymentRepository;
            _appointmentRepository = appointmentRepository;
            _context = context;
            _userRepository = userRepository;
            _cachingStore = cachingStore;
        }

        public async Task<Result<bool, ResultError>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            using var scope = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await _paymentRepository.ReassignPayments(request.UserFrom, request.UserTo);
                await _appointmentRepository.ReassignAppointments(request.UserFrom, request.UserTo);
                await _userRepository.Delete(request.UserFrom);
                await scope.CommitAsync(cancellationToken);
                await _cachingStore.EvictByTagAsync(CacheKeys.Users, cancellationToken);
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
