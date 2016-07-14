using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.DAL
{
    public interface ISampleDataService
    {
        Task<int?> GetThisSingleInteger(int input1, string input2);
        Task<IEnumerable<IComplexType>> GetComplexTypes(DateTime? dt);

        Task<IEnumerable<IComplexType>> GetWithTableValuedParameter(IEnumerable<int> filters);
        Task<IManyDatasetResult> GetMultipleResultSets();
    }
}