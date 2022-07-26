using API.Services;
using BusinessLayer.Account;
using BusinessLayer.Common;
using Data;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.Account;
using Models.Constant.Authorization;
using Models.Constant.ListItem;
using System;
using System.Linq;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly AuthenticationService _authenticationService;
        private readonly DataContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountService _accountService;
        private readonly IConfiguration _config;
        private readonly ICommonService _iCommonService;

        public AccountController(DataContext context, TokenService tokenService, IAccountService accountService,
                                ILogger<AccountController> logger, AuthenticationService authenticationService,
                                IConfiguration config, ICommonService iCommonService)
        {
            _logger = logger;
            _context = context;
            _tokenService = tokenService;
            _authenticationService = authenticationService;
            _accountService = accountService;
            _config = config;
            _iCommonService = iCommonService;
        }


        [HttpPost("Login")]
        public async Task<ActionResult<PersonModel>> Login(LoginModel loginModel)
        {
            try
            {
                if (loginModel.UserName == SystemUser.UserName)
                {
                    if (loginModel.Password != _config["SystemAdmin:password"])
                    {
                        _logger.LogWarning($"Error. Failed on authentication. Username: {loginModel.UserName} Time: {DateTimeOffset.UtcNow}");
                        return Unauthorized();
                    }
                    else
                    {
                        var person = await GetPerson(loginModel.UserName);
                        return await CreateUserObject(person);
                    }
                }
                else
                {
                    var authServerToken = await _authenticationService.AuthenticateUserAsync(loginModel);
                    if (authServerToken.Errors != null && authServerToken.Data == null)
                    {
                        var errorDetail = authServerToken.Errors.FirstOrDefault()?.Detail;
                        _logger.LogWarning($"Error: {errorDetail}. Username: {loginModel.UserName} Password:{loginModel.Password} Time: {DateTimeOffset.UtcNow}");
                        return Unauthorized();
                    }
                    var person = await GetPerson(loginModel.UserName);
                    if (person == null)
                    {
                        var authServerUserProfile = await _authenticationService.GetUserDetailsAsync(authServerToken.Data.AccessToken);
                        if (authServerUserProfile.Errors != null && authServerUserProfile.Data == null)
                        {
                            var errorDetail = authServerUserProfile.Errors.FirstOrDefault()?.Detail;
                            _logger.LogWarning($"{errorDetail}. Failed while fetching UserDetails from auth server. Username: {loginModel.UserName} Time: {DateTimeOffset.UtcNow}");
                            return Unauthorized();
                        }
                        var newPerson = await _accountService.CreatePersonAsync(authServerUserProfile.Data);
                        return await CreateUserObject(newPerson);
                    }
                    else return await CreateUserObject(person);
                }

            }
            catch
            {
                _logger.LogWarning($"BadRequest: Something went wrong when authenticating. Email: {loginModel.UserName}  Time: {DateTimeOffset.UtcNow}");
                return Unauthorized();
            }
        }

        private async Task<PersonModel> CreateUserObject(PersonViewModel person)
        {
            if (!_context.PersonPersonalizationSetting.Any(x => x.Person.PersonId == person.PersonId))
            {
                var personThemeListItemId = await (_iCommonService.GetListItemDetailByListItemSystemName(ThemeListItem.DarkMode.ToString()));
                var nepalStandardTime = TZConvert.GetTimeZoneInfo("Nepal Standard Time");
                PersonPersonalizationSetting personPersonalization = new()
                {
                    PersonId = person.PersonId,
                    ThemeListItemId = personThemeListItemId.ListItemId,
                    TimeZone = Convert.ToString(nepalStandardTime.DisplayName)

                };
                await _context.AddAsync(personPersonalization);
                _context.SaveChanges();
            }
            var token = await _tokenService.CreateToken(person);
            return new PersonModel
            {
                Name = person.Name,
                PersonId = person.PersonId,
                Token = token
            };
        }

        private async Task<PersonViewModel> GetPerson(string userName)
        {
            return await (from person in _context.Person
                          join personalize in _context.PersonPersonalizationSetting
                          on person.PersonId equals personalize.PersonId into personalization
                          from personalize in personalization.DefaultIfEmpty()
                          join themeList in _context.ListItem
                          on personalize.ThemeListItemId equals themeList.ListItemId into theme
                          from themeList in theme.DefaultIfEmpty()
                          where person.UserName == userName
                          select new PersonViewModel
                          {
                              Name = person.Name,
                              PersonId = person.PersonId,
                              RoleId = person.UserRoleListItemId,
                              Timezone = personalize.TimeZone,
                              Theme = themeList.ListItemSystemName
                          }).FirstOrDefaultAsync();

        }
    }
}