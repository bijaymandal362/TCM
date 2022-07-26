using BusinessLayer.Common;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Constant.ListItem;
using Models.Constant.ReturnMessage;
using Models.Core;
using Models.GridTableFilterModel;
using Models.GridTableProperty;
using Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.User
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly ILogger<UserService> _logger;
        private readonly ICommonService _iCommonService;

        public UserService(DataContext context, ILogger<UserService> logger, ICommonService iCommonService)
        {
            _context = context;
            _logger = logger;
            _iCommonService = iCommonService;
        }
        public async Task<Result<List<UserListModel>>> GetUserListAsync()
        {
            var projectList = await (from p in _context.Person
                                     join pm in _context.ProjectMember on p.PersonId equals pm.PersonId into proj
                                     from pm in proj.DefaultIfEmpty()
                                     group new { p, pm } by new { p.PersonId, p.Name, p.UserRoleListItemId, p.UserRoleListItem.ListItemSystemName, p.UserMarketListItemId, p.UserMarketListItem.ListItemName } into g
                                     select new UserListModel
                                     {
                                         UserId = g.Key.PersonId,
                                         Name = g.Key.Name,
                                         ProjectCount = g.Count(x => x.pm != null),
                                         CreatedOn = g.Max(x => x.p.InsertDate),
                                         LastActivity = g.Max(x => x.p.UpdateDate),
                                         UserRoleListItemId = g.Key.UserRoleListItemId,
                                         UserRoleName = g.Key.ListItemSystemName,
                                         UserMarketId = g.Key.UserMarketListItemId == null ? 0 : (int)g.Key.UserMarketListItemId,
                                         UserMarket = g.Key.ListItemName == null ? "" : g.Key.ListItemName
                                     }).ToListAsync();


            if (projectList.Any())
            {
                return Result<List<UserListModel>>.Success(projectList);
            }
            else
            {
                return Result<List<UserListModel>>.Success(null);
            }

        }

        public async Task<Result<PagedResponseModel<List<UserModel>>>> GetUserListFilterByRoleAsync(PaginationFilterModel filter, int? roleId)
        {
            IQueryable<UserModel> getUserListFilterByRoleAsync;


            getUserListFilterByRoleAsync = from p in _context.Person
                                           join pm in _context.ProjectMember on p.PersonId equals pm.PersonId into proj
                                           from pm in proj.DefaultIfEmpty()

                                           group new { p, pm } by new { p.PersonId, p.Name, p.Email, p.UserRoleListItemId, p.UserRoleListItem.ListItemSystemName } into g
                                           select new UserModel
                                           {
                                               UserId = g.Key.PersonId,
                                               Name = g.Key.Name,
                                               Email = g.Key.Email,
                                               ProjectCount = g.Count(x => x.pm != null),
                                               CreatedOn = g.Max(x => x.p.InsertDate),
                                               LastActivity = g.Max(x => x.p.UpdateDate),
                                               UserRoleListItemId = g.Key.UserRoleListItemId,
                                               RoleName = g.Key.ListItemSystemName
                                           };

            if (roleId != null)
            {

                getUserListFilterByRoleAsync = getUserListFilterByRoleAsync.Where
                    (
                        x => x.UserRoleListItemId == roleId
                    );

            }

            if (!string.IsNullOrEmpty(filter.SearchValue))
            {
                getUserListFilterByRoleAsync = getUserListFilterByRoleAsync.Where
                    (
                          x => x.Name.ToLower().Contains(filter.SearchValue.ToLower())

                    );
            }

           
            var filteredData = await getUserListFilterByRoleAsync
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var totalRecords = filteredData.Count;

            if (filter.PageSize > totalRecords && totalRecords > 0)
            {
                filter.PageSize = totalRecords;
            }

            var totalPages = (totalRecords / filter.PageSize);

            var data = new PagedResponseModel<List<UserModel>>(filteredData, filter.PageNumber, filter.PageSize, totalRecords, totalPages);

            if (filteredData.Count > 0)
            {
                return Result<PagedResponseModel<List<UserModel>>>.Success(data);
            }
            else
            {
                return Result<PagedResponseModel<List<UserModel>>>.Success(null);
            }
        }

        public async Task<Result<List<UserRoleListModel>>> GetRoleListAsync()
        {

            var getRoleList = await (from lt in _context.ListItem
                                     join p in _context.Person on lt.ListItemId equals p.UserRoleListItemId into pr
                                     from p in pr.DefaultIfEmpty()
                                     where lt.ListItemCategory.ListItemCategorySystemName == Models.Enum.ListItemCategory.UserRole.ToString()
                                     group new { lt, p } by new { lt.ListItemId, lt.ListItemName } into g
                                     select new UserRoleListModel
                                     {
                                         RoleId = g.Key.ListItemId,
                                         Name = g.Key.ListItemName,
                                         UserCount = g.Count(x => x.p != null),
                                     }).ToListAsync();

            if (getRoleList.Any())
            {
                return Result<List<UserRoleListModel>>.Success(getRoleList);
            }
            else
            {
                return Result<List<UserRoleListModel>>.Success(null);
            }
        }

        public async Task<Result<PagedResponseModel<List<UserModel>>>> GetUserListFilterByNameAsync(PaginationFilterModel filter)
        {

            var getUserListFilterByNameAsync = from p in _context.Person
                                               join pm in _context.ProjectMember on p.PersonId equals pm.PersonId into proj
                                               from pm in proj.DefaultIfEmpty()
                                               group new { p, pm } by new { p.PersonId, p.Name, p.UserRoleListItemId, p.UserRoleListItem.ListItemSystemName } into g
                                               select new UserModel
                                               {
                                                   UserId = g.Key.PersonId,
                                                   Name = g.Key.Name,
                                                   ProjectCount = g.Count(x => x.pm != null),
                                                   CreatedOn = g.Max(x => x.p.InsertDate),
                                                   LastActivity = g.Max(x => x.p.UpdateDate),
                                                   UserRoleListItemId = g.Key.UserRoleListItemId,
                                                   RoleName = g.Key.ListItemSystemName
                                               };

            if (!string.IsNullOrEmpty(filter.SearchValue))
            {
                getUserListFilterByNameAsync = getUserListFilterByNameAsync.Where
                    (
                        x => EF.Functions.ILike(x.Name.ToLower(), $"%{filter.SearchValue.ToLower()}%")
                                        );
            }

            var records = await getUserListFilterByNameAsync.ToListAsync();

            var filteredData = records
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();
            var totalRecords = filteredData.Count;
            if (filter.PageSize > totalRecords && totalRecords > 0)
            {
                filter.PageSize = totalRecords;
            }

            var totalPages = (totalRecords / filter.PageSize);

            var data = new PagedResponseModel<List<UserModel>>(filteredData, filter.PageNumber, filter.PageSize, totalRecords, totalPages);

            if (filteredData.Count > 0)
            {
                return Result<PagedResponseModel<List<UserModel>>>.Success(data);
            }
            else
            {
                return Result<PagedResponseModel<List<UserModel>>>.Success(null);
            }

        }

        public async Task<Result<string>> UpdateUserAsync(UserUpdateModel model)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var updateUser = await _context.Person.Include(x => x.UserRoleListItem).Include(y => y.UserMarketListItem)
                                                      .Where(x => x.PersonId == model.UserId).FirstOrDefaultAsync();
                if (updateUser != null)
                {

                    updateUser.UserRoleListItemId = model.UserRoleListItemId;
                    var onsiteListItem = await  _iCommonService.GetListItemDetailByListItemSystemName(UserRoleListItem.Onsite.ToString());
                    if (model.UserRoleListItemId == onsiteListItem.ListItemId)
                    {
                        updateUser.UserMarketListItemId = model.UserMarketListItemId == 0 ? null : model.UserMarketListItemId;

                    }
                    else
                    {
                        updateUser.UserMarketListItemId = null;
                    }
                }

                else
                {
                    return Result<string>.Success(null);
                }
                _context.Person.Update(updateUser);
                await _context.SaveChangesAsync();
                transaction.Commit();
                return Result<string>.Success(ReturnMessage.UpdatedSuccessfully);
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not update the User. Exception message is: ", ex.Message);
                transaction.Rollback();
                return Result<string>.Error(ReturnMessage.FailedToUpdateUser);
            }
        }

        public async Task<Result<UserDetailModel>> GetUserDetailByIdAsync(int userId)
        {

            var result = await (from p in _context.Person
                                join lt in _context.ListItem on p.UserRoleListItemId equals lt.ListItemId
                                join li in _context.ListItem on p.UserMarketListItemId equals li.ListItemId
                                into list
                                from m in list.DefaultIfEmpty()
                                where p.PersonId == userId
                                select new UserDetailModel
                                {
                                    UserId = p.PersonId,
                                    Name = p.Name,
                                    Email = p.Email,
                                    Username = p.UserName,
                                    RoleId = lt.ListItemId,
                                    Role = lt.ListItemName,
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                                    MarketId = m.ListItemId == null ? 0 : m.ListItemId,
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                                    Market = m.ListItemSystemName == null ? string.Empty : m.ListItemSystemName

                                }).FirstOrDefaultAsync();
            if (result != null)
            {
                return Result<UserDetailModel>.Success(result);
            }
            else
            {
                return Result<UserDetailModel>.Success(null);
            }
        }

    }
}
