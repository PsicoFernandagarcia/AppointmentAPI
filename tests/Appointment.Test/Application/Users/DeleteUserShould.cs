using Appointment.Application.UsersUseCase.DeleteUser;
using Appointment.Domain.Interfaces;
using Moq;
using Xunit;

namespace Appointment.Test.Application.Users
{
    public class DeleteUserShould
    {
        private readonly Mock<IPaymentRepository> _paymentRepository = new();
        private readonly Mock<IAppointmentRepository> _appointmentRepository=new();
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly DeleteUserHandler _handler;

        public DeleteUserShould()
        {
            var _context = MockAppDbContext.GetMock();
            _handler = new(_paymentRepository.Object, _appointmentRepository.Object, _context.Object, _userRepository.Object);

        }

        [Fact]
        public async Task Reassign_Appointments_And_Payments_To_Other_User_Then_Delete()
        {
            var userToDelete = 1;
            var userToReassign = 2;
           
            await _handler.Handle(new DeleteUserCommand
            {
                UserFrom = userToDelete,
                UserTo = userToReassign
            }, CancellationToken.None);

            _paymentRepository.Verify(pr => pr.ReassignPayments(userToDelete, userToReassign), Times.Once);
            _appointmentRepository.Verify(ap => ap.ReassignAppointments(userToDelete, userToReassign), Times.Once);
            _userRepository.Verify(u=> u.Delete(userToDelete), Times.Once);
        }
    }
}
