using Models.Core;
using Models.Profile;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.UserProfile
{
    public interface IUserProfileService
    {
        Task<Result<UserProfileModel>> GetUserProfileDetailsAsync();
        Task<Result<string>> UpdatePersonPersonalizationAsync(PersonPersonalizationModel model);
        Task<Result<PersonPersonalizationListModel>> GetPersonPersonalizationByCurrentPersonIdAsync();
    }
}
