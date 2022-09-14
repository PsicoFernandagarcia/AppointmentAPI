using Appointment.Application.PaymentUseCases.GetPaymentsFromPatientByHost;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Appointment.Test.Application.Payments
{
    public class GetPaymentsFromPatientByHostHandlerShould
    {
        private readonly Mock<IPaymentRepository> _paymentRepository = new();
        private readonly GetPaymentsFromPatientByHostHandler _handler;

        public GetPaymentsFromPatientByHostHandlerShould()
        {
            _handler = new(_paymentRepository.Object);
        }

        [Fact]
        public async Task Get_Payments_From_Specific_Patient()
        {
            var request = new GetPaymentsFromPatientByHostQuery(1, 5, 2);
            _paymentRepository.Setup(p => p.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(() => new List<Payment>
                {
                    Payment.Create(1,DateTime.Now,1,1,1,"test",1,1).Value,
                    Payment.Create(2,DateTime.Now,2,1,1,"test",1,1).Value
                });

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
        }

        [Fact]
        public async Task Return_Empty_List_If_No_Payments()
        {
            var request = new GetPaymentsFromPatientByHostQuery(1, 5, 2);
            _paymentRepository.Setup(p => p.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(() => new List<Payment>
                {
                });

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(0);
        }

    }
}
