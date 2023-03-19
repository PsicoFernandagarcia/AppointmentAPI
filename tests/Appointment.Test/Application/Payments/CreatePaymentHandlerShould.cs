﻿using Appointment.Application.PaymentUseCases.AddPayment;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using FluentAssertions;
using Moq;
using Xunit;

namespace Appointment.Test.Application.Payments
{
    public class CreatePaymentHandlerShould
    {
        private readonly Mock<IPaymentRepository> _paymentRepository = new();
        private readonly CreatePaymentHandler _handler;

        public CreatePaymentHandlerShould()
        {
            _handler = new(_paymentRepository.Object);
        }
        [Fact]
        public async Task Create_First_Payment_To_Patient()
        {
            var request = new CreatePaymentCommand(1, 12, 200, 100, "USD",DateTime.Now);
            _paymentRepository.Setup(p => p.GetLast(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(null as Payment);

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            _paymentRepository.Verify(p => p.Insert(It.IsAny<Payment>()), Times.Once);
        }

        [Fact]
        public async Task Create_Payment_To_Be_Last_To_Patient()
        {
            var request = new CreatePaymentCommand(1, 12, 200, 100, "USD", DateTime.Now);

            _paymentRepository.Setup(p => p.GetLast(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(Payment.Create(1, DateTime.Now.AddDays(-1), 1, 12, 1, "USD", 1, 1).Value);

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            _paymentRepository.Verify(p => p.Insert(It.IsAny<Payment>()), Times.Once);
            _paymentRepository.Verify(p => p.Update(It.IsAny<Payment>()), Times.Never);
        }

        [Fact]
        public async Task Create_Payment_Not_To_Be_Last_To_Patient()
        {
            var request = new CreatePaymentCommand(1, 12, 200, 100, "USD", DateTime.Now.AddDays(-1));

            var paymentCreated = Payment.Create(1, DateTime.Now, 1, 12, 1, "USD", 1, 1).Value;
            _paymentRepository.Setup(p => p.GetLast(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(paymentCreated);

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            paymentCreated.SessionsLeft = 101;
            _paymentRepository.Verify(p => p.Insert(It.IsAny<Payment>()), Times.Once);
            _paymentRepository.Verify(p => p.Update(paymentCreated), Times.Once);
        }

        [Fact]
        public async Task Not_Create_Payment_Due_To_Invalid_Command()
        {
            var request = new CreatePaymentCommand(1, -12, 200, 100, "USD", DateTime.Now);
            _paymentRepository.Setup(p => p.GetLast(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(Payment.Create(1, DateTime.Now, 1, 12, 1, "USD", 1, 1).Value);

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeOfType<CreationError>();
            _paymentRepository.Verify(p => p.Insert(It.IsAny<Payment>()), Times.Never);
        }

    }
}
