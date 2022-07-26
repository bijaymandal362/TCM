using BusinessLayer.Common;

using Data;
using Data.Exceptions;



using Infrastructure;
using Infrastructure.Helper.Excel;
using Infrastructure.Helper.ExcelExport;
using Infrastructure.Helper.Exceptions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Models.Constant.Authorization;
using Models.Constant.ReturnMessage;
using Models.Core;
using Models.Enum;
using Models.GridTableFilterModel;
using Models.GridTableProperty;
using Models.Import;
using Models.ProjectModule;

using NPOI.OpenXmlFormats.Wordprocessing;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace BusinessLayer.ProjectModule
{
	public class ProjectModuleService : IProjectModuleService
	{
		private readonly DataContext _context;
		private readonly ILogger<ProjectModuleService> _iLogger;
		private readonly ICommonService _iCommonService;

		public ProjectModuleService(DataContext context, ILogger<ProjectModuleService> iLogger,
		 ICommonService iCommonService)
		{
			_context = context;
			_iLogger = iLogger;
			_iCommonService = iCommonService;
		}


		#region Functions
		private List<ProjectModuleModel> ChildModule(List<ProjectModuleModel> list, List<ProjectModuleDeveloperListModel> developerList, int parentProjectModuleId)
		{
			var testCaseDetail = (from tcd in _context.TestCaseDetail
								  join pm in _context.ProjectModule on tcd.ProjectModuleId equals pm.ProjectModuleId
								  join p in _context.Project on pm.ProjectId equals p.ProjectId
								  where pm.ProjectId == p.ProjectId && pm.IsDeleted == false
								  select new
								  {
									  ProjectModuleId = tcd.ProjectModuleId,
									  ExpectedResult = tcd.ExpectedResult,
									  ProjectId = p.ProjectId
								  });


			return list
				   .Where(c => c.ParentProjectModuleId == parentProjectModuleId && c.IsDeleted == false)
				   .Select(c => new ProjectModuleModel
				   {
					   ProjectModuleId = c.ProjectModuleId,
					   ModuleName = c.ModuleName,
					   ParentProjectModuleId = c.ParentProjectModuleId,
					   ProjectModuleType = c.ProjectModuleType,
					   ProjectId = c.ProjectId,
					   OrderDate = c.OrderDate,
					   ProjectModuleListItemId = c.ProjectModuleListItemId,
					   Description = c.Description,
					   IsDeleted = c.IsDeleted,
					   ChildModule = ChildModule(list, developerList, c.ProjectModuleId),
					   ExpectedResult = testCaseDetail.Where(x => x.ProjectModuleId == c.ProjectModuleId).Select(x => x.ExpectedResult).FirstOrDefault() == null ? string.Empty : testCaseDetail.Where(x => x.ProjectModuleId == c.ProjectModuleId).Select(x => x.ExpectedResult).FirstOrDefault(),
					   DeveloperListId = c.ProjectModuleType == nameof(ListItem.Function) ?
										developerList.Where(d => d.ProjectModuleId == c.ProjectModuleId).Select(d => d.PersonId).ToList() : null
				   }).OrderBy(x => x.ProjectModuleId).ToList();
		}

		private async Task TestCaseDelete(Entities.ProjectModule projectModuleId)
		{
			projectModuleId.ModuleName = projectModuleId.ModuleName + "_" + "deleted" + "_" + Guid.NewGuid();
			projectModuleId.IsDeleted = true;
			var project = _context.Project.Where(x => x.ProjectId == projectModuleId.ProjectId).FirstOrDefault();
			project.UpdateDate = DateTimeOffset.UtcNow;
			_context.ProjectModule.Update(projectModuleId);
			await _context.SaveChangesAsync();
		}

		private async Task TestCaseDelete(List<int> projectModuleId)
		{
			var testCaseDetail = _context.TestCaseDetail.Where(x => projectModuleId.Contains(x.ProjectModuleId));

			var testCaseStepDetail = _context.TestCaseStepDetail.Where(x => projectModuleId.Contains(x.ProjectModuleId));
			if (testCaseStepDetail != null)
			{
				_context.TestCaseStepDetail.RemoveRange(testCaseStepDetail);
				await _context.SaveChangesAsync();
			}

			if (testCaseDetail != null)
			{
				_context.TestCaseDetail.RemoveRange(testCaseDetail);
				await _context.SaveChangesAsync();
			}
		}

		private async Task ProjectModuleDeveloperDelete(int projectModuleId)
		{
			var projectModuleDeveloper = _context.ProjectModuleDeveloper.Where(x => x.ProjectModuleId == projectModuleId).FirstOrDefault();
			if (projectModuleDeveloper != null)
			{
				_context.ProjectModuleDeveloper.Remove(projectModuleDeveloper);
				await _context.SaveChangesAsync();
			}
		}

		private async Task ProjectModuleDeveloperDelete(List<int> projectModuleId)
		{
			var projectModuleDeveloper = _context.ProjectModuleDeveloper.Where(x => projectModuleId.Contains(x.ProjectModuleId));
			if (projectModuleDeveloper != null)
			{
				_context.ProjectModuleDeveloper.RemoveRange(projectModuleDeveloper);
				await _context.SaveChangesAsync();
			}
		}
		private async Task ModuleDelete(Entities.ProjectModule projectModuleId)
		{

			var module = await _context.ProjectModule.Where(x => x.ParentProjectModuleId == projectModuleId.ProjectModuleId && x.ProjectModuleListItem.ListItemName == ListItem.Module.ToString() && x.IsDeleted == false).ToListAsync();

			if (projectModuleId != null)
			{

				await ModuleFunctionDelete(projectModuleId.ProjectModuleId);

			}
		}


		private async Task ModuleFunctionDelete(int projectModuleId)
		{
			var nestedModule = await _context.ProjectModule.Where(x => x.ParentProjectModuleId == projectModuleId && x.ProjectModuleListItem.ListItemName == ListItem.Module.ToString() && x.IsDeleted == false).ToListAsync();

			var function = await _context.ProjectModule.Where(x => x.ParentProjectModuleId == projectModuleId && x.ProjectModuleListItem.ListItemName == ListItem.Function.ToString() && x.IsDeleted == false).ToListAsync();


			if (nestedModule.Count > 0)
			{
				throw new ModuleContainsNestedModuleException();
			}
			else if (function.Count > 0)
			{
				throw new ModuleContainsFunctionException();
			}
			else
			{
				var pmId = await _context.ProjectModule.Where(x => x.ProjectModuleId == projectModuleId && x.IsDeleted == false).FirstOrDefaultAsync();
				if (pmId != null)
				{
					pmId.ModuleName = pmId.ModuleName + "_" + "deleted" + "_" + Guid.NewGuid();
					pmId.IsDeleted = true;
					var project = _context.Project.Where(x => x.ProjectId == pmId.ProjectId).FirstOrDefault();
					project.UpdateDate = DateTimeOffset.UtcNow;
					_context.ProjectModule.Update(pmId);
					await _context.SaveChangesAsync();
				}

			}
		}


		private async Task FunctionDelete(Entities.ProjectModule projectModuleId)
		{
			var testCases = await _context.ProjectModule.Where(x => x.ParentProjectModuleId == projectModuleId.ProjectModuleId && x.IsDeleted == false).ToListAsync();
			if (testCases.Count > 0)
			{
				throw new FunctionContainsTestCasesException();
			}
			else
			{


				projectModuleId.ModuleName = projectModuleId.ModuleName + "_" + "deleted" + "_" + Guid.NewGuid();
				projectModuleId.IsDeleted = true;
				var project = _context.Project.Where(x => x.ProjectId == projectModuleId.ProjectId).FirstOrDefault();
				project.UpdateDate = DateTimeOffset.UtcNow;
				_context.ProjectModule.Update(projectModuleId);
				await _context.SaveChangesAsync();
			}
		}

		private List<TestCaseStepDetailModel> TestDetailStep(int parentProjectModuleId)
		{
			return _context.TestCaseStepDetail
				   .Where(c => c.ProjectModuleId == parentProjectModuleId)
				   .Select(c => new TestCaseStepDetailModel
				   {
					   TestCaseStepDetailId = c.TestCaseStepDetailId,
					   TestCaseStepDetailProjectModuleId = c.ProjectModuleId,
					   StepDescription = c.StepDescription,
					   StepNumber = c.StepNumber,
					   ExpectedResultTestStep = c.ExpectedResult,
				   }).OrderBy(x => x.TestCaseStepDetailId).ToList();
		}

		private List<ProjectModuleDeveloperFunctionModel> DeveloperList(int functionId)
		{
			var list = (from pm in _context.ProjectModuleDeveloper
						join p in _context.ProjectMember on pm.ProjectMemberId equals p.ProjectMemberId
						join per in _context.Person on p.PersonId equals per.PersonId
						where pm.ProjectModuleId == functionId
						select new ProjectModuleDeveloperFunctionModel
						{
							ProjectModuleDeveloperId = pm.ProjectModuleDeveloperId,
							ProjectMemberId = pm.ProjectMemberId,
							IsDisabled = pm.IsDisabled,
							Member = p.Person.Name
						}).ToList();

			return list;
		}

		private async Task OrderProjectModule(List<DragDropProjectModuleModel> list, string projectSlug)
		{
			if (list.Count > 0)
			{
				var projectModuleEntity = await (from p in _context.Project
												 join pm in _context.ProjectModule on p.ProjectId equals pm.ProjectId
												 where p.ProjectSlug == projectSlug
												 select pm).ToListAsync();
				var project = _context.Project.Where(x => x.ProjectSlug == projectSlug).FirstOrDefault();

				if (projectModuleEntity.Count > 0)
				{
					var currentDateTime = DateTimeOffset.UtcNow;
					OrderChildModule(list, projectModuleEntity, currentDateTime);
					project.UpdateDate = DateTimeOffset.UtcNow;
					_context.UpdateRange(projectModuleEntity);
					await _context.SaveChangesAsync();
				}
			}
		}

		private void OrderChildModule(List<DragDropProjectModuleModel> list, List<Entities.ProjectModule> projectModuleEntity, DateTimeOffset currentDateTime)
		{
			for (int i = 0; i < list.Count; i++)
			{
				var projectModule = list.ElementAtOrDefault(i);
				if (projectModule != null)
				{
					var singleProjectModuleEntity = (from proj in projectModuleEntity
													 where proj.ProjectModuleId == projectModule.ProjectModuleId
													 select proj).FirstOrDefault();
					if (singleProjectModuleEntity != null)
					{
						singleProjectModuleEntity.OrderDate = currentDateTime;
						currentDateTime = currentDateTime.AddMilliseconds(10);

						if (projectModule.ChildModule != null && projectModule.ChildModule.Count > 0)
							OrderChildModule(projectModule.ChildModule, projectModuleEntity, currentDateTime);
					}
				}
			}
		}


		#endregion


		#region Implementation
		public async Task<Result<string>> AddProjectModuleAysnc(AddUpdateProjectModuleModel model)
		{
			using var transaction = _context.Database.BeginTransaction();
			try
			{

				bool hasProjectPermission = await _iCommonService.CheckProjectPermission(model.ProjectSlug, ProjectRoleSlug.CreateProjectModule);
				if (hasProjectPermission)
				{
					var projectId = _context.Project.Where(x => x.ProjectSlug == model.ProjectSlug).FirstOrDefault().ProjectId;

					if (projectId > 0)
					{
						if (string.IsNullOrEmpty(model.ModuleName) || string.IsNullOrWhiteSpace(model.ModuleName))
						{
							throw new ModuleNameNullOrEmptyException();
						}
						else
						{
							var projectModuleListItemid = await _context.ListItem.FirstOrDefaultAsync(x => x.ListItemSystemName == model.ProjectModuleType);
							if (projectModuleListItemid != null)
							{


								bool moduleNameValid = await _context.ProjectModule.Where(x => x.ModuleName.ToLower().Trim() == model.ModuleName.ToLower().Trim() && x.ParentProjectModuleId == model.ParentProjectModuleId && x.ProjectId == projectId && x.ProjectModuleListItem.ListItemSystemName == ListItem.Module.ToString()).AnyAsync();

								if (moduleNameValid == false)
								{
									bool functionNameValid = await _context.ProjectModule.Where(x => x.ModuleName.ToLower().Trim() == model.ModuleName.ToLower().Trim() && x.ParentProjectModuleId == model.ParentProjectModuleId && x.ProjectId == projectId && x.ProjectModuleListItem.ListItemSystemName == ListItem.Function.ToString()).AnyAsync();
									if (functionNameValid == false)
									{
										bool testCaseNameValid = await _context.ProjectModule.Where(x => x.ModuleName.ToLower().Trim() == model.ModuleName.ToLower().Trim() && x.ParentProjectModuleId == model.ParentProjectModuleId && x.ProjectId == projectId && x.ProjectModuleListItem.ListItemSystemName == ListItem.TestCase.ToString()).AnyAsync();
										if (testCaseNameValid == false)
										{


											Entities.ProjectModule projectModule = new Entities.ProjectModule();
											var project = _context.Project.Where(x => x.ProjectSlug == model.ProjectSlug).FirstOrDefault();
											bool module = model.ProjectModuleType == nameof(ListItem.Module);
											bool testCase = model.ProjectModuleType == nameof(ListItem.TestCase);

											projectModule.ParentProjectModuleId = module && model.ParentProjectModuleId == null ? null : model.ParentProjectModuleId;
											projectModule.ProjectId = projectId;
											projectModule.ModuleName = model.ModuleName;
											projectModule.ProjectModuleListItemId = projectModuleListItemid.ListItemId;
											projectModule.Description = model.Description;
											projectModule.OrderDate = DateTimeOffset.UtcNow;
											projectModule.IsDeleted = false;
											project.UpdateDate = DateTimeOffset.UtcNow;
											await _context.AddAsync(projectModule);
											await _context.SaveChangesAsync();


											if (testCase)
											{


												foreach (var testCaseDetail in model.TestCaseStepDetailModel)
												{
													if (string.IsNullOrEmpty(testCaseDetail.ExpectedResultTestStep) || string.IsNullOrWhiteSpace(testCaseDetail.ExpectedResultTestStep) || string.IsNullOrEmpty(testCaseDetail.StepDescription) || string.IsNullOrWhiteSpace(testCaseDetail.StepDescription))
													{
														throw new TestCaseStepDetailDataNullOrEmptyException();

													}
													else
													{
														var testCaseStepDetails = new Entities.TestCaseStepDetail
														{
															ProjectModuleId = projectModule.ProjectModuleId,
															StepNumber = testCaseDetail.StepNumber,
															StepDescription = testCaseDetail.StepDescription,
															ExpectedResult = testCaseDetail.ExpectedResultTestStep,
															TestCaseListItemId = model.TestCaseListItemId
														};
														project.UpdateDate = DateTimeOffset.UtcNow;
														await _context.AddAsync(testCaseStepDetails);
														await _context.SaveChangesAsync();
													}


												}
												if (string.IsNullOrEmpty(model.PreCondition) || string.IsNullOrWhiteSpace(model.PreCondition) || string.IsNullOrEmpty(model.ExpectedResult) || string.IsNullOrWhiteSpace(model.ExpectedResult))
												{
													throw new TestCaseDataNullOrEmptyException();
												}
												else
												{
													var testCases = new Entities.TestCaseDetail
													{
														ProjectModuleId = projectModule.ProjectModuleId,
														PreCondition = model.PreCondition,
														ExpectedResult = model.ExpectedResult,
														TestCaseListItemId = model.TestCaseListItemId

													};
													project.UpdateDate = DateTimeOffset.UtcNow;
													await _context.AddAsync(testCases);
													await _context.SaveChangesAsync();
												}
											}

											transaction.Commit();
											return Result<string>.Success(ReturnMessage.SavedSuccessfully);

										}
										else
										{
											throw new ProjectTestCaseNameAlreadyExistException();
										}

									}
									else
									{
										throw new ProjectFunctionNameAlreadyExistException();
									}
								}

								else
								{
									throw new ProjectModuleNameAlreadyExistException();
								}
							}
							else
							{
								throw new ProjectModuleTypeIsNotFoundException();
							}
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

				transaction.Rollback();
				if (ex is ProjectSlugNotFoundException)
				{
					_iLogger.LogError("Could not find the ProjectSlug. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ProjectSlugDoesNotExists);
				}
				else if (ex is UnAuthorizedUserException)
				{
					_iLogger.LogError("Unauthorized User. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.UnauthorizedUser);
				}
				else if (ex is ProjectModuleTypeIsNotFoundException)
				{
					_iLogger.LogError("Could not find the ProjectModuleListItemId. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ProjectModuleTypeIsNotFound);
				}
				else if (ex is ModuleNameNullOrEmptyException)
				{
					_iLogger.LogError("Module Name cannot be empty. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ModuleNameCannotBeEmpty);
				}
				else if (ex is TestCaseDataNullOrEmptyException)
				{
					_iLogger.LogError("Either Pre-Condition Or Expected Result is empty. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestCaseDataCannotBeEmpty);
				}
				else if (ex is TestCaseStepDetailDataNullOrEmptyException)
				{
					_iLogger.LogError("Either Step Description Or Expected Result  is empty. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestCaseStepDetailDataCannotBeEmpty);
				}
				else if (ex is ProjectModuleNameAlreadyExistException)
				{
					_iLogger.LogError("Module Name is already exist. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ProjectModuleNameAlreadyExist);
				}
				else if (ex is ProjectFunctionNameAlreadyExistException)
				{
					_iLogger.LogError("Function Name is already exist. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ProjectFunctionNameAlreadyExist);
				}
				else if (ex is ProjectTestCaseNameAlreadyExistException)
				{
					_iLogger.LogError("TestCase Name is already exist. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ProjectTestCaseNameAlreadyExist);
				}

				_iLogger.LogError("Could not add the ProjectModule. Exception message is: ", ex.Message);
				return Result<string>.Error(ReturnMessage.FailedToAddProjectModule);
			}
		}
		public async Task<Result<string>> UpdateProjectModuleAysnc(AddUpdateProjectModuleModel model)
		{
			using var transaction = _context.Database.BeginTransaction();
			try
			{

				bool hasProjectPermission = await _iCommonService.CheckProjectPermission(model.ProjectSlug, ProjectRoleSlug.UpdateProjectModule);
				if (hasProjectPermission)

				{
					var projectId = _context.Project.Where(x => x.ProjectSlug == model.ProjectSlug).FirstOrDefault().ProjectId;
					if (projectId > 0)
					{
						if (string.IsNullOrEmpty(model.ModuleName) || string.IsNullOrWhiteSpace(model.ModuleName))
						{
							throw new ModuleNameNullOrEmptyException();
						}
						else
						{
							var projectModuleListItemid = await _context.ListItem.FirstOrDefaultAsync(x => x.ListItemSystemName == model.ProjectModuleType);
							if (projectModuleListItemid != null)
							{
								bool moduleNameValid = await _context.ProjectModule.Where(x => x.ModuleName.ToLower().Trim() == model.ModuleName.ToLower().Trim() && x.ParentProjectModuleId == model.ParentProjectModuleId && x.ProjectId == projectId && x.ProjectModuleListItem.ListItemSystemName == ListItem.Module.ToString() && x.ProjectModuleId != model.ProjectModuleId).AnyAsync();

								if (moduleNameValid == false)
								{
									bool functionNameValid = await _context.ProjectModule.Where(x => x.ModuleName.ToLower().Trim() == model.ModuleName.ToLower().Trim() && x.ParentProjectModuleId == model.ParentProjectModuleId && x.ProjectId == projectId && x.ProjectModuleListItem.ListItemSystemName == ListItem.Function.ToString() && x.ProjectModuleId != model.ProjectModuleId).AnyAsync();
									if (functionNameValid == false)
									{
										bool testCaseNameValid = await _context.ProjectModule.Where(x => x.ModuleName.ToLower().Trim() == model.ModuleName.ToLower().Trim() && x.ParentProjectModuleId == model.ParentProjectModuleId && x.ProjectId == projectId && x.ProjectModuleListItem.ListItemSystemName == ListItem.TestCase.ToString() && x.ProjectModuleId != model.ProjectModuleId).AnyAsync();

										if (testCaseNameValid == false)
										{
											var updateProjectModule = await _context.ProjectModule.FirstOrDefaultAsync(x => x.ProjectModuleId == model.ProjectModuleId);
											var project = _context.Project.Where(x => x.ProjectSlug == model.ProjectSlug).FirstOrDefault();

											bool module = model.ProjectModuleType == nameof(ListItem.Module);
											bool testCase = model.ProjectModuleType == nameof(ListItem.TestCase);

											updateProjectModule.ParentProjectModuleId = module && updateProjectModule.ParentProjectModuleId == null ? null : model.ParentProjectModuleId;
											updateProjectModule.ProjectId = projectId;
											updateProjectModule.ModuleName = model.ModuleName;
											updateProjectModule.ProjectModuleListItemId = projectModuleListItemid.ListItemId;
											updateProjectModule.Description = model.Description;
											project.UpdateDate = DateTimeOffset.UtcNow;

											_context.ProjectModule.Update(updateProjectModule);
											await _context.SaveChangesAsync();


											if (testCase)
											{
												List<int> deleteTestCasesStepDetails = model.DeleteTestCaseStepDetailId;

												var deleteTestCaseStepDetailId = await _context.TestCaseStepDetail.Where(x => deleteTestCasesStepDetails.Contains(x.TestCaseStepDetailId)).ToListAsync();
												if (deleteTestCaseStepDetailId.Count > 0)
												{
													_context.TestCaseStepDetail.RemoveRange(deleteTestCaseStepDetailId);
													await _context.SaveChangesAsync();
												}



												var testCaseDetail = await _context.TestCaseDetail
													.FirstOrDefaultAsync(x =>
													x.TestCaseDetailId == model.TestCaseDetailId);


												if (model.TestCaseStepDetailModel != null)
												{
													foreach (var listItemTestCaseStepDetail in model.TestCaseStepDetailModel)
													{
														var updateTestCaseStepDetail = await _context.TestCaseStepDetail
															.Where(x =>
															x.TestCaseStepDetailId == listItemTestCaseStepDetail.TestCaseStepDetailId).FirstOrDefaultAsync();
														if (updateTestCaseStepDetail != null)
														{
															if (string.IsNullOrEmpty(listItemTestCaseStepDetail.ExpectedResultTestStep)
																|| string.IsNullOrWhiteSpace(listItemTestCaseStepDetail.ExpectedResultTestStep)
																|| string.IsNullOrEmpty(listItemTestCaseStepDetail.StepDescription)
																|| string.IsNullOrWhiteSpace(listItemTestCaseStepDetail.StepDescription))
															{
																throw new TestCaseStepDetailDataNullOrEmptyException();

															}
															else
															{
																updateTestCaseStepDetail.TestCaseStepDetailId = listItemTestCaseStepDetail.TestCaseStepDetailId;
																updateTestCaseStepDetail.StepDescription = listItemTestCaseStepDetail.StepDescription;
																updateTestCaseStepDetail.StepNumber = listItemTestCaseStepDetail.StepNumber;
																updateTestCaseStepDetail.ExpectedResult = listItemTestCaseStepDetail.ExpectedResultTestStep;
																updateTestCaseStepDetail.TestCaseListItemId = model.TestCaseListItemId;
																project.UpdateDate = DateTimeOffset.UtcNow;
																_context.TestCaseStepDetail.Update(updateTestCaseStepDetail);
																await _context.SaveChangesAsync();
															}

														}
														else
														{
															if (string.IsNullOrEmpty(listItemTestCaseStepDetail.ExpectedResultTestStep)
																 || string.IsNullOrWhiteSpace(listItemTestCaseStepDetail.ExpectedResultTestStep)
																 || string.IsNullOrEmpty(listItemTestCaseStepDetail.StepDescription)
																 || string.IsNullOrWhiteSpace(listItemTestCaseStepDetail.StepDescription))
															{
																throw new TestCaseStepDetailDataNullOrEmptyException();

															}
															else
															{
																var testCaseStepDetail = new Entities.TestCaseStepDetail
																{
																	ProjectModuleId = updateProjectModule.ProjectModuleId,
																	StepNumber = listItemTestCaseStepDetail.StepNumber,
																	StepDescription = listItemTestCaseStepDetail.StepDescription,
																	ExpectedResult = listItemTestCaseStepDetail.ExpectedResultTestStep,
																	TestCaseListItemId = model.TestCaseListItemId
																};
																project.UpdateDate = DateTimeOffset.UtcNow;
																await _context.AddAsync(testCaseStepDetail);
																await _context.SaveChangesAsync();
															}

														}
													}
												}
												else
												{
													throw new TestCaseStepDetailIdNotFoundException();

												}


												if (testCaseDetail != null)
												{
													if (string.IsNullOrEmpty(model.PreCondition) || string.IsNullOrWhiteSpace(model.PreCondition) || string.IsNullOrEmpty(model.ExpectedResult) || string.IsNullOrWhiteSpace(model.ExpectedResult))
													{
														throw new TestCaseDataNullOrEmptyException();
													}
													else
													{
														testCaseDetail.TestCaseDetailId = model.TestCaseDetailId;
														testCaseDetail.ProjectModuleId = model.ProjectModuleId;
														testCaseDetail.PreCondition = model.PreCondition;
														testCaseDetail.ExpectedResult = model.ExpectedResult;
														testCaseDetail.TestCaseListItemId = model.TestCaseListItemId;
														project.UpdateDate = DateTimeOffset.UtcNow;
														_context.TestCaseDetail.Update(testCaseDetail);
														await _context.SaveChangesAsync();
													}

												}
												else
												{
													throw new TestCaseDetailIdNotFoundException();
												}


											}

											transaction.Commit();
											return Result<string>.Success(ReturnMessage.UpdatedSuccessfully);
										}
										else
										{
											throw new ProjectTestCaseNameAlreadyExistException();
										}

									}
									else
									{
										throw new ProjectFunctionNameAlreadyExistException();
									}
								}

								else
								{
									throw new ProjectModuleNameAlreadyExistException();
								}
							}

							else
							{
								throw new ProjectModuleTypeIsNotFoundException();
							}
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
				transaction.Rollback();
				if (ex is ProjectSlugNotFoundException)
				{
					_iLogger.LogError("Could not find the ProjectSlug. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ProjectSlugDoesNotExists);
				}
				else if (ex is UnAuthorizedUserException)
				{
					_iLogger.LogError("Unauthorized User. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.UnauthorizedUser);
				}
				else if (ex is ProjectModuleTypeIsNotFoundException)
				{
					_iLogger.LogError("Could not find the ProjectModuleListItemId. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ProjectModuleTypeIsNotFound);
				}
				else if (ex is TestCaseDetailIdNotFoundException)
				{
					_iLogger.LogError("Could not find the TestCaseDetailId. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestCaseDetailId);
				}
				else if (ex is TestCaseStepDetailIdNotFoundException)
				{
					_iLogger.LogError("Could not find the TestCaseStepDetail. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestCaseStepDetail);
				}
				else if (ex is ModuleNameNullOrEmptyException)
				{
					_iLogger.LogError("Module Name cannot be empty. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ModuleNameCannotBeEmpty);
				}
				else if (ex is TestCaseDataNullOrEmptyException)
				{
					_iLogger.LogError("Either Pre-Condition Or Expected Result is empty. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestCaseDataCannotBeEmpty);
				}
				else if (ex is TestCaseStepDetailDataNullOrEmptyException)
				{
					_iLogger.LogError("Either Step Description Or Expected Result  is empty. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestCaseStepDetailDataCannotBeEmpty);
				}
				else if (ex is ProjectModuleNameAlreadyExistException)
				{
					_iLogger.LogError("Module Name is already exist. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ProjectModuleNameAlreadyExist);
				}
				else if (ex is ProjectFunctionNameAlreadyExistException)
				{
					_iLogger.LogError("Function Name is already exist. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ProjectFunctionNameAlreadyExist);
				}
				else if (ex is ProjectTestCaseNameAlreadyExistException)
				{
					_iLogger.LogError("TestCase Name is already exist. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ProjectTestCaseNameAlreadyExist);
				}

				_iLogger.LogError("Could not update the ProjectModule. Exception message is: ", ex.Message);
				return Result<string>.Error(ReturnMessage.FailedToUpdateProjectModule);
			}
		}
		public async Task<Result<string>> AddProjectModuleDeveloperAsync(AddProjectModuleDeveloperModel model)
		{
			using var transaction = _context.Database.BeginTransaction();
			try
			{
				var slug = _context.ProjectModule.Include(x => x.Project).Where(x => x.ProjectModuleId == model.ProjectModuleId).FirstOrDefault().Project.ProjectSlug;
				var projectId = _context.ProjectModule.Include(x => x.Project).Where(x => x.ProjectModuleId == model.ProjectModuleId).FirstOrDefault().Project.ProjectId;

				if (slug != null)
				{
					bool hasProjectPermission = await _iCommonService.CheckProjectPermission(slug, ProjectRoleSlug.CreateProjectModuleDeveloper);
					if (hasProjectPermission)
					{

						List<int> projectMemberId = model.ProjectMemberId;
						var projectModuleDeveloper = projectMemberId.Select(x => new Entities.ProjectModuleDeveloper
						{
							ProjectModuleId = model.ProjectModuleId,
							ProjectMemberId = x,
							IsDisabled = model.IsDisabled,
						}).ToList();

						var project = _context.Project.Where(x => x.ProjectId == projectId).FirstOrDefault();
						project.UpdateDate = DateTimeOffset.UtcNow;

						await _context.AddRangeAsync(projectModuleDeveloper);
						await _context.SaveChangesAsync();
						transaction.Commit();
						return Result<string>.Success(ReturnMessage.SavedSuccessfully);
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
					_iLogger.LogError("Unauthorized User. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.UnauthorizedUser);
				}
				else if (ex is ProjectSlugNotFoundException)
				{
					_iLogger.LogError("Could not find the ProjectSlug. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ProjectSlugDoesNotExists);

				}
				_iLogger.LogError("Could not add the ProjectModuleDeveloper. Exception message is: ", ex.Message);
				return Result<string>.Error(ReturnMessage.FailedToAddProjectModuleDeveloper);

			}

		}
		public async Task<Result<string>> UpdateProjectModuleDeveloperAsync(UpdateProjectModuleListDeveloperModel model)
		{
			using var transaction = _context.Database.BeginTransaction();
			try
			{
				var slug = _context.ProjectModule.Include(x => x.Project).Where(x => x.ProjectModuleId == model.UpdateProjectModuleDeveloperModel.Select(x => x.ProjectModuleId).FirstOrDefault()).FirstOrDefault().Project.ProjectSlug;
				var projectId = _context.ProjectModule.Include(x => x.Project).Where(x => x.ProjectModuleId == model.UpdateProjectModuleDeveloperModel.Select(x => x.ProjectModuleId).FirstOrDefault()).FirstOrDefault().Project.ProjectId;

				if (slug != null)
				{
					bool hasProjectPermission = await _iCommonService.CheckProjectPermission(slug, ProjectRoleSlug.UpdateProjectModuleDeveloper);
					if (hasProjectPermission)
					{

						foreach (var listItemProjectModuleDeveloper in model.UpdateProjectModuleDeveloperModel)
						{

							var updateProjectModuleDeveloper = await _context.ProjectModuleDeveloper
								.Where(x =>
								x.ProjectModuleDeveloperId == listItemProjectModuleDeveloper.ProjectModuleDeveloperId).FirstOrDefaultAsync();
							if (updateProjectModuleDeveloper != null)
							{
								updateProjectModuleDeveloper.ProjectModuleDeveloperId = listItemProjectModuleDeveloper.ProjectModuleDeveloperId;
								updateProjectModuleDeveloper.ProjectMemberId = listItemProjectModuleDeveloper.ProjectMemberId;
								updateProjectModuleDeveloper.ProjectModuleId = listItemProjectModuleDeveloper.ProjectModuleId;
								updateProjectModuleDeveloper.IsDisabled = listItemProjectModuleDeveloper.IsDisabled;
								var project = _context.Project.Where(x => x.ProjectId == projectId).FirstOrDefault();
								project.UpdateDate = DateTimeOffset.UtcNow;
								_context.ProjectModuleDeveloper.Update(updateProjectModuleDeveloper);
								await _context.SaveChangesAsync();
							}
							else
							{
								return Result<string>.Success(null);
							}

						}

						transaction.Commit();
						return Result<string>.Success(ReturnMessage.UpdatedSuccessfully);
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
					_iLogger.LogError("Unauthorized User. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.UnauthorizedUser);
				}
				else if (ex is ProjectSlugNotFoundException)
				{
					_iLogger.LogError("Could not find the ProjectSlug. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ProjectSlugDoesNotExists);

				}
				_iLogger.LogError("Could not update the ProjectModuleDeveloper. Exception message is: ", ex.Message);
				return Result<string>.Error(ReturnMessage.FailedToUpdateProjectModuleDeveloper);
			}
		}
		public async Task<Result<List<ProjectModuleModel>>> GetProjectModuleListAsync(string projectSlug)
		{
			try
			{
				if (projectSlug != null)
				{
					//Project Role Permission Check
					bool hasProjectPermission = await _iCommonService.CheckProjectPermission(projectSlug, ProjectRoleSlug.ViewProjectModule);
					if (hasProjectPermission)
					{
						var getProjectIdByProjectSlug = await _context.Project.FirstOrDefaultAsync(x => x.ProjectSlug == projectSlug);

						if (getProjectIdByProjectSlug != null)
						{
							var getProjectModuleDeveloperListAsync = await (from pmd in _context.ProjectModuleDeveloper
																			join pm in _context.ProjectMember on pmd.ProjectMemberId equals pm.ProjectMemberId
																			join p in _context.Project on pm.ProjectId equals p.ProjectId
																			where pm.ProjectId == p.ProjectId && !pmd.IsDisabled

																			select new ProjectModuleDeveloperListModel
																			{
																				ProjectModuleDeveloperId = pmd.ProjectModuleDeveloperId,
																				ProjectModuleId = pmd.ProjectModuleId,
																				PersonId = pm.PersonId

																			}).ToListAsync();

							var getProjectModuleListAsync = await (from pm in _context.ProjectModule
																   join li in _context.ListItem on pm.ProjectModuleListItemId equals li.ListItemId
																   where pm.ProjectId == getProjectIdByProjectSlug.ProjectId && pm.IsDeleted == false
																   orderby pm.OrderDate
																   select new ProjectModuleModel
																   {
																	   ProjectModuleId = pm.ProjectModuleId,
																	   ModuleName = pm.ModuleName,
																	   OrderDate = pm.OrderDate,
																	   ParentProjectModuleId = pm.ParentProjectModuleId,
																	   ProjectModuleType = li.ListItemSystemName,
																	   ProjectId = pm.ProjectId,
																	   Description = pm.Description,
																	   ProjectModuleListItemId = pm.ProjectModuleListItemId,
																	   IsDeleted = pm.IsDeleted

																   }).ToListAsync();

							getProjectModuleListAsync = getProjectModuleListAsync.OrderBy(x => x.OrderDate)
											.Where(c => c.ParentProjectModuleId == null && c.IsDeleted == false)
											.Select(c => new ProjectModuleModel()
											{
												ProjectModuleId = c.ProjectModuleId,
												ModuleName = c.ModuleName,
												ParentProjectModuleId = c.ParentProjectModuleId,
												ProjectModuleType = c.ProjectModuleType,
												ProjectId = c.ProjectId,
												OrderDate = c.OrderDate,
												ProjectModuleListItemId = c.ProjectModuleListItemId,
												Description = c.Description,
												IsDeleted = c.IsDeleted,
												ChildModule = ChildModule(getProjectModuleListAsync
												.Where(x => x.ParentProjectModuleId != null).ToList(),
												getProjectModuleDeveloperListAsync, c.ProjectModuleId)
											})
											.ToList();

							if (getProjectModuleListAsync.Any())
							{
								return Result<List<ProjectModuleModel>>.Success(getProjectModuleListAsync);
							}
							else
							{
								return Result<List<ProjectModuleModel>>.Success(null);
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
				else
				{
					throw new ProjectSlugNotFoundException();

				}
			}
			catch (Exception ex)
			{
				if (ex is UnAuthorizedUserException)
				{
					_iLogger.LogError("Unauthorized User. Exception message is: ", ex.Message);
					return Result<List<ProjectModuleModel>>.Error(ReturnMessage.UnauthorizedUser);
				}
				else if (ex is ProjectSlugNotFoundException)
				{
					_iLogger.LogError("Could not find the ProjectSlug. Exception message is: ", ex.Message);
					return Result<List<ProjectModuleModel>>.Error(ReturnMessage.ProjectSlugDoesNotExists);

				}
				else
				{

					return Result<List<ProjectModuleModel>>.Error(null);
				}

			}


		}
		public async Task<Result<List<ProjectModuleDeveloperModelList>>> GetProjectModuleDeveloperListAsync()
		{
			var getProjectModuleDeveloperList = await (from pmd in _context.ProjectModuleDeveloper

													   join pm in _context.ProjectMember
													   on pmd.ProjectMemberId equals pm.ProjectMemberId

													   join pmo in _context.ProjectModule
													   on pmd.ProjectModuleId equals pmo.ProjectModuleId

													   join p in _context.Person on pm.PersonId equals p.PersonId
													   select new ProjectModuleDeveloperModelList
													   {
														   ProjectModuleDeveloperId = pmd.ProjectModuleDeveloperId,
														   ProjectModuleId = pmd.ProjectModuleId,
														   ProjectMemberId = pmd.ProjectMemberId,
														   Member = p.Name,
														   IsDisabled = pmd.IsDisabled

													   }).ToListAsync();
			if (getProjectModuleDeveloperList.Any())
			{
				return Result<List<ProjectModuleDeveloperModelList>>.Success(getProjectModuleDeveloperList);
			}
			else
			{
				return Result<List<ProjectModuleDeveloperModelList>>.Success(null);
			}


		}
		public async Task<Result<List<TestCaseModel>>> GetTestCaseListAsync()
		{
			var getTestCaseListAsync = await (from t in _context.TestCaseDetail
											  join pmo in _context.ProjectModule on t.ProjectModuleId equals pmo.ProjectModuleId

											  select new TestCaseModel
											  {
												  ProjectModuleId = t.ProjectModuleId,
												  PreCondition = t.PreCondition,
												  ExpectedResult = t.ExpectedResult
											  }).ToListAsync();
			if (getTestCaseListAsync.Any())
			{
				return Result<List<TestCaseModel>>.Success(getTestCaseListAsync);
			}
			else
			{
				return Result<List<TestCaseModel>>.Success(null);
			}
		}
		public async Task<Result<string>> DeleteProjectModuleAsync(int projectModuleId)
		{
			using var transaction = _context.Database.BeginTransaction();
			try
			{
				var projectModule = await _context.ProjectModule.Include(x => x.ProjectModuleListItem).Include(x => x.Project)
					.Where(x =>
					x.ProjectModuleId == projectModuleId).FirstOrDefaultAsync();

				if (projectModule != null)
				{
					bool hasProjectPermission = await _iCommonService.CheckProjectPermission(projectModule.Project.ProjectSlug, ProjectRoleSlug.DeleteProjectModule);
					if (hasProjectPermission)
					{

						switch (projectModule.ProjectModuleListItem.ListItemSystemName)
						{
							case nameof(ListItem.Module):
								await ModuleDelete(projectModule);
								break;
							case nameof(ListItem.Function):
								await FunctionDelete(projectModule);
								break;
							case nameof(ListItem.TestCase):
								await TestCaseDelete(projectModule);

								break;

							default:
								throw new FailedToDeleteProjectModuleException();
						}

						transaction.Commit();
						if (projectModule.ProjectModuleListItem.ListItemSystemName == ListItem.Module.ToString())
						{
							return Result<string>.Success(ReturnMessage.ModuleDeletedSuccessfully);

						}
						else if (projectModule.ProjectModuleListItem.ListItemSystemName == ListItem.Function.ToString())
						{
							return Result<string>.Success(ReturnMessage.FunctionDeletedSuccessfully);

						}
						else
						{
							return Result<string>.Success(ReturnMessage.TestCaseDeletedSuccessfully);

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
				else if (ex is FunctionContainsTestCasesException)
				{
					_iLogger.LogError("Function Contains TestCases. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.FunctionContainsTestCases);

				}
				else if (ex is ModuleContainsFunctionException)
				{
					_iLogger.LogError("Modules Contain Function. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ModuleContainsFunction);

				}
				else if (ex is ModuleContainsNestedModuleException)
				{
					_iLogger.LogError("Modules Contain Nested Module. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ModuleContainsNestedModule);

				}

				else
				{
					_iLogger.LogError("Could not delete the ProjectModule. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.FailedToDeleteProjectModule);

				}

			}
		}
		public async Task<Result<string>> DeleteTestCaseAsync(int testCaseId)
		{
			using var transaction = _context.Database.BeginTransaction();
			try
			{
				var testCase = await _context.TestCaseDetail.Where(x => x.TestCaseDetailId == testCaseId).FirstOrDefaultAsync();
				if (testCase != null)
				{

					_context.TestCaseDetail.Remove(testCase);
					await _context.SaveChangesAsync();
					transaction.Commit();
					return Result<string>.Success(ReturnMessage.DeletedSuccessfully);

				}
				else
				{
					return Result<string>.Success(null);
				}
			}
			catch (System.Exception ex)
			{
				_iLogger.LogError("Could not delete the TestCase. Exception message is: ", ex.Message);
				transaction.Rollback();
				return Result<string>.Error(ReturnMessage.FailedToDeleteTestCase);
			}
		}
		public async Task<Result<string>> DeleteProjectModuleDeveloperAsync(int projectModuleDeveloperId)
		{
			using var transaction = _context.Database.BeginTransaction();
			try
			{
				var projectModuleDeveloper = await _context.ProjectModuleDeveloper.Where(x => x.ProjectModuleDeveloperId == projectModuleDeveloperId).FirstOrDefaultAsync();
				var pmd = await _context.ProjectModuleDeveloper.Include(x => x.ProjectModule).Where(x => x.ProjectModuleDeveloperId == projectModuleDeveloperId).FirstOrDefaultAsync();

				if (projectModuleDeveloper != null)
				{
					_context.ProjectModuleDeveloper.Remove(projectModuleDeveloper);
					var project = _context.Project.Where(x => x.ProjectId == pmd.ProjectModule.ProjectId).FirstOrDefault();
					project.UpdateDate = DateTimeOffset.UtcNow;

					await _context.SaveChangesAsync();
					transaction.Commit();
					return Result<string>.Success(ReturnMessage.DeletedSuccessfully);

				}
				else
				{
					return Result<string>.Success(null);
				}
			}
			catch (System.Exception ex)
			{
				_iLogger.LogError("Could not delete the ProjectModuleDeveloper. Exception message is: ", ex.Message);
				transaction.Rollback();
				return Result<string>.Error(ReturnMessage.FailedToDeleteProjectModuleDeveloper);
			}
		}	
		public async Task<Result<List<ProjectMemberDeveloperListModel>>> GetProjectMemberDeveloperListAsync(string projectSlug)
		{
			try
			{
				if (projectSlug != null)
				{
					//Project Role Permission Check
					bool hasProjectPermission = await _iCommonService.CheckProjectPermission(projectSlug, ProjectRoleSlug.ViewProjectModuleDeveloper);
					if (hasProjectPermission)
					{
						var projectMemberDeveloperList = await (from p in _context.Person
																join pm in _context.ProjectMember on p.PersonId equals pm.PersonId
																join proj in _context.Project on pm.ProjectId equals proj.ProjectId
																where proj.ProjectSlug == projectSlug
																select new ProjectMemberDeveloperListModel
																{
																	ProjectMemberId = pm.ProjectMemberId,
																	PersonId = p.PersonId,
																	ProjectSlug = proj.ProjectSlug,
																	Developer = p.Name
																}).ToListAsync();

						if (projectMemberDeveloperList.Any())
						{
							return Result<List<ProjectMemberDeveloperListModel>>.Success(projectMemberDeveloperList);
						}
						else
						{
							return Result<List<ProjectMemberDeveloperListModel>>.Success(null);
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
					_iLogger.LogError("Unauthorized User. Exception message is: ", ex.Message);
					return Result<List<ProjectMemberDeveloperListModel>>.Error(ReturnMessage.UnauthorizedUser);
				}
				else if (ex is ProjectSlugNotFoundException)
				{
					_iLogger.LogError("Could not find the ProjectSlug. Exception message is: ", ex.Message);
					return Result<List<ProjectMemberDeveloperListModel>>.Error(ReturnMessage.ProjectSlugDoesNotExists);

				}
				return Result<List<ProjectMemberDeveloperListModel>>.Error(null);

			}


		}
		public async Task<Result<GetTestCaseDetailListModel>> GetTestCaseDetailListById(int testCaseId)
		{
			try
			{
				var testCaseDetail = await _context.TestCaseDetail.Where(x => x.ProjectModuleId == testCaseId).FirstOrDefaultAsync();
				if (testCaseDetail != null)
				{
					var getTestCaseListByIdAsync = await (from t in _context.TestCaseDetail
														  join tc in _context.TestCaseStepDetail on t.ProjectModuleId equals tc.ProjectModuleId
														  join pi in _context.Person on t.InsertPersonId equals pi.PersonId
														  join pu in _context.Person on t.UpdatePersonId equals pu.PersonId
														  where t.ProjectModuleId == testCaseId
														  select new GetTestCaseDetailListModel
														  {
															  TestCaseDetailId = t.TestCaseDetailId,
															  ParentProjectModuleId = t.ProjectModuleId,
															  ExpectedResult = t.ExpectedResult,
															  PreCondition = t.PreCondition,
															  CreatedBy = pi.Name,
															  UpdatedBy = t.InsertPersonId == t.UpdatePersonId ? pi.Name : pu.Name,
															  TestCaseListItemId = t.TestCaseListItemId,
															  TestCaseStepDetailModel = TestDetailStep(testCaseDetail.ProjectModuleId)

														  }).FirstOrDefaultAsync();


					if (getTestCaseListByIdAsync != null)
					{
						return Result<GetTestCaseDetailListModel>.Success(getTestCaseListByIdAsync);
					}
					else
					{
						return Result<GetTestCaseDetailListModel>.Success(null);
					}

				}
				else
				{
					throw new ProjectModuleIdNotFoundException();
				}
			}
			catch (Exception ex)
			{
				if (ex is UnAuthorizedUserException)
				{
					_iLogger.LogError("Unauthorized User. Exception message is: ", ex.Message);
					return Result<GetTestCaseDetailListModel>.Error(ReturnMessage.UnauthorizedUser);
				}
				else if (ex is ProjectModuleIdNotFoundException)
				{
					_iLogger.LogError("Could not find the ProjectSlug. Exception message is: ", ex.Message);
					return Result<GetTestCaseDetailListModel>.Error(ReturnMessage.ProjectModuleIdNotFound);

				}
				_iLogger.LogError("Could not find the TestCaseDetailId. Exception message is: ", ex.Message);
				return Result<GetTestCaseDetailListModel>.Error(ReturnMessage.TestCaseDetailIdNotFound);
			}

		}
		public async Task<Result<GetDeveloperDetailModel>> GetDeveloperDetailByFunctionId(int functionId)
		{
			try
			{
				var function = await _context.ProjectModule.Include(x => x.Project).Where(x => x.ProjectModuleId == functionId).FirstOrDefaultAsync();
				if (function != null)
				{
					//Project Role Permission Check
					bool hasProjectPermission = await _iCommonService.CheckProjectPermission(function.Project.ProjectSlug, ProjectRoleSlug.ViewProjectModuleDeveloper);
					if (hasProjectPermission)
					{
						var getProjectModuleDeveloperListAsync = await (from pmd in _context.ProjectModuleDeveloper
																		join pmo in _context.ProjectModule on pmd.ProjectModuleId equals pmo.ProjectModuleId
																		where pmo.ProjectModuleId == functionId
																		select new GetDeveloperDetailModel
																		{
																			FunctionId = pmo.ProjectModuleId,
																			ProjectModuleDeveloperId = pmd.ProjectModuleDeveloperId,
																			ProjectModuleId = pmd.ProjectModuleId,
																			ProjectModuleDeveloperFunctionModel = DeveloperList(functionId)
																		}).FirstOrDefaultAsync();


						if (getProjectModuleDeveloperListAsync != null)
						{
							return Result<GetDeveloperDetailModel>.Success(getProjectModuleDeveloperListAsync);
						}
						else
						{
							return Result<GetDeveloperDetailModel>.Success(null);
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
					_iLogger.LogError("Unauthorized User. Exception message is: ", ex.Message);
					return Result<GetDeveloperDetailModel>.Error(ReturnMessage.UnauthorizedUser);
				}
				else if (ex is ProjectSlugNotFoundException)
				{
					_iLogger.LogError("Could not find the ProjectSlug. Exception message is: ", ex.Message);
					return Result<GetDeveloperDetailModel>.Error(ReturnMessage.ProjectSlugDoesNotExists);

				}
				_iLogger.LogError("Could not find the FunctionId. Exception message is: ", ex.Message);
				return Result<GetDeveloperDetailModel>.Error(ReturnMessage.FunctionIdNotFound);

			}

		}

		//Function for test  case import
		public async Task<Result<string>> ImportProjectModuleTestCaseAsync(ImportProjectModuleModel projectModule)
		{
			using var transaction = _context.Database.BeginTransaction();
			try
			{
				var projectSlug = await _context.Project.Where(x => x.ProjectSlug == projectModule.ProjectSlug).AnyAsync();
				if (projectSlug)
				{


					var projects = await (from pm in _context.ProjectModule
										  join p in _context.Project on pm.ProjectId equals p.ProjectId
										  join li in _context.ListItem on pm.ProjectModuleListItemId equals li.ListItemId
										  where p.ProjectSlug == projectModule.ProjectSlug && pm.ProjectModuleId == projectModule.ProjectModuleId
										  select new ImportProjectModel
										  {
											  ProjectModuleId = pm.ProjectModuleId,
											  ProjectSlug = p.ProjectSlug,
											  ProjectId = p.ProjectId,
											  ParentProjectModuleId = pm.ParentProjectModuleId,
											  ProjectModuleType = pm.ProjectModuleListItem.ListItemSystemName,
										  }).FirstOrDefaultAsync();

					if (projects != null)
					{
						//Project Role Permission Check
						bool hasProjectPermission = await _iCommonService.CheckProjectPermission(projects.ProjectSlug, ProjectRoleSlug.CreateProjectModule);
						if (hasProjectPermission)
						{
							if (projects.ProjectModuleType == ListItem.Function.ToString())
							{
								var getExcelData = TestCaseFileImportHelper.ImportFile(projectModule);
								var projectDate = _context.Project.Where(x => x.ProjectSlug == projectModule.ProjectSlug).FirstOrDefault();
								var projectModuleListItem = await (_iCommonService.GetListItemDetailByListItemSystemName(ListItem.TestCase.ToString()));

								var testCaseTypeLists = await (from li in _context.ListItem
															   join lic in _context.ListItemCategory on li.ListItemCategoryId equals lic.ListItemCategoryId
															   where lic.ListItemCategoryName == ListItemCategory.TestCaseType.ToString()
															   select new ListItemModel
															   {
																   ListItemId = li.ListItemId,
																   ListItemSystemName = li.ListItemSystemName,

															   }).ToListAsync();



								var projectModuleList = await (from pm in _context.ProjectModule
															   join li in _context.ListItem on pm.ProjectModuleListItemId equals li.ListItemId
															   where pm.ProjectId == projects.ProjectId && pm.IsDeleted == false && (pm.ProjectModuleId == projectModule.ProjectModuleId || pm.ParentProjectModuleId == projectModule.ProjectModuleId)
															   select new ProjectModuleListModel
															   {
																   ModuleName = pm.ModuleName,
																   ParentProjectModuleId = pm.ParentProjectModuleId,
																   ProjectModuleId = pm.ProjectModuleId,
																   ProjectModuleListItemId = pm.ProjectModuleListItemId,
																   Description = pm.Description
															   }).ToListAsync();




								var testCaseTypeList = getExcelData.Select(x => x.Type).ToList();
								var testCaseType = testCaseTypeList
									.Any(x => x != ListItem.Automation.ToString()
									&& x != ListItem.Manual.ToString());

								if (testCaseType == false)
								{
									bool emptyExcelDataValidation = getExcelData.Where(x => x.Description == null || x.Description == string.Empty ||
									x.ExpectedResult == null || x.ExpectedResult == string.Empty).Any();

									var filterTestCaseStepDetail = getExcelData.Where(x => x.Steps > 0).ToList();
									var filterTestCaseStepZero = getExcelData.Where(x => x.Steps == 0).ToList();

									if (emptyExcelDataValidation)
									{
										throw new TestCaseDataNullOrEmptyException();
									}
									else
									{
										var testCaseNameList = getExcelData.Select(x => x.TestCaseName).ToList();

										var updateProjectModule = projectModuleList.Where(x => testCaseNameList.Contains(x.ModuleName) && x.ParentProjectModuleId == projectModule.ProjectModuleId).ToList();



										if (updateProjectModule.Count > 0)
										{
											var updateProjectModuleIdList = updateProjectModule.Select(x => x.ProjectModuleId).ToList();
											var updateProjectModuleIdListName = updateProjectModule.Select(x => x.ModuleName).ToList();


											//Update ProjectModule 
											var projectModList = getExcelData.Where(x => x.Steps == 0 && updateProjectModuleIdListName.Contains(x.TestCaseName)).ToList();
											if (projectModList.Count > 0)
											{
												var testCaseList = await (from tcd in _context.TestCaseDetail
																		  join pm in _context.ProjectModule on tcd.ProjectModuleId equals pm.ProjectModuleId
																		  where pm.ProjectId == projects.ProjectId && pm.IsDeleted == false && (pm.ProjectModuleId == projectModule.ProjectModuleId || pm.ParentProjectModuleId == projectModule.ProjectModuleId)
																		  select new TestCaseListModel
																		  {
																			  TestCaseDetailId = tcd.TestCaseDetailId,
																			  ModuleName = pm.ModuleName,
																			  ParentProjectModuleId = pm.ParentProjectModuleId,
																			  ProjectModuleId = pm.ProjectModuleId,
																			  PreCondition = tcd.PreCondition,
																			  ExpectedResult = tcd.ExpectedResult,
																			  TestCaseListItemId = tcd.TestCaseListItemId,
																		  }).ToListAsync();

												var testCaseStepList = await (from tcsd in _context.TestCaseStepDetail
																			  join pm in _context.ProjectModule on tcsd.ProjectModuleId equals pm.ProjectModuleId
																			  where pm.ProjectId == projects.ProjectId && pm.IsDeleted == false && (pm.ProjectModuleId == projectModule.ProjectModuleId || pm.ParentProjectModuleId == projectModule.ProjectModuleId)
																			  select new TestCaseStepListModel
																			  {
																				  TestCaseStepDetailId = tcsd.TestCaseStepDetailId,
																				  ModuleName = pm.ModuleName,
																				  ParentProjectModuleId = pm.ParentProjectModuleId,
																				  ProjectModuleId = pm.ProjectModuleId,
																				  StepNumber = tcsd.StepNumber,
																				  StepDescription = tcsd.StepDescription,
																				  ExpectedResult = tcsd.ExpectedResult,
																			  }).ToListAsync();


												var updateProjectModules = projectModList.Where(x => updateProjectModuleIdListName.Contains(x.TestCaseName)).Select(x => new Entities.ProjectModule
												{
													ModuleName = x.TestCaseName,
													ProjectId = projects.ProjectId,
													ParentProjectModuleId = updateProjectModule.Where(y => y.ModuleName == x.TestCaseName).Select(x => x.ParentProjectModuleId).FirstOrDefault(),
													ProjectModuleId = updateProjectModule.Where(y => y.ModuleName == x.TestCaseName).Select(x => x.ProjectModuleId).FirstOrDefault(),
													ProjectModuleListItemId = updateProjectModule.Where(y => y.ModuleName == x.TestCaseName).Select(x => x.ProjectModuleListItemId).FirstOrDefault(),
													Description = x.TestScenario,
													OrderDate = DateTimeOffset.UtcNow,
													UpdateDate = DateTimeOffset.UtcNow,

												}).ToList();
												_context.ProjectModule.UpdateRange(updateProjectModules);
												await _context.SaveChangesAsync();

												var projectModuleIds = updateProjectModules.Select(x => x.ProjectModuleId).ToList();
												var testCaseDetailsIds = testCaseList.Where(x => projectModuleIds.Contains(x.ProjectModuleId)).ToList();
												var updateProjectModulesName = updateProjectModules.Select(x => x.ModuleName).ToList();
												var updatingProjectModuleIds = updateProjectModules.Select(x => new { x.ModuleName, x.ProjectModuleId }).ToList();
												var testCasesDetailList = getExcelData.Where(x => x.Steps == 0 && updateProjectModuleIdListName.Contains(x.TestCaseName)).ToList();
												var type = testCasesDetailList.Select(x => x.Type).ToList();

												var updateTestCaseDetails = testCasesDetailList.Select(x => new Entities.TestCaseDetail
												{
													PreCondition = x.Description,
													ProjectModuleId = updatingProjectModuleIds.Where(y => y.ModuleName == x.TestCaseName).Select(x => x.ProjectModuleId).FirstOrDefault(),
													ExpectedResult = x.ExpectedResult,
													TestCaseDetailId = testCaseDetailsIds.Where(y => y.ModuleName == x.TestCaseName).Select(x => x.TestCaseDetailId).FirstOrDefault(),
													TestCaseListItemId = testCaseTypeLists.Where(y => y.ListItemSystemName == x.Type).Select(x => x.ListItemId).FirstOrDefault(),
													UpdateDate = DateTimeOffset.UtcNow,
												}).ToList();
												_context.TestCaseDetail.UpdateRange(updateTestCaseDetails);
												await _context.SaveChangesAsync();

												var testCasesStepDetailList = getExcelData.Where(x => x.Steps > 0 && updateProjectModuleIdListName.Contains(x.TestCaseName)).ToList();

												var projectModuleIdsTestCaseStepDetail = updateProjectModules.Select(x => new { x.ModuleName, x.ProjectModuleId }).ToList();
												var projectModuleIdsStepDetail = updateProjectModules.Select(x => x.ProjectModuleId).ToList();
												var projectModuleNamesStepDetail = updateProjectModules.Select(x => x.ModuleName).ToList();

												var stepNumber = testCaseStepList.Where(x => projectModuleIdsStepDetail.Contains(x.ProjectModuleId)).Select(x => x.StepNumber).ToList();
												var testCaseStepDetailsIds = testCaseStepList.Where(x => projectModuleIdsStepDetail.Contains(x.ProjectModuleId)).ToList();


												var updateTestCaseStepDetails = testCasesStepDetailList.Where(x => updateProjectModulesName.Contains(x.TestCaseName) && stepNumber.Contains(x.Steps)).Select(x => new Entities.TestCaseStepDetail
												{
													ProjectModuleId = projectModuleIdsTestCaseStepDetail.Where(y => y.ModuleName == x.TestCaseName).Select(x => x.ProjectModuleId).FirstOrDefault(),
													StepNumber = x.Steps,
													TestCaseStepDetailId = testCaseStepDetailsIds.Where(y => y.ModuleName == x.TestCaseName && y.StepNumber == x.Steps).Select(x => x.TestCaseStepDetailId).FirstOrDefault(),
													StepDescription = x.Description,
													ExpectedResult = x.ExpectedResult,
													TestCaseListItemId = testCaseTypeLists.Where(y => y.ListItemSystemName == x.Type).Select(x => x.ListItemId).FirstOrDefault(),


												}).ToList();

												_context.TestCaseStepDetail.UpdateRange(updateTestCaseStepDetails);
												await _context.SaveChangesAsync();

												var newStepNumber = testCasesStepDetailList.Where(x => !stepNumber.Contains(x.Steps)).ToList();
												if (newStepNumber.Count > 0)
												{
													var addTestCaseStepDetails = testCasesStepDetailList.Where(x => updateProjectModulesName.Contains(x.TestCaseName) && !stepNumber.Contains(x.Steps)).Select(x => new Entities.TestCaseStepDetail
													{
														ProjectModuleId = projectModuleList.Where(y => y.ModuleName == x.TestCaseName).Select(x => x.ProjectModuleId).FirstOrDefault(),
														StepDescription = x.Description,
														ExpectedResult = x.ExpectedResult,
														TestCaseListItemId = testCaseTypeLists.Where(y => y.ListItemSystemName == x.Type).Select(x => x.ListItemId).FirstOrDefault(),
														StepNumber = x.Steps

													}).ToList();

													await _context.TestCaseStepDetail.AddRangeAsync(addTestCaseStepDetails);
													await _context.SaveChangesAsync();

												}

											}

											//Add new ProjectModule 
											var projectModAddList = getExcelData.Where(x => x.Steps == 0 && !updateProjectModuleIdListName.Contains(x.TestCaseName)).ToList();
											if (projectModAddList.Count > 0)
											{
												var addNewProjectModuleList = projectModAddList.Select(x => new Entities.ProjectModule
												{

													ParentProjectModuleId = projects.ProjectModuleId,
													ModuleName = x.TestCaseName,
													Description = x.TestScenario,
													ProjectId = projects.ProjectId,
													ProjectModuleListItemId = projectModuleListItem.ListItemId,
													OrderDate = DateTimeOffset.UtcNow,
												}).ToList();
												await _context.ProjectModule.AddRangeAsync(addNewProjectModuleList);
												await _context.SaveChangesAsync();



												//Add new TestCaseDetail
												var addTestCaseDetailList = getExcelData.Where(x => x.Steps == 0 && !updateProjectModuleIdListName.Contains(x.TestCaseName)).ToList();
												if (addTestCaseDetailList.Count > 0)
												{
													var projectModuelNameListAdd = addTestCaseDetailList.Select(x => x.TestCaseName).ToList();
													var addnewTestCaseDetailList = projectModAddList.Where(x => projectModuelNameListAdd.Contains(x.TestCaseName)).Select(x => new Entities.TestCaseDetail
													{
														ProjectModuleId = addNewProjectModuleList.Where(y => y.ModuleName == x.TestCaseName).Select(x => x.ProjectModuleId).FirstOrDefault(),
														PreCondition = x.Description,
														ExpectedResult = x.ExpectedResult,
														TestCaseListItemId = testCaseTypeLists.Where(y => y.ListItemSystemName == x.Type).Select(x => x.ListItemId).FirstOrDefault(),

													}).ToList();
													await _context.TestCaseDetail.AddRangeAsync(addnewTestCaseDetailList);
													await _context.SaveChangesAsync();

													var testStepDetailsProjectModuleIds = addnewTestCaseDetailList.Select(x => x.ProjectModuleId).ToList();
													var res = getExcelData.Where(x => x.Steps > 0 && !updateProjectModuleIdListName.Contains(x.TestCaseName)).ToList();
													if (res.Count > 0)
													{

														var addTestCaseStepDetails = res.Select(x => new Entities.TestCaseStepDetail
														{
															ProjectModuleId = addNewProjectModuleList.Where(y => y.ModuleName == x.TestCaseName).Select(x => x.ProjectModuleId).FirstOrDefault(),
															StepDescription = x.Description,
															ExpectedResult = x.ExpectedResult,
															TestCaseListItemId = testCaseTypeLists.Where(y => y.ListItemSystemName == x.Type).Select(x => x.ListItemId).FirstOrDefault(),
															StepNumber = x.Steps

														}).ToList();

														await _context.TestCaseStepDetail.AddRangeAsync(addTestCaseStepDetails);
														await _context.SaveChangesAsync();
													}
												}
											}
										}
										//Adding new projectModule , TestCaseDetail and TestCaseStepDetail
										else
										{
											//Adding new ProjectModule
											var addProjectModuleList = filterTestCaseStepZero.Select(x => new Entities.ProjectModule
											{

												ParentProjectModuleId = projects.ProjectModuleId,
												ModuleName = x.TestCaseName,
												Description = x.TestScenario,
												ProjectId = projects.ProjectId,
												ProjectModuleListItemId = projectModuleListItem.ListItemId,
												OrderDate = DateTimeOffset.UtcNow,
												IsDeleted = false
											}).ToList();
											await _context.ProjectModule.AddRangeAsync(addProjectModuleList);
											await _context.SaveChangesAsync();

											//Adding new TestCaseDetail
											var projectModuelNameList = addProjectModuleList.Select(x => x.ModuleName).ToList();
											var projectModuelId = addProjectModuleList.Select(x => x.ProjectModuleId).ToList();
											var addTestCaseDetailList = filterTestCaseStepZero.Where(x => projectModuelNameList.Contains(x.TestCaseName)).Select(x => new Entities.TestCaseDetail
											{
												ProjectModuleId = addProjectModuleList.Where(y => y.ModuleName == x.TestCaseName).Select(x => x.ProjectModuleId).FirstOrDefault(),
												PreCondition = x.Description,
												ExpectedResult = x.ExpectedResult,
												TestCaseListItemId = testCaseTypeLists.Where(y => y.ListItemSystemName == x.Type).Select(x => x.ListItemId).FirstOrDefault(),

											}).ToList();
											await _context.TestCaseDetail.AddRangeAsync(addTestCaseDetailList);
											await _context.SaveChangesAsync();


											//Adding new TestCaseStepDetail
											var moduleName = addProjectModuleList.Where(x => testCaseNameList.Contains(x.ModuleName) && x.ParentProjectModuleId == projectModule.ProjectModuleId).Select(x => x.ModuleName).ToList();
											var addTestCaseStepDetailList = filterTestCaseStepDetail.Where(x => moduleName.Contains(x.TestCaseName)).Select(x => new Entities.TestCaseStepDetail
											{
												ProjectModuleId = addProjectModuleList.Where(y => y.ModuleName == x.TestCaseName).Select(x => x.ProjectModuleId).FirstOrDefault(),
												StepDescription = x.Description,
												ExpectedResult = x.ExpectedResult,
												StepNumber = x.Steps,
												TestCaseListItemId = testCaseTypeLists.Where(y => y.ListItemSystemName == x.Type).Select(x => x.ListItemId).FirstOrDefault(),
											}).ToList();
											await _context.TestCaseStepDetail.AddRangeAsync(addTestCaseStepDetailList);
											await _context.SaveChangesAsync();

										}
									}
								}
								else
								{
									throw new TestCaseTypeException();

								}


								var count = getExcelData.Where(x => x.Steps == 0).ToList().Count();
								transaction.Commit();
								return Result<string>.Success(ReturnMessage.TestCaseImportedSuccessfully + " " + "Total TestCases Imported: " + " " + count);
							}
							else
							{
								throw new ProjectModuleTypeNotValidException();
							}
						}
						else
						{
							throw new UnAuthorizedUserException();
						}
					}
					else
					{
						throw new ProjectModuleIdNotFoundException();

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

				if (ex.Message == "File Formate not-supported, Please select excel file only")
				{
					_iLogger.LogError("Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.FileFormateNotSupportedPleaseSelectExcelFileOnly);
				}
				else if (ex is InvalidDataException)
				{
					_iLogger.LogError("Exception message is: ", ex.Message);
					return Result<string>.Error(ex.Message);
				}
				else if (ex is UnAuthorizedUserException)
				{
					_iLogger.LogError("Unauthorized User. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.UnauthorizedUser);
				}
				else if (ex is TestCaseTypeException)
				{
					_iLogger.LogError("Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.PleaseCheckTestCaseType);
				}
				else if (ex is ProjectModuleIdNotFoundException)
				{
					_iLogger.LogError("Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ProjectMemberIdNotFound);
				}
				else if (ex.Message == "Failed to upload excel, Step value is alphanumeric")
				{
					_iLogger.LogError("Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.FailedToUploadExcelStepValueIsAlphanumeric);


				}
				else if (ex is ProjectSlugNotFoundException)
				{
					_iLogger.LogError("Could not find the ProjectSlug. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ProjectSlugDoesNotExists);
				}
				else if (ex is TestCaseDataNullOrEmptyException)
				{
					_iLogger.LogError("Either Pre-Condition Or Expected Result is empty. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestCaseDataCannotBeEmpty);
				}
				else if (ex is ProjectModuleTypeNotValidException)
				{
					_iLogger.LogError("ProjectModule Type Is Not Valid. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ProjectModuleTypeIsNotValid);
				}
				else if (ex is ProjectModuleTypeIsNotFoundException)
				{
					_iLogger.LogError("ProjectModuleType is not found . Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ProjectModuleTypeIsNotFound);
				}
				else if (ex.Message == "Failed to upload excel, number of header column exceeded.")
				{
					_iLogger.LogError("Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ImproperFormat);
				}

				_iLogger.LogError("Exception message is: ", ex.Message);
				return Result<string>.Error(ReturnMessage.FailedToImportTestCase);
			}
		}

		//Download  test cases of given functionId
		public async Task<byte[]> DownloadTestByFunctionIdAsync(int functionId)
		{
			try
			{

				var testCase = await (from pm in _context.ProjectModule
									  join p in _context.Project on pm.ProjectId equals p.ProjectId
									  where pm.ProjectModuleId == functionId
									  select new DownloadTestCaseModelByFunctionId
									  {
										  ProjectId = p.ProjectId,
										  ProjectSlug = p.ProjectSlug,
										  FunctionId = pm.ProjectModuleId,
										  ModuleName = pm.ModuleName

									  }).FirstOrDefaultAsync();




				if (testCase != null)
				{
					//Project Role Permission Check
					bool hasProjectPermission = await _iCommonService.CheckProjectPermission(testCase.ProjectSlug, ProjectRoleSlug.ViewProjectModule);
					if (hasProjectPermission)
					{

						var testDataDetail = await (from t in _context.TestCaseDetail
													join pmo in _context.ProjectModule on t.ProjectModuleId equals pmo.ProjectModuleId
													join li in _context.ListItem on t.TestCaseListItemId equals li.ListItemId
													where pmo.ParentProjectModuleId == functionId && pmo.IsDeleted == false
													select new TestCaseViewModelForExcel
													{
														TestCaseName = pmo.ModuleName,
														TestScenario = pmo.Description,
														Type = li.ListItemSystemName,
														Steps = 0,
														Description = t.PreCondition,
														ExpectedResult = t.ExpectedResult,
														OrderDate = pmo.OrderDate,
														FunctionName = testCase.ModuleName
													}).ToListAsync();


						var testDataStepDetail = await (from t in _context.TestCaseStepDetail
														join pmo in _context.ProjectModule on t.ProjectModuleId equals pmo.ProjectModuleId
														join li in _context.ListItem on t.TestCaseListItemId equals li.ListItemId
														where pmo.ParentProjectModuleId == functionId && pmo.IsDeleted == false
														select new TestCaseViewModelForExcel
														{
															TestCaseName = pmo.ModuleName,
															TestScenario = pmo.Description,
															Type = li.ListItemSystemName,
															Steps = t.StepNumber,
															Description = t.StepDescription,
															ExpectedResult = t.ExpectedResult,
															OrderDate = pmo.OrderDate,
															FunctionName = testCase.ModuleName

														}).ToListAsync();


						var fullOuterJoin = testDataDetail.Union(testDataStepDetail).OrderBy(x => x.OrderDate).ToList();
						var getExcelData = ExcelExportHelper.TestCaseDetailsToExcel(fullOuterJoin);
						var projectUpdateDate = _context.Project.Where(x => x.ProjectId == testCase.ProjectId).FirstOrDefault();
						projectUpdateDate.UpdateDate = DateTimeOffset.UtcNow;
						return getExcelData;
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
					_iLogger.LogError("Unauthorized User. Exception message is: ", ex.Message);
					throw new UnAuthorizedUserException();
				}
				else if (ex is ProjectSlugNotFoundException)
				{
					_iLogger.LogError("Could not find the ProjectSlug. Exception message is: ", ex.Message);
					throw new ProjectSlugNotFoundException();
				}
				throw new DownloadFailedException();
			}

		}

		// Drag and Drop Project Module, Function, TestCase
		public async Task<Result<string>> DragDropProjectModuleFunctionTestCaseAsync(DragDropTestCaseDetail model)
		{
			using var transaction = _context.Database.BeginTransaction();
			try
			{
				var projectId = _context.Project.Where(x => x.ProjectSlug == model.ProjectSlug).FirstOrDefault().ProjectId;
				var project = _context.Project.Where(x => x.ProjectId == projectId).FirstOrDefault();
				if (projectId > 0)
				{
					bool hasProjectPermission = await _iCommonService.CheckProjectPermission(model.ProjectSlug, ProjectRoleSlug.UpdateProjectModule);
					if (hasProjectPermission)
					{
						var projectModuleListItemid = await _context.ListItem.FirstOrDefaultAsync(x => x.ListItemSystemName == model.ProjectModuleType);
						if (projectModuleListItemid != null)
						{
							List<DragDropProjectModuleModel> dragdrop = model.DragDropProjectModuleModel;
							var dragDropProjectModuleId = dragdrop.Select(x => x.ProjectModuleId).ToList();
							var updateProjectModule = await _context.ProjectModule.FirstOrDefaultAsync(x => x.ProjectModuleId == model.ProjectModuleId);
							var dragDropListItemSystemName = await _context.ProjectModule.Include(x => x.ProjectModuleListItem)
								.Where(x =>
								x.ProjectModuleId == model.DragDropParentProjectModuleId).FirstOrDefaultAsync();



							if (dragDropListItemSystemName != null)
							{
								bool dragDropModule = dragDropListItemSystemName.ProjectModuleListItem.ListItemSystemName == nameof(ListItem.Module);
								bool dragDropFunction = dragDropListItemSystemName.ProjectModuleListItem.ListItemSystemName == nameof(ListItem.Function);

								bool updateModule = updateProjectModule.ProjectModuleListItem.ListItemSystemName == nameof(ListItem.Module);
								bool updateFunction = updateProjectModule.ProjectModuleListItem.ListItemSystemName == nameof(ListItem.Function);
								bool updateTestCase = updateProjectModule.ProjectModuleListItem.ListItemSystemName == nameof(ListItem.TestCase);
								var checkSameTestCase = await _context.ProjectModule.Where(x => x.ProjectModuleId == model.ProjectModuleId).FirstOrDefaultAsync();

                                var checkDuplcate = await _context.ProjectModule.Include(x => x.ProjectModuleListItem).Where(x => x.ParentProjectModuleId == model.DragDropParentProjectModuleId && x.IsDeleted==false).ToListAsync();
                                if (checkDuplcate.Count > 0)
                                {
                                    bool moduleNameWithFunctionName = true;
                                    var moduleName = checkDuplcate.Select(x => x.ModuleName).ToList();
                                    var moduleNameWithSameFunctionName = checkDuplcate.Where(x => checkSameTestCase.ModuleName.Contains(x.ModuleName) && checkSameTestCase.ProjectModuleListItem.ListItemName.Contains(x.ProjectModuleListItem.ListItemSystemName)).Any();
                                    var duplicateName = await _context.ProjectModule.Where(x => (x.ProjectModuleId == model.ProjectModuleId && moduleName.Contains(x.ModuleName)) || (x.ParentProjectModuleId == model.ProjectModuleId && moduleName.Contains(x.ModuleName))).AnyAsync();
                                    if (checkSameTestCase.ParentProjectModuleId == model.DragDropParentProjectModuleId)
                                    {
                                        duplicateName = false;
                                    }
                                    if (checkSameTestCase.ModuleName == dragDropListItemSystemName.ModuleName && checkSameTestCase.ParentProjectModuleId==dragDropListItemSystemName.ParentProjectModuleId)
                                    {
                                        duplicateName = true;
                                        moduleNameWithFunctionName = false;
                                    }
                                    if (moduleNameWithFunctionName && moduleNameWithSameFunctionName==false)
                                    {
                                        duplicateName = false;
                                    }

									if (duplicateName)
									{
										if (model.ProjectModuleType == ListItem.Module.ToString())
										{
											throw new DuplicateModuleNameFoundException();
										}
										else if (model.ProjectModuleType == ListItem.Function.ToString())
										{
											throw new DuplicateFunctionNameFoundException();
										}
										else
										{
											if (model.ProjectModuleType == ListItem.TestCase.ToString())
											{
												throw new DuplicateTestCaseException();
											}
										}
									}
								}

								if (updateModule && dragDropModule
									|| updateFunction && dragDropModule
									|| updateTestCase && dragDropFunction)
								{
									updateProjectModule.ParentProjectModuleId = model.DragDropParentProjectModuleId;
									updateProjectModule.ProjectId = projectId;
									updateProjectModule.ModuleName = model.ModuleName;
									updateProjectModule.ProjectModuleListItemId = projectModuleListItemid.ListItemId;
									updateProjectModule.Description = model.Description;
									project.UpdateDate = DateTimeOffset.UtcNow;
									_context.ProjectModule.Update(updateProjectModule);
									await _context.SaveChangesAsync();
								}
								else
								{
									throw new ModuleFunctionTestCaseConditionException();
								}

							}
							else
							{
								if (updateProjectModule.ProjectModuleListItem.ListItemSystemName == nameof(ListItem.Module)
								&& model.DragDropParentProjectModuleId == null)
								{
									updateProjectModule.ParentProjectModuleId = model.DragDropParentProjectModuleId;
									updateProjectModule.ProjectId = projectId;
									updateProjectModule.ModuleName = model.ModuleName;
									updateProjectModule.ProjectModuleListItemId = projectModuleListItemid.ListItemId;
									updateProjectModule.Description = model.Description;
									project.UpdateDate = DateTimeOffset.UtcNow;
									_context.ProjectModule.Update(updateProjectModule);
									await _context.SaveChangesAsync();
								}
								else
								{
									throw new ModuleCanDragToModuleOnlyException();
								}

							}


							await OrderProjectModule(dragdrop, model.ProjectSlug);

							project.UpdateDate = DateTimeOffset.UtcNow;
							transaction.Commit();
							return Result<string>.Success(ReturnMessage.UpdatedSuccessfully);
						}
						else
						{
							throw new ProjectModuleTypeIsNotFoundException();
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
				if (ex is ProjectSlugNotFoundException)
				{
					_iLogger.LogError("Could not find the ProjectSlug. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ProjectSlugDoesNotExists);
				}
				else if (ex is UnAuthorizedUserException)
				{
					_iLogger.LogError("Unauthorized User. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.UnauthorizedUser);
				}
				else if (ex is ProjectModuleTypeIsNotFoundException)
				{
					_iLogger.LogError("Could not find the ProjectModuleListItemId. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ProjectModuleTypeIsNotFound);
				}
				else if (ex is ModuleCanDragToModuleOnlyException)
				{
					_iLogger.LogError("Could not update the ProjectModule. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ModuleCanOnlyBeDragDropToModule);
				}

				else if (ex is DuplicateModuleNameFoundException)
				{
					_iLogger.LogError("Duplicate module name found. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.DuplicateModuleFound);
				}
				else if (ex is DuplicateFunctionNameFoundException)
				{
					_iLogger.LogError("Duplicate function name found. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.DuplicateFunctionFound);
				}
				else if (ex is DuplicateTestCaseException)
				{
					_iLogger.LogError("Duplicate test case found. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.DuplicateTestCaseFound);
				}

				_iLogger.LogError("Could not update the ProjectModule. Exception message is: ", ex.Message);
				return Result<string>.Error(ReturnMessage.FailedToUpdateProjectModule);
			}

		}
	
		//Download All test cases having each function a separate sheet 
		public async Task<byte[]> DownloadTestCaseAsync(string projectSlug)
		{
			try
			{
				var slug = await (from p in _context.Project
								  join pm in _context.ProjectModule on p.ProjectId equals pm.ProjectId
								  where p.ProjectSlug == projectSlug
								  select new DownloadTestCaseModel
								  {
									  ProjectId = p.ProjectId,
									  ProjectSlug = p.ProjectSlug,

								  }).FirstOrDefaultAsync();

				if (slug != null)
				{
					//Project Role Permission Check
					bool hasProjectPermission = await _iCommonService.CheckProjectPermission(slug.ProjectSlug, ProjectRoleSlug.ViewProjectModule);
					if (hasProjectPermission)
					{
						List<TestCaseViewModelForExcel> modelForExcel = new();

						var queryTestCaseDetail = await (from project in _context.Project

														 join funcProjectModule in _context.ProjectModule
														 on project.ProjectId equals funcProjectModule.ProjectId

														 join funcListItem in _context.ListItem
														 on funcProjectModule.ProjectModuleListItemId equals funcListItem.ListItemId

														 join projectModule in _context.ProjectModule
														 on funcProjectModule.ProjectModuleId equals projectModule.ParentProjectModuleId


														 join testCase in _context.TestCaseDetail
														 on projectModule.ProjectModuleId equals testCase.ProjectModuleId into testCaseDetail
														 from testCase in testCaseDetail.DefaultIfEmpty()

														 join testCaseListItem in _context.ListItem
														 on testCase.TestCaseListItemId equals testCaseListItem.ListItemId into testCaseLstItm
														 from testCaseListItem in testCaseLstItm.DefaultIfEmpty()

														 where project.ProjectSlug == projectSlug && funcListItem.ListItemSystemName == ListItem.Function.ToString() && projectModule.IsDeleted == false && funcProjectModule.IsDeleted == false
														 select new TestCaseViewModelForExcel
														 {
															 FunctionId = funcProjectModule.ProjectModuleId,
															 FunctionName = funcProjectModule.ModuleName,
															 TestCaseName = projectModule.ModuleName,
															 TestScenario = projectModule.Description,
															 Type = testCaseListItem.ListItemSystemName,
															 Steps = 0,
															 Description = testCase.PreCondition,
															 ExpectedResult = testCase.ExpectedResult,
															 ProjectModuleId = projectModule.ProjectModuleId,
															 OrderDate = projectModule.OrderDate
														 }).ToListAsync();
						modelForExcel.AddRange(queryTestCaseDetail);

						var queryTestStepCaseDetail = await (from project in _context.Project

															 join funcProjectModule in _context.ProjectModule
															 on project.ProjectId equals funcProjectModule.ProjectId

															 join funcListItem in _context.ListItem
															 on funcProjectModule.ProjectModuleListItemId equals funcListItem.ListItemId

															 join projectModule in _context.ProjectModule
															 on funcProjectModule.ProjectModuleId equals projectModule.ParentProjectModuleId

															 join testCaseStep in _context.TestCaseStepDetail
															 on projectModule.ProjectModuleId equals testCaseStep.ProjectModuleId

															 join testCaseListItem in _context.ListItem
															 on testCaseStep.TestCaseListItemId equals testCaseListItem.ListItemId into testCaseLstItm
															 from testCaseListItem in testCaseLstItm.DefaultIfEmpty()

															 where project.ProjectSlug == projectSlug && funcListItem.ListItemSystemName == ListItem.Function.ToString() && projectModule.IsDeleted == false && funcProjectModule.IsDeleted == false
															 select new TestCaseViewModelForExcel
															 {
																 FunctionId = funcProjectModule.ProjectModuleId,
																 FunctionName = funcProjectModule.ModuleName,
																 TestCaseName = projectModule.ModuleName,
																 TestScenario = projectModule.Description,
																 Type = testCaseListItem.ListItemSystemName,
																 Steps = testCaseStep.StepNumber,
																 Description = testCaseStep.StepDescription,
																 ExpectedResult = testCaseStep.ExpectedResult,
																 ProjectModuleId = projectModule.ProjectModuleId,
																 OrderDate = projectModule.OrderDate
															 }).ToListAsync();

						modelForExcel.AddRange(queryTestStepCaseDetail);

						var getExcelData = ExportAllTestCaseHelper.TestCaseDetailsToExcel(modelForExcel);
						var projectUpdateDate = _context.Project.Where(x => x.ProjectId == slug.ProjectId).FirstOrDefault();
						projectUpdateDate.UpdateDate = DateTimeOffset.UtcNow;

						return getExcelData;
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
					_iLogger.LogError("Unauthorized User. Exception message is: ", ex.Message);

					throw new UnAuthorizedUserException();
				}
				else if (ex is ProjectSlugNotFoundException)
				{
					_iLogger.LogError("Could not find the ProjectSlug. Exception message is: ", ex.Message);
					throw new ProjectSlugNotFoundException();
				}
				throw new DownloadFailedException();

			}
		}

		//Get Project Module List FilterBy Name 
		public async Task<Result<PagedResponsePersonModel<List<ProjectModuleModel>>>> GetProjectModuleListFilterByModuleAsync(FilterModel filter, string projectSlug)
		{
			try
			{
				if (projectSlug != null)
				{
					//Project Role Permission Check
					bool hasProjectPermission = await _iCommonService.CheckProjectPermission(projectSlug, ProjectRoleSlug.ViewProjectModule);
					if (hasProjectPermission)
					{
						var getProjectIdByProjectSlug = await _context.Project.FirstOrDefaultAsync(x => x.ProjectSlug == projectSlug);
						if (getProjectIdByProjectSlug != null)
						{
							IQueryable<ProjectModuleModel> getProjectModuleQueryableAsync;
							IQueryable<ProjectModuleDeveloperListModel> getProjectModuleDeveloperQueryableAsync;

							getProjectModuleDeveloperQueryableAsync = (from pmd in _context.ProjectModuleDeveloper
																	   join pm in _context.ProjectMember on pmd.ProjectMemberId equals pm.ProjectMemberId
																	   join p in _context.Project on pm.ProjectId equals p.ProjectId
																	   where pm.ProjectId == p.ProjectId && !pmd.IsDisabled

																	   select new ProjectModuleDeveloperListModel
																	   {
																		   ProjectModuleDeveloperId = pmd.ProjectModuleDeveloperId,
																		   ProjectModuleId = pmd.ProjectModuleId,
																		   PersonId = pm.PersonId

																	   });

							getProjectModuleQueryableAsync = (from pm in _context.ProjectModule
															  join li in _context.ListItem on pm.ProjectModuleListItemId equals li.ListItemId
															  where pm.ProjectId == getProjectIdByProjectSlug.ProjectId && pm.IsDeleted == false
															  orderby pm.OrderDate
															  select new ProjectModuleModel
															  {
																  ProjectModuleId = pm.ProjectModuleId,
																  ModuleName = pm.ModuleName,
																  OrderDate = pm.OrderDate,
																  ParentProjectModuleId = pm.ParentProjectModuleId,
																  ProjectModuleType = li.ListItemSystemName,
																  ProjectId = pm.ProjectId,
																  Description = pm.Description,
																  ProjectModuleListItemId = pm.ProjectModuleListItemId,
																  IsDeleted = pm.IsDeleted

															  });


							IEnumerable<ProjectModuleModel> getProjectModuleListAsync;
							IEnumerable<ProjectModuleDeveloperListModel> getProjectModuleDeveloperListAsync;
							getProjectModuleListAsync = await getProjectModuleQueryableAsync.ToListAsync();
							getProjectModuleDeveloperListAsync = await getProjectModuleDeveloperQueryableAsync.ToListAsync();


							getProjectModuleListAsync = getProjectModuleListAsync.OrderBy(x => x.OrderDate)
											.Where(c => c.ParentProjectModuleId == null && c.IsDeleted == false)
											.Select(c => new ProjectModuleModel()
											{
												ProjectModuleId = c.ProjectModuleId,
												ModuleName = c.ModuleName,
												ParentProjectModuleId = c.ParentProjectModuleId,
												ProjectModuleType = c.ProjectModuleType,
												ProjectId = c.ProjectId,
												OrderDate = c.OrderDate,
												ProjectModuleListItemId = c.ProjectModuleListItemId,
												Description = c.Description,
												IsDeleted = c.IsDeleted,
												ChildModule = ChildModule(getProjectModuleListAsync.Where(x => x.ParentProjectModuleId != null).ToList(),
												getProjectModuleDeveloperListAsync.ToList(), c.ProjectModuleId)
											}).ToList();


							var results = getProjectModuleListAsync;
							var finalResult = new List<ProjectModuleModel>();

							if (!string.IsNullOrEmpty(filter.SearchValue))
							{
								results = getProjectModuleListAsync.Flatten().Where(x => x.ModuleName.ToLower().Contains(filter.SearchValue.ToLower()));
								var filteredData = results.ToList();

								foreach (var item in filteredData)
								{

									//Searching Module Case for parent module ie null 
									if (item.ParentProjectModuleId == null)
									{

										var testcase = getProjectModuleListAsync.Flatten().Where(x => x.ParentProjectModuleId == item.ProjectModuleId).ToList();
										var functions = getProjectModuleListAsync.Flatten().Where(x => x.ProjectModuleId == testcase.Select(x => x.ParentProjectModuleId).FirstOrDefault()).ToList();
										if (testcase.Count > 0 || functions.Count > 0)
										{
											finalResult.AddRange(functions);
										}
										else
										{

											var module = getProjectModuleListAsync.Flatten().Where(x => x.ProjectModuleId == item.ProjectModuleId).ToList();
											finalResult.AddRange(module);

										}


									}
									//Searching functions or testcase 
									else
									{
										//Searching Testcase 
										if (item.ProjectModuleListItemId == 32)
										{
											int? testCaseId = item.ParentProjectModuleId;
											List<ProjectModuleModel> projectModuleModelListIdsTestCase = new List<ProjectModuleModel>();
											do
											{
												var parentProjectModuleIdList = getProjectModuleListAsync.Flatten().Select(x => x.ParentProjectModuleId).ToList();

												var testCaseIdList = getProjectModuleListAsync.Flatten().Where(x => x.ParentProjectModuleId == testCaseId).ToList();
												var functionIdList = getProjectModuleListAsync.Flatten().Where(x => x.ProjectModuleId == testCaseIdList.Select(x => x.ParentProjectModuleId).FirstOrDefault()).ToList();
												var moduleIdList = getProjectModuleListAsync.Flatten().Where(x => x.ProjectModuleId == functionIdList.Select(x => x.ParentProjectModuleId).FirstOrDefault()).ToList();

												if (moduleIdList.Count > 0)
												{
													testCaseId = moduleIdList.Where(x => parentProjectModuleIdList.Contains(x.ProjectModuleId)).Select(x => x.ParentProjectModuleId).FirstOrDefault();
													projectModuleModelListIdsTestCase.AddRange(moduleIdList.Where(x => x.ParentProjectModuleId == null).DistinctBy(x => x.ProjectModuleId));

												}
												else
												{
													testCaseId = functionIdList.Where(x => parentProjectModuleIdList.Contains(x.ProjectModuleId)).Select(x => x.ParentProjectModuleId).FirstOrDefault();
													projectModuleModelListIdsTestCase.AddRange(functionIdList.Where(x => x.ParentProjectModuleId == null).DistinctBy(x => x.ProjectModuleId));
												}
											} while (testCaseId != null);

											finalResult.AddRange(projectModuleModelListIdsTestCase.Distinct());

										}
										//Searching Functions and Nested Module
										else
										{
											//Searching Nested Module
											if (item.ProjectModuleListItemId == 30 && item.ParentProjectModuleId != null)
											{
												int? parentProjectModuleId = item.ParentProjectModuleId;
												List<ProjectModuleModel> projectModuleModelListIdsFunction = new List<ProjectModuleModel>();
												do
												{
													var parentProjectModuleIdLists = getProjectModuleListAsync.Flatten().Select(x => x.ParentProjectModuleId).ToList();

													var parentProjectModuleIdList2 = getProjectModuleListAsync.Flatten().Where(x => x.ProjectModuleId == parentProjectModuleId).ToList();
													var parentProjectModuleIdList3 = getProjectModuleListAsync.Flatten().Where(x => x.ProjectModuleId == parentProjectModuleIdList2.Select(x => x.ParentProjectModuleId).FirstOrDefault()).ToList();
													if (parentProjectModuleIdList2.Count > 0)
													{
														parentProjectModuleId = parentProjectModuleIdList2.Where(x => parentProjectModuleIdLists.Contains(x.ProjectModuleId)).Select(x => x.ParentProjectModuleId).FirstOrDefault();
														projectModuleModelListIdsFunction.AddRange(parentProjectModuleIdList2.Where(x => x.ParentProjectModuleId == null).DistinctBy(x => x.ProjectModuleId));
													}

													else
													{
														var parentList = parentProjectModuleIdList3.Flatten().Where(x => x.ProjectModuleId == item.ParentProjectModuleId).ToList();

														if (parentList.Count > 0)
														{
															parentProjectModuleId = parentList.Where(x => parentProjectModuleIdLists.Contains(x.ProjectModuleId)).Select(x => x.ParentProjectModuleId).FirstOrDefault();
															projectModuleModelListIdsFunction.AddRange(parentList.Where(x => x.ParentProjectModuleId == null).DistinctBy(x => x.ProjectModuleId));
														}
														else
														{
															parentProjectModuleId = parentProjectModuleIdList3.Where(x => parentProjectModuleIdLists.Contains(x.ProjectModuleId)).Select(x => x.ParentProjectModuleId).FirstOrDefault();
															projectModuleModelListIdsFunction.AddRange(parentProjectModuleIdList3.Where(x => x.ParentProjectModuleId == null).DistinctBy(x => x.ProjectModuleId));

														}
													}
												} while (parentProjectModuleId != null);

												finalResult.AddRange(projectModuleModelListIdsFunction.Distinct());
											}

											//Searching only functions
											else
											{

												int? parentProjectModuleIdfunction = item.ParentProjectModuleId;
												List<ProjectModuleModel> projectModuleModelLists = new List<ProjectModuleModel>();
												do
												{
													var parentProjectModuleIdListfunctions = getProjectModuleListAsync.Flatten().Select(x => x.ParentProjectModuleId).ToList();
													var parentProjectModuleIdListfunctions2 = getProjectModuleListAsync.Flatten().Where(x => x.ProjectModuleId == parentProjectModuleIdfunction).ToList();
													var parentProjectModuleIdListfunctions3 = getProjectModuleListAsync.Flatten().Where(x => x.ProjectModuleId == parentProjectModuleIdListfunctions2.Select(x => x.ParentProjectModuleId).FirstOrDefault()).ToList();
													if (parentProjectModuleIdListfunctions3.Count > 0)
													{
														parentProjectModuleIdfunction = parentProjectModuleIdListfunctions3.Where(x => parentProjectModuleIdListfunctions.Contains(x.ProjectModuleId)).Select(x => x.ParentProjectModuleId).FirstOrDefault();
														projectModuleModelLists.AddRange(parentProjectModuleIdListfunctions3.Where(x => x.ParentProjectModuleId == null).DistinctBy(x => x.ProjectModuleId));
													}
													else
													{
														var parentProjectModuleIdListfunctions4 = parentProjectModuleIdListfunctions2.Flatten().Where(x => x.ProjectModuleId == item.ParentProjectModuleId).ToList();
														if (parentProjectModuleIdListfunctions4.Count > 0)
														{
															parentProjectModuleIdfunction = parentProjectModuleIdListfunctions4.Where(x => parentProjectModuleIdListfunctions.Contains(x.ProjectModuleId)).Select(x => x.ParentProjectModuleId).FirstOrDefault();
															projectModuleModelLists.AddRange(parentProjectModuleIdListfunctions4.Where(x => x.ParentProjectModuleId == null).DistinctBy(x => x.ProjectModuleId));
														}
														else
														{
															parentProjectModuleIdfunction = parentProjectModuleIdListfunctions3.Where(x => parentProjectModuleIdListfunctions.Contains(x.ProjectModuleId)).Select(x => x.ParentProjectModuleId).FirstOrDefault();

															projectModuleModelLists.AddRange(parentProjectModuleIdListfunctions3.Where(x => x.ParentProjectModuleId == null).DistinctBy(x => x.ProjectModuleId));

														}
													}
												} while (parentProjectModuleIdfunction != null);

												finalResult.AddRange(projectModuleModelLists.Distinct());

											}
										}
									}
								}


							}
							else
							{
								finalResult = results.ToList();

							}



							var data = new PagedResponsePersonModel<List<ProjectModuleModel>>(finalResult.DistinctBy(x => x.ProjectModuleId).ToList());

							if (finalResult.Count > 0)
							{
								return Result<PagedResponsePersonModel<List<ProjectModuleModel>>>.Success(data);
							}
							else
							{
								return Result<PagedResponsePersonModel<List<ProjectModuleModel>>>.Success(null);
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
				else
				{
					throw new ProjectSlugNotFoundException();

				}
			}
			catch (Exception ex)
			{
				if (ex is UnAuthorizedUserException)
				{
					_iLogger.LogError("Unauthorized User. Exception message is: ", ex.Message);
					return Result<PagedResponsePersonModel<List<ProjectModuleModel>>>.Error(ReturnMessage.UnauthorizedUser);
				}
				else if (ex is ProjectSlugNotFoundException)
				{
					_iLogger.LogError("Could not find the ProjectSlug. Exception message is: ", ex.Message);
					return Result<PagedResponsePersonModel<List<ProjectModuleModel>>>.Error(ReturnMessage.ProjectSlugDoesNotExists);

				}
				else
				{

					return Result<PagedResponsePersonModel<List<ProjectModuleModel>>>.Error(null);
				}
			}
		}

		//Delete Import TestCase
		public async Task<Result<string>> DeleteImportTestCaseAsync(int projectModuleId)
		{

			using var transaction = _context.Database.BeginTransaction();
			try
			{
				var testCases = await _context.ProjectModule.Where(x => x.ProjectModuleId == projectModuleId).ToListAsync();
				if (testCases.Count != 0)
				{
					await TestCaseDelete(testCases.Select(y => y.ProjectModuleId).ToList());
					_context.ProjectModule.RemoveRange(testCases);
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
				_iLogger.LogError("Could not delete the TestCase. Exception message is: ", ex.Message);
				transaction.Rollback();
				return Result<string>.Error(ReturnMessage.FailedToDeleteTestCase);
			}
		}

		#endregion
	}
}
