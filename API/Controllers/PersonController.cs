using BusinessLayer.PersonUser;
using Microsoft.AspNetCore.Mvc;
using Models.GridTableFilterModel;
using System.Threading.Tasks;

namespace API.Controllers
{

    public class PersonController : BaseApiController
    {
        private readonly IPersonService _iPersonService;

        public PersonController(IPersonService iPersonService)
        {
            _iPersonService = iPersonService;
        }

        [HttpGet("GetPersonList")]
        public async Task<IActionResult> GetPersonList()
        {
            return HandleResult(await _iPersonService.GetPersonListAsync());
        }

        [HttpPost("GetPersonListFilterByName")]
        public async Task<IActionResult> GetPersonListFilterByName([FromQuery]FilterModel filter)
        {
            return HandleResult(await _iPersonService.GetPersonListFilterByNameAsync(filter));
        }
    }
}
