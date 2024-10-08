﻿using Appointment.Application.PaymentUseCases.GetLatestsPaymentsByHost;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Appointment.Test.Application.Payments
{
    public class GetLatestPaymentsByHostHandlerShould
    {
        private readonly Mock<IPaymentRepository> _paymentRepository = new();
        private readonly GetLatestsPaymentsByHostHandler _handler;

        public GetLatestPaymentsByHostHandlerShould()
        {
            _handler = new(_paymentRepository.Object);
        }

        [Fact]
        public async Task Get_Latest_Payments_From_Specific_Host()
        {
            var request = new GetLatestsPaymentsByHostQuery(1);
            _paymentRepository.Setup(p => p.GetLatest(It.IsAny<int>()))
                .ReturnsAsync(() => new List<LastPaymentDto>
                {
                    LastPaymentDto.FromPaymnet(Payment.Create(1,DateTime.Now,1,1,1,"test",1,1, string.Empty, []).Value)
                });

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
        }
        [Fact]
        public async Task Return_Empty_List_If_No_Payments()
        {
            var request = new GetLatestsPaymentsByHostQuery(1);
            _paymentRepository.Setup(p => p.GetLatest(It.IsAny<int>()))
                .ReturnsAsync(() => new List<LastPaymentDto>
                {
                });

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(0);
        }

    }
}
