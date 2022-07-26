using Data;
using Entities;
using Microsoft.EntityFrameworkCore;
using Models.Core;
using Models.GridTableFilterModel;
using Models.GridTableProperty;
using Models.PersonUserModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.PersonUser
{
    public class PersonService : IPersonService
    {
        private readonly DataContext _context;

        public PersonService(DataContext context)
        {
            _context = context;
        }
        public async Task<Result<List<PersonUserModel>>> GetPersonListAsync()
        {
            var getPersonList =  await(from p in _context.Person
                                      select new PersonUserModel
                                      {
                                          PersonId = p.PersonId,
                                          Name = p.Name
                                      }).ToListAsync();

            if (getPersonList.Any())
            {
                return Result<List<PersonUserModel>>.Success(getPersonList);
            }
            else
            {
                return Result<List<PersonUserModel>>.Success(null);
            }
        }

        public async Task<Result<PagedResponsePersonModel<List<Person>>>> GetPersonListFilterByNameAsync(FilterModel filter)
        {
            var getPersonListFilterByNameAsync = from p in _context.Person
                                                 select new PersonUserModel
                                                 {
                                                     PersonId = p.PersonId,
                                                     Name = p.Name
                                                 };

            if (!string.IsNullOrEmpty(filter.SearchValue))
            {
                getPersonListFilterByNameAsync = getPersonListFilterByNameAsync.Where
                    (
                      x => x.Name.ToLower().Contains(filter.SearchValue.ToLower())
                

                    );
            }


            var filteredData = await getPersonListFilterByNameAsync.ToListAsync();



            List<Person> person = new List<Person>();

            foreach (var item in filteredData)
            {
                person.Add(new Person
                {
                    PersonId = item.PersonId,
                    Name = item.Name

                });
            }
            var data = new PagedResponsePersonModel<List<Person>>(person);

            if (filteredData.Count > 0)
            {
                return Result<PagedResponsePersonModel<List<Person>>>.Success(data);
            }
            else
            {
                return Result<PagedResponsePersonModel<List<Person>>>.Success(null);
            }
        }
    }
}
