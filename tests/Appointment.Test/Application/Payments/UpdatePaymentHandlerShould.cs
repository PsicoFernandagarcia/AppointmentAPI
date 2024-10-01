using Appointment.Application.PaymentUseCases.AddPayment;
using Appointment.Domain;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using FluentAssertions;
using Microsoft.AspNetCore.OutputCaching;
using Moq;
using Xunit;

namespace Appointment.Test.Application.Payments
{
    public class UpdatePaymentHandlerShould
    {
        private readonly Mock<IPaymentRepository> _paymentRepository = new();
        private readonly Mock<IAppointmentRepository> _appointmentRepository = new();
        private readonly UpdatePaymentHandler _handler;
        private readonly Mock<IOutputCacheStore> _cacheStore = new();


        public UpdatePaymentHandlerShould()
        {
            _handler = new(_paymentRepository.Object, _cacheStore.Object, _appointmentRepository.Object);
        }
        //[Fact]
        //public async Task Update_First_Payment_To_Patient()
        //{

        //    _paymentRepository.Setup(p => p.Get(It.IsAny<int>()))
        //        .ReturnsAsync(Payment.Create(1, DateTime.Now, 1, 12, 1, "USD", 1, 1, string.Empty, []).Value);

        //    _paymentRepository.Setup(p => p.GetLast(It.IsAny<int>(), It.IsAny<int>()))
        //       .ReturnsAsync(Payment.Create(1, DateTime.Now, 1, 12, 1, "USD", 1, 1, string.Empty, []).Value);
        //    var request = new UpdatePaymentCommand(1, 12, 200, 100, "USD",1, DateTime.Now, string.Empty, []);

        //    var result = await _handler.Handle(request, CancellationToken.None);
        //    result.IsSuccess.Should().BeTrue();
        //    _paymentRepository.Verify(p => p.Update(It.IsAny<Payment>()), Times.Once);
        //    _cacheStore.Verify(cs => cs.EvictByTagAsync(CacheKeys.Payments, It.IsAny<CancellationToken>()), Times.Once);

        //}

        [Fact]
        public async Task Not_Update_Payment_Due_To_Invalid_Command()
        {
            var request = new UpdatePaymentCommand(-1, -12, 200, 100, "USD",1, DateTime.Now, string.Empty, []);
            _paymentRepository.Setup(p => p.Get(It.IsAny<int>()))
                .ReturnsAsync(Payment.Create(1, DateTime.Now, 1, 12, 1, "USD", 1, 1, string.Empty, []).Value);

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeOfType<CreationError>();
            _paymentRepository.Verify(p => p.Update(It.IsAny<Payment>()), Times.Never);
            _cacheStore.Verify(cs => cs.EvictByTagAsync(CacheKeys.Payments, It.IsAny<CancellationToken>()), Times.Never);

        }

    }
}
