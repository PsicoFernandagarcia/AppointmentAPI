using Appointment.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Appointment.Test.Domain
{
    public class PaymentShould
    {

        [InlineData(1, "2021/10/25", 10, 1, 100, "USD", 1, 1)]
        [InlineData(0, "2021/10/25", 10, 1, 100, "USD", 1, 1)]
        [InlineData(0, "2021/10/25", 10, 1, 100, "EUR", 1, 1)]
        [InlineData(0, "2021/10/25", 10, 1, 100, "EUR", 0, 1)]
        [InlineData(0, "2021/10/25", 10, 1, 100, "EUR", 1, 0)]
        [InlineData(0, "2022/08/25", 10, 1, 100, "EUR", 1, -1)]
        [Theory]
        public void Be_Created_Because_Of_Valid_Properties(int id, DateTime paidAt, int patientId, int hostId, decimal amount, string currency, int sessionsPaid, int sessionsLeft)
        {
            var paymentResult = Payment.Create(id, paidAt, patientId, hostId, amount, currency, sessionsPaid, sessionsLeft);
            paymentResult.IsSuccess.Should().BeTrue();
            paymentResult.Value.Should().BeOfType<Payment>();
        }

        [InlineData(-1, "2021/10/25", 10, 1, 100, "USD", 1, 1)]
        [InlineData(0, "3021/10/25", 10, 1, 100, "USD", 1, 1)]
        [InlineData(0, "2021/10/25", 0, 1, 100, "EUR", 1, 1)]
        [InlineData(0, "2021/10/25", 10, 0, 100, "EUR", 0, 1)]
        [Theory]
        public void Not_Be_Created_Because_Of_Inalid_Properties(int id, DateTime paidAt, int patientId, int hostId, decimal amount, string currency, int sessionsPaid, int sessionsLeft)
        {
            var paymentResult = Payment.Create(id, paidAt, patientId, hostId, amount, currency, sessionsPaid, sessionsLeft);
            paymentResult.IsSuccess.Should().BeFalse();
            paymentResult.Error.Should().NotBeNullOrWhiteSpace();
        }

    }
}
