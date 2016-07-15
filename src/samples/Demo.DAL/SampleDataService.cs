using Dapper.Hat.SqlServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.DAL
{
    public class SampleDataService : ISampleDataService
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;

        public SampleDataService(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int?> GetThisSingleInteger(int input1, string input2)
        {
            var prm = _connectionFactory.CreateParameters();
            prm.Add(name: "@input1", value: input1, dbType: DbType.Int32);
            prm.Add(name: "@input2", value: input2, dbType: DbType.String, size: 50);
            prm.AddOutput(name: "@output", dbType: DbType.Int32);

            using (var connection = await _connectionFactory.Create())
            {
                await connection.Execute(sql: "dbo.GetThisSingleInteger", param: prm);
                return prm.Get<int?>("@output");
            }
        }

        public async Task<IEnumerable<IComplexType>> GetComplexTypes(DateTime? dt)
        {
            var prm = _connectionFactory.CreateParameters();
            prm.Add(name: "@dt", value: dt, dbType: DbType.DateTime);
            prm.AddReturnValue(name: "@result");

            using (var connection = await _connectionFactory.Create())
            {
                var result = await connection.Query<InternalComplexType>(sql: "dbo.GetComplexTypes", param: prm);
                return result;
            }
        }

        public async Task<IEnumerable<IComplexType>> GetWithTableValuedParameter(IEnumerable<int> filters)
        {
            var prm = _connectionFactory.CreateParameters();
            prm.AddTable("@filters", filters.Select(i => new InternalIntegerList { Id = i }));

            using (var connection = await _connectionFactory.Create())
            {
                var result = await connection.Query<InternalComplexType>(sql: "dbo.GetWithTableValuedParameter", param: prm);
                return result;
            }
        }

        public async Task<IManyDatasetResult> GetMultipleResultSets()
        {
            using (var connection = await _connectionFactory.Create())
            using (var multiResult = await connection.MultiQuery(sql: "dbo.GetMultipleResultSets"))
            {
                return new InternalManyDatasetResult
                {
                    Result1 = await multiResult.Read<int>(),
                    Result2 = await multiResult.Read<string>()
                };
            }
        }
    }
}