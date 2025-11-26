using System.Data;
using InbentorySystem.Infrastructure.Interfaces;
using InbentorySystem.Infrastructure.Repository;
using Moq;

namespace InbentorySystem.Tests.Unit.Repositories
{
    public partial class ShiireRepositoryTestsSetup
    {
        protected readonly Mock<ISqlExecutor> _mockExecutor;
        protected readonly ShiireRepository _repository;

        public ShiireRepositoryTestsSetup()
        {
            _mockExecutor = new Mock<ISqlExecutor>();

            var mockFactory = new Mock<IDbConnectionFactory>();
            mockFactory.Setup(f => f.CreateConnection()).Returns(Mock.Of<IDbConnection>());

            _repository = new ShiireRepository(mockFactory.Object, _mockExecutor.Object);
        }
    }
}
