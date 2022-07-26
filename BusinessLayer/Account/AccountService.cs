using BusinessLayer.Common;
using Data;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Account;
using Models.AuthServer;
using Models.Constant.Authorization;
using Models.Constant.ListItem;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Account
{
    public class AccountService : IAccountService
    {
        private readonly DataContext _context;
		private readonly ICommonService _iCommonService;


        public AccountService(DataContext context, ICommonService iCommonService)
        {
            _context = context;
            _iCommonService = iCommonService;

        }
        public async Task<PersonViewModel> CreatePersonAsync(AuthServerUserProfileResponse userProfile)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var currentDateTime = DateTimeOffset.UtcNow;

                var userMemberListItemModel = await (_iCommonService.GetListItemDetailByListItemSystemName(Models.Enum.ListItem.UserMember.ToString()));      
                var superAdmin = await _context.Person.Where(x => x.UserName == SystemUser.UserName).FirstOrDefaultAsync();
  
                Person person = new()
                {
                    Name = userProfile.Name,
                    Email = userProfile.Email,
                    PhoneNumber = userProfile.Phone,
                    UserName = userProfile.UserName,
                    InsertDate = currentDateTime,
                    UpdateDate = currentDateTime,
                    UserRoleListItemId = userMemberListItemModel.ListItemId, //default usermember role
                    InsertPersonId = superAdmin.PersonId,
                    UpdatePersonId = superAdmin.PersonId,
                    UserMarketListItemId = null

                };
                await _context.AddAsync(person);
                _context.SaveChanges();


                var personThemeListItemId = await (_iCommonService.GetListItemDetailByListItemSystemName(ThemeListItem.DarkMode.ToString()));
                var nepalStandardTime = TimeZoneInfo.FindSystemTimeZoneById("Nepal Standard Time");
                PersonPersonalizationSetting personPersonalization = new()
                {
                    PersonId = person.PersonId,
                    ThemeListItemId = personThemeListItemId.ListItemId,
                    TimeZone = Convert.ToString(nepalStandardTime)

                };
                await _context.AddAsync(personPersonalization);
                _context.SaveChanges();
                transaction.Commit();

                return new PersonViewModel
                {
                    Name = person.Name,
                    PersonId = person.PersonId,
                    RoleId = person.UserRoleListItemId,
                    Theme = ThemeListItem.DarkMode.ToString(),
                    Timezone = personPersonalization.TimeZone
                };
            }
            catch 
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
