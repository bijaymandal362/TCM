using BusinessLayer.Common;

using Data;
using Data.Exceptions;
using Data.MenuRoleSeed;

using Infrastructure;
using Infrastructure.Helper.Excel;
using Infrastructure.Helper.ExcelExport;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Constant.Dashboard;
using Models.Constant.ListItem;
using Models.Constant.ReturnMessage;
using Models.Core;
using Models.Dashboard;
using Models.Enum;
using Models.GridTableFilterModel;
using Models.GridTableProperty;
using Models.Import;
using Models.Project;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace BusinessLayer.Project
{
    public class ProjectService : IProjectService


    {
        private readonly DataContext _context;
        private readonly ILogger<ProjectService> _logger;
        private readonly IPersonAccessor _iPersonAccessor;
        private readonly ICommonService _commonService;
        public ProjectService(DataContext context, ILogger<ProjectService> logger, IPersonAccessor iPersonAccessor, ICommonService commonService)
        {
            _context = context;
            _logger = logger;
            _iPersonAccessor = iPersonAccessor;
            _commonService = commonService;
        }

        public async Task<Result<List<ListProjectModel>>> GetProjectListAsync()
        {
            var currentPersonId = (_iPersonAccessor.GetPersonId());
            bool admin = false;
            admin = await IsAdmin(currentPersonId);

            bool onsite = false;
            onsite = await IsOnsite(currentPersonId);

            List<ListProjectModel> result;
            if (!admin)
            {
                if (onsite)
                {
                    var test = await (from p in _context.Person
                                      where p.PersonId == currentPersonId
                                      select new
                                      {
                                          PersonMarket = p.UserMarketListItemId
                                      }).FirstOrDefaultAsync();

                    result = await (from p in _context.Project
                                    where p.ProjectMarketListItemId == test.PersonMarket
                                    orderby p.UpdateDate descending

                                    select new ListProjectModel
                                    {
                                        ProjectId = p.ProjectId,
                                        ProjectName = p.ProjectName,
                                        ProjectSlug = p.ProjectSlug,
                                        ProjectMarketId = p.ProjectMarketListItemId,
                                        ProjectMarketName = p.ProjectMarketListItem.ListItemSystemName
                                    }).ToListAsync();
                }
                else
                {
                    result = await (from p in _context.Project
                                    join pm in _context.ProjectMember on p.ProjectId equals pm.ProjectId
                                    where pm.PersonId == currentPersonId
                                    orderby p.UpdateDate descending
                                    select new ListProjectModel
                                    {
                                        ProjectId = p.ProjectId,
                                        ProjectName = p.ProjectName,
                                        ProjectSlug = p.ProjectSlug,
                                        ProjectMarketId = p.ProjectMarketListItemId,
                                        ProjectMarketName = p.ProjectMarketListItem.ListItemSystemName
                                    }).ToListAsync();

                }
            }
            else
            {
                result = await (from p in _context.Project
                                orderby p.UpdateDate descending
                                select new ListProjectModel
                                {
                                    ProjectId = p.ProjectId,
                                    ProjectName = p.ProjectName,
                                    ProjectSlug = p.ProjectSlug,
                                    ProjectMarketId = p.ProjectMarketListItemId,
                                    ProjectMarketName = p.ProjectMarketListItem.ListItemSystemName
                                }).ToListAsync();

            }

            if (result.Any())
            {
                return Result<List<ListProjectModel>>.Success(result);
            }
            else
            {
                return Result<List<ListProjectModel>>.Success(null);
            }
        }
        public async Task<Result<string>> AddProjectAsync(ProjectModel model)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(model.ProjectName) || string.IsNullOrWhiteSpace(model.ProjectName))
                {
                    throw new ProjectNameCannotBeEmptyException();
                }
                else
                {
                    var addProject = new Entities.Project();
                    addProject.ProjectName = model.ProjectName.Trim();
                    if (addProject.ProjectName.ToLower() == ListItem.Admin.ToString().ToLower())
                    {
                        throw new ProjectNameCannotBeAdminException();
                    }
                    var projectNameValidation = await _context.Project.AnyAsync(x => x.ProjectName.Trim() == addProject.ProjectName);
                    if (projectNameValidation)
                    {
                        throw new ProjectNameisalreadyexistsPleaseSpecifyAUniqueNameException();
                    }
                    addProject.StartDate = model.StartDate;
                    addProject.ProjectMarketListItemId = model.ProjectMarketListItemId;
                    addProject.ProjectDescription = model.ProjectDescription;
                    string projectSlug = addProject.ProjectName.Replace(" ", "-").ToLower();
                    var checkForprojectSlug = await _context.Project.AnyAsync(x => x.ProjectSlug == projectSlug);
                    if (checkForprojectSlug)
                    {
                        int lastNumber = 1;
                        var projectSlugValue = projectSlug;
                        var newProjectSlug = projectSlugValue + "-" + lastNumber.ToString();
                        var checkNewProjectSlugIfExists = _context.Project.Any(x => x.ProjectSlug == newProjectSlug);
                        if (checkNewProjectSlugIfExists)
                        {
                            addProject.ProjectSlug = newProjectSlug + "-" + lastNumber;
                        }
                        else
                        {
                            addProject.ProjectSlug = newProjectSlug;
                        }
                    }
                    else
                    {
                        addProject.ProjectSlug = projectSlug;
                    }

                    await _context.Project.AddAsync(addProject);
                    await _context.SaveChangesAsync();

                    var ownerListItemModel = await (_commonService.GetListItemDetailByListItemSystemName(ListItem.Owner.ToString()));

                    var projectMember = new Entities.ProjectMember
                    {
                        ProjectId = addProject.ProjectId,
                        PersonId = addProject.InsertPersonId,
                        ProjectRoleListItemId = ownerListItemModel.ListItemId
                    };

                    await _context.ProjectMember.AddAsync(projectMember);
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    return Result<string>.Success(ReturnMessage.SavedSuccessfully);
                }

            }

            catch (Exception ex)
            {
                _logger.LogError("Could not add the Project. Exception message is: ", ex.Message);
                transaction.Rollback();
                if (ex is ProjectNameCannotBeAdminException)
                {
                    return Result<string>.Error(ReturnMessage.ProjectNameCannotBeAdmin);

                }
                else if (ex is ProjectNameisalreadyexistsPleaseSpecifyAUniqueNameException)
                {
                    return Result<string>.Error(ReturnMessage.ProjectNameisalreadyexistsPleasespecifyauniquename);
                }
                else if (ex is ProjectNameCannotBeEmptyException)
                {
                    return Result<string>.Error(ReturnMessage.ProjectNameCannotBeEmpty);
                }

                return Result<string>.Error(ReturnMessage.FailedToAddProject);
            }
        }
        public async Task<Result<string>> UpdateProjectAsync(ProjectModel model)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var updateProject = await _context.Project.Where(x => x.ProjectId == model.ProjectId).FirstOrDefaultAsync();
                if (updateProject != null)
                {
                    if (string.IsNullOrEmpty(model.ProjectName) || string.IsNullOrWhiteSpace(model.ProjectName))
                    {
                        throw new ProjectNameCannotBeEmptyException();
                    }
                    else
                    {
                        updateProject.ProjectId = model.ProjectId;

                        updateProject.ProjectName = model.ProjectName.Trim();
                        if (updateProject.ProjectName.ToLower() == ListItem.Admin.ToString().ToLower())
                        {
                            throw new ProjectNameCannotBeAdminException();
                        }
                        var projectSlug = updateProject.ProjectName.Trim().Replace(' ', '-').ToLower();
                        var checkForprojectSlug = await _context.Project.AnyAsync(x => x.ProjectSlug == projectSlug);
                        if (checkForprojectSlug)
                        {
                            int lastNumber = 1;
                            var projectSlugValue = projectSlug;
                            var newProjectSlug = projectSlugValue + "-" + lastNumber.ToString();
                            var checkNewProjectSlugIfExists = _context.Project.Any(x => x.ProjectSlug == newProjectSlug);
                            if (checkNewProjectSlugIfExists)
                            {
                                updateProject.ProjectSlug = newProjectSlug + "-" + lastNumber;
                            }
                            else
                            {
                                updateProject.ProjectSlug = newProjectSlug;
                            }
                        }
                        else
                        {
                            updateProject.ProjectSlug = projectSlug;
                        }
                        updateProject.ProjectMarketListItemId = model.ProjectMarketListItemId;
                        updateProject.ProjectDescription = model.ProjectDescription;
                        _context.Project.Update(updateProject);
                        await _context.SaveChangesAsync();
                        transaction.Commit();

                        return Result<string>.Success(ReturnMessage.UpdatedSuccessfully);
                    }
                }
                else
                {
                    return Result<string>.Success(null);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Could not update the Project. Exception message is: ", ex.Message);
                transaction.Rollback();
                if (ex is ProjectNameCannotBeAdminException)
                {
                    return Result<string>.Error(ReturnMessage.ProjectNameCannotBeAdmin);
                }
                else if (ex is ProjectNameCannotBeEmptyException)
                {
                    return Result<string>.Error(ReturnMessage.ProjectNameCannotBeEmpty);
                }
                return Result<string>.Error(ReturnMessage.FailedToUpdateProject);
            }
        }
        public async Task<Result<GetAllListProjectModel>> GetProjectByProjectIdAsync(int projectId)
        {
            var currentPersonId = (_iPersonAccessor.GetPersonId());
            bool admin = false;
            admin = await IsAdmin(currentPersonId);
            if (admin)
            {
                var result = await GetFilterProjectListForAdminAsync().Where(x => x.ProjectId == projectId).FirstOrDefaultAsync();
                if (result != null)
                {
                    return Result<GetAllListProjectModel>.Success(result);
                }
                else
                {
                    return Result<GetAllListProjectModel>.Success(null);
                }
            }
            else
            {
                var onsite = await IsOnsite(currentPersonId);
                if (onsite)
                {
                    var result = await GetFilterProjectListForOnsiteAsync(currentPersonId).Where(x => x.ProjectId == projectId).FirstOrDefaultAsync();
                    if (result != null)
                    {
                        return Result<GetAllListProjectModel>.Success(result);
                    }
                    else
                    {
                        return Result<GetAllListProjectModel>.Success(null);
                    }

                }
                else
                {
                    var result = await GetFilterProjectListForNotAdminAsync(currentPersonId).Where(x => x.ProjectId == projectId).FirstOrDefaultAsync();
                    if (result != null)
                    {
                        return Result<GetAllListProjectModel>.Success(result);
                    }
                    else
                    {
                        return Result<GetAllListProjectModel>.Success(null);
                    }
                }

            }
        }

        public async Task<Result<string>> DeleteProjectAsync(int projectId)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {

                var project = await _context.Project.Where(x => x.ProjectId == projectId).FirstOrDefaultAsync();
                if (project != null)
                {

                    var projectMember = _context.ProjectMember.Where(x => x.ProjectMemberId == project.ProjectId).FirstOrDefault();
                    if (projectMember != null)
                    {
                        _context.ProjectMember.Remove(projectMember);

                    }

                    _context.Project.Remove(project);
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    return Result<string>.Success(ReturnMessage.DeletedSuccessfully);


                }
                else
                {
                    return Result<string>.Success(null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not delete the Project. Exception message is: ", ex.Message);
                transaction.Rollback();
                return Result<string>.Error(ReturnMessage.FailedToDeleteProject);
            }

        }
        public async Task<Result<PagedResponseModel<List<GetAllListProjectModel>>>> GetProjectListFilterByModuleAsync(PaginationFilterModel filter)
        {
            var currentPersonId = (_iPersonAccessor.GetPersonId());
            bool admin = false;
            bool onsite = false;
            admin = await IsAdmin(currentPersonId);
            onsite = await IsOnsite(currentPersonId);
            IQueryable<GetAllListProjectModel> getProjectListAllAsync;
            var testCaseList = await _context.ProjectModule.Where(x => x.ProjectModuleListItem.ListItemSystemName == ListItem.TestCase.ToString() && x.IsDeleted == false).ToListAsync();

            var testRunList = await _context.TestRun.ToListAsync();

            var testPlanList = await _context.TestPlan.Where(x => x.TestPlanTypeListItem.ListItemSystemName == ListItem.TestPlan.ToString() && x.IsDeleted == false).ToListAsync();


            if (admin)
            {
                getProjectListAllAsync = GetFilterProjectListForAdminAsync();

            }
            else
            {
                if (onsite)
                {
                    getProjectListAllAsync = GetFilterProjectListForOnsiteAsync(currentPersonId);


                }
                else
                {
                    getProjectListAllAsync = GetFilterProjectListForNotAdminAsync(currentPersonId);


                }
            }

            if (!string.IsNullOrEmpty(filter.SearchValue))
            {
                getProjectListAllAsync = getProjectListAllAsync.Where
                    (
                        x => x.ProjectName.ToLower().Contains(filter.SearchValue.ToLower())
                    );
            }
            var records = getProjectListAllAsync;
            var totalRecords = records.Count();
            var filteredData = getProjectListAllAsync
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            foreach (var itemInFilteredData in filteredData)
            {
                itemInFilteredData.TestCaseCount = GetTestCaseCount(testCaseList, itemInFilteredData.ProjectId);
                itemInFilteredData.TestPlanCount = GetTestPlanCount(testPlanList, itemInFilteredData.ProjectId);
                itemInFilteredData.TestRunCount = GetTestRunCount(testRunList, itemInFilteredData.ProjectId);
            }
            if (filter.PageSize > totalRecords && totalRecords > 0)
            {
                filter.PageSize = totalRecords;
            }

            var totalPages = (totalRecords / filter.PageSize);

            var data = new PagedResponseModel<List<GetAllListProjectModel>>(filteredData, filter.PageNumber, filter.PageSize, totalRecords, totalPages);

            if (filteredData.Count > 0)
            {
                return Result<PagedResponseModel<List<GetAllListProjectModel>>>.Success(data);
            }
            else
            {
                return Result<PagedResponseModel<List<GetAllListProjectModel>>>.Success(null);
            }

        }
        public async Task<Result<List<string>>> GetProjectRolePermissionAsync(string projectSlug)
        {
            try
            {
                var projectSlugIfExist = _context.Project.Where(x => x.ProjectSlug == projectSlug).FirstOrDefault();
                if (projectSlugIfExist != null)
                {
                    var currentPersonId = _iPersonAccessor.GetPersonId();
                    bool isAdmin = false;
                    isAdmin = await IsAdmin(currentPersonId);
                    List<string> listProjectRolePermissionSlugs = new();
                    if (isAdmin)
                    {
                        listProjectRolePermissionSlugs = ProjectRolePermission.GetProjectRolePermission(ProjectRoleListItem.Owner);
                    }
                    else
                    {
                        var projectRole = await (from project in _context.Project
                                                 join projectMember in _context.ProjectMember
                                                 on project.ProjectId equals projectMember.ProjectId
                                                 join listItem in _context.ListItem
                                                 on projectMember.ProjectRoleListItemId equals listItem.ListItemId
                                                 where project.ProjectSlug == projectSlug && projectMember.PersonId == currentPersonId
                                                 select listItem.ListItemSystemName).FirstOrDefaultAsync();
                        if (!string.IsNullOrEmpty(projectRole))
                        {
                            listProjectRolePermissionSlugs = ProjectRolePermission.GetProjectRolePermission(projectRole);
                        }
                    }
                    return Result<List<string>>.Success(listProjectRolePermissionSlugs);
                }
                else
                {
                    throw new ProjectSlugNotFoundException();
                }

            }
            catch (Exception ex)
            {
                if (ex is ProjectSlugNotFoundException)
                {
                    _logger.LogError("Could not find the ProjectSlug. Exception message is: ", ex.Message);
                    return Result<List<string>>.Error(ReturnMessage.ProjectSlugDoesNotExists);
                }
                return Result<List<string>>.Error(ReturnMessage.UnauthorizedUser);
            }

        }

        private int GetTestRunCount(List<Entities.TestRun> testRunList, int projectId)
        {
            var testRunCount = testRunList.Where(x => x.ProjectId == projectId).ToList().Count;
            int count = testRunCount == 0 ? 0 : testRunCount;
            return count;
        }

        private int GetTestPlanCount(List<Entities.TestPlan> testPlanlist, int projectId)
        {
            var testPlanCount = testPlanlist.Where(x => x.ProjectId == projectId).ToList().Count;
            int count = testPlanCount == 0 ? 0 : testPlanCount;
            return count;
        }

        private int GetTestCaseCount(List<Entities.ProjectModule> list, int projectId)
        {
            var testCaseCount = list.Where(x => x.ProjectId == projectId).ToList().Count;
            int count = testCaseCount == 0 ? 0 : testCaseCount;
            return count;
        }
        private async Task<bool> IsAdmin(int currentPersonId)
        {
            return await (from person in _context.Person
                          join personRole in _context.ListItem
                          on person.UserRoleListItemId equals personRole.ListItemId
                          where person.PersonId == currentPersonId && personRole.ListItemSystemName == UserRoleListItem.Admin
                          select person.PersonId).AnyAsync();
        }
        private async Task<bool> IsOnsite(int currentPersonId)
        {
            return await (from person in _context.Person
                          join personRole in _context.ListItem
                          on person.UserRoleListItemId equals personRole.ListItemId
                          where person.PersonId == currentPersonId && personRole.ListItemSystemName == UserRoleListItem.Onsite
                          select person.PersonId).AnyAsync();
        }
        private IQueryable<GetAllListProjectModel> GetFilterProjectListForNotAdminAsync(int currentPersonId)
        {


            var result = from p in _context.Project
                         join pm in _context.ProjectMember on p.ProjectId equals pm.ProjectId into projectMember
                         from pm in projectMember.DefaultIfEmpty()
                         join li in _context.ListItem on p.ProjectMarketListItemId equals li.ListItemId into projectMarketListItemId
                         from li in projectMarketListItemId.DefaultIfEmpty()
                         join lit in _context.ListItem on pm.ProjectRoleListItemId equals lit.ListItemId into projectRoleListItemId
                         from lit in projectRoleListItemId.DefaultIfEmpty()
                         where pm.PersonId == currentPersonId
                         orderby p.UpdateDate descending
                         select new GetAllListProjectModel
                         {
                             ProjectId = p.ProjectId,
                             ProjectName = p.ProjectName,
                             ProjectMarketListItemId = p.ProjectMarketListItemId == null ? 0 : p.ProjectMarketListItemId,
                             ProjectMarket = p.ProjectMarketListItem.ListItemSystemName == null ? string.Empty : p.ProjectMarketListItem.ListItemSystemName,
                             ProjectRoleId = pm.ProjectRoleListItemId == null ? 0 : pm.ProjectRoleListItemId,
                             ProjectRole = pm.ProjectRoleListItem.ListItemSystemName == null ? string.Empty : pm.ProjectRoleListItem.ListItemSystemName,
                             Date = p.UpdateDate,
                             ProjectDescription = p.ProjectDescription,
                             ProjectSlug = p.ProjectSlug

                         };
            return result;


        }
        private IQueryable<GetAllListProjectModel> GetFilterProjectListForAdminAsync()
        {
            var list = from p in _context.Project
                       join pm in _context.ProjectMember on p.ProjectId equals pm.ProjectId into projectMember
                       from pm in projectMember.DefaultIfEmpty()
                       join per in _context.Person on pm.PersonId equals per.PersonId into person
                       from per in person.DefaultIfEmpty()
                       join li in _context.ListItem on p.ProjectMarketListItemId equals li.ListItemId into projectMarketListItemId
                       from li in projectMarketListItemId.DefaultIfEmpty()
                       join lit in _context.ListItem on pm.ProjectRoleListItemId equals lit.ListItemId into projectRoleListItemId
                       from lit in projectRoleListItemId.DefaultIfEmpty()
                       orderby p.UpdateDate descending
                       select new
                       {
                           ProjectId = p.ProjectId,
                           ProjectName = p.ProjectName,
                           ProjectMarketListItemId = p.ProjectMarketListItemId == null ? 0 : p.ProjectMarketListItemId,
                           ProjectMarket = p.ProjectMarketListItem.ListItemSystemName == null ? string.Empty : p.ProjectMarketListItem.ListItemSystemName,
                           ProjectRoleId = pm.ProjectRoleListItemId == null ? 0 : pm.ProjectRoleListItemId,
                           ProjectRole = pm.ProjectRoleListItem.ListItemSystemName == null ? string.Empty : pm.ProjectRoleListItem.ListItemSystemName,
                           Date = p.UpdateDate,
                           ProjectDescription = p.ProjectDescription,
                           ProjectSlug = p.ProjectSlug
                       };




            var result = list.GroupBy(x => x.ProjectId).Select(x => new GetAllListProjectModel
            {
                ProjectId = x.Select(x => x.ProjectId).FirstOrDefault(),
                ProjectName = x.Select(x => x.ProjectName).FirstOrDefault(),
                ProjectMarketListItemId = x.Select(x => x.ProjectMarketListItemId).FirstOrDefault(),
                ProjectRoleId = x.Select(x => x.ProjectRoleId).FirstOrDefault(),
                ProjectRole = x.Select(x => x.ProjectRole).FirstOrDefault(),
                Date = x.Select(x => x.Date).FirstOrDefault(),
                ProjectMarket = x.Select(x => x.ProjectMarket).FirstOrDefault(),
                ProjectDescription = x.Select(x => x.ProjectDescription).FirstOrDefault(),
                ProjectSlug = x.Select(x => x.ProjectSlug).FirstOrDefault(),
            }).OrderByDescending(x => x.Date);




            return result;
        }

        private IQueryable<GetAllListProjectModel> GetFilterProjectListForOnsiteAsync(int currentPersonId)
        {
            var person = (from per in _context.Person
                          where per.PersonId == currentPersonId
                          select new
                          {
                              PersonMarket = per.UserMarketListItemId,
                              PersonId = per.PersonId
                          }).FirstOrDefault();

            var projectRole = (from p in _context.Project
                               join pm in _context.ProjectMember on p.ProjectId equals pm.ProjectId into projectMember
                               from pm in projectMember.DefaultIfEmpty()
                               join li in _context.ListItem on pm.ProjectRoleListItemId equals li.ListItemId into projectRoleListItemId
                               from li in projectRoleListItemId.DefaultIfEmpty()
                               join lit in _context.ListItem on p.ProjectMarketListItemId equals lit.ListItemId into projectMarketListItemId
                               from lit in projectMarketListItemId.DefaultIfEmpty()
                               where p.ProjectMarketListItemId == person.PersonMarket || pm.PersonId == person.PersonId
                               orderby p.UpdateDate descending
                               select new
                               {
                                   ProjectId = p.ProjectId,
                                   ProjectName = p.ProjectName,
                                   ProjectMarketListItemId = p.ProjectMarketListItemId == null ? 0 : p.ProjectMarketListItemId,
                                   ProjectMarket = p.ProjectMarketListItem.ListItemSystemName == null ? string.Empty : p.ProjectMarketListItem.ListItemSystemName,
                                   ProjectRoleId = pm.ProjectRoleListItemId == null ? 0 : pm.ProjectRoleListItemId,
                                   ProjectRole = pm.ProjectRoleListItem.ListItemSystemName == null ? string.Empty : pm.ProjectRoleListItem.ListItemSystemName,
                                   Date = p.UpdateDate,
                                   ProjectDescription = p.ProjectDescription,
                                   ProjectSlug = p.ProjectSlug,
                                   PersonId = pm.PersonId == null ? 0 : pm.PersonId

                               });

            var result = projectRole.GroupBy(x => x.ProjectId).Select(x => new GetAllListProjectModel
            {
                ProjectId = x.Select(x => x.ProjectId).FirstOrDefault(),
                ProjectName = x.Select(x => x.ProjectName).FirstOrDefault(),
                ProjectMarketListItemId = x.Select(x => x.ProjectMarketListItemId).FirstOrDefault(),
                ProjectRoleId = x.Select(x => x.ProjectRoleId).FirstOrDefault(),
                ProjectRole = x.Select(x => x.ProjectRole).FirstOrDefault(),
                Date = x.Select(x => x.Date).FirstOrDefault(),
                ProjectMarket = x.Select(x => x.ProjectMarket).FirstOrDefault(),
                ProjectDescription = x.Select(x => x.ProjectDescription).FirstOrDefault(),
                ProjectSlug = x.Select(x => x.ProjectSlug).FirstOrDefault(),
            }).OrderByDescending(x => x.Date);

            return result;
        }

        public async Task<Result<PagedResponseModel<List<ProjectListViewModel>>>> GetAllProjectListFilterByModuleAsync(PaginationFilterModel filter)
        {
            IQueryable<ProjectListViewModel> getProjectListAllAsync;

            var testCaseList = await _context.ProjectModule.Where(x => x.ProjectModuleListItem.ListItemSystemName == ListItem.TestCase.ToString() && !x.IsDeleted).ToListAsync();

            var testRunList = await _context.TestRun.ToListAsync();

            var testPlanList = await _context.TestPlan.Where(x => x.TestPlanTypeListItem.ListItemSystemName == ListItem.TestPlan.ToString() && !x.IsDeleted).ToListAsync();

            getProjectListAllAsync = (from p in _context.Project
                                      join li in _context.ListItem on
                                      p.ProjectMarketListItemId equals li.ListItemId
                                      orderby p.StartDate descending
                                      select new ProjectListViewModel
                                      {
                                          ProjectId = p.ProjectId,
                                          ProjectName = p.ProjectName,
                                          ProjectMarketName = li.ListItemName,
                                          ProjectStartDate = GetDateInStringFormate(p.StartDate),
                                      });

            if (!string.IsNullOrEmpty(filter.SearchValue) && !string.IsNullOrEmpty(filter.FilterValue))
            {
                getProjectListAllAsync = getProjectListAllAsync
                    .Where(x => x.ProjectName.ToLower().Trim()
                    .Contains(filter.SearchValue.ToLower().Trim())
                    &&
                    x.ProjectMarketName.ToLower().Trim()
                    .Contains(filter.FilterValue.ToLower().Trim())
                    );
            }

            else if (!string.IsNullOrEmpty(filter.SearchValue))
            {
                getProjectListAllAsync = getProjectListAllAsync
                    .Where(x => x.ProjectName.ToLower().Trim()
                    .Contains(filter.SearchValue.ToLower().Trim())
                    );
            }

            else if (!string.IsNullOrEmpty(filter.FilterValue))
            {
                getProjectListAllAsync = getProjectListAllAsync
                    .Where(x => x.ProjectMarketName.ToLower().Trim()
                    .Contains(filter.FilterValue.ToLower().Trim())
                    );
            }

            var records = getProjectListAllAsync;
            var totalRecords = records.Count();
            var filteredData = await getProjectListAllAsync
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            foreach (var itemInFilteredData in filteredData)
            {
                itemInFilteredData.TestCaseCount = GetTestCaseCount(testCaseList, itemInFilteredData.ProjectId);
                itemInFilteredData.TestPlanCount = GetTestPlanCount(testPlanList, itemInFilteredData.ProjectId);
                itemInFilteredData.TestRunCount = GetTestRunCount(testRunList, itemInFilteredData.ProjectId);
            }

            if (filter.PageSize > totalRecords && totalRecords > 0)
            {
                filter.PageSize = totalRecords;
            }

            int totalPages = (totalRecords - 1) / filter.PageSize + 1;

            var data = new PagedResponseModel<List<ProjectListViewModel>>(filteredData, filter.PageNumber, filter.PageSize, totalRecords, totalPages);

            return Result<PagedResponseModel<List<ProjectListViewModel>>>.Success(data);
        }

        #region helper
        private static string GetDateInStringFormate(DateTimeOffset dateTime)
        {
            string dateInString = dateTime.ToString();
            string date = dateInString.Split(" ").First();
            return date;
        }

        #endregion
    }

}
