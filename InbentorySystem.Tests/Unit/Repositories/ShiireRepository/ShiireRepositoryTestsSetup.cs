using System.Data;
using InbentorySystem.Infrastructure.Interfaces;
using Moq;

namespace InbentorySystem.Tests.Unit.Repositories.ShiireRepository
{
    public partial class ShiireRepositoryTestsSetup
    {
        protected readonly Mock<ISqlExecutor> _mockExcutor;
        protected readonly Mock<ISqlExecutor> _repository;

        public ShiireRepositoryTestsSetup()
        {
            _mockExcutor = new Mock<ISqlExecutor>();

            var mockFactory = new Mock<IDbConnectionFactory>();
            mockFactory.Setup(f => f.CreateConnection()).Returns(Mock.Of<IDbConnection>());
        }
    }
}
