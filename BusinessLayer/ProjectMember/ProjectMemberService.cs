using BusinessLayer.Common;
using Data;
using Data.Exceptions;
using Infrastructure;
using Infrastructure.Helper.Exceptions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Constant.Authorization;
using Models.Constant.ReturnMessage;
using Models.Core;
using Models.Enum;
using Models.GridTableFilterModel;
using Models.GridTableProperty;
using Models.ProjectMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.ProjectMember
{
    public class ProjectMemberService : IProjectMemberService
    {
        private readonly DataContext _context;
        private readonly ILogger<ProjectMemberService> _logger;
        private readonly IPersonAccessor _iPersonAccessor;
        private readonly ICommonService _iCommonService;

        public ProjectMemberService(
            DataContext context,
            ILogger<ProjectMemberService> logger,
            IPersonAccessor iPersonAccessor,
            ICommonService iCommonService
            )
        {
            _context = context;
            _logger = logger;
            _iPersonAccessor = iPersonAccessor;
            _iCommonService = iCommonService;
        }

        public async Task<Result<string>> AddProjectMemberAsync(ProjectMemberModel model)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var projectId = _context.Project.FirstOrDefault(x => x.ProjectSlug == model.ProjectSlug).ProjectId;
                var project = _context.Project.Where(x => x.ProjectSlug == model.ProjectSlug).FirstOrDefault();

                bool hasProjectPermission = await _iCommonService.CheckProjectPermission(model.ProjectSlug, ProjectRoleSlug.CreateProjectMember);
                if (hasProjectPermission)
                {

                    bool personValid = await _context.ProjectMember.Include(x=>x.Project).Where(x => x.Project.ProjectSlug == model.ProjectSlug && model.PersonId.Contains(x.PersonId)).AnyAsync();

                    if (personValid == true)
                    {
                        throw new PersonAlreadyExistException();
                    }
                    else
                    {
                        List<int> personIds = model.PersonId;
                        var memberPersonId = personIds.Select(x => new Entities.ProjectMember
                        {
                            PersonId = x,
                            ProjectId = projectId,
                            ProjectRoleListItemId = model.ProjectRoleListItemId
                        }).ToList();
                        project.UpdateDate = DateTimeOffset.UtcNow;
                        await _context.AddRangeAsync(memberPersonId);
                        await _context.SaveChangesAsync();
                        transaction.Commit();
                        return Result<string>.Success(ReturnMessage.UserAddedSuccessfully);
                    }
                }
                else
                {
                    throw new UnAuthorizedUserException();
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                if (ex is UnAuthorizedUserException)
                {
                    _logger.LogError("Unauthorized User. Exception message is: ", ex.Message);
                    return Result<string>.Error(ReturnMessage.UnauthorizedUser);
                }
				else if (ex is PersonAlreadyExistException)
				{
                    _logger.LogError("Project member already added!. Exception message is: ", ex.Message);
                    return Result<string>.Error(ReturnMessage.PersonAlreadyExist);
                }
                else 
                {
                    _logger.LogError("Could not invite the projectmember. Exception message is: ", ex.Message);
                    return Result<string>.Error(ReturnMessage.FailedToAddProjectMember);
                }
            }
        }

        public async Task<Result<string>> DeleteProjectMemberAsync(int projectMemberId)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var projectMember = await _context.ProjectMember.Include(x => x.Project).Where(x => x.ProjectMemberId == projectMemberId).FirstOrDefaultAsync();
                var project = _context.Project.Where(x => x.ProjectId == projectMember.ProjectId).FirstOrDefault();
                if (projectMember != null)
                {
                    bool hasProjectPermission = await _iCommonService.CheckProjectPermission(projectMember.Project.ProjectSlug, ProjectRoleSlug.DeleteProjectMember);
                    if (hasProjectPermission)
                    {
                        var deleteProjectMemberAsync = await _context.ProjectMember.Include(x => x.ProjectRoleListItem).FirstOrDefaultAsync(x => x.ProjectMemberId == projectMemberId);
                        if (deleteProjectMemberAsync != null)
                        {
                            if (deleteProjectMemberAsync.ProjectRoleListItem.ListItemSystemName == ListItem.Owner.ToString())
                            {
                                throw new OwnerCannotDeleteTheOwnerException();
                            }
                            else
                            {
                                _context.Remove(deleteProjectMemberAsync);
                                project.UpdateDate = DateTimeOffset.UtcNow;
                                await _context.SaveChangesAsync();
                                transaction.Commit();
                                return Result<string>.Success(ReturnMessage.UserDeletedSuccessfully);
                            }
                        }
                        else
                        {
                            return Result<string>.Success(null);
                        }
                    }
                    else
                    {
                        throw new UnAuthorizedUserException();
                    }
                }
                else
                {
                    throw new ProjectMemberIdNotFoundException();
                }

            }
            catch (Exception ex)
            {

                transaction.Rollback();
                if (ex is OwnerCannotDeleteTheOwnerException)
                {
                    _logger.LogError("Could not delete the ProjectMember. Exception message is: ", ex.Message);
                    return Result<string>.Error("Failed to delete the projectMember, owner cannot delete the owner");
                }
                else if (ex is UnAuthorizedUserException)
                {
                    _logger.LogError("Unauthorized User. Exception message is: ", ex.Message);
                    return Result<string>.Error(ReturnMessage.UnauthorizedUser);
                }
                else if (ex is ProjectMemberIdNotFoundException)
                {
                    _logger.LogError("Could not find the ProjectMemberId. Exception message is: ", ex.Message);
                    return Result<string>.Error(ReturnMessage.ProjectMemberIdNotFound);
                }
                else
                {
                    _logger.LogError("Failed to delete the ProjectMember. Exception message is: ", ex.Message);
                    return Result<string>.Error(ReturnMessage.FailedToDeleteProjectMember);
                }
            }
        }


        public async Task<Result<List<ProjectMemberInviteModel>>> GetProjectMemberListAllAsync()
        {

            var getProjectMemberListAsync = await GetAllProjectMemberAsync().ToListAsync();

            if (getProjectMemberListAsync.Any())
            {
                return Result<List<ProjectMemberInviteModel>>.Success(getProjectMemberListAsync);
            }
            else
            {
                return Result<List<ProjectMemberInviteModel>>.Success(null);
            }


        }
        public async Task<Result<PagedResponseProjectMemberModel<List<ProjectMemberInviteModel>>>> GetProjectMemberListFilterByProjectIdAsync(PaginationFilterModel filter, string projectSlug)
        {
            try
            {
                var projectId = await _context.Project.FirstOrDefaultAsync(x => x.ProjectSlug == projectSlug);
                if (projectId != null)
                {

                    //New Project Role Permission Check
                    bool hasProjectPermission = await _iCommonService.CheckProjectPermission(projectSlug, ProjectRoleSlug.ViewProjectMember);
                    if (hasProjectPermission)
                    {
                        var getProjectMemberListAsync = GetAllProjectMemberAsync();
                        getProjectMemberListAsync = getProjectMemberListAsync.Where(z => z.ProjectSlug == projectSlug);

                        if (!string.IsNullOrEmpty(filter.SearchValue))
                        {
                            getProjectMemberListAsync = getProjectMemberListAsync.Where
                                (
                                  x => x.PersonName.ToLower().Contains(filter.SearchValue.ToLower())

                                );
                        }

                        var records = getProjectMemberListAsync;
                        var totalRecords = records.Count();
                        var filteredData = await getProjectMemberListAsync
                            .Skip((filter.PageNumber - 1) * filter.PageSize)
                            .Take(filter.PageSize)
                            .ToListAsync();

                        if (filter.PageSize > totalRecords && totalRecords > 0)
                        {
                            filter.PageSize = totalRecords;
                        }
                        var totalPages = (totalRecords / filter.PageSize);


                        var data = new PagedResponseProjectMemberModel<List<ProjectMemberInviteModel>>(filteredData, filter.PageNumber, filter.PageSize, totalRecords, totalPages);

                        if (filteredData.Count > 0)
                        {
                            return Result<PagedResponseProjectMemberModel<List<ProjectMemberInviteModel>>>.Success(data);
                        }
                        else
                        {
                            return Result<PagedResponseProjectMemberModel<List<ProjectMemberInviteModel>>>.Success(null);
                        }
                    }
                    else
                    {
                        throw new UnAuthorizedUserException();
                    }
                }
                else
                {
                    throw new ProjectSlugNotFoundException();
                }
            }
            catch (Exception ex)
            {
                if (ex is UnAuthorizedUserException)
                {
                    _logger.LogError("Unauthorized User. Exception message is: ", ex.Message);
                    return Result<PagedResponseProjectMemberModel<List<ProjectMemberInviteModel>>>.Error(ReturnMessage.UnauthorizedUser);
                }
                _logger.LogError("Could not find the Projectslug. Exception message is: ", ex.Message);
                return Result<PagedResponseProjectMemberModel<List<ProjectMemberInviteModel>>>.Error(ReturnMessage.ProjectSlugDoesNotExists);
            }



        }

        public async Task<Result<string>> UpdateProjectMemberAsync(ProjectMemberModel model)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var projectId = _context.Project.FirstOrDefault(x => x.ProjectSlug == model.ProjectSlug).ProjectId;
                var project = _context.Project.Where(x => x.ProjectSlug == model.ProjectSlug).FirstOrDefault();

                if (projectId > 0)
                {
                    bool hasProjectPermission = await _iCommonService.CheckProjectPermission(model.ProjectSlug, ProjectRoleSlug.UpdateProjectMember);
                    if (hasProjectPermission)
                    {
                        var updateProjectMemberAsync = await _context.ProjectMember.FirstOrDefaultAsync(x => x.ProjectMemberId == model.ProjectMemberId);
                        if (updateProjectMemberAsync != null)
                        {
                            bool personValid = await _context.ProjectMember.Include(x => x.Project).Where(x => x.Project.ProjectSlug == model.ProjectSlug && model.PersonId.Contains(x.PersonId) && x.ProjectMemberId != model.ProjectMemberId).AnyAsync();

                            if (personValid == true)
                            {
                                throw new PersonAlreadyExistException();
                            }
                            else
                            {
                                updateProjectMemberAsync.ProjectMemberId = model.ProjectMemberId;
                                updateProjectMemberAsync.ProjectId = projectId;
                                updateProjectMemberAsync.ProjectRoleListItemId = model.ProjectRoleListItemId;
                                project.UpdateDate = DateTimeOffset.UtcNow;
                                _context.ProjectMember.Update(updateProjectMemberAsync);
                                await _context.SaveChangesAsync();
                                transaction.Commit();
                                return Result<string>.Success(ReturnMessage.UserUpdatedSuccessfully);
                            }
                          
                        }
                        else
                        {
                            return Result<string>.Success(null);
                        }
                    }
                    else
                    {
                        throw new UnAuthorizedUserException();
                    }
                }
                else
                {
                    throw new ProjectSlugNotFoundException();
                }


            }
            catch (Exception ex)
            {

                transaction.Rollback();
                if (ex is UnAuthorizedUserException)
                {
                    _logger.LogError("Unauthorized User. Exception message is: ", ex.Message);
                    return Result<string>.Error(ReturnMessage.UnauthorizedUser);
                }
                else if (ex is ProjectSlugNotFoundException)
                {
                    _logger.LogError("Could not find the ProjectSlug. Exception message is: ", ex.Message);
                    return Result<string>.Error(ReturnMessage.ProjectSlugDoesNotExists);
                }
                else if (ex is PersonAlreadyExistException)
                {
                    _logger.LogError("Project member already added!. Exception message is: ", ex.Message);
                    return Result<string>.Error(ReturnMessage.PersonAlreadyExist);
                }
                else
                {
                    _logger.LogError("Could not update the ProjectMember. Exception message is: ", ex.Message);
                    return Result<string>.Error(ReturnMessage.FailedToUpdateProjectMember);
                }

            }
        }
        private IQueryable<ProjectMemberInviteModel> GetAllProjectMemberAsync()
        {
            var getProjectMemberListAsync = from pm in _context.ProjectMember
                                            orderby pm.InsertDate ascending
                                            join p in _context.Project on pm.ProjectId equals p.ProjectId
                                            join li in _context.ListItem on pm.ProjectRoleListItemId equals li.ListItemId
                                            join per in _context.Person on pm.PersonId equals per.PersonId
                                            join pi in _context.Person on pm.InsertPersonId equals pi.PersonId
                                            join pu in _context.Person on pm.UpdatePersonId equals pu.PersonId

                                            select new ProjectMemberInviteModel
                                            {
                                                ProjectMemberId = pm.ProjectMemberId,
                                                ProjectId = pm.ProjectId,
                                                ProjectRole = li.ListItemName,
                                                ProjectRoleListItemId = li.ListItemId,
                                                PersonId = per.PersonId,
                                                PersonName = per.Name,
                                                ProjectSlug = p.ProjectSlug,
                                                InviteDate = pm.InsertDate,
                                                LastModifiedDate = pm.UpdateDate,
                                                InsertedPersonName = p.InsertPersonId != pm.PersonId ? pi.Name : string.Empty,
                                                LatestUpdatedPersonName = p.UpdatePersonId != pm.PersonId ? pu.Name : string.Empty
                                            };
            return getProjectMemberListAsync;
        }

        public async Task<Result<List<ProjectMemberModelList>>> GetProjectMemberListBySlugAsync(string projectSlug)
        {
            try
            {
                var projectId = _context.Project.FirstOrDefault(x => x.ProjectSlug == projectSlug).ProjectId;
                if (projectId > 0)
                {
                    //New Project Role Permission Check
                    bool hasProjectPermission = await _iCommonService.CheckProjectPermission(projectSlug, ProjectRoleSlug.ViewProjectMember);
                    if (hasProjectPermission)
                    {
                        var currentPersonId = _iPersonAccessor.GetPersonId();

                        var getProjectMemberListBySlugAsync = await (from p in _context.Person
                                                                     join pm in _context.ProjectMember on p.PersonId equals pm.PersonId
                                                                     join pr in _context.Project on pm.ProjectId equals pr.ProjectId
                                                                     join li in _context.ListItem on pm.ProjectRoleListItem.ListItemId equals li.ListItemId
                                                                     join lit in _context.ListItem on p.UserRoleListItem.ListItemId equals lit.ListItemId

                                                                     where pm.PersonId == currentPersonId && pr.ProjectSlug == projectSlug

                                                                     select new ProjectMemberModelList
                                                                     {
                                                                         ProjectMemberId = pm.ProjectMemberId,
                                                                         ProjectId = pr.ProjectId,
                                                                         ProjectRole = li.ListItemName,
                                                                         ProjectRoleListItemId = li.ListItemId,
                                                                         UserRole = lit.ListItemName,
                                                                         UserRoleListItemId = lit.ListItemId,
                                                                         PersonId = p.PersonId,
                                                                         PersonName = p.Name,
                                                                         ProjectSlug = pr.ProjectSlug,

                                                                     }).ToListAsync();


                        if (getProjectMemberListBySlugAsync.Any())
                        {
                            return Result<List<ProjectMemberModelList>>.Success(getProjectMemberListBySlugAsync);
                        }
                        else
                        {
                            return Result<List<ProjectMemberModelList>>.Success(null);
                        }
                    }
                    else
                    {
                        throw new UnAuthorizedUserException();
                    }
                }
                else
                {
                    throw new ProjectSlugNotFoundException();
                }
            }
            catch (Exception ex)
            {
                if (ex is UnAuthorizedUserException)
                {
                    _logger.LogError("Unauthorized User. Exception message is: ", ex.Message);
                    return Result<List<ProjectMemberModelList>>.Error(ReturnMessage.UnauthorizedUser);
                }
                _logger.LogError("Could not find the Projectslug. Exception message is: ", ex.Message);
                return Result<List<ProjectMemberModelList>>.Error(ReturnMessage.ProjectSlugDoesNotExists);
            }

        }

    }
}
