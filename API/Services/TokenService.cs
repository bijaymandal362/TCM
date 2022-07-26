using Data;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.Account;
using Models.Constant.Authorization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace API.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;
        private readonly DataContext _context;
        public TokenService(IConfiguration config, DataContext context)
        {
            _config = config;
            _context = context;
        }

        public async Task<string> CreateToken(PersonViewModel person)
        {
            var roleMenuPermissions = await (from menuPermission in _context.MenuPermission
                                             join menu in _context.Menu
                                             on menuPermission.MenuId equals menu.MenuId
                                             join listItem in _context.ListItem
                                             on menuPermission.RoleId equals listItem.ListItemId
                                             where menuPermission.RoleId == person.RoleId
                                             select new { MenuSlug = menu.MenuSlug, RoleName = listItem.ListItemSystemName }).ToListAsync();

            var roleName = roleMenuPermissions.Select(x => x.RoleName).FirstOrDefault();
            var menuPermissions = roleMenuPermissions.Where(x => x.MenuSlug != null).Select(x => x.MenuSlug).ToList();

            var claims = new List<Claim>{
              new Claim(ClaimTypes.Name, person.Name),
              new Claim(ClaimTypes.NameIdentifier,person.PersonId.ToString()),
              new Claim(TokenKey.RoleId, person.RoleId.ToString()),
              new Claim(TokenKey.RoleName, roleName),
              new Claim(TokenKey.Timezone, person.Timezone),
              new Claim(TokenKey.Theme, person.Theme),
              new Claim(TokenKey.Permission, JsonConvert.SerializeObject(menuPermissions)),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(Convert.ToInt32(_config["Jwt:ExpiresInDays"])),
                SigningCredentials = credentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}