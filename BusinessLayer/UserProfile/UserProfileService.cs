using Data;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Constant.ReturnMessage;
using Models.Core;
using Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.UserProfile
{
    public class UserProfileService : IUserProfileService
    {
        private readonly DataContext _context;
        private readonly IPersonAccessor _iPersonAccessor;
        private readonly ILogger<UserProfileService> _iLogger;

        public UserProfileService(DataContext context,
            IPersonAccessor iPersonAccessor,
            ILogger<UserProfileService> iLogger)
        {
            _context = context;
            _iPersonAccessor = iPersonAccessor;
            _iLogger = iLogger;
        }

        public async Task<Result<PersonPersonalizationListModel>> GetPersonPersonalizationByCurrentPersonIdAsync()
        {

            var userLoginInInfo = await _context.Person.FindAsync(_iPersonAccessor.GetPersonId());
            var listByUserLoginInfo = await (from pp in _context.PersonPersonalizationSetting
                                             join li in _context.ListItem on pp.ThemeListItemId equals li.ListItemId
                                             join p in _context.Person on pp.PersonId equals p.PersonId
                                             where pp.PersonId == userLoginInInfo.PersonId
                                             select new PersonPersonalizationListModel
                                             {
                                                 PersonPersonalizationId = pp.PersonPersonalizationId,
                                                 PersonId = pp.PersonId,
                                                 ThemeListItemId = li.ListItemId,
                                                 ThemeType = li.ListItemSystemName,
                                                 TimeZone = pp.TimeZone
                                             }).FirstOrDefaultAsync();

            if (listByUserLoginInfo != null)
            {
                return Result<PersonPersonalizationListModel>.Success(listByUserLoginInfo);
            }
            else
            {
                return Result<PersonPersonalizationListModel>.Success(null);
            }

        }

        public async Task<Result<UserProfileModel>> GetUserProfileDetailsAsync()
        {
            var userLoginInInfo = await _context.Person.FindAsync(_iPersonAccessor.GetPersonId());
            var getUserProfile = (from p in _context.Person
                                  join li in _context.ListItem on p.UserRoleListItemId equals li.ListItemId
                                  where p.PersonId == userLoginInInfo.PersonId
                                  select new UserProfileModel
                                  {
                                      UserProfileId = p.PersonId,
                                      Name = p.Name,
                                      Email = p.Email,
                                      UserName = p.UserName,
                                      PhoneNumber = p.PhoneNumber,
                                      RoleId = li.ListItemId,
                                      Role = li.ListItemSystemName

                                  }).FirstOrDefault();

            if (getUserProfile != null)
            {
                return Result<UserProfileModel>.Success(getUserProfile);
            }
            else
            {
                return Result<UserProfileModel>.Success(null);
            }
        }

        public async Task<Result<string>> UpdatePersonPersonalizationAsync(PersonPersonalizationModel model)
        {
            try
            {
                await _context.Database.BeginTransactionAsync();
                var personPersonalization = await _context.PersonPersonalizationSetting
                    .FirstOrDefaultAsync(x =>
                    x.PersonPersonalizationId == model.PersonPersonalizationId);
                if (personPersonalization != null)
                {
                    personPersonalization.PersonPersonalizationId = model.PersonPersonalizationId;
                    personPersonalization.PersonId = model.PersonId;
                    personPersonalization.ThemeListItemId = model.ThemeListItemId;
                    personPersonalization.TimeZone = model.TimeZone;
                    _context.PersonPersonalizationSetting.Update(personPersonalization);
                    await _context.SaveChangesAsync();
                    await _context.Database.CommitTransactionAsync();
                    return Result<string>.Success(ReturnMessage.UpdatedSuccessfully);
                }
                else
                {
                    return Result<string>.Success(null);
                }

            }
            catch (Exception ex)
            {

                _iLogger.LogError("Could not update the UpdatePersonPersonalization. Exception message is: ", ex.Message);
                return Result<string>.Error(ReturnMessage.FailedToUpdatePersonPersonalization);
            }
        }
    }
}
