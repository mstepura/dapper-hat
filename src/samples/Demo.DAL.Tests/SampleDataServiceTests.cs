using Dapper.Hat.SqlServer.Testing;
using NUnit.Framework;
using System.Configuration;

namespace Demo.DAL.Tests
{
    [TestFixture]
    public class SampleDataServiceTests
    {
        private ISampleDataService _sut;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var connectionFactory = new ValidatingDatabaseConnectionFactory(
                ConfigurationManager.ConnectionStrings["IntegrationTests"].ConnectionString
                );
            _sut = new SampleDataService(connectionFactory);
        }

        [Test]
        public void GetThisSingleInteger()
        {
            var result = _sut.GetThisSingleInteger(1, "hello").Result;
        }

        [Test]
        public void GetComplexTypes()
        {
            var result = _sut.GetComplexTypes(null).Result;
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetWithTableValuedParameter()
        {
            var result = _sut.GetWithTableValuedParameter(new[] { 1, 2, 3 }).Result;
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetMultipleResultSets()
        {
            var result = _sut.GetMultipleResultSets().Result;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result1);
            Assert.IsNotNull(result.Result2);
        }
    }
}
