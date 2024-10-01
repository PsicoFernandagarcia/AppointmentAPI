using Appointment.Application.PaymentUseCases.UpdateLatestPaymentSessions;
using Appointment.Domain;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.OutputCaching;
using Moq;
using Xunit;

namespace Appointment.Test.Application.Payments
{
    public class UpdateLastPaymentHandlerShould
    {
        private readonly Mock<IPaymentRepository> _paymentRepository = new();
        private readonly UpdateLastPaymentSessionsHandler _handler;
        private readonly Mock<IOutputCacheStore> _cacheStore = new();


        public UpdateLastPaymentHandlerShould()
        {
            _handler = new(_paymentRepository.Object, _cacheStore.Object);
        }
        [Fact]
        public async Task Update_Payment_Because_There_Is_One_In_Database()
        {
            var request = new UpdateLastPaymentSessionsCommand(1, 12, true, "USD");
            _paymentRepository.Setup(p => p.GetLast(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(Payment.Create(1, DateTime.Now, 1, 12, 1, "USD", 1, 1, string.Empty, []).Value);

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            _paymentRepository.Verify(p => p.Insert(It.IsAny<Payment>()), Times.Never);
            _paymentRepository.Verify(p => p.Update(It.IsAny<Payment>()), Times.Once);
            _cacheStore.Verify(cs => cs.EvictByTagAsync(CacheKeys.Payments, It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task Create_Payment_Because_There_Is_No_Payment_In_Database()
        {
            var request = new UpdateLastPaymentSessionsCommand(1, 12, true, "USD");
            _paymentRepository.Setup(p => p.GetLast(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(null as Payment);

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            _paymentRepository.Verify(p => p.Insert(It.IsAny<Payment>()), Times.Once);
            _paymentRepository.Verify(p => p.Update(It.IsAny<Payment>()), Times.Never);
            _cacheStore.Verify(cs => cs.EvictByTagAsync(CacheKeys.Payments, It.IsAny<CancellationToken>()), Times.Once);

        }


    }
}
