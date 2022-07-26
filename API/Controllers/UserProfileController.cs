using BusinessLayer.UserProfile;
using Microsoft.AspNetCore.Mvc;
using Models.Profile;
using System.Threading.Tasks;

namespace API.Controllers
{

    public class UserProfileController : BaseApiController
    {
        private readonly IUserProfileService _iUserProfileService;

        public UserProfileController(IUserProfileService iUserProfileService)
        {
            _iUserProfileService = iUserProfileService;
        }
        [HttpGet]
        [Route("GetUserProfileDetails")]
        public async Task<IActionResult> GetUserProfileDetails()
        {
            return HandleResult(await _iUserProfileService.GetUserProfileDetailsAsync());
        }
        
        [HttpPut]
        [Route("UpdatePersonPersonalization")]
        public async Task<IActionResult> UpdatePersonPersonalization([FromBody]PersonPersonalizationModel model)
        {
            return HandleResult(await _iUserProfileService.UpdatePersonPersonalizationAsync(model));
        } 
        
        [HttpGet]
        [Route("GetPersonPersonalizationByCurrentPersonId")]
        public async Task<IActionResult> GetPersonPersonalizationByCurrentPersonId()
        {
            return HandleResult(await _iUserProfileService.GetPersonPersonalizationByCurrentPersonIdAsync());
        }
    }
}
