using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dapper.Hat.SqlServer
{
    sealed class MultiQueryResult : IMultiQueryResult
    {
        private readonly SqlMapper.GridReader _results;

        public MultiQueryResult(SqlMapper.GridReader results)
        {
            _results = results;
        }

        async public Task<IEnumerable<T>> Read<T>()
        {
            return await _results.ReadAsync<T>()
                .ConfigureAwait(false);
        }

        public void Dispose()
        {
            _results.Dispose();
        }
    }
}
