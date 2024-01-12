using Appointment.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace Appointment.Test.Application
{

    public static class MockAppDbContext
    {
        public static Mock<AppDbContext> GetMock()
        {
            Mock<AppDbContext> app = new ();
            Mock<DatabaseFacade> dbf = new(app.Object);
            Mock<IDbContextTransaction> dbct = new();
            app.Setup(con => con.Database).Returns(dbf.Object);
            dbf.Setup(con => con.BeginTransactionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(dbct.Object);
            return app;
        }
    }
}
