using Entities;
using Models.Account;
using Models.AuthServer;
using System.Threading.Tasks;

namespace BusinessLayer.Account
{
    public interface IAccountService
    {
        Task<PersonViewModel> CreatePersonAsync(AuthServerUserProfileResponse userProfile);
    }
}
