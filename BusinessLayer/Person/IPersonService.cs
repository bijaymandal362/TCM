using Entities;
using Models.Core;
using Models.GridTableFilterModel;
using Models.GridTableProperty;
using Models.PersonUserModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.PersonUser
{
    public interface IPersonService
    {
        Task<Result<List<PersonUserModel>>> GetPersonListAsync();
        Task<Result<PagedResponsePersonModel<List<Person>>>> GetPersonListFilterByNameAsync(FilterModel filter);
    }
}
