using Data;

using Infrastructure;
using Infrastructure.Helper.Exceptions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Models.Constant.ReturnMessage;
using Models.Core;
using Models.Enum;
using Models.GridTableFilterModel;
using Models.GridTableProperty;
using Models.ProjectStarred;

using NPOI.OpenXmlFormats.Dml;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ProjectStarred
{
	public class ProjectStarredService : IProjectStarredService
	{

		private readonly DataContext _context;
		private readonly ILogger<ProjectStarredService> _logger;
		private readonly IPersonAccessor _iPersonAccessor;

		public ProjectStarredService(DataContext context, ILogger<ProjectStarredService> logger, IPersonAccessor iPersonAccessor)
		{
			_context = context;
			_logger = logger;
			_iPersonAccessor = iPersonAccessor;

		}

		public async Task<Result<string>> AssignProjectToProjectStarredAsync(string projectSlug)
		{
			using var transaction = _context.Database.BeginTransaction();
			try
			{
				var project = await _context.Project.Where(x => x.ProjectSlug == projectSlug).FirstOrDefaultAsync();
				var personId = _iPersonAccessor.GetPersonId();

				if (project != null)
				{
					var addProjectStarred = new Entities.ProjectStarred();

					addProjectStarred.ProjectId = project.ProjectId;
					addProjectStarred.PersonId = personId;

					await _context.ProjectStarred.AddAsync(addProjectStarred);
					await _context.SaveChangesAsync();
					await _context.Database.CommitTransactionAsync();
					return Result<string>.Success(ReturnMessage.SavedSuccessfully);

				}
				else
				{
					throw new ProjectIdNotFoundException();
				}

			}
			catch (Exception ex)
			{
				_logger.LogError("Could not add the Project to favourite .Exception message is: ", ex.Message);
				transaction.Rollback();
				return Result<string>.Error(ReturnMessage.FailedToAddProjectStarred);
			}
		}

		public async Task<Result<PagedResponseModel<List<ProjectStarredModel>>>> GetProjectStarredListFilterByModuleAsync(PaginationFilterModel filter)
		{

			var currentPersonId = (_iPersonAccessor.GetPersonId());


			IQueryable<ProjectStarredModel> getProjectStarredListAllAsync;


			var testCaseList = await _context.ProjectModule.Where(x => x.ProjectModuleListItem.ListItemSystemName == ListItem.TestCase.ToString() && x.IsDeleted == false).ToListAsync();

			var testRunList = await _context.TestRun.ToListAsync();

			var testPlanList = await _context.TestPlan.Where(x => x.TestPlanTypeListItem.ListItemSystemName == ListItem.TestPlan.ToString() && x.IsDeleted == false).ToListAsync();

			getProjectStarredListAllAsync = GetFilterProjectStarredListAsync(currentPersonId);


		

			if (!string.IsNullOrEmpty(filter.SearchValue))
			{
				getProjectStarredListAllAsync = getProjectStarredListAllAsync.Where
						(
							x => x.ProjectName.ToLower().Contains(filter.SearchValue.ToLower())
						);
			}
			var records = getProjectStarredListAllAsync;
			var totalRecords = records.Count();
			var filteredData = getProjectStarredListAllAsync
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

			var data = new PagedResponseModel<List<ProjectStarredModel>>(filteredData, filter.PageNumber, filter.PageSize, totalRecords, totalPages);

			if (filteredData.Count > 0)
			{
				return Result<PagedResponseModel<List<ProjectStarredModel>>>.Success(data);
			}
			else
			{
				return Result<PagedResponseModel<List<ProjectStarredModel>>>.Success(null);
			}


		}




		public async Task<Result<string>> UnAssignProjectToProjectStarredAsync(string projectSlug)
		{
			using var transaction = _context.Database.BeginTransaction();
			try
			{
				var project = await _context.Project.Where(x => x.ProjectSlug == projectSlug).FirstOrDefaultAsync();
				var personId = _iPersonAccessor.GetPersonId();

				if (project != null)
				{
					var deleteProjectStarred = _context.ProjectStarred.Where(x => x.ProjectId == project.ProjectId && x.PersonId == personId).FirstOrDefault();

					_context.ProjectStarred.Remove(deleteProjectStarred);
					await _context.SaveChangesAsync();
					await _context.Database.CommitTransactionAsync();
					return Result<string>.Success(ReturnMessage.DeletedSuccessfully);

				}
				else
				{
					throw new ProjectIdNotFoundException();
				}

			}
			catch (Exception ex)
			{
				_logger.LogError("Could not add the Project to favourite .Exception message is: ", ex.Message);
				transaction.Rollback();
				return Result<string>.Error(ReturnMessage.FailedToDeleteProjectStarred);
			}
		}

		private IQueryable<ProjectStarredModel> GetFilterProjectStarredListAsync(int currentPersonId)
		{
			var list = from ps in _context.ProjectStarred
					   join p in _context.Project on ps.ProjectId equals p.ProjectId
					   join per in _context.Person on ps.PersonId equals per.PersonId
					   where ps.PersonId == currentPersonId
					   orderby p.UpdateDate descending
					   select new
					   {
						   ProjectStarredId = ps.ProjectStarredId,
						   ProjectId = ps.ProjectId,
						   ProjectName = ps.Project.ProjectName,
						   ProjectSlug = ps.Project.ProjectSlug,
						   Date = ps.UpdateDate,
						   PersonId = ps.PersonId,
						   PersonName = ps.Person.Name,


					   };



			var result = list.GroupBy(x => x.ProjectId).Select(x => new ProjectStarredModel
			{
				ProjectStarredId = x.Select(x => x.ProjectStarredId).FirstOrDefault(),
				ProjectId = x.Select(x => x.ProjectId).FirstOrDefault(),
				ProjectName = x.Select(x => x.ProjectName).FirstOrDefault(),
				ProjectSlug = x.Select(x => x.ProjectSlug).FirstOrDefault(),
				PersonId = x.Select(x => x.PersonId).FirstOrDefault(),
				PersonName = x.Select(x => x.PersonName).FirstOrDefault(),
				Date = x.Select(x => x.Date).FirstOrDefault(),
			}).OrderByDescending(x => x.Date);

			return result;
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
	}
}
