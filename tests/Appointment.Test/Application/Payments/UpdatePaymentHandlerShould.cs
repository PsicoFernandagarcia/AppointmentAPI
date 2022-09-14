using Appointment.Application.PaymentUseCases.AddPayment;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using FluentAssertions;
using Moq;
using Xunit;

namespace Appointment.Test.Application.Payments
{
    public class UpdatePaymentHandlerShould
    {
        private readonly Mock<IPaymentRepository> _paymentRepository = new();
        private readonly UpdatePaymentHandler _handler;

        public UpdatePaymentHandlerShould()
        {
            _handler = new(_paymentRepository.Object);
        }
        [Fact]
        public async Task Update_First_Payment_To_Patient()
        {

            _paymentRepository.Setup(p => p.Get(It.IsAny<int>()))
                .ReturnsAsync(Payment.Create(1, DateTime.Now, 1, 12, 1, "USD", 1, 1).Value);

            _paymentRepository.Setup(p => p.GetLast(It.IsAny<int>(), It.IsAny<int>()))
               .ReturnsAsync(Payment.Create(1, DateTime.Now, 1, 12, 1, "USD", 1, 1).Value);
            var request = new UpdatePaymentCommand(1, 12, 200, 100, "USD",1, DateTime.Now);

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            _paymentRepository.Verify(p => p.Update(It.IsAny<Payment>()), Times.Once);
        }

        [Fact]
        public async Task Not_Update_Payment_Due_To_Invalid_Command()
        {
            var request = new UpdatePaymentCommand(-1, -12, 200, 100, "USD",1, DateTime.Now);
            _paymentRepository.Setup(p => p.Get(It.IsAny<int>()))
                .ReturnsAsync(Payment.Create(1, DateTime.Now, 1, 12, 1, "USD", 1, 1).Value);

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeOfType<CreationError>();
            _paymentRepository.Verify(p => p.Update(It.IsAny<Payment>()), Times.Never);
        }

    }
}
