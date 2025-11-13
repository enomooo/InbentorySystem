using InbentorySystem.Infrastructure.Interfaces;
using InbentorySystem.Infrastructure.Repository;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InbentorySystem.Tests.Unit.Repositories
{
    public partial class ShohinRepositoryTestsSetup
    {
        // SQL実行をモック化するためのフィールド。
        private Mock<ISqlExecutor> _mockExecutor;

        // テスト対象の商品リポジトリ
        private ShohinRepository _repository;

        /// <summary>
        /// ShohinRepositoryTests の共通初期化処理。
        /// </summary>
        public ShohinRepositoryTestsSetup()
        {
            // IDbConnectionのモックを作成
            _mockExecutor = new Mock<ISqlExecutor>();

            // IDbConnectionFactoryのモックを作成し、テスト用の接続を返すように設定
            var mockFactory = new Mock<IDbConnectionFactory>();
            mockFactory.Setup(f => f.CreateConnection()).Returns(Mock.Of<IDbConnection>());

            // モックのFactoryを使ってリポジトリを初期化
            _repository = new ShohinRepository(mockFactory.Object, _mockExecutor.Object);
        }
    }
}
