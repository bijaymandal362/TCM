using BusinessLayer.Common;
using BusinessLayer.ProjectModule;

using Data;
using Data.Exceptions;

using Infrastructure.Helper.Exceptions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Models.Constant.Authorization;
using Models.Constant.ReturnMessage;
using Models.Core;
using Models.Enum;
using Models.GridTableFilterModel;
using Models.GridTableProperty;
using Models.TestPlan;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.TestPlan
{
	public class TestPlanService : ITestPlanService
	{
		private readonly DataContext _context;
		private readonly ILogger<TestPlanService> _iLogger;
		private readonly ICommonService _iCommonService;

		public TestPlanService(DataContext context, ILogger<TestPlanService> iLogger, ICommonService iCommonService)
		{
			_context = context;
			_iLogger = iLogger;
			_iCommonService = iCommonService;
		}

		#region function
		//function for TestPlanType==Folder||TestPlan
		private async Task<string> TestPlanType(int TestPlanTypeListItemId)
		{
			return await (from li in _context.ListItem
						  where li.ListItemId == TestPlanTypeListItemId
						  select li.ListItemSystemName).FirstOrDefaultAsync();
		}

		private async Task<int> GetProjectId(string projectSlug)
		{
			return await (from p in _context.Project
						  where p.ProjectSlug == projectSlug
						  select p.ProjectId).FirstOrDefaultAsync();
		}

		private List<TestPlanListModel> TestPlanChildModule(List<TestPlanListModel> list, int parentTestPlanId)
		{
			return list.Where(c => c.ParentTestPlanId == parentTestPlanId).Select(c => new TestPlanListModel
			{
				TestPlanId = c.TestPlanId,
				ParentTestPlanId = c.ParentTestPlanId,
				TestPlanName = c.TestPlanName,
				OrderDate = c.OrderDate,
				Title = c.Title,
				ProjectId = c.ProjectId,
				ProjectSlug = c.ProjectSlug,
				Description = c.Description,
				TestPlanType = c.TestPlanType,
				TestPlanTypeListItemId = c.TestPlanTypeListItemId,
				TestPlanChildModule = TestPlanChildModule(list, c.TestPlanId)
			}).ToList();
		}
		private async Task OrderTestCase(List<DragDropOrderingTestPlanModel> list, int projectId)
		{
			if (list.Count > 0)
			{
				var testPlanEntity = await (from tp in _context.TestPlan
											join p in _context.Project on tp.ProjectId equals p.ProjectId
											where p.ProjectId == projectId
											select tp).ToListAsync();
				if (testPlanEntity != null)
				{
					var currentDateTime = DateTimeOffset.UtcNow;
					OrderTestPlanChildModule(list, testPlanEntity, currentDateTime);
					_context.TestPlan.UpdateRange(testPlanEntity);
					await _context.SaveChangesAsync();

				}

			}
		}
		private void OrderTestPlanChildModule(List<DragDropOrderingTestPlanModel> list, List<Entities.TestPlan> testPlanEntity, DateTimeOffset currentDateTime)
		{
			for (int i = 0; i < list.Count; i++)
			{
				var testPlan = list.ElementAtOrDefault(i);
				if (testPlan != null)
				{
					var singleTestPlanEntiy = (from tpe in testPlanEntity
											   where tpe.TestPlanId == testPlan.TestPlanId
											   select tpe).FirstOrDefault();
					if (singleTestPlanEntiy != null)
					{
						singleTestPlanEntiy.OrderDate = currentDateTime;
						currentDateTime = currentDateTime.AddMilliseconds(10);
						if (testPlan.TestPlanChildModule != null && testPlan.TestPlanChildModule.Count > 0)
						{
							OrderTestPlanChildModule(testPlan.TestPlanChildModule, testPlanEntity, currentDateTime);
						}
					}
				}
			}
		}

		private async Task FolderDelete(Entities.TestPlan testPlanId)
		{

			var module = await _context.TestPlan.Where(x => x.ParentTestPlanId == testPlanId.TestPlanId && x.TestPlanTypeListItem.ListItemName == ListItem.Folder.ToString()).ToListAsync();


			if (testPlanId != null)
			{

				if (module.Count != 0)
				{
					var moduleId = module.Select(x => x.TestPlanId).ToList();
					await FolderTestPlanDeleteList(moduleId);
					_context.TestPlan.UpdateRange(module);
					await _context.SaveChangesAsync();

					await FolderTestPlanDelete(testPlanId.TestPlanId);
					var project = _context.Project.Where(x => x.ProjectId == testPlanId.ProjectId).FirstOrDefault();
					project.UpdateDate = DateTimeOffset.UtcNow;
					testPlanId.TestPlanName = testPlanId.TestPlanName + testPlanId.TestPlanId;
					testPlanId.IsDeleted = true;
					_context.TestPlan.Update(testPlanId);
					await _context.SaveChangesAsync();
				}
				else
				{
					await FolderTestPlanDelete(testPlanId.TestPlanId);
					var project = _context.Project.Where(x => x.ProjectId == testPlanId.ProjectId).FirstOrDefault();
					project.UpdateDate = DateTimeOffset.UtcNow;
					testPlanId.TestPlanName = testPlanId.TestPlanName + testPlanId.TestPlanId;
					testPlanId.IsDeleted = true;
					_context.TestPlan.Update(testPlanId);
					await _context.SaveChangesAsync();
				}
			}
		}

		private async Task FolderTestPlanDelete(int testPlanId)
		{
			var testPlans = await _context.TestPlan.Where(x => x.ParentTestPlanId == testPlanId).ToListAsync();
			if (testPlans.Count != 0)
			{
				foreach (var item in testPlans)
				{
					item.TestPlanName = item.TestPlanName + item.TestPlanId;
					item.IsDeleted = true;
				}
				await TestPlanTestCaseDelete(testPlans.Select(y => y.TestPlanId).ToList());
			}
			_context.TestPlan.UpdateRange(testPlans);
			await _context.SaveChangesAsync();

		}

		private async Task FolderTestPlanDeleteList(List<int> list)
		{
			var testPlan = await _context.TestPlan.Where(x => list.Contains((int)x.ParentTestPlanId)).ToListAsync();

			if (testPlan.Count != 0)
			{
				foreach (var item in testPlan)
				{
					item.TestPlanName = item.TestPlanName + item.TestPlanId;
					item.IsDeleted = true;
				}
				await TestPlanTestCaseDelete(testPlan.Select(y => y.TestPlanId).ToList());

			}
			_context.TestPlan.UpdateRange(testPlan);
			await _context.SaveChangesAsync();

		}

		private async Task TestPlanTestCaseDelete(List<int> list)
		{
			var testPlanTestCase = await _context.TestPlanTestCase.Where(x => list.Contains(x.TestPlanId)).ToListAsync();

			if (testPlanTestCase.Count != 0)
			{
				foreach (var item in testPlanTestCase)
				{

					item.IsDeleted = true;
				}
				_context.TestPlanTestCase.UpdateRange(testPlanTestCase);
				await _context.SaveChangesAsync();
			}

		}

		private async Task TestPlanDelete(Entities.TestPlan testPlanId)
		{
			var testPlanDetail = _context.TestPlan.Where(x => x.TestPlanId == testPlanId.TestPlanId).FirstOrDefault();

			var testPlanTestCase = _context.TestPlanTestCase.Where(x => x.TestPlanId == testPlanDetail.TestPlanId).ToList();

			if (testPlanId != null)
			{

				if (testPlanTestCase.Count != 0)
				{
					foreach (var item in testPlanTestCase)
					{
						item.IsDeleted = true;
					}
					_context.TestPlanTestCase.UpdateRange(testPlanTestCase);
					var project = _context.Project.Where(x => x.ProjectId == testPlanId.ProjectId).FirstOrDefault();
					project.UpdateDate = DateTimeOffset.UtcNow;
					await _context.SaveChangesAsync();
				}

				if (testPlanDetail != null)
				{
					testPlanDetail.TestPlanName = testPlanDetail.TestPlanName + testPlanDetail.TestPlanId;
					testPlanDetail.IsDeleted = true;
					_context.TestPlan.Update(testPlanDetail);
					await _context.SaveChangesAsync();
				}

			}
		}
		#endregion

		#region Implementation
		public async Task<Result<string>> AddTestPlanAsync(AddUpdateTestPlanModel model)
		{
			await _context.Database.BeginTransactionAsync();
			try
			{
				bool hasProjectPermission = await _iCommonService.CheckProjectPermission(model.ProjectSlug, ProjectRoleSlug.CreateTestPlan);
				if (hasProjectPermission)
				{
					var projectId = await GetProjectId(model.ProjectSlug);
					var project = _context.Project.Where(x => x.ProjectSlug == model.ProjectSlug).FirstOrDefault();

					if (projectId > 0)
					{
						var testPlanType = await TestPlanType(model.TestPlanTypeListItemId);
						if (testPlanType == (nameof(ListItem.Folder)) || testPlanType == nameof(ListItem.TestPlan))
						{
							if (string.IsNullOrEmpty(model.TestPlanName) || string.IsNullOrWhiteSpace(model.TestPlanName))
							{
								throw new TestPlanNameCannotBeEmpty();

							}
							else
							{
								
								bool folderValid = false;
								bool testPlanValid = false;
								if (model.TestPlanType.ToString() == ListItem.Folder.ToString())
								{
									folderValid =  await _context.TestPlan.Where(x => x.TestPlanName.ToLower().Trim() == model.TestPlanName.ToLower().Trim() && x.ProjectId == projectId && x.IsDeleted == false && x.TestPlanTypeListItem.ListItemSystemName == model.TestPlanType.ToString() && x.ParentTestPlanId == model.ParentTestPlanId).AnyAsync();
								}

								if (model.TestPlanType.ToString() == ListItem.TestPlan.ToString())
								{
									testPlanValid = await _context.TestPlan.Where(x => x.TestPlanName.ToLower().Trim() == model.TestPlanName.ToLower().Trim() && x.ProjectId == projectId && x.IsDeleted == false && x.TestPlanTypeListItem.ListItemSystemName == ListItem.TestPlan.ToString() && x.ParentTestPlanId == model.ParentTestPlanId).AnyAsync();

								}
								if (testPlanValid)
								{
									throw new TestPlanNameAlreadyExistException();
								}

								if (folderValid)
								{
									throw new FolderNameAlreadyExistException();
								}
								else
								{


									Entities.TestPlan testPlan = new()
									{
										ParentTestPlanId = model.ParentTestPlanId,
										TestPlanName = model.TestPlanName,
										OrderDate = DateTimeOffset.UtcNow,
										IsDeleted = false,
										Title = model.Title,
										ProjectId = projectId,
										TestPlanTypeListItemId = model.TestPlanTypeListItemId,
										Description = model.Description
									};
									project.UpdateDate = DateTimeOffset.UtcNow;
									await _context.AddAsync(testPlan);
									await _context.SaveChangesAsync();

									if (testPlanType == nameof(ListItem.TestPlan))
									{
										List<int> projectModuleIds = model.ProjectModuleId;

										List<int> duplicateProjectModuleIds = projectModuleIds.GroupBy(x => x).Where(g => g.Count() > 1).Select(x => x.Key).ToList();

										if (duplicateProjectModuleIds.Count > 0)
										{
											throw new DuplicateTestCaseException();
										}

										var testCaseAlreadyExisted = await _context.ProjectModule.Where(x =>  x.IsDeleted == false && projectModuleIds.Contains(x.ProjectModuleId)).ToListAsync();


										List<string> projectModuleNames = testCaseAlreadyExisted.Select(x => x.ModuleName).ToList();


										List<string> duplicateProjectModuleNames = projectModuleNames.GroupBy(x => x).Where(g => g.Count() > 1).Select(x => x.Key).ToList();

										if (duplicateProjectModuleNames.Count > 0)
										{
											throw new DuplicateTestCaseException();
										}


										if (projectModuleIds.Count > 0)
										{
											var testPlans = projectModuleIds.Select(x => new Entities.TestPlanTestCase
											{
												TestPlanId = testPlan.TestPlanId,
												ProjectModuleId = x

											}).ToList();
											project.UpdateDate = DateTimeOffset.UtcNow;
											await _context.TestPlanTestCase.AddRangeAsync(testPlans);
											await _context.SaveChangesAsync();
										}
										else
										{
											throw new TestPlanTestCaseCountZeroException();
										}

									}
									await _context.Database.CommitTransactionAsync();
									return Result<string>.Success(ReturnMessage.SavedSuccessfully);
								}
							}
						}
						else
						{
							throw new TestPlanTypeListItemIdNotFoundException();
						}
					}
					else
					{
						throw new ProjectSlugNotFoundException();
					}
				}
				else
				{
					throw new UnAuthorizedUserException();
				}
			}
			catch (Exception ex)
			{

				await _context.Database.RollbackTransactionAsync();
				if (ex is TestPlanTestCaseCountZeroException)
				{
					_iLogger.LogError("Could not find the TestCaseId for adding TestPlan. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestPlanTestCaseCountZeroException);
				}
				else if (ex is TestPlanTypeListItemIdNotFoundException)
				{
					_iLogger.LogError("Could not find the TestPlanTypeListItemId For TestPlan. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestPlanTypeListItemIdNotFoundForTestPlan);
				}
				else if (ex is ProjectSlugNotFoundException)
				{
					_iLogger.LogError("Could not find the ProjectSlug. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ProjectSlugDoesNotExists);
				}
				else if (ex is UnAuthorizedUserException)
				{
					_iLogger.LogError("Unauthorized User. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.UnauthorizedUser);
				}
				else if (ex is TestPlanNameCannotBeEmpty)
				{
					_iLogger.LogError("TestPlanName Not Found. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestPlanNameCannotBeEmpty);

				}
				
				else if (ex is DuplicateTestCaseException)
				{
					_iLogger.LogError("	Duplicate testcase found. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.DuplicateTestCaseFound);

				}
				else if (ex is FolderNameAlreadyExistException)
				{
					_iLogger.LogError("FolderName already exist. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.FolderNameAlreadyExist);

				}
				else if (ex is TestPlanNameAlreadyExistException)
				{
					_iLogger.LogError("TEstPlan already exist. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestPlanNameAlreadyExist);

				}
				_iLogger.LogError("Could not add the ProjectModule. Exception message is: ", ex.Message);
				return Result<string>.Error(ReturnMessage.FailedToAddTestCase);
			}
		}

		public async Task<Result<string>> UpdateTestPlanAsync(AddUpdateTestPlanModel model)
		{
			await _context.Database.BeginTransactionAsync();
			try
			{
				bool hasProjectPermission = await _iCommonService.CheckProjectPermission(model.ProjectSlug, ProjectRoleSlug.UpdateTestPlan);
				if (hasProjectPermission)
				{
					var updateTestPlan = await _context.TestPlan
					.Where(x =>
								x.TestPlanId == model.TestPlanId).FirstOrDefaultAsync();
					var project = _context.Project.Where(x => x.ProjectSlug == model.ProjectSlug).FirstOrDefault();

					if (updateTestPlan != null)
					{
						var testPlanType = await TestPlanType(model.TestPlanTypeListItemId);
						if (testPlanType == (nameof(ListItem.Folder)) || testPlanType == nameof(ListItem.TestPlan))
						{
							if (string.IsNullOrEmpty(model.TestPlanName) || string.IsNullOrWhiteSpace(model.TestPlanName))
							{
								throw new TestPlanNameCannotBeEmpty();

							}
							else
							{
								bool folderValid = false;
								if (model.TestPlanType.ToString() == ListItem.Folder.ToString())
								{
									 folderValid = await _context.TestPlan.Include(x => x.Project).Where(x => x.TestPlanName.ToLower().Trim() == model.TestPlanName.ToLower().Trim() && x.Project.ProjectSlug == model.ProjectSlug && x.IsDeleted == false && x.TestPlanId != model.TestPlanId && x.TestPlanTypeListItem.ListItemSystemName == model.TestPlanType.ToString()).AnyAsync();
								}

							
								 if (folderValid)
								{
									throw new FolderNameAlreadyExistException();
								}
								else
								{
									updateTestPlan.TestPlanId = model.TestPlanId;
									updateTestPlan.TestPlanName = model.TestPlanName;
									updateTestPlan.Title = model.Title;
									updateTestPlan.Description = model.Description;
									project.UpdateDate = DateTimeOffset.UtcNow;
									_context.TestPlan.Update(updateTestPlan);
									await _context.SaveChangesAsync();



									if ((testPlanType == nameof(ListItem.TestPlan)) && model.ProjectModuleId.Count > 0)
									{
										List<int> projectModuleIds = model.ProjectModuleId;
										List<int> duplicateProjectModuleIds = projectModuleIds.GroupBy(x => x).Where(g => g.Count() > 1).Select(x => x.Key).ToList();

										if (duplicateProjectModuleIds.Count > 0)
										{
											throw new DuplicateTestCaseException();
										}

										var testCaseAlreadyExisted = await _context.ProjectModule.Where(x => x.IsDeleted == false && projectModuleIds.Contains(x.ProjectModuleId)).ToListAsync();

										var updatedprojectModuleIds = await _context.TestPlanTestCase.Include(x => x.ProjectModule).Where(x => x.IsDeleted == false && x.TestPlanId == updateTestPlan.TestPlanId && x.ProjectModule.IsDeleted == false).ToListAsync();



										List<string> projectModuleNames = testCaseAlreadyExisted.Select(x => x.ModuleName).ToList();

										List<string> updatedProjectModuleNames = updatedprojectModuleIds.Select(x => x.ProjectModule.ModuleName).ToList();

										List<string> totalModuleName = projectModuleNames.Concat(updatedProjectModuleNames).ToList();

										List<string> duplicateProjectModuleNames = totalModuleName.GroupBy(x => x).Where(g => g.Count() > 1).Select(x => x.Key).ToList();

									
										if (duplicateProjectModuleNames.Count > 0)
										{
											throw new InvalidDataException("Duplicate TestCases Found"+ " " + ":" + " " + duplicateProjectModuleNames.Aggregate((x, y) => x + ", " + Environment.NewLine + y));

										}


										if (projectModuleIds.Count > 0)
										{
											var testPlans = projectModuleIds.Select(x => new Entities.TestPlanTestCase
											{
												TestPlanId = model.TestPlanId,
												ProjectModuleId = x

											}).ToList();
											project.UpdateDate = DateTimeOffset.UtcNow;
											await _context.TestPlanTestCase.AddRangeAsync(testPlans);
											await _context.SaveChangesAsync();

										}
										


									
									}
									if (model.testPlanTestCaseId.Count > 0)
									{
										List<int> testPlanTestCaseIds = model.testPlanTestCaseId;
										var testPlanTestCase = await _context.TestPlanTestCase
										  .Where(x => testPlanTestCaseIds.Contains(x.TestPlanTestCaseId) && x.IsDeleted == false).ToListAsync();
										if (testPlanTestCase.Count > 0)
										{

											_context.TestPlanTestCase.RemoveRange(testPlanTestCase);
											await _context.SaveChangesAsync();
										}
										else
										{
											throw new FailedToDeleteTestPlanTestCaseException();
										}
									}
									if ((testPlanType == nameof(ListItem.TestPlan)))
									{
										var checkTestCaseIdInTestPlanTestCase = await _context.TestPlanTestCase.Where(x => x.TestPlanId == updateTestPlan.TestPlanId && x.IsDeleted == false).ToListAsync();
										if (checkTestCaseIdInTestPlanTestCase.Count == 0)
										{
											throw new TestCaseNeverBeZeroInTestPlanException();
										}

									}
									await _context.Database.CommitTransactionAsync();
									return Result<string>.Success(ReturnMessage.UpdatedSuccessfully);
								}
							}
						}
						else
						{
							throw new TestPlanTypeListItemIdNotFoundException();
						}

					}
					else
					{
						throw new TestPlanIdNotFoundException();
					}
				}
				else
				{
					throw new UnAuthorizedUserException();
				}

			}
			catch (Exception ex)
			{

				await _context.Database.RollbackTransactionAsync();
				if (ex is TestPlanTestCaseCountZeroException)
				{
					_iLogger.LogError("Could not find the TestCaseId for adding TestPlan. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestPlanTestCaseCountZeroException);
				}
				else if (ex is TestPlanIdNotFoundException)
				{
					_iLogger.LogError("Could not find the TestPlanId. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestPlanIdNotFound);
				}
				else if (ex is TestPlanTypeListItemIdNotFoundException)
				{
					_iLogger.LogError("Could not find the TestPlanTypeListItemId For TestPlan. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestPlanTypeListItemIdNotFoundForTestPlan);

				}
				else if (ex is UnAuthorizedUserException)
				{
					_iLogger.LogError("Unauthorized User. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.UnauthorizedUser);
				}
				else if (ex is FailedToDeleteTestPlanTestCaseException)
				{
					_iLogger.LogError("Could not delete the TestPlanTestCase. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.FailedToDeleteTestPlanTestCase);
				}
				else if (ex is TestCaseNeverBeZeroInTestPlanException)
				{
					_iLogger.LogError("There most be atlest one TestCase for TestPlan. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestCaseNeverBeZeroInTestPlanException);

				}
				else if (ex is TestPlanNameCannotBeEmpty)
				{
					_iLogger.LogError("TestPlanName Not Found. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestPlanNameCannotBeEmpty);

				}
				
				else if (ex is FolderNameAlreadyExistException)
				{
					_iLogger.LogError("FolderName already exist. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.FolderNameAlreadyExist);

				}
				else if (ex is DuplicateTestCaseException)
				{
					_iLogger.LogError("	Duplicate testcase found. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.DuplicateTestCaseFound);

				}
				else if (ex is InvalidDataException)
				{
					_iLogger.LogError("Exception message is: ", ex.Message);
					return Result<string>.Error(ex.Message);
				}

				_iLogger.LogError("Could not update the TestPlan. Exception message is: ", ex.Message);
				return Result<string>.Error(ReturnMessage.FailedToUpdateTestCase);
			}
		}

		public async Task<Result<PagedResponsePersonModel<List<TestPlanListModel>>>> GetTestPlanListAsync(FilterModel filter, string projectSlug)
		{
			try
			{
				if (projectSlug != null)
				{
					IQueryable<TestPlanListModel> getTestPlanQueryableList;

					getTestPlanQueryableList =  (from tp in _context.TestPlan
												 join p in _context.Project on tp.ProjectId equals p.ProjectId
												 join li in _context.ListItem on tp.TestPlanTypeListItemId equals li.ListItemId
												 where p.ProjectSlug == projectSlug && tp.IsDeleted == false
												 orderby tp.OrderDate
												 select new TestPlanListModel
												 {
													 TestPlanId = tp.TestPlanId,
													 ParentTestPlanId = tp.ParentTestPlanId,
													 TestPlanName = tp.TestPlanName,
													 OrderDate = tp.OrderDate,
													 Title = tp.Title,
													 ProjectId = tp.ProjectId,
													 ProjectSlug = p.ProjectSlug,
													 Description = tp.Description,
													 TestPlanType = li.ListItemSystemName,
													 TestPlanTypeListItemId = li.ListItemId

												 });


					IEnumerable<TestPlanListModel> getTestPlanList;
					getTestPlanList = await getTestPlanQueryableList.ToListAsync();


					getTestPlanList = getTestPlanList.OrderBy(c => c.OrderDate)
						.Where(c => c.ParentTestPlanId == null).Select(c => new TestPlanListModel()
						{
							TestPlanId = c.TestPlanId,
							ParentTestPlanId = c.ParentTestPlanId,
							TestPlanName = c.TestPlanName,
							OrderDate = c.OrderDate,
							Title = c.Title,
							ProjectId = c.ProjectId,
							ProjectSlug = c.ProjectSlug,
							Description = c.Description,
							TestPlanType = c.TestPlanType,
							TestPlanTypeListItemId = c.TestPlanTypeListItemId,
							TestPlanChildModule = TestPlanChildModule(getTestPlanList.Where(z => z.ParentTestPlanId != null).ToList(), c.TestPlanId)

						}).ToList();
					var result = getTestPlanList;

					if (!string.IsNullOrEmpty(filter.SearchValue))
					{
						result = getTestPlanList.Searching().Where(x => x.TestPlanName.ToLower().Contains(filter.SearchValue.ToLower())).ToList();

					}

					var filteredData = result.ToList();

					var finalResult = new List<TestPlanListModel>();
					foreach (var item in filteredData)
					{

						//Searching Folder Case for parent module ie null 
						if (item.ParentTestPlanId == null)
						{

							var testPlanIdList = getTestPlanList.Searching().Where(x => x.ParentTestPlanId == item.TestPlanId).ToList();
							var functionIdList = getTestPlanList.Searching().Where(x => x.TestPlanId == testPlanIdList.Select(x => x.ParentTestPlanId).FirstOrDefault()).ToList();
							if (testPlanIdList.Count > 0 || functionIdList.Count > 0)
							{
								finalResult.AddRange(functionIdList);
							}
							else
							{

								var moduleIdList = getTestPlanList.Searching().Where(x => x.TestPlanId == item.TestPlanId).ToList();
								finalResult.AddRange(moduleIdList);

							}


						}
						//Searching TestPlan or Folder 
						else
						{
							//Searching TestPlan 
							if (item.ParentTestPlanId == 38)
							{
								int? parentTestPlanId = item.ParentTestPlanId;
								List<TestPlanListModel> testPlanListModels = new List<TestPlanListModel>();
								do
								{
									var parentTestPlanIdsList = getTestPlanList.Searching().Select(x => x.ParentTestPlanId).ToList();

									var parentTestPlanIdsList1 = getTestPlanList.Searching().Where(x => x.ParentTestPlanId == parentTestPlanId).ToList();
									var parentTestPlanIdsList2 = getTestPlanList.Searching().Where(x => x.TestPlanId == parentTestPlanIdsList1.Select(x => x.ParentTestPlanId).FirstOrDefault()).ToList();
									var parentTestPlanIdsListFolder = getTestPlanList.Searching().Where(x => x.TestPlanId == parentTestPlanIdsList2.Select(x => x.ParentTestPlanId).FirstOrDefault()).ToList();

									if (parentTestPlanIdsListFolder.Count > 0)
									{
										parentTestPlanId = parentTestPlanIdsListFolder.Where(x => parentTestPlanIdsList.Contains(x.TestPlanId)).Select(x => x.ParentTestPlanId).FirstOrDefault();
										testPlanListModels.AddRange(parentTestPlanIdsListFolder.Where(x => x.ParentTestPlanId == null).DistinctBy(x => x.TestPlanId));

									}
									else
									{
										parentTestPlanId = parentTestPlanIdsList2.Where(x => parentTestPlanIdsList.Contains(x.TestPlanId)).Select(x => x.ParentTestPlanId).FirstOrDefault();
										testPlanListModels.AddRange(parentTestPlanIdsList2.Where(x => x.ParentTestPlanId == null).DistinctBy(x => x.TestPlanId));
									}
								} while (parentTestPlanId != null);

								finalResult.AddRange(testPlanListModels.Distinct());

							}
							else
							{
								//Searching Nested folder
								if (item.ParentTestPlanId == 37 && item.ParentTestPlanId != null)
								{
									int? folderParentTestPlanId = item.ParentTestPlanId;
									List<TestPlanListModel> testPlanListModelsNestedFolder = new List<TestPlanListModel>();
									do
									{
										var parentTestPlanIdList = getTestPlanList.Searching().Select(x => x.ParentTestPlanId).ToList();

										var parentTestPlanIdList1 = getTestPlanList.Searching().Where(x => x.TestPlanId == folderParentTestPlanId).ToList();
										var parentTestPlanIdList2 = getTestPlanList.Searching().Where(x => x.TestPlanId == parentTestPlanIdList1.Select(x => x.ParentTestPlanId).FirstOrDefault()).ToList();
										if (parentTestPlanIdList1.Count > 0)
										{
											folderParentTestPlanId = parentTestPlanIdList1.Where(x => parentTestPlanIdList.Contains(x.TestPlanId)).Select(x => x.ParentTestPlanId).FirstOrDefault();
											testPlanListModelsNestedFolder.AddRange(parentTestPlanIdList1.Where(x => x.ParentTestPlanId == null).DistinctBy(x => x.TestPlanId));
										}

										else
										{
											var parentList = parentTestPlanIdList2.Searching().Where(x => x.TestPlanId == item.ParentTestPlanId).ToList();

											if (parentList.Count > 0)
											{
												folderParentTestPlanId = parentList.Where(x => parentTestPlanIdList.Contains(x.TestPlanId)).Select(x => x.ParentTestPlanId).FirstOrDefault();
												testPlanListModelsNestedFolder.AddRange(parentList.Where(x => x.ParentTestPlanId == null).DistinctBy(x => x.TestPlanId));
											}
											else
											{
												folderParentTestPlanId = parentTestPlanIdList2.Where(x => parentTestPlanIdList.Contains(x.TestPlanId)).Select(x => x.ParentTestPlanId).FirstOrDefault();
												testPlanListModelsNestedFolder.AddRange(parentTestPlanIdList2.Where(x => x.ParentTestPlanId == null).DistinctBy(x => x.TestPlanId));

											}
										}
									} while (folderParentTestPlanId != null);

									finalResult.AddRange(testPlanListModelsNestedFolder.Distinct());
								}

								//Searching only testplan
								else
								{

									int? testPlanParentTestPlanId = item.ParentTestPlanId;
									List<TestPlanListModel> listModelsTestPlan = new List<TestPlanListModel>();
									do
									{
										var parentTestPlanIdsList = getTestPlanList.Searching().Select(x => x.ParentTestPlanId).ToList();
										var parentTestPlanIdsList1 = getTestPlanList.Searching().Where(x => x.TestPlanId == testPlanParentTestPlanId).ToList();
										var parentTestPlanIdsList2 = getTestPlanList.Searching().Where(x => x.TestPlanId == parentTestPlanIdsList1.Select(x => x.ParentTestPlanId).FirstOrDefault()).ToList();
										if (parentTestPlanIdsList2.Count > 0)
										{
											testPlanParentTestPlanId = parentTestPlanIdsList2.Where(x => parentTestPlanIdsList.Contains(x.TestPlanId)).Select(x => x.ParentTestPlanId).FirstOrDefault();
											listModelsTestPlan.AddRange(parentTestPlanIdsList2.Where(x => x.ParentTestPlanId == null).DistinctBy(x => x.TestPlanId));
										}
										else
										{
											var parentTestPlanIdsList3 = parentTestPlanIdsList1.Searching().Where(x => x.TestPlanId == item.ParentTestPlanId).ToList();
											if (parentTestPlanIdsList3.Count > 0)
											{
												testPlanParentTestPlanId = parentTestPlanIdsList3.Where(x => parentTestPlanIdsList.Contains(x.TestPlanId)).Select(x => x.ParentTestPlanId).FirstOrDefault();
												listModelsTestPlan.AddRange(parentTestPlanIdsList3.Where(x => x.ParentTestPlanId == null).DistinctBy(x => x.TestPlanId));
											}
											else
											{
												testPlanParentTestPlanId = parentTestPlanIdsList2.Where(x => parentTestPlanIdsList.Contains(x.TestPlanId)).Select(x => x.ParentTestPlanId).FirstOrDefault();

												listModelsTestPlan.AddRange(parentTestPlanIdsList2.Where(x => x.ParentTestPlanId == null).DistinctBy(x => x.TestPlanId));

											}
										}
									} while (testPlanParentTestPlanId != null);

									finalResult.AddRange(listModelsTestPlan.Distinct());

								}
							}
						}
					}


					var data = new PagedResponsePersonModel<List<TestPlanListModel>>(finalResult.DistinctBy(x => x.TestPlanId).ToList());
					if (finalResult.Count > 0)

					{
						return Result<PagedResponsePersonModel<List<TestPlanListModel>>>.Success(data);
					}
					else
					{
						return Result<PagedResponsePersonModel<List<TestPlanListModel>>>.Success(null);
					}
				}
				else
				{
					throw new ProjectSlugNotFoundException();
				}
			}
			catch (Exception ex)
			{
				_iLogger.LogError("Could not find the ProjectSlug. Exception message is: ", ex.Message);
				return Result<PagedResponsePersonModel<List<TestPlanListModel>>>.Error(ReturnMessage.ProjectSlugDoesNotExists);
			}

		}

		public async Task<Result<string>> DragDropTestPlanAsync(DragDropTestPlanModel model)
		{
			await _context.Database.BeginTransactionAsync();
			try
			{
				var projectId = await GetProjectId(model.ProjectSlug);
				if (projectId > 0)
				{
					var updateTestPlan = await _context.TestPlan.Include(x => x.TestPlanTypeListItem).Where(x => x.TestPlanId == model.TestPlanId).FirstOrDefaultAsync();
					if (updateTestPlan != null)
					{
						bool updateFolderListItemSystemName = updateTestPlan.TestPlanTypeListItem.ListItemSystemName == nameof(ListItem.Folder);
						bool updateTestPlanListItemSystemName = updateTestPlan.TestPlanTypeListItem.ListItemSystemName == nameof(ListItem.TestPlan);

						var dragDropListItemSystemName = await _context.TestPlan.Include(x => x.TestPlanTypeListItem)
								.Where(x =>
								x.TestPlanId == model.DragDropTestPlanId).FirstOrDefaultAsync();
						if (dragDropListItemSystemName != null)
						{
							bool dragDropfolderListItemSystemName = dragDropListItemSystemName.TestPlanTypeListItem.ListItemSystemName == nameof(ListItem.Folder);

							if ((updateFolderListItemSystemName || updateTestPlanListItemSystemName) && dragDropfolderListItemSystemName
							   )
							{
								updateTestPlan.TestPlanId = model.TestPlanId;
								updateTestPlan.ProjectId = projectId;
								updateTestPlan.ParentTestPlanId = model.DragDropTestPlanId;

								_context.TestPlan.Update(updateTestPlan);
								await _context.SaveChangesAsync();
							}
							else
							{
								throw new TestPlanTypeListItemIdNotFoundException();
							}
						}
						else
						{
							if (updateFolderListItemSystemName && model.DragDropTestPlanId == null)
							{
								updateTestPlan.TestPlanId = model.TestPlanId;
								updateTestPlan.ProjectId = projectId;
								updateTestPlan.ParentTestPlanId = model.DragDropTestPlanId;
								_context.TestPlan.Update(updateTestPlan);
								await _context.SaveChangesAsync();
							}
							else
							{
								throw new FailedToDragOrDropTestPlanException();
							}

						}
						List<DragDropOrderingTestPlanModel> dragDrop = model.DragDropOrderingView;
						await OrderTestCase(dragDrop, projectId);
						var project = _context.Project.Where(x => x.ProjectId == projectId).FirstOrDefault();
						project.UpdateDate = DateTimeOffset.UtcNow;
						await _context.Database.CommitTransactionAsync();
						return Result<string>.Success(ReturnMessage.UpdatedSuccessfully);
					}
					else
					{
						throw new TestPlanIdNotFoundException();
					}

				}
				else
				{
					throw new ProjectSlugNotFoundException();
				}
			}
			catch (Exception ex)
			{
				await _context.Database.RollbackTransactionAsync()
	;
				if (ex is ProjectSlugNotFoundException)
				{
					_iLogger.LogError("Could not find the ProjectSlug. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ProjectSlugDoesNotExists);
				}
				else if (ex is TestPlanIdNotFoundException)
				{
					_iLogger.LogError("Could not find the TestPlanId. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestPlanIdNotFound);
				}
				else if (ex is TestPlanTypeListItemIdNotFoundException)
				{
					_iLogger.LogError("Could not find the TestPlanTypeListItemId. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestPlanTypeListItemIdNotFound);
				}
				_iLogger.LogError("Could not find the FailedToDragOrDropTestPlan. Exception message is: ", ex.Message);
				return Result<string>.Error(ReturnMessage.FailedToDragOrDropTestPlan);
			}

		}
		public async Task<Result<PagedResponseModel<List<TestPlanTestCaseModel>>>> GetTestPlanTestCaseByTestPlanIdAsync(PaginationFilterModel filter, int testPlanId)
		{
			var getTestPlanTestCaseByTestPlanId = (from tptc in _context.TestPlanTestCase
												   join tp in _context.TestPlan on tptc.TestPlanId equals tp.TestPlanId
												   join p in _context.ProjectModule on tptc.ProjectModuleId equals p.ProjectModuleId
												   join tcd in _context.TestCaseDetail on p.ProjectModuleId equals tcd.ProjectModuleId
												   join per in _context.Person on tptc.InsertPersonId equals per.PersonId
												   where tptc.TestPlanId == testPlanId && tptc.IsDeleted == false && p.IsDeleted == false
												   orderby p.ProjectModuleId
												   select new TestPlanTestCaseModel
												   {
													   TestPlanName = tp.TestPlanName,
													   TestPlanTestCaseId = tptc.TestPlanTestCaseId,
													   Description = tp.Description,
													   TestCaseName = p.ModuleName,
													   Title = tp.Title,
													   ProjectModuleId = p.ProjectModuleId,
													   Scenario = p.Description,
													   ExpectedResult = tcd.ExpectedResult,
													   Author = per.Name

												   });

			if (!string.IsNullOrEmpty(filter.SearchValue))
			{
				getTestPlanTestCaseByTestPlanId = getTestPlanTestCaseByTestPlanId.Where
					 (
					x => x.TestCaseName.ToLower().Contains(filter.SearchValue.ToLower())
					);
			}

			var records = getTestPlanTestCaseByTestPlanId;
			var totalRecords = records.Count();

			var filteredData = await getTestPlanTestCaseByTestPlanId
				.Skip((filter.PageNumber - 1) * filter.PageSize)
				.Take(filter.PageSize)
				.ToListAsync();

			if (filter.PageSize > totalRecords && totalRecords > 0)
			{
				filter.PageSize = totalRecords;
			}

			var totalPages = (totalRecords / filter.PageSize);

			var data = new PagedResponseModel<List<TestPlanTestCaseModel>>(filteredData, filter.PageNumber, filter.PageSize, totalRecords, totalPages);

			if (filteredData.Count > 0)
			{
				return Result<PagedResponseModel<List<TestPlanTestCaseModel>>>.Success(data);
			}
			else
			{
				return Result<PagedResponseModel<List<TestPlanTestCaseModel>>>.Success(null);
			}
		}

		public async Task<Result<string>> DeleteTestPlanAsync(int testPlanId)
		{
			using var transaction = _context.Database.BeginTransaction();
			try
			{
				var testPlan = await _context.TestPlan.Include(x => x.TestPlanTypeListItem).Include(x => x.Project)
					.Where(x =>
					x.TestPlanId == testPlanId).FirstOrDefaultAsync();

				if (testPlan != null)
				{
					bool hasProjectPermission = await _iCommonService.CheckProjectPermission(testPlan.Project.ProjectSlug, ProjectRoleSlug.DeleteTestPlan);
					if (hasProjectPermission)
					{

						switch (testPlan.TestPlanTypeListItem.ListItemSystemName)
						{
							case nameof(ListItem.Folder):
								await FolderDelete(testPlan);
								break;
							case nameof(ListItem.TestPlan):
								await TestPlanDelete(testPlan);
								break;

							default:
								throw new TestPlanTypeListItemIdNotFoundException();
						}
						transaction.Commit();
						return Result<string>.Success(ReturnMessage.DeletedSuccessfully);


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
			catch (System.Exception ex)
			{
				transaction.Rollback();
				if (ex is UnAuthorizedUserException)
				{
					_iLogger.LogError("Unauthorized User. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.UnauthorizedUser);
				}
				else if (ex is ProjectSlugNotFoundException)
				{
					_iLogger.LogError("Could not find the ProjectSlug. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ProjectSlugDoesNotExists);

				}
				else if (ex is TestPlanIdCannotBeDeletedException)
				{
					_iLogger.LogError("TestPlanId is used in TestRun. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestPlanIdCannotBeDeleted);

				}
				else
				{
					_iLogger.LogError("Could not delete the TestPlan. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.FailedToDeleteTestPlan);

				}

			}
		}
		#endregion
	}
}
