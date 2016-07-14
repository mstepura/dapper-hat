using Dapper.Hat.SqlServer.Testing;
using NUnit.Framework;
using System.Configuration;

namespace Demo.DAL.Tests
{
    [TestFixture]
    public class SampleDataServiceTests
    {
        private ISampleDataService _target;

        [SetUp]
        public void SetUp()
        {
            var connectionFactory = new ValidatingDatabaseConnectionFactory(
                ConfigurationManager.ConnectionStrings["IntegrationTests"].ConnectionString
                );
            _target = new SampleDataService(connectionFactory);
        }

        [Test]
        public void GetThisSingleInteger()
        {
            var result = _target.GetThisSingleInteger(1, "hello").Result;
        }

        [Test]
        public void GetComplexTypes()
        {
            var result = _target.GetComplexTypes(null).Result;
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetWithTableValuedParameter()
        {
            var result = _target.GetWithTableValuedParameter(new[] { 1, 2, 3 }).Result;
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetMultipleResultSets()
        {
            var result = _target.GetMultipleResultSets().Result;
            Assert.IsNotNull(result);
        }
    }
}
