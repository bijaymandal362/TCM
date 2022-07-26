using BusinessLayer.Common;

using Data;
using Data.Exceptions;
using Data.Migrations.core;

using Infrastructure;
using Infrastructure.Helper.ExcelExport;
using Infrastructure.Helper.Exceptions;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

using Models.Constant.ReturnMessage;
using Models.Core;
using Models.Enum;
using Models.GridTableFilterModel;
using Models.GridTableProperty;
using Models.TestRun;

using NPOI.HSSF.Record.Aggregates;
using NPOI.HSSF.Record.Chart;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusinessLayer.TestRun
{
	public class TestRunService : ITestRunService
	{
		private readonly DataContext _context;
		private readonly ILogger<TestRunService> _iLogger;
		private readonly ICommonService _iCommonService;
		private readonly IPersonAccessor _iPersonAccessor;


		public TestRunService(DataContext context, ILogger<TestRunService> iLogger, ICommonService iCommonService, IPersonAccessor iPersonAccessor)
		{
			_context = context;
			_iLogger = iLogger;
			_iCommonService = iCommonService;
			_iPersonAccessor = iPersonAccessor;
		}

		#region Function 

		private async Task<PdfExcelTestRunTestCaseReportModel> PdfExcelTestRunTestCaseReport(int testRunId)
		{
			try
			{

				var testRunDetails = await (from tr in _context.TestRun
											join p in _context.Project on tr.ProjectId equals p.ProjectId
											join en in _context.Environment on tr.EnvironmentId equals en.EnvironmentId into env
											from en in env.DefaultIfEmpty()
											where tr.TestRunId == testRunId
											select new TestRunExcelModel
											{
												ProjectName = p.ProjectName,
												Environment = en.EnvironmentName == null ? null : en.EnvironmentName,
												TestCaseManagementSystem = tr.Title,
												Description = tr.Description
											}).FirstOrDefaultAsync();



				#region funtionTestCaseCount

				var testRunPlanDetailFunctionTestCase = await (from t in _context.TestRunPlanDetail
															   join trp in _context.TestRunPlan on t.TestRunPlanId equals trp.TestRunPlanId
															   where trp.TestRunId == testRunId
															   select new TestRunPlanDetailFunctionTestCaseModel()
															   {
																   TestCaseDetail = t.TestCaseDetailJson,
															   }).ToListAsync();

				foreach (var itemInTestRunPlanDetailFunctionTestCase in testRunPlanDetailFunctionTestCase)
				{
					itemInTestRunPlanDetailFunctionTestCase.TestCaseDetailJson = JsonSerializer.Deserialize<List<TestPlanTestCaseJson>>(itemInTestRunPlanDetailFunctionTestCase.TestCaseDetail);
				}

				var funtionTestCaseCount = await (from trtc in _context.TestRunTestCaseHistory
												  join trh in _context.TestRunHistory on trtc.TestRunHistoryId equals trh.TestRunHistoryId
												  join tr in _context.TestRun on trh.TestRunId equals tr.TestRunId
												  join pm in _context.ProjectModule on trtc.ProjectModuleId equals pm.ProjectModuleId into testCase
												  from pm in testCase.DefaultIfEmpty()
												  where trh.TestRunId == testRunId
												  group new { pm.ParentProjectModuleId } by new { pm.ModuleName, pm.ParentProjectModuleId } into g
												  select new FunctionTestCaseCountModel()
												  {
													  ParentProjectModuleId = g.Key.ParentProjectModuleId,
													  Count = g.Count(x => x.ParentProjectModuleId != null),
													  TestCaseName = g.Key.ModuleName

												  }).ToListAsync();
				List<FunctionModuleModel> functionTestCaseTotalCount = new List<FunctionModuleModel>();
				var funtionNameTestCaseCount = funtionTestCaseCount.GroupBy(x => x.ParentProjectModuleId).Select(x => new FunctionModuleModel
				{
					FunctionId = x.Select(x => x.ParentProjectModuleId).FirstOrDefault(),
					TotalCountTestCaseByTestRunId = x.Count(x => x.ParentProjectModuleId != null)
				}).ToList();

				functionTestCaseTotalCount.AddRange(funtionNameTestCaseCount);

				foreach (var itemInfunctionTestCaseTotalCount in functionTestCaseTotalCount)
				{
					foreach (var itemIntestRunPlanDetailFunctionTestCase in testRunPlanDetailFunctionTestCase)
					{
						itemInfunctionTestCaseTotalCount.FunctionName = itemIntestRunPlanDetailFunctionTestCase.TestCaseDetailJson.Where(x => x.ParentProjectModuleId == itemInfunctionTestCaseTotalCount.FunctionId).Select(x => x.FunctionName).FirstOrDefault();
						if (itemInfunctionTestCaseTotalCount.FunctionName != null)
						{
							break;
						}
					}

				}

				//FunctionNameWithStatus By FunctionId

				var functionNameTestCaseCountResult = await (from t in _context.TestRun
															 join trh in _context.TestRunHistory on t.TestRunId equals trh.TestRunId
															 join trtch in _context.TestRunTestCaseHistory on trh.TestRunHistoryId equals trtch.TestRunHistoryId
															 where t.TestRunId == testRunId
															 select new FunctionNameTestCaseCountModel()
															 {
																 TestRunHistoryId = trh.TestRunHistoryId,
																 TestRunStatusListItemId = trtch.TestRunStatusListItemId,
																 TestRunTestCaseStatus = trtch.TestRunStatusListItem.ListItemSystemName,
																 TestRunNameTitle = t.Title,
																 InsertDate = trtch.UpdateDate,
																 TestRunTestCaseHistoryId = trtch.TestRunTestCaseHistoryId,
																 ProjectModuleId = trtch.ProjectModuleId,
																 ParentProjectModuleId = trtch.ProjectModule.ParentProjectModuleId == null ? null : trtch.ProjectModule.ParentProjectModuleId,
																 TestRunId = t.TestRunId,
																 TestCaseId = trtch.ProjectModuleId,
																 TestPlanId = trtch.TestPlanId,
																 TestCaseDetailStatus = trtch.TestRunStatusListItem.ListItemSystemName,
															 }).ToListAsync();

				var testPlanIds = functionNameTestCaseCountResult.Select(x => x.TestPlanId).ToList();
				var projectModuleIds = functionNameTestCaseCountResult.Select(x => x.ProjectModuleId).ToList();


				var testRunTestCaseHistoryIds = functionNameTestCaseCountResult.Select(x => x.TestRunTestCaseHistoryId).ToList();
				List<FunctionNameTestCaseJsonModel> testRunPlanDetailResult = new List<FunctionNameTestCaseJsonModel>();
				var testRunPlanDetailList =  (from trpd in _context.TestRunPlanDetail
													 join trp in _context.TestRunPlan on trpd.TestRunPlanId equals trp.TestRunPlanId													
													 join trtc in _context.TestRunTestCaseHistory on trp.TestPlanId equals trtc.TestPlanId
													 where trp.TestRun.TestRunId == testRunId && testPlanIds.Contains(trp.TestPlanId)
													 select new FunctionNameTestCaseJsonModel
													 {
														 TestRunTestCaseHistoryId = trtc.TestRunTestCaseHistoryId,
														 ProjectModuleId = trtc.ProjectModuleId,
														 TestPlanId = trp.TestPlanId,
														 TestCaseDetail = trpd.TestCaseDetailJson,
														 TestPlanDetail = trpd.TestPlanDetailJson,
														 UpdateDate = trtc.UpdateDate

													 }).AsQueryable();
				testRunPlanDetailResult.AddRange(testRunPlanDetailList);
				foreach (var itemInFunctionNameTestCaseCountResult in testRunPlanDetailResult)
				{
					itemInFunctionNameTestCaseCountResult.TestCaseDetailJson = JsonSerializer.Deserialize<List<TestPlanTestCaseJson>>(itemInFunctionNameTestCaseCountResult.TestCaseDetail);
					itemInFunctionNameTestCaseCountResult.TestPlanDetailJson = JsonSerializer.Deserialize<List<TestPlanJson>>(itemInFunctionNameTestCaseCountResult.TestPlanDetail);
				}

				var functionNameWithTestCaseStatus = new List<FunctionNameWithTestCaseDetailModel>();
				var comment = await _context.TestRunTestCaseHistoryDocument.Where(x => testRunTestCaseHistoryIds.Contains(x.TestRunTestCaseHistoryId)).ToListAsync();
				var commentResult = comment.OrderByDescending(x => x.UpdateDate).GroupBy(x => x.TestRunTestCaseHistoryId).Select(x => new
				{
					TestRunTestCaseHistoryId = x.OrderByDescending(x => x.UpdateDate).Select(x => x.TestRunTestCaseHistoryId).FirstOrDefault(),
					Remarks = x.OrderByDescending(x => x.UpdateDate).Select(x => x.Comment).FirstOrDefault(),
				}).ToList();


				foreach (var functionNameOutputWithTestCase in testRunPlanDetailResult)
				{
					var fucntionProjectModuleResult = functionNameOutputWithTestCase.TestCaseDetailJson.Where(x => x.TestPlanId == functionNameOutputWithTestCase.TestPlanId).GroupBy(x => x.TestPlanId).Select(x => new FunctionNameWithTestCaseDetailModel
					{
						TestPlanName = x.Select(x => x.TestPlanName).FirstOrDefault(),
						TestPlanId = x.Select(x => x.TestPlanId).FirstOrDefault(),
						TestCaseDetail = x.OrderByDescending(x => x.InsertDate).Where(x => x.TestPlanId == functionNameOutputWithTestCase.TestPlanId && x.TestRunId == testRunId).GroupBy(x => x.ProjectModuleId).Select(y => new TestCaseDetailModel
						{
							TestPlanId = y.Select(y => y.TestPlanId).FirstOrDefault(),
							ProjectModuleId = y.Select(y => y.ProjectModuleId).FirstOrDefault(),
							TestCaseName = y.Select(y => y.ProjectModuleName).FirstOrDefault(),
							ExceptedResult = y.Select(y => y.ExpectedResult).FirstOrDefault(),
							PreConditon = y.Select(y => y.ProjectModuleDescription).FirstOrDefault(),


						}).ToList()
					}).DistinctBy(x => x.FunctionId).ToList();
					functionNameWithTestCaseStatus.AddRange(fucntionProjectModuleResult);
				}

				functionNameWithTestCaseStatus = functionNameWithTestCaseStatus.DistinctBy(x => x.TestPlanId).ToList();
				foreach (var itemInFunctionNameWithTestCaseStatus in functionNameWithTestCaseStatus)
				{
					foreach (var itemInTestCaseDetail in itemInFunctionNameWithTestCaseStatus.TestCaseDetail)
					{
						itemInTestCaseDetail.TestRunTestCaseHistoryId = testRunPlanDetailResult.OrderByDescending(x => x.UpdateDate).Where(x => x.ProjectModuleId == itemInTestCaseDetail.ProjectModuleId && x.TestPlanId== itemInFunctionNameWithTestCaseStatus.TestPlanId).Select(x => x.TestRunTestCaseHistoryId).FirstOrDefault();
						itemInTestCaseDetail.Remarks = commentResult.Where(x => x.TestRunTestCaseHistoryId == itemInTestCaseDetail.TestRunTestCaseHistoryId ).Select(x => x.Remarks).FirstOrDefault();
					}
				}

				var testPlanIdList = functionNameWithTestCaseStatus.Select(x => x.TestPlanId).ToList();
				var testCaseDetailList = functionNameWithTestCaseStatus.SelectMany(y => y.TestCaseDetail).ToList();
				var projectModuelIdLists = testCaseDetailList.Select(y => y.ProjectModuleId).ToList();
				functionNameWithTestCaseStatus = functionNameWithTestCaseStatus.DistinctBy(x => x.TestPlanId).ToList();

				var testCaseStatusList = await (from trtch in _context.TestRunTestCaseHistory
												join trh in _context.TestRunHistory on trtch.TestRunHistoryId equals trh.TestRunHistoryId
												where trh.TestRunId == testRunId && testPlanIdList.Contains(trtch.TestPlanId) && projectModuelIdLists.Contains(trtch.ProjectModuleId)
												select new TestCaseStatusListModel()
												{
													TestRunId = trh.TestRunId,
													InsertDate = trtch.InsertDate,
													ProjectModuleId = trtch.ProjectModuleId,
													TestPlanId = trtch.TestPlanId,
													TestCaseStatus = trtch.TestRunStatusListItem.ListItemSystemName
												}).ToListAsync();

				foreach (var valueForStatus in functionNameWithTestCaseStatus)
				{
					foreach (var status in valueForStatus.TestCaseDetail)
					{
						status.Status = testCaseStatusList.OrderByDescending(x => x.InsertDate).Where(x => x.TestRunId == testRunId && x.TestPlanId == status.TestPlanId && x.ProjectModuleId == status.ProjectModuleId).Select(x => x.TestCaseStatus).FirstOrDefault();
					}

				}

				functionNameWithTestCaseStatus = functionNameWithTestCaseStatus.GroupBy(x => x.TestPlanId).Select(y => new FunctionNameWithTestCaseDetailModel
				{
					TestPlanId = y.Select(z => z.TestPlanId).FirstOrDefault(),
					TestPlanName = y.Select(z => z.TestPlanName).FirstOrDefault(),
					TestCaseDetail = y.SelectMany(z => z.TestCaseDetail).ToList()
				}).ToList();

				#endregion

				#region TestCaseResultCount and TestRunCountPercentage
				var testPlanTestCaseCount = await TestPlanStatusResult(testRunId);
				var testRunStatusCountPercentage = PassedCount(testPlanTestCaseCount);
				#endregion

				#region TestCasesWithStatus

				var testRunTestCaseStatusResults = new List<TestRunTestCaseStatusResultModel>();

				var testRunTestCaseStatusResult = functionNameTestCaseCountResult.OrderByDescending(x => x.InsertDate).GroupBy(x => new { x.TestPlanId, x.ProjectModuleId }).Select(x => new TestRunTestCaseStatusResultModel
				{
					TestPlanId = x.Select(x => x.TestPlanId).FirstOrDefault(),
					ProjectModuleId = x.Select(x => x.ProjectModuleId).FirstOrDefault()

				}).ToList();

				var testRunTestCaseResult = new List<TestRunTestCaseExportModel>();

				foreach (var testRunTestCases in testRunPlanDetailResult)
				{
					var testRunTestCaseByTestRunId = testRunTestCases.TestCaseDetailJson.OrderByDescending(x => x.InsertDate).Where(x => x.TestRunId == testRunId && projectModuelIdLists.Contains(x.ProjectModuleId) && x.TestPlanId == testRunTestCases.TestPlanId).GroupBy(x => new { x.TestPlanId, x.ProjectModuleId }).Select(x => new TestRunTestCaseExportModel
					{
						TestCaseName = x.Select(x => x.ProjectModuleName).FirstOrDefault(),
						TestPlanName = x.Select(x => x.TestPlanName).FirstOrDefault(),
						TestPlanId = x.Select(x => x.TestPlanId).FirstOrDefault(),
						TestCaseId = x.Select(x => x.ProjectModuleId).FirstOrDefault(),

					}).ToList();
					testRunTestCaseResult.AddRange(testRunTestCaseByTestRunId);
				}
				testRunTestCaseResult = testRunTestCaseResult.DistinctBy(x => x.TestCaseId).ToList();
				foreach (var itemInTestRunTestCaseByTestRunId in testRunTestCaseResult)
				{
					itemInTestRunTestCaseByTestRunId.Status = testCaseStatusList.OrderByDescending(x => x.InsertDate).Where(x => x.TestRunId == testRunId && x.TestPlanId == itemInTestRunTestCaseByTestRunId.TestPlanId && x.ProjectModuleId == itemInTestRunTestCaseByTestRunId.TestCaseId).Select(x => x.TestCaseStatus).FirstOrDefault();
				}
				#endregion
				var pdfExcelReport = new PdfExcelTestRunTestCaseReportModel();

				pdfExcelReport.TestRunTestCaseExportModelForExcelReport = testRunTestCaseResult;
				pdfExcelReport.TestRunExcelModelTitle = testRunDetails;
				pdfExcelReport.RedarDiagram = functionTestCaseTotalCount;
				pdfExcelReport.PieChart = testRunStatusCountPercentage;
				pdfExcelReport.BarChart = testPlanTestCaseCount;
				pdfExcelReport.FunctionTestCase = functionNameWithTestCaseStatus;

				return pdfExcelReport;



			}
			catch (Exception ex)
			{
				if (ex is PdfOrExcelNameIsNotValidException)
				{
					_iLogger.LogError("Could not find the pdf or excel to print. Exception message is: ", ex.Message);
					throw new PdfOrExcelNameIsNotValidException();
				}
				_iLogger.LogError("Could not find the TestRunId. Exception message is: ", ex.Message);
				throw new TestRunIdNotFoundException();
			}
		}

		private async Task<int> TestRunTestCaseStepStatusCount(int testRunTestCaseHistoryId, int testRunStatusListItemId)
		{
			int blocked = 0;
			int passed = 0;
			int pending = 0;
			int failed = 0;
			TestRunTestCaseStepStatusCount model = new TestRunTestCaseStepStatusCount();

			var testRunTestCaseStepStaus = await _context.TestRunTestCaseStepHistory.Include(x => x.TestRunStatusListItem).
				Where(x => x.TestRunTestCaseHistoryId == testRunTestCaseHistoryId).ToListAsync();

			var status = testRunTestCaseStepStaus.OrderByDescending(x => x.TestRunStatusListItemId).FirstOrDefault();
			foreach (var item in testRunTestCaseStepStaus)
			{
				if (item.TestRunStatusListItem.ListItemSystemName == ListItem.Passed.ToString())
				{
					passed++;
				}
				else if (item.TestRunStatusListItem.ListItemSystemName == ListItem.Failed.ToString())
				{
					failed++;
				}
				else if (item.TestRunStatusListItem.ListItemSystemName == ListItem.Blocked.ToString())
				{
					blocked++;
				}
				else if (item.TestRunStatusListItem.ListItemSystemName == ListItem.Pending.ToString())
				{
					pending++;
				}
			}
			if (testRunTestCaseHistoryId == 45)
			{
				model.TestRunStatusListItemIdForNewHistory = 45;

			}
			else if (pending >= 1)
			{
				model.TestRunStatusListItemIdForNewHistory = 45;

			}
			else if (blocked >= 1)
			{
				model.TestRunStatusListItemIdForNewHistory = 44;
			}
			else if (failed >= 1)
			{
				model.TestRunStatusListItemIdForNewHistory = 43;
			}
			else if (passed >= 1)
			{
				model.TestRunStatusListItemIdForNewHistory = 42;
			}
			var result = model.TestRunStatusListItemIdForNewHistory;
			return result;
		}

		private static List<TestRunStatus> GetStatus(List<string> list)
		{
			int pending = 0;
			int failed = 0;
			int passed = 0;
			int blocked = 0;

			foreach (var item in list)
			{
				if (item == ListItem.Pending.ToString())
				{
					pending++;
				}
				if (item == ListItem.Passed.ToString())
				{
					passed++;
				}
				if (item == ListItem.Failed.ToString())
				{
					failed++;
				}
				if (item == ListItem.Blocked.ToString())
				{
					blocked++;
				}
			}

			return new List<TestRunStatus> {
				new TestRunStatus
				{
					StatusId = 45,
					Status = nameof(ListItem.Pending),
					StatusCount = pending
				},
				new TestRunStatus
				{
					StatusId = 42,
					Status = nameof(ListItem.Passed),
					StatusCount = passed
				},
				new TestRunStatus
				{
					StatusId = 43,
					Status = nameof(ListItem.Failed),
					StatusCount = failed
				},
				new TestRunStatus
				{
					StatusId = 44,
					Status = nameof(ListItem.Blocked),
					StatusCount = blocked
				}
			};


		}

		private async Task TestRunTestCaseHistoryDocument(int testRunTestCaseHistoryId, string? comment, int? documentId = null)
		{

			Entities.TestRunTestCaseHistoryDocument testRunHistoryDocument = new Entities.TestRunTestCaseHistoryDocument()
			{
				TestRunTestCaseHistoryId = testRunTestCaseHistoryId,
				DocumentId = documentId,
				Comment = comment
			};
			await _context.TestRunTestCaseHistoryDocument.AddAsync(testRunHistoryDocument);
			await _context.SaveChangesAsync();
		}

		private async Task TestRunTestCaseHistoryDocumentStep(int testRunTestCaseHistoryId, string? comment, int? documentId = null)
		{

			Entities.TestRunTestCaseHistoryDocument testRunHistoryDocument = new Entities.TestRunTestCaseHistoryDocument()
			{
				TestRunTestCaseHistoryId = testRunTestCaseHistoryId,
				DocumentId = documentId,
				//Comment = comment
			};
			await _context.TestRunTestCaseHistoryDocument.AddAsync(testRunHistoryDocument);
			await _context.SaveChangesAsync();
		}


		private async Task TestRunTestCaseStepHistoryListDocument(List<int> testRunTestCaseHistoryId, string? comment, int? documentId = null)
		{

			var testRunHistoryDocument = testRunTestCaseHistoryId.Select(x => new Entities.TestRunTestCaseStepHistoryDocument
			{
				TestRunTestCaseStepHistoryId = x,
				DocumentId = documentId,
				//Comment = comment
			}).ToList();
			await _context.TestRunTestCaseStepHistoryDocument.AddRangeAsync(testRunHistoryDocument);
			await _context.SaveChangesAsync();
		}
		private async Task TestRunTestCaseStepHistoryDocument(int testRunTestCaseStepHistoryId, string? comment, int? documentId = null)
		{
			Entities.TestRunTestCaseStepHistoryDocument testRunHistoryDocument = new Entities.TestRunTestCaseStepHistoryDocument()
			{
				TestRunTestCaseStepHistoryId = testRunTestCaseStepHistoryId,
				DocumentId = documentId,
				Comment = comment
			};
			await _context.TestRunTestCaseStepHistoryDocument.AddAsync(testRunHistoryDocument);
			await _context.SaveChangesAsync();
		}
		private async Task<int> FileUploadTestRunTestCaseWizard(IFormFile files)
		{

			var documentId = 0;
			if (files != null)
			{
				if (files.Length > 0)
				{
					//Getting FileName
					var fileName = Path.GetFileName(files.FileName);
					//Getting file Extension
					var fileExtension = Path.GetExtension(fileName);
					// concatenating  FileName + FileExtension
					var newFileName = String.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);

					Entities.Document objfiles = new Entities.Document()
					{
						Name = fileName,
						Extension = fileExtension,
					};

					using (var target = new MemoryStream())
					{
						files.CopyTo(target);
						objfiles.File = target.ToArray();
					}

					await _context.Document.AddAsync(objfiles);
					await _context.SaveChangesAsync();
					documentId = objfiles.DocumentId;

				}
				return documentId;
			}
			else
			{
				throw new FileNotUploadException();
			}
		}
		private async Task<List<TestRunTestCaseExportTestResultCountModel>> TestPlanStatusResult(int testRunId)
		{
			var latestTestPlanStatus = await (from trtc in _context.TestRunTestCaseHistory
											  join trh in _context.TestRunHistory on trtc.TestRunHistoryId equals trh.TestRunHistoryId
											  join tr in _context.TestRun on trh.TestRunId equals tr.TestRunId
											  join tp in _context.TestPlan on trtc.TestPlanId equals tp.TestPlanId
											  join li in _context.ListItem on trtc.TestRunStatusListItemId equals li.ListItemId into status
											  from li in status.DefaultIfEmpty()
											  where trh.TestRunId == testRunId
											  select new TestPlanStatusModel
											  {
												  TestRunTestCaseHistoryId = trtc.TestRunTestCaseHistoryId,
												  TestPlanId = tp.TestPlanId,
												  InsertDate = trtc.InsertDate,
												  ProjectModuleId = trtc.ProjectModuleId,
												  TestRunStatusName = li.ListItemSystemName

											  }).ToListAsync();

			var latestTestPlanStatusCountResult = latestTestPlanStatus.GroupBy(x => x.TestPlanId).Select(x => new TestPlanStatusFilterMode
			{
				TestPlanId = x.Select(x => x.TestPlanId).FirstOrDefault(),
				TestCaseHistoryList = latestTestPlanStatus.OrderByDescending(y => y.InsertDate).Where(y => y.TestPlanId == x.Key).GroupBy(x => new { x.TestPlanId, x.ProjectModuleId }).Select(y => new TestCaseHistoryListModel
				{
					TestPlanId = y.Select(y => y.TestPlanId).FirstOrDefault(),
					TestRunTestCaseHistoryId = y.Select(y => y.TestRunTestCaseHistoryId).FirstOrDefault(),
					ProjectModuleId = y.Select(y => y.ProjectModuleId).FirstOrDefault(),
				}).ToList()

			}).ToList();


			foreach (var itemInLatestTestPlanStatusCountResult in latestTestPlanStatusCountResult)
			{
				foreach (var itemInTestCaseHistoryList in itemInLatestTestPlanStatusCountResult.TestCaseHistoryList)
				{
					itemInTestCaseHistoryList.TestRunStatusName = latestTestPlanStatus.Where(x => x.TestRunTestCaseHistoryId == itemInTestCaseHistoryList.TestRunTestCaseHistoryId && x.ProjectModuleId == itemInTestCaseHistoryList.ProjectModuleId && x.TestPlanId == itemInLatestTestPlanStatusCountResult.TestPlanId).Select(y => y.TestRunStatusName).FirstOrDefault();
					itemInTestCaseHistoryList.PassedCount = latestTestPlanStatus.Where(x => x.TestRunTestCaseHistoryId == itemInTestCaseHistoryList.TestRunTestCaseHistoryId && x.ProjectModuleId == itemInTestCaseHistoryList.ProjectModuleId && x.TestPlanId == itemInLatestTestPlanStatusCountResult.TestPlanId).Count(y => y.TestRunStatusName == ListItem.Passed.ToString());
					itemInTestCaseHistoryList.PendingCount = latestTestPlanStatus.Where(x => x.TestRunTestCaseHistoryId == itemInTestCaseHistoryList.TestRunTestCaseHistoryId && x.ProjectModuleId == itemInTestCaseHistoryList.ProjectModuleId && x.TestPlanId == itemInLatestTestPlanStatusCountResult.TestPlanId).Count(y => y.TestRunStatusName == ListItem.Pending.ToString());
					itemInTestCaseHistoryList.FailedCount = latestTestPlanStatus.Where(x => x.TestRunTestCaseHistoryId == itemInTestCaseHistoryList.TestRunTestCaseHistoryId && x.ProjectModuleId == itemInTestCaseHistoryList.ProjectModuleId && x.TestPlanId == itemInLatestTestPlanStatusCountResult.TestPlanId).Count(y => y.TestRunStatusName == ListItem.Failed.ToString());
					itemInTestCaseHistoryList.BlockedCount = latestTestPlanStatus.Where(x => x.TestRunTestCaseHistoryId == itemInTestCaseHistoryList.TestRunTestCaseHistoryId && x.ProjectModuleId == itemInTestCaseHistoryList.ProjectModuleId && x.TestPlanId == itemInLatestTestPlanStatusCountResult.TestPlanId).Count(y => y.TestRunStatusName == ListItem.Blocked.ToString());

				}
			}

			var testPlanDetailJson = await (from trpd in _context.TestRunPlanDetail
											join trp in _context.TestRunPlan on trpd.TestRunPlanId equals trp.TestRunPlanId
											join tr in _context.TestRun on trp.TestRunId equals tr.TestRunId
											where tr.TestRunId == testRunId
											select new TestPlanDetailJsonModel
											{
												TestPlanId = trp.TestPlanId,
												TestPlanDetail = trpd.TestCaseDetailJson,
											}).ToListAsync();

			foreach (var itemInTestPlanDetailJson in testPlanDetailJson)
			{
				itemInTestPlanDetailJson.TestPlanDetailJson = JsonSerializer.Deserialize<List<TestPlanJson>>(itemInTestPlanDetailJson.TestPlanDetail);
			}

			var testPlanTestCaseCountResult = new List<TestRunTestCaseExportTestResultCountModel>();

			foreach (var itemTestPlanDetailJson in testPlanDetailJson)
			{
				foreach (var item in latestTestPlanStatusCountResult)
				{
					var testPlanTestCaseCount = item.TestCaseHistoryList.Where(x => x.TestPlanId == itemTestPlanDetailJson.TestPlanId).GroupBy(x => x.TestPlanId).Select(x => new TestRunTestCaseExportTestResultCountModel
					{
						TestPlanId = x.Select(x => x.TestPlanId).FirstOrDefault(),
						TestPlanNameForCount = itemTestPlanDetailJson.TestPlanDetailJson.Where(y => y.TestPlanId == x.Key).Select(x => x.TestPlanName).FirstOrDefault(),
						TotalPassedCount = x.Sum(x => x.PassedCount),
						TotalFailedCount = x.Sum(x => x.FailedCount),
						TotalPendingCount = x.Sum(x => x.PendingCount),
						TotalBlockCount = x.Sum(x => x.BlockedCount)

					}).ToList();
					testPlanTestCaseCountResult.AddRange(testPlanTestCaseCount);
				}

			}


			return testPlanTestCaseCountResult;
		}

		private decimal TestRunStatusPercentage(decimal averageTotal, decimal perStatusTotal)
		{
			return ((perStatusTotal / averageTotal) * 100);
		}
		private TestRunTestCaseCountPercentageModel PassedCount(List<TestRunTestCaseExportTestResultCountModel> counts)
		{
			decimal averageSum = 0;
			decimal totalPassed = 0;
			decimal totalPending = 0;
			decimal totalFailed = 0;
			decimal totalBlocked = 0;
			var countList = counts.ToList();
			foreach (var countSum in countList)
			{
				averageSum += countSum.TotalBlockCount + countSum.TotalFailedCount + countSum.TotalPassedCount + countSum.TotalPendingCount;
				totalBlocked += countSum.TotalBlockCount;
				totalPassed += countSum.TotalPassedCount;
				totalPending += countSum.TotalPendingCount;
				totalFailed += countSum.TotalFailedCount;

			}

			var countPercentage = new TestRunTestCaseCountPercentageModel();
			{
				countPercentage.PassedPercentage = TestRunStatusPercentage(averageSum, totalPassed);
				countPercentage.FailedPercentage = TestRunStatusPercentage(averageSum, totalFailed);
				countPercentage.PendingPercentage = TestRunStatusPercentage(averageSum, totalPending);
				countPercentage.BlockedPercentage = TestRunStatusPercentage(averageSum, totalBlocked);
			}

			return countPercentage;
		}

		private Dictionary<string, string> GetMimeTypes()
		{
			return new Dictionary<string, string>
		{
			{".txt", "text/plain"},
			{".pdf", "application/pdf"},
			{".doc", "application/msword"},
			{".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
			{".png", "image/png"},
			{".jpg", "image/jpeg"},
			{".xlsx","application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
			{".xls","application/vnd.ms-excel" },
			{".ppt","application/vnd.ms-powerpoint" },
			{".pptx","application/vnd.openxmlformats-officedocument.presentationml.presentation"},

		};
		}
		private int StatusCountByTestRunId(int testRunId, int testPlanId, string status, IQueryable<TestRunTestCaseHistoryStatusModel> testRunTestCaseHistory)
		{
			testRunTestCaseHistory = testRunTestCaseHistory.Where(x => x.TestPlanId == testPlanId && x.TestRunId == testRunId).AsQueryable();

			var statusCount = testRunTestCaseHistory.OrderByDescending(x => x.UpdateDate).GroupBy(x => x.ProjectModuleId).Select(x => new TestRunStatusModle
			{
				ProjectModuleId = x.Key,
				Status = x.OrderByDescending(x => x.UpdateDate).Select(x => x.Status).FirstOrDefault(),
				StatusListItemSystemName = x.OrderByDescending(x => x.UpdateDate).Select(x => x.StatusListItemSystemName).FirstOrDefault(),
			}).AsQueryable();

			var statusCountResult = (from sc in statusCount
									 where sc.StatusListItemSystemName == status
									 select sc.Status).Count();

			return statusCountResult;

		}



		private int? TotalTimeSpent(int testRunId, int testPlanId, IQueryable<TestRunTestCaseHistoryStatusModel> testRunTestCaseHistory)
		{

			testRunTestCaseHistory = testRunTestCaseHistory.Where(x => x.TestPlanId == testPlanId && x.TestRunId == testRunId).AsQueryable();

			var timeSpentSum = testRunTestCaseHistory.OrderByDescending(x => x.UpdateDate).GroupBy(x => new { x.ProjectModuleId, x.TestPlanId }).Select(x => new TimeSpentModel
			{
				ProjectModuleId = x.Key.ProjectModuleId,
				TestRunTestCaseHistoryId = x.OrderByDescending(x => x.UpdateDate).Select(x => x.TestRunTestCaseHistoryId).FirstOrDefault(),
				TestRunId = x.OrderByDescending(x => x.UpdateDate).Select(x => x.TestRunId).FirstOrDefault(),
				TestPlanId = x.OrderByDescending(x => x.UpdateDate).Select(x => x.TestPlanId).FirstOrDefault(),
				TotalTimeSpent = x.OrderByDescending(x => x.UpdateDate).Select(x => x.TimeSpent).FirstOrDefault(),
				UpdateDate = x.OrderByDescending(x => x.UpdateDate).Select(x => x.UpdateDate).FirstOrDefault(),
			}).AsQueryable();


			int totalSum = timeSpentSum.Where(x => x.TotalTimeSpent != null).Select(x => (int)x.TotalTimeSpent).ToList().Sum();
			return totalSum;


		}
		#endregion


		#region Implementation

		//Add Test Run 
		public async Task<Result<string>> AddTestRunAsync(TestRunModel model)
		{
			await _context.Database.BeginTransactionAsync();
			try
			{
				var projectId = await _iCommonService.GetProjectIdFromProjectSlug(model.ProjectSlug);
				var project = _context.Project.Where(x => x.ProjectSlug == model.ProjectSlug).FirstOrDefault();
				if (projectId > 0)
				{
					if (string.IsNullOrEmpty(model.Title) || string.IsNullOrWhiteSpace(model.Title))
					{
						throw new TestRunTitleCannotBeEmptyException();
					}
					else
					{
						//Add TestRun
						Entities.TestRun testRun = new()
						{
							Title = model.Title,
							Description = model.Description,
							ProjectId = projectId,
							EnvironmentId = model?.EnvironmentId == null ? null : model.EnvironmentId,
							DefaultAssigneeProjectMemberId = model?.DefaultAssigneeProjectMemberId == null ? null : model.DefaultAssigneeProjectMemberId
						};
						project.UpdateDate = DateTimeOffset.UtcNow;
						await _context.AddAsync(testRun);
						await _context.SaveChangesAsync();

						//Add TestRunPlan
						List<int> testPlanIds = model.TestPlanId;
						var testRunPlan = testPlanIds.Select(z => new Entities.TestRunPlan
						{
							TestPlanId = z,
							TestRunId = testRun.TestRunId
						}).ToList();

						await _context.AddRangeAsync(testRunPlan);
						await _context.SaveChangesAsync();

						//Add TestRunHistory
						var testRunStatus = await (_iCommonService.GetListItemDetailByListItemSystemName(ListItem.Pending.ToString()));
						Entities.TestRunHistory testRunHistory = new()
						{
							StartTime = null,
							EndTime = null,
							TotalTimeSpent = null,
							TestRunId = testRun.TestRunId
						};

						await _context.AddAsync(testRunHistory);
						await _context.SaveChangesAsync();

						//Add TestRunTestCaseHistory
						List<int> testPlanId = model.TestPlanId;
						var projectModuleId = await (from tptc in _context.TestPlanTestCase
													 join tp in _context.TestPlan on tptc.TestPlanId equals tp.TestPlanId
													 join li in _context.ListItem on tp.TestPlanTypeListItemId equals li.ListItemId
													 join pm in _context.ProjectModule on tptc.ProjectModuleId equals pm.ProjectModuleId
													 join li2 in _context.ListItem on pm.ProjectModuleListItemId equals li2.ListItemId
													 where testPlanId.Contains(tp.TestPlanId) && tptc.IsDeleted == false && pm.IsDeleted == false
													 select new TestPlanTestCaseModel
													 {
														 ProjectModuleId = pm.ProjectModuleId,
														 ProjectModuleListItemId = li2.ListItemId,
														 ProjectModuleListItemName = li2.ListItemSystemName,
														 TestPlanTypeListItemId = li.ListItemId,
														 TestPlanTypeListItemName = li.ListItemSystemName,
														 TestPlanId = tp.TestPlanId,
													 }).ToListAsync();


						var testRunTestCaseHistory = projectModuleId.Select(x => new Entities.TestRunTestCaseHistory
						{
							StartTime = null,
							EndTime = null,
							TotalTimeSpent = null,
							TestRunStatusListItemId = testRunStatus.ListItemId,
							AssigneeProjectMemberId = testRun.DefaultAssigneeProjectMemberId,
							ProjectModuleId = x.ProjectModuleId,
							TestRunHistoryId = testRunHistory.TestRunHistoryId,
							TestPlanId = x.TestPlanId,

						}).ToList();
						await _context.TestRunTestCaseHistory.AddRangeAsync(testRunTestCaseHistory);
						await _context.SaveChangesAsync();



						List<Entities.TestRunTestCaseStepHistory> addTestRunTestCaseStepHistories = new();
						//Add TestRunTestCaseStepHistory
						foreach (var TestRunTestCaseHistory in testRunTestCaseHistory)
						{
							var testCaseStepDetail = _context.TestCaseStepDetail.Where(x => x.ProjectModuleId == TestRunTestCaseHistory.ProjectModuleId).Select(x => x.TestCaseStepDetailId).ToList();

							foreach (var TestCaseStepDetailId in testCaseStepDetail)
							{
								Entities.TestRunTestCaseStepHistory testRunTestCaseStepHistory = new Entities.TestRunTestCaseStepHistory();

								testRunTestCaseStepHistory.StartTime = null;
								testRunTestCaseStepHistory.EndTime = null;
								testRunTestCaseStepHistory.TotalTimeSpent = null;
								testRunTestCaseStepHistory.TestRunStatusListItemId = testRunStatus.ListItemId;
								testRunTestCaseStepHistory.TestRunTestCaseHistoryId = TestRunTestCaseHistory.TestRunTestCaseHistoryId;
								testRunTestCaseStepHistory.TestCaseStepDetailId = TestCaseStepDetailId;
								addTestRunTestCaseStepHistories.Add(testRunTestCaseStepHistory);
							}
						}
						await _context.TestRunTestCaseStepHistory.AddRangeAsync(addTestRunTestCaseStepHistories);
						await _context.SaveChangesAsync();



						//TestPlanDetailJson
						var testPlanDetail = new List<TestPlanJson>();
						testPlanDetail = await (from trp in _context.TestRunPlan
												join tp in _context.TestPlan on trp.TestPlanId equals tp.TestPlanId
												join tr in _context.TestRun on trp.TestRunId equals tr.TestRunId
												join p in _context.Project on tp.ProjectId equals p.ProjectId
												join li in _context.ListItem on p.ProjectMarketListItemId equals li.ListItemId
												join li2 in _context.ListItem on tp.TestPlanTypeListItemId equals li2.ListItemId
												where testPlanIds.Contains(tp.TestPlanId) && tr.TestRunId == testRun.TestRunId && tp.IsDeleted == false
												select new TestPlanJson()
												{
													TestRunPlanId = trp.TestRunPlanId,
													TestPlanId = tp.TestPlanId,
													ParentTestPlanId = tp.ParentTestPlanId == null ? null : tp.ParentTestPlanId,
													TestPlanName = tp.TestPlanName == null ? string.Empty : tp.TestPlanName,
													TestPlanTypeListItemId = li2.ListItemId,
													TestPlanTypeListItemName = li2.ListItemSystemName,
													TestPlanTitle = tp.Title == null ? string.Empty : tp.Title,
													TestPlanDescription = tp.Description == null ? string.Empty : tp.Description,
													ProjectId = p.ProjectId,
													ProjectName = p.ProjectName == null ? string.Empty : p.ProjectName,
													ProjectDescription = p.ProjectDescription == null ? string.Empty : p.ProjectDescription,
													ProjectMarketListItemId = li.ListItemId,
													ProjectMarketListItemName = li.ListItemSystemName,
													ProjectSlug = p.ProjectSlug,
													DefaultAssigneeProjectMemberId = model.DefaultAssigneeProjectMemberId == null ? null : model.DefaultAssigneeProjectMemberId,
													EnvironmentId = model.EnvironmentId == null ? null : model.EnvironmentId,
													TestRunId = tr.TestRunId,
													TestRunName = tr.Title,
													TestRunDescription = tr.Description == null ? string.Empty : tr.Description,
													InsertDate = tr.InsertDate,
													InsertPersonId = tr.InsertPersonId,
													UpdateDate = tr.UpdateDate,
													UpdatedPersonId = tr.UpdatePersonId,
												}).ToListAsync();

						List<int> testPlanList = testPlanDetail.Select(x => x.TestPlanId).ToList();
						//TestCaseDetailJson	
						var testCaseDetails = new List<TestPlanTestCaseJson>();
						testCaseDetails = await (from tptc in _context.TestPlanTestCase
												 join pm in _context.ProjectModule on tptc.ProjectModuleId equals pm.ProjectModuleId
												 join li in _context.ListItem on pm.ProjectModuleListItemId equals li.ListItemId
												 join p in _context.Project on pm.ProjectId equals p.ProjectId
												 join tp in _context.TestPlan on tptc.TestPlanId equals tp.TestPlanId
												 join li2 in _context.ListItem on tp.TestPlanTypeListItemId equals li2.ListItemId
												 where testPlanList.Contains(tp.TestPlanId) && tptc.IsDeleted == false && pm.IsDeleted == false
												 select new TestPlanTestCaseJson()
												 {
													 TestPlanTestCaseId = tptc.TestPlanTestCaseId,
													 ProjectModuleId = pm.ProjectModuleId,
													 ParentProjectModuleId = pm.ParentProjectModuleId,
													 ProjectModuleName = pm.ModuleName,
													 ProjectModuleListItemId = pm.ProjectModuleListItemId,
													 OrderDate = pm.OrderDate,
													 ProjectModuleListItemName = li.ListItemSystemName,
													 ProjectId = p.ProjectId,
													 ProjectName = p.ProjectName,
													 ProjectSlug = p.ProjectSlug,
													 ProjectModuleDescription = pm.Description == null ? string.Empty : pm.Description,
													 ProjectDescription = p.ProjectDescription == null ? string.Empty : p.ProjectDescription,
													 TestPlanId = tp.TestPlanId,
													 TestPlanName = tp.TestPlanName,
													 TestPlanTypeListItemName = li2.ListItemSystemName,
													 TestPlanTitle = tp.Title,
													 ParentTestPlanId = tp.ParentTestPlanId,
													 TestPlanDescription = tp.Description == null ? string.Empty : tp.Description,
													 TestPlanTypeListItemId = li2.ListItemId,
													 TestRunId = testRun.TestRunId,
													 TestRunName = testRun.Title,
													 InsertDate = DateTimeOffset.UtcNow,
													 InsertPersonId = _iPersonAccessor.GetPersonId(),
													 UpdateDate = DateTimeOffset.UtcNow,
													 UpdatedPersonId = _iPersonAccessor.GetPersonId()
												 }).ToListAsync();


						var projectModuleIdList = testCaseDetails.Select(x => x.ProjectModuleId).ToList();


						var testCaseDetail = await (from tcd in _context.TestCaseDetail
													join pm in _context.ProjectModule on tcd.ProjectModuleId equals pm.ProjectModuleId
													join li in _context.ListItem on tcd.TestCaseListItemId equals li.ListItemId
													where projectModuleIdList.Contains(pm.ProjectModuleId) && pm.IsDeleted == false
													select new TestCaseDetail
													{
														TestCaseDetailId = tcd.TestCaseDetailId,
														ProjectModuleId = pm.ProjectModuleId,
														PreCondition = tcd.PreCondition,
														ExpectedResult = tcd.ExpectedResult,
														TestCaseListItemId = li.ListItemId,
														TestCaseListItemName = li.ListItemSystemName,

													}).ToListAsync();



						foreach (var item in testCaseDetails)
						{
							item.FunctionName = _context.ProjectModule.Where(x => x.ProjectModuleId == item.ParentProjectModuleId).Select(x => x.ModuleName).FirstOrDefault();
							item.TestCaseDetailId = testCaseDetail.Where(z => z.ProjectModuleId == item.ProjectModuleId).Select(z => z.TestCaseDetailId).FirstOrDefault();
							item.PreCondition = testCaseDetail.Where(z => z.ProjectModuleId == item.ProjectModuleId).Select(z => z.PreCondition).FirstOrDefault();
							item.ExpectedResult = testCaseDetail.Where(z => z.ProjectModuleId == item.ProjectModuleId).Select(z => z.ExpectedResult).FirstOrDefault();
							item.TestCaseListItemId = testCaseDetail.Where(z => z.ProjectModuleId == item.ProjectModuleId).Select(z => z.TestCaseListItemId).FirstOrDefault();
							item.TestCaseListItemName = testCaseDetail.Where(z => z.ProjectModuleId == item.ProjectModuleId).Select(z => z.TestCaseListItemName).FirstOrDefault();

						}
						List<TestCaseStepsUpdateModel> testCaseStepUpdateJsonResult = new List<TestCaseStepsUpdateModel>();
						foreach (var item in testRunTestCaseHistory)
						{
							var testCaseStepUpdateJson = addTestRunTestCaseStepHistories.Where(x => x.TestRunTestCaseHistoryId == item.TestRunTestCaseHistoryId).Select(x => new TestCaseStepsUpdateModel
							{
								TestRunTestCaseStepHistoryId = x.TestRunTestCaseStepHistoryId,
								TestRunTestCaseHistoryId = x.TestRunTestCaseHistoryId,
								TestPlanId = item.TestPlanId,
								ProjectModuleId = item.ProjectModuleId,
								StartTime = null,
								EndTime = null,
								TotalTimeSpent = null,
								TestRunStatusListItemId = testRunStatus.ListItemId,

								TestCaseStepDetailId = x.TestCaseStepDetailId,
							}).ToList();

							testCaseStepUpdateJsonResult.AddRange(testCaseStepUpdateJson);
						}

						//TestCaseStepDetailJson
						var planIdlist = testCaseDetails.Select(x => x.TestPlanId).ToList();
						var testCaseHistoryIds = testCaseStepUpdateJsonResult.Select(x => x.TestRunTestCaseHistoryId).ToList();


						var testCaseStepDetails = await (
							from trtch in _context.TestRunTestCaseHistory
							join trtcsh in _context.TestRunTestCaseStepHistory on trtch.TestRunTestCaseHistoryId equals trtcsh.TestRunTestCaseHistoryId
							join tcsd in _context.TestCaseStepDetail on trtcsh.TestCaseStepDetailId equals tcsd.TestCaseStepDetailId
							join pm in _context.ProjectModule on trtch.ProjectModuleId equals pm.ProjectModuleId
							join li in _context.ListItem on pm.ProjectModuleListItemId equals li.ListItemId
							join li2 in _context.ListItem on tcsd.TestCaseListItemId equals li2.ListItemId

							join tp in _context.TestPlan on trtch.TestPlanId equals tp.TestPlanId
							where projectModuleIdList.Contains(pm.ProjectModuleId) && pm.IsDeleted == false && testCaseHistoryIds.Contains(trtch.TestRunTestCaseHistoryId)

							select new TestCaseStepDetailsJson
							{
								TestCaseStepDetailId = tcsd.TestCaseStepDetailId,
								StepNumber = tcsd.StepNumber,
								StepDescription = tcsd.StepDescription,
								ExpectedResult = tcsd.ExpectedResult,
								TestCaseListItemId = li2.ListItemId,
								TestCaseListItemName = li2.ListItemName,
								ProjectId = pm.ProjectId,
								ProjectModuleId = pm.ProjectModuleId,
								ProjectModuleName = pm.ModuleName,
								ParentProjectModuleId = pm.ParentProjectModuleId,
								ProjectModuleDescription = pm.Description,
								ProjectModuleListItemId = li.ListItemId,
								ProjectModuleListItemName = li.ListItemSystemName,
								OrderDate = DateTimeOffset.UtcNow,
								InsertDate = DateTimeOffset.UtcNow,
								InsertPersonId = _iPersonAccessor.GetPersonId(),
								UpdateDate = DateTimeOffset.UtcNow,
								UpdatePersonId = _iPersonAccessor.GetPersonId(),
								TestRunTestCaseHistoryId = trtch.TestRunTestCaseHistoryId,
								TestRunTestCaseStepHistoryId = trtcsh.TestRunTestCaseStepHistoryId,
								TestPlanId = tp.TestPlanId,
								TestCaseStepStatusName = trtcsh.TestRunStatusListItem.ListItemSystemName
							}).ToListAsync();


						//Add TestRunPlanDetail (JsonData)
						var testRunPlanDetails = testRunPlan.Select(z => new Entities.TestRunPlanDetail
						{
							TestRunPlanId = z.TestRunPlanId,
							TestPlanDetailJson = JsonSerializer.Serialize(testPlanDetail.Where(x => x.TestPlanId == z.TestPlanId).ToList()).ToString(),
							TestCaseDetailJson = JsonSerializer.Serialize(testCaseDetails.Where(x => x.TestPlanId == z.TestPlanId).ToList()).ToString(),
							TestCaseStepDetailJson = JsonSerializer.Serialize(testCaseStepDetails.Where(x => testCaseDetails.Where(y => y.TestPlanId == z.TestPlanId).Select(y => y.ProjectModuleId).Contains(x.ProjectModuleId)).ToList()).ToString(),
						}).ToList();
						await _context.AddRangeAsync(testRunPlanDetails);
						await _context.SaveChangesAsync();


						await _context.Database.CommitTransactionAsync();
						return Result<string>.Success(ReturnMessage.SavedSuccessfully);
					}
				}
				else
				{
					throw new ProjectSlugNotFoundException();
				}

			}
			catch (Exception ex)
			{
				await _context.Database.RollbackTransactionAsync();
				if (ex is ProjectSlugNotFoundException)
				{
					_iLogger.LogError("Could not find the ProjectSlug. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ProjectSlugDoesNotExists);
				}
				else if (ex is TestRunTitleCannotBeEmptyException)
				{
					_iLogger.LogError("TestRunTitle Cannot Be Empty. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestRunTitleCannotBeEmpty);
				}
				_iLogger.LogError("Could not add the TestRun. Exception message is: ", ex.Message);
				return Result<string>.Error(ReturnMessage.FailedToAddTestRun);
			}
		}

		//Update Test Run 
		public async Task<Result<string>> UpdateTestRunAsync(UpdateTestRunModel model)
		{
			await _context.Database.BeginTransactionAsync();
			try
			{
				var updateTestRun = await _context.TestRun.Where(x =>
				x.TestRunId == model.TestRunId)
					.FirstOrDefaultAsync();


				var project = _context.Project.Where(x => x.ProjectId == updateTestRun.ProjectId).FirstOrDefault();


				if (updateTestRun != null)
				{

					if (string.IsNullOrEmpty(model.Title) || string.IsNullOrWhiteSpace(model.Title))
					{
						throw new TestRunTitleCannotBeEmptyException();
					}
					else
					{
						updateTestRun.TestRunId = model.TestRunId;
						updateTestRun.Title = model.Title;
						updateTestRun.EnvironmentId = model?.EnvironmentId;
						updateTestRun.Description = model.Description;
						project.UpdateDate = DateTimeOffset.UtcNow;

						_context.TestRun.Update(updateTestRun);
						await _context.SaveChangesAsync();
						await _context.Database.CommitTransactionAsync();
						return Result<string>.Success(ReturnMessage.UpdatedSuccessfully);
					}
				}
				else
				{
					throw new TestRunIdNotFoundException();
				}

			}
			catch (Exception ex)
			{

				await _context.Database.RollbackTransactionAsync();
				if (ex is TestRunIdNotFoundException)
				{
					_iLogger.LogError("Could not find the TestRunId. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestRunIdDoesnotExists);
				}
				else if (ex is TestRunTitleCannotBeEmptyException)
				{
					_iLogger.LogError("TestRunTitle Cannot Be Empty. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestRunTitleCannotBeEmpty);
				}

				_iLogger.LogError("Could not update the TestRun. Exception message is: ", ex.Message);
				return Result<string>.Error(ReturnMessage.FailedToUpdateTestRun);
			}
		}

		//Delete Test Run 
		public async Task<Result<string>> DeleteTestRunAsync(int testRunId)
		{
			await _context.Database.BeginTransactionAsync();
			try
			{
				var deleteTestRunId = await _context.TestRun.Where(x => x.TestRunId == testRunId).FirstOrDefaultAsync();
				if (deleteTestRunId != null)
				{
					var deleteTestRunPlanId = await _context.TestRunPlan.Where(x => x.TestRunId == deleteTestRunId.TestRunId).ToListAsync();

					var deleteTestRunHistory = await _context.TestRunHistory.Where(x => x.TestRunId == deleteTestRunId.TestRunId).ToListAsync();

					var deleteTestRunTestCaseHistory = await _context.TestRunTestCaseHistory.Include(x => x.TestRunHistory).Where(x => x.TestRunHistory.TestRunId == deleteTestRunId.TestRunId).ToListAsync();

					var deleteTestRunTestCaseStepHistory = await _context.TestRunTestCaseStepHistory.Include(x => x.TestRunTestCaseHistory).ThenInclude(x => x.TestRunHistory)
						.Where(x => x.TestRunTestCaseHistory.TestRunHistory.TestRunId == deleteTestRunId.TestRunId).ToListAsync();

					var deleteTestRunPlanDetail = await _context.TestRunPlanDetail.Include(x => x.TestRunPlan).Where(x => x.TestRunPlan.TestRunId == deleteTestRunId.TestRunId).ToListAsync();


					if (deleteTestRunPlanId.Count != 0)
					{
						if (deleteTestRunHistory.Count != 0)
						{
							if (deleteTestRunTestCaseHistory.Count != 0)
							{
								if (deleteTestRunTestCaseStepHistory.Count != 0)
								{
									_context.TestRunTestCaseStepHistory.RemoveRange(deleteTestRunTestCaseStepHistory);
									await _context.SaveChangesAsync();

								}
								_context.TestRunTestCaseHistory.RemoveRange(deleteTestRunTestCaseHistory);
								await _context.SaveChangesAsync();

							}
							_context.TestRunHistory.RemoveRange(deleteTestRunHistory);
							await _context.SaveChangesAsync();

						}

						_context.TestRunPlanDetail.RemoveRange(deleteTestRunPlanDetail);
						await _context.SaveChangesAsync();


						_context.TestRunPlan.RemoveRange(deleteTestRunPlanId);
						await _context.SaveChangesAsync();


						_context.TestRun.Remove(deleteTestRunId);

						var project = _context.Project.Where(x => x.ProjectId == deleteTestRunId.ProjectId).FirstOrDefault();
						project.UpdateDate = DateTimeOffset.UtcNow;

						await _context.SaveChangesAsync();



						await _context.Database.CommitTransactionAsync();
						return Result<string>.Success(ReturnMessage.DeletedSuccessfully);
					}
					else
					{
						throw new TestRunPlanIdNotFoundException();
					}

				}
				else
				{
					throw new TestRunIdNotFoundException();
				}
			}
			catch (Exception ex)
			{
				await _context.Database.RollbackTransactionAsync();

				if (ex is TestRunIdNotFoundException)
				{
					_iLogger.LogError("Could not find the TestRunId. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestRunIdDoesnotExists);
				}
				else if (ex is TestRunPlanIdNotFoundException)
				{
					_iLogger.LogError("Could not find the TestRunPlanId. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestRunPlanIdDoesnotExists);
				}
				else
				{
					_iLogger.LogError("Failed to delete the TestRun. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.FailedToDeleteTestRun);
				}
			}
		}

		//GetTestPlanByProjectSlug  
		public async Task<Result<PagedResponsePersonModel<List<TestPlanByProjectSlugModel>>>> GetTestPlanbyProjectSlugAsync(FilterModel filter, string projectSlug)
		{
			try
			{
				var projectId = await _iCommonService.GetProjectIdFromProjectSlug(projectSlug);
				if (projectId > 0)
				{
					var getTestPlan = await (from tp in _context.TestPlan
											 join pr in _context.Project on tp.ProjectId equals pr.ProjectId
											 join li in _context.ListItem on tp.TestPlanTypeListItemId equals li.ListItemId
											 where tp.ProjectId == projectId && li.ListItemSystemName == nameof(ListItem.TestPlan) && tp.IsDeleted == false
											 select new TestPlanByProjectSlugModel
											 {
												 TestPlanId = tp.TestPlanId,
												 TestPlanName = tp.TestPlanName
											 }).ToListAsync();



					if (!string.IsNullOrEmpty(filter.SearchValue))
					{

						getTestPlan = getTestPlan.Where
						  (
							  x => x.TestPlanName.ToLower().Contains(filter.SearchValue.ToLower())
						  ).ToList();

					}

					var filteredData = getTestPlan;
					var data = new PagedResponsePersonModel<List<TestPlanByProjectSlugModel>>(filteredData);



					if (filteredData.Count > 0)
					{
						return Result<PagedResponsePersonModel<List<TestPlanByProjectSlugModel>>>.Success(data);
					}
					else
					{
						return Result<PagedResponsePersonModel<List<TestPlanByProjectSlugModel>>>.Success(null);
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
				return Result<PagedResponsePersonModel<List<TestPlanByProjectSlugModel>>>.Error(ReturnMessage.ProjectSlugDoesNotExists);

			}
		}

		//GetProjectMemberListBySlug
		public async Task<Result<PagedResponsePersonModel<List<ProjectMemberByProjectSlugModel>>>> GetProjectMemberListBySlugAsync(FilterModel filter, string projectSlug)
		{
			try
			{

				var projectId = await _iCommonService.GetProjectIdFromProjectSlug(projectSlug);
				if (projectId > 0)
				{
					var getProjectMemberList = await (from tp in _context.ProjectMember
													  join p in _context.Project on tp.ProjectId equals p.ProjectId
													  join per in _context.Person on tp.PersonId equals per.PersonId
													  where tp.ProjectId == projectId
													  select new ProjectMemberByProjectSlugModel
													  {
														  ProjectMemberId = tp.ProjectMemberId,
														  Name = per.Name,
														  UserName = per.UserName
													  }).ToListAsync();


					if (!string.IsNullOrEmpty(filter.SearchValue))
					{

						getProjectMemberList = getProjectMemberList.Where
						  (
							  x => x.Name.ToLower().Contains(filter.SearchValue.ToLower())
						  ).ToList();

					}

					var filteredData = getProjectMemberList;
					var data = new PagedResponsePersonModel<List<ProjectMemberByProjectSlugModel>>(filteredData);



					if (filteredData.Count > 0)
					{

						return Result<PagedResponsePersonModel<List<ProjectMemberByProjectSlugModel>>>.Success(data);
					}
					else
					{
						return Result<PagedResponsePersonModel<List<ProjectMemberByProjectSlugModel>>>.Success(null);
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
				return Result<PagedResponsePersonModel<List<ProjectMemberByProjectSlugModel>>>.Error(ReturnMessage.ProjectSlugDoesNotExists);
			}
		}

		//AssignUserToTestCase
		public async Task<Result<string>> AssignUserToTestCaseAsync(AssignUserToTestCaseModel model)
		{
			await _context.Database.BeginTransactionAsync();
			try
			{

				List<int> testRunTestCaseHistoryIds = model.TestRunTestCaseHistoryId;
				var testRunTestCaseHistory = await _context.TestRunTestCaseHistory.Where(x => testRunTestCaseHistoryIds.Contains(x.TestRunTestCaseHistoryId)).ToListAsync();

				if (testRunTestCaseHistory.Count > 0)
				{
					foreach (var assignProjectMember in testRunTestCaseHistory)
					{
						assignProjectMember.AssigneeProjectMemberId = model.AssigneProjectMemberId;
						_context.TestRunTestCaseHistory.Update(assignProjectMember);
						await _context.SaveChangesAsync();
					}

					await _context.Database.CommitTransactionAsync();
					return Result<string>.Success(ReturnMessage.UserSuccessfullyAssignedToTestCases);
				}
				else
				{
					throw new UserFailedAssignedToTestCasesException();
				}

			}
			catch (Exception ex)
			{
				await _context.Database.RollbackTransactionAsync();
				_iLogger.LogError("Could not assign the user to testcase the ProjectSlug. Exception message is: ", ex.Message);
				return Result<string>.Error(ReturnMessage.UserFailedAssignedToTestCases);
			}
		}

		//UnAssignUserToTestCase
		public async Task<Result<string>> UnAssignUserToTestCaseAsync(UnAssignUserToTestCaseModel model)
		{
			await _context.Database.BeginTransactionAsync();
			try
			{
				List<int> testRunTestCaseHistoryIds = model.TestRunTestCaseHistoryId;
				var testRunTestCaseHistory = await _context.TestRunTestCaseHistory.Where(x => testRunTestCaseHistoryIds.Contains(x.TestRunTestCaseHistoryId)).ToListAsync();
				if (testRunTestCaseHistory.Count > 0)
				{
					foreach (var assignProjectMember in testRunTestCaseHistory)
					{
						assignProjectMember.AssigneeProjectMemberId = null;
						_context.TestRunTestCaseHistory.Update(assignProjectMember);
						await _context.SaveChangesAsync();
					}
					await _context.Database.CommitTransactionAsync();
					return Result<string>.Success(ReturnMessage.UserSuccessfullyUnAssignedToTestCases);
				}
				else
				{
					throw new UserFailedUnAssignedToTestCasesException();
				}
			}
			catch (Exception ex)
			{
				await _context.Database.RollbackTransactionAsync();
				_iLogger.LogError("Could not unassign the user to testcase the ProjectSlug. Exception message is: ", ex.Message);
				return Result<string>.Error(ReturnMessage.UserFailedUnAssignedToTestCases);
			}
		}

		//GetTestRunList
		public async Task<Result<PagedResponseModel<List<TestRunListModel>>>> GetTestRunListAsync(PaginationFilterModel filter, string projectSlug)
		{
			try
			{
				var projectId = await _iCommonService.GetProjectIdFromProjectSlug(projectSlug);
				if (projectId > 0)
				{
					List<TestRunListModel> getTestRunListAllAsync;

					getTestRunListAllAsync = await (from tr in _context.TestRun
													join pr in _context.Project on tr.ProjectId equals pr.ProjectId
													join trh in _context.TestRunHistory on tr.TestRunId equals trh.TestRunId
													join trtch in _context.TestRunTestCaseHistory on trh.TestRunHistoryId equals trtch.TestRunHistoryId into test
													from trtch in test.DefaultIfEmpty()
													where tr.ProjectId == projectId
													select new TestRunListModel
													{

														TestRunId = tr.TestRunId,
														Title = tr.Title,
														EnvironmentId = tr.EnvironmentId == null ? null : tr.EnvironmentId,
														Environment = tr.Environment.EnvironmentName == null ? string.Empty : tr.Environment.EnvironmentName,
														Time = trh.TotalTimeSpent == null ? 0 : trh.TotalTimeSpent,
														StatusId = trtch.TestRunStatusListItemId == null ? 0 : trtch.TestRunStatusListItemId,
														StatusName = trtch.TestRunStatusListItem.ListItemSystemName == null ? string.Empty : trtch.TestRunStatusListItem.ListItemSystemName,
														TestRunHistoryId = trtch.TestRunHistoryId == null ? 0 : trtch.TestRunHistoryId,
														InsertDate = trtch.InsertDate == null ? DateTimeOffset.MinValue : trtch.InsertDate,
														ProjectModuleId = trtch.ProjectModuleId == null ? 0 : trtch.ProjectModuleId,
														TestPlanId = trtch.TestPlanId == null ? 0 : trtch.TestPlanId,
													}).OrderByDescending(x => x.TestRunId).ToListAsync();


					var getTestRunListAllAsyncs = getTestRunListAllAsync.GroupBy(a => a.TestRunId).Select(x => new TestRunListModel
					{
						TestRunId = x.Key,
						Title = x.Select(x => x.Title).FirstOrDefault(),
						Environment = x.Select(x => x.Environment).FirstOrDefault(),
						Time = x.Select(x => x.Time).FirstOrDefault(),
						EnvironmentId = x.Select(x => x.EnvironmentId).FirstOrDefault(),
						TestRunHistoryId = x.Select(x => x.TestRunHistoryId).FirstOrDefault(),
						ProjectModuleId = x.Select(x => x.ProjectModuleId).FirstOrDefault(),
						TestPlanId = x.Select(x => x.TestPlanId).FirstOrDefault()

					}).ToList();


					foreach (var item in getTestRunListAllAsyncs)
					{
						var res = getTestRunListAllAsync.OrderByDescending(x => x.InsertDate).Where(x => x.TestRunHistoryId == item.TestRunHistoryId).GroupBy(x => new { x.TestPlanId, x.ProjectModuleId }).Select(x => new TestRunListModel
						{
							StatusName = x.Select(x => x.StatusName).FirstOrDefault(),
						}).ToList();
						item.Status = GetStatus(res.Select(x => x.StatusName).ToList());
					}


					var records = getTestRunListAllAsyncs.ToList();
					if (!string.IsNullOrEmpty(filter.SearchValue))
					{
						records = records.Where
							(
								x => x.Title.ToLower().Contains(filter.SearchValue.ToLower())
							).ToList();
					}

					var totalRecords = records.Count();
					var filteredData = records
						.Skip((filter.PageNumber - 1) * filter.PageSize)
						.Take(filter.PageSize).ToList();


					if (filter.PageSize > totalRecords && totalRecords > 0)
					{
						filter.PageSize = totalRecords;
					}

					var totalPages = (totalRecords / filter.PageSize);

					var data = new PagedResponseModel<List<TestRunListModel>>(filteredData, filter.PageNumber, filter.PageSize, totalRecords, totalPages);

					if (filteredData.Count > 0)
					{
						return Result<PagedResponseModel<List<TestRunListModel>>>.Success(data);
					}
					else
					{
						return Result<PagedResponseModel<List<TestRunListModel>>>.Success(null);
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
				return Result<PagedResponseModel<List<TestRunListModel>>>.Error(ReturnMessage.ProjectSlugDoesNotExists);

			}
		}

		//GetTestRunListbyId
		public async Task<Result<GetAllTestRunListModel>> GetTestRunListbyIdAsync(int testRunId)
		{
			try
			{
				var testRunDetail = await _context.TestRun.Where(x => x.TestRunId == testRunId).FirstOrDefaultAsync();

				if (testRunDetail != null)
				{
					var getTestRunDetailByIdAsync = (from t in _context.TestRun
													 join trh in _context.TestRunHistory on t.TestRunId equals trh.TestRunId
													 join trtch in _context.TestRunTestCaseHistory on trh.TestRunHistoryId equals trtch.TestRunHistoryId
													 join li in _context.ListItem on trtch.TestRunStatusListItemId equals li.ListItemId
													 join p in _context.Person on t.InsertPersonId equals p.PersonId
													 where t.TestRunId == testRunId
													 select new
													 {
														 TestRunId = t.TestRunId,
														 Name = t.Title,
														 Description = t.Description,
														 StatusId = trtch.TestRunStatusListItemId,
														 StatusName = trtch.TestRunStatusListItem.ListItemSystemName,
														 CompletionRate = "",
														 StartedBy = p.Name,
														 TimeSpent = trh.TotalTimeSpent,
														 ProjectModuleId = trtch.ProjectModuleId,
														 InsertDate = trtch.InsertDate,
														 TestPlanId = trtch.TestPlanId,
														 TestRunTestCaseHistoryId = trtch.TestRunTestCaseHistoryId,
														 TestCaseName = trtch.ProjectModule.ModuleName,
														 TestRunTestCaseStatusListItemId = trtch.TestRunStatusListItemId,
														 TestRunTestCaseStatusListItemSystemName = trtch.TestRunStatusListItem.ListItemSystemName,
														 AssigneeListItemId = trtch.AssigneeProjectMemberId,
														 TotalTimeSpent = trtch.TotalTimeSpent,
													 });






					var res = getTestRunDetailByIdAsync.GroupBy(x => new { x.TestRunId, x.TestPlanId, x.ProjectModuleId }).Select(x => new
					{
						TestRunId = x.OrderByDescending(x => x.InsertDate).Select(x => x.TestRunId).FirstOrDefault(),
						TestPlanId = x.OrderByDescending(x => x.InsertDate).Select(x => x.TestPlanId).FirstOrDefault(),
						TestRunName = x.OrderByDescending(x => x.InsertDate).Select(x => x.Name).FirstOrDefault(),
						TestRunDescription = x.OrderByDescending(x => x.InsertDate).Select(x => x.Description).FirstOrDefault(),
						StatusId = x.OrderByDescending(x => x.InsertDate).Select(x => x.StatusId).FirstOrDefault(),
						StatusName = x.OrderByDescending(x => x.InsertDate).Select(x => x.StatusName).FirstOrDefault(),
						StartedBy = x.OrderByDescending(x => x.InsertDate).Select(x => x.StartedBy).FirstOrDefault(),
						TimeSpent = x.OrderByDescending(x => x.InsertDate).Select(x => x.TimeSpent).FirstOrDefault(),
						TotalTimeSpent = x.OrderByDescending(x => x.InsertDate).Select(x => x.TotalTimeSpent).FirstOrDefault(),
						CompletionRate = x.OrderByDescending(x => x.InsertDate).Select(x => x.CompletionRate).FirstOrDefault(),
						ProjectModuleId = x.OrderByDescending(x => x.InsertDate).Select(x => x.ProjectModuleId).FirstOrDefault(),
						InsertDate = x.OrderByDescending(x => x.InsertDate).Select(x => x.InsertDate).FirstOrDefault(),
						TestRunTestCaseHistoryId = x.OrderByDescending(x => x.InsertDate).Select(x => x.TestRunTestCaseHistoryId).FirstOrDefault()

					}).ToList();

					var getTime = res.Where(x => x.TotalTimeSpent != null).Select(x => x.TotalTimeSpent).ToList();

					var totaltime = getTime.Sum();


					var updateTotalTimeSpent = await _context.TestRunHistory.Include(x => x.TestRun).Where(x => x.TestRunId == testRunId).FirstOrDefaultAsync();
					updateTotalTimeSpent.TotalTimeSpent = totaltime == null ? null : totaltime;
					_context.TestRunHistory.Update(updateTotalTimeSpent);
					await _context.SaveChangesAsync();

					var getTestRutnDetailById = res.Select(x => new GetAllTestRunListModel
					{
						TestRunId = x.TestRunId,
						Name = x.TestRunName,
						Description = x.TestRunDescription,
						StartedBy = x.StartedBy,
						TimeSpent = totaltime,
						CompletionRate = x.CompletionRate,
						Status = GetStatus(res.Select(x => x.StatusName).ToList()),

					}).FirstOrDefault();

					if (getTestRutnDetailById != null)
					{
						return Result<GetAllTestRunListModel>.Success(getTestRutnDetailById);
					}
					else
					{
						return Result<GetAllTestRunListModel>.Success(null);
					}

				}
				else
				{
					throw new TestRunIdNotFoundException();
				}

			}
			catch (Exception ex)
			{
				_iLogger.LogError("Could not find the TestRunId. Exception message is: ", ex.Message);
				return Result<GetAllTestRunListModel>.Error(ReturnMessage.TestRunIdDoesnotExists);

			}
		}

		//GetTestRunTestCases
		public async Task<Result<PagedResponsePersonModel<List<TestRunTestCasesModel>>>> GetTestRunTestCasesModelAsync(FilterModel filter, int testRunId)
		{
			try
			{
				var testRunTestCaseDetail = await _context.TestRun.Where(x => x.TestRunId == testRunId).FirstOrDefaultAsync();
				IEnumerable<TestRunTestCasesModel> record;
				List<int> planIds = new List<int>();

				if (testRunTestCaseDetail != null)
				{
					var getTestRunTestCaseDetails = await (from t in _context.TestRunPlanDetail
														   join trp in _context.TestRunPlan on t.TestRunPlanId equals trp.TestRunPlanId
														   join tr in _context.TestRun on trp.TestRunId equals tr.TestRunId
														   where tr.TestRunId == testRunId
														   select new
														   {
															   TestRunId = trp.TestRunId,
															   TestPlanId = trp.TestPlanId,
															   TestRunPlanDetailId = t.TestRunPlanDetailId,
															   TestPlanDetailJson = t.TestPlanDetailJson,
															   TestCaseDetailJson = t.TestCaseDetailJson,
															   TestCaseStepDetailJson = t.TestCaseStepDetailJson,

														   }).ToListAsync();




					var getTestRunTestCase = getTestRunTestCaseDetails.GroupBy(x => x.TestRunPlanDetailId).Select(x => new
					{
						TestRunId = x.Select(x => x.TestRunId).FirstOrDefault(),
						TestPlanId = x.Select(x => x.TestPlanId).FirstOrDefault(),
						TestPlanDetailJsons = JsonSerializer.Deserialize<List<TestPlanJson>>(x.Select(x => x.TestPlanDetailJson).FirstOrDefault()),
						TestCaseDetailJsons = JsonSerializer.Deserialize<List<TestPlanTestCaseJson>>(x.Select(x => x.TestCaseDetailJson).FirstOrDefault()),
						TestCaseStepDetailJsons = JsonSerializer.Deserialize<List<TestCaseStepDetailsJson>>(x.Select(x => x.TestCaseStepDetailJson).FirstOrDefault()),

					}).ToList();

					if (getTestRunTestCase.Count > 0)
					{
						foreach (var item in getTestRunTestCase)
						{
							if (item != null)
							{
								foreach (var item2 in item.TestPlanDetailJsons)
								{
									planIds.Add(item2.TestPlanId);
								}
							}
						}
					}

					var result = new List<TestRunTestCasesModel>();



					var query = (from trtch in _context.TestRunTestCaseHistory
								 join trh in _context.TestRunHistory on trtch.TestRunHistoryId equals trh.TestRunHistoryId
								 join tr in _context.TestRun on trh.TestRunId equals tr.TestRunId
								 join trp in _context.TestRunPlan on tr.TestRunId equals trp.TestRunId
								 join tp in _context.TestPlan on trp.TestPlanId equals tp.TestPlanId
								 join pm in _context.ProjectModule on trtch.ProjectModuleId equals pm.ProjectModuleId
								 where tr.TestRunId == testRunId
								 select new
								 {
									 TestRunTestCaseHistoryId = trtch.TestRunTestCaseHistoryId,
									 Status = trtch.TestRunStatusListItemId,
									 StatusName = trtch.TestRunStatusListItem.ListItemSystemName,
									 ProjectModuleId = trtch.ProjectModuleId,
									 ProjectMemberAsssignee = trtch.AssigneeProjectMemberId,
									 TestPlanId = trtch.TestPlanId,
									 TestRunHistoryId = trtch.TestRunHistoryId,

									 TestRunId = tr.TestRunId,
									 Timespent = trtch.TotalTimeSpent,
									 Assignee = trtch.ProjectMember.Person.Email,
									 InsertDate = trtch.InsertDate

								 }).ToList();

					var queryBuffer = query.DistinctBy(x => x.TestRunTestCaseHistoryId).ToList();


					foreach (var testRun in getTestRunTestCase)
					{
						var testPlanDetailList = testRun.TestPlanDetailJsons.Where(x => x.TestRunId == testRunId).Select(a => new TestRunTestCasesModel
						{
							TestPlanId = a.TestPlanId,
							TestPlanTitle = a.TestPlanName,
							TestCases = testRun.TestCaseDetailJsons.OrderByDescending(x => x.InsertDate).Where(x => x.TestPlanId == a.TestPlanId).GroupBy(x => x.ProjectModuleId).Select(a => new TestCases
							{
								TestPlanTestCaseId = a.OrderByDescending(x => x.InsertDate).Select(x => x.TestPlanTestCaseId).FirstOrDefault(),
								ProjectModuleId = a.OrderByDescending(x => x.InsertDate).Select(x => x.ProjectModuleId).FirstOrDefault(),
								TestCaseTitle = a.OrderByDescending(x => x.InsertDate).Select(x => x.ProjectModuleName).FirstOrDefault(),
								TestPlanId = a.OrderByDescending(x => x.InsertDate).Select(x => x.TestPlanId).FirstOrDefault(),

							}).ToList(),
						}).ToList();
						result.AddRange(testPlanDetailList);
					}

					foreach (var item in result)
					{
						foreach (var items in item.TestCases)
						{
							items.TestRunHistoryId = (queryBuffer.OrderByDescending(x => x.InsertDate).Where(x => x.ProjectModuleId == items.ProjectModuleId && x.TestPlanId == items.TestPlanId).Select(x => x.TestRunHistoryId)).FirstOrDefault();

							items.Assignee = (queryBuffer.OrderByDescending(x => x.InsertDate).Where(x => x.ProjectModuleId == items.ProjectModuleId && x.TestPlanId == items.TestPlanId).Select(x => x.Assignee)).FirstOrDefault();
							items.TestRunTestCaseHistoryId = (queryBuffer.OrderByDescending(x => x.InsertDate).Where(x => x.ProjectModuleId == items.ProjectModuleId && x.TestPlanId == items.TestPlanId).Select(x => x.TestRunTestCaseHistoryId)).FirstOrDefault();
							items.Results = (queryBuffer.OrderByDescending(x => x.InsertDate).Where(x => x.ProjectModuleId == items.ProjectModuleId && x.TestPlanId == items.TestPlanId).Select(x => x.StatusName)).FirstOrDefault();
							items.TimeSpent = (queryBuffer.OrderByDescending(x => x.InsertDate).Where(x => x.ProjectModuleId == items.ProjectModuleId && x.TestPlanId == items.TestPlanId).Select(x => x.Timespent)).FirstOrDefault();
							items.InsertDate = (queryBuffer.OrderByDescending(x => x.InsertDate).Where(x => x.ProjectModuleId == items.ProjectModuleId && x.TestPlanId == items.TestPlanId).Select(x => x.InsertDate)).FirstOrDefault();
						}
					}

					planIds = planIds.Distinct().ToList();
					record = result.Where(x => planIds.Contains(x.TestPlanId)).DistinctBy(x => x.TestPlanId).ToList();



					if (!string.IsNullOrEmpty(filter.SearchValue))
					{
						foreach (var item in record)
						{
							if (item.TestCases.Count > 0)
							{
								item.TestCases = item.TestCases.Where(x => x.TestCaseTitle.ToLower().Contains(filter.SearchValue.ToLower()) || item.TestPlanTitle.ToLower().Contains(filter.SearchValue.ToLower())).ToList();
							}
						}
					}

					var filteredData = record.ToList();
					var data = new PagedResponsePersonModel<List<TestRunTestCasesModel>>(filteredData);

					if (filteredData.Count > 0)
					{
						return Result<PagedResponsePersonModel<List<TestRunTestCasesModel>>>.Success(data);
					}
					else
					{
						return Result<PagedResponsePersonModel<List<TestRunTestCasesModel>>>.Success(null);
					}

				}
				else
				{
					throw new TestRunIdNotFoundException();
				}
			}
			catch (Exception ex)
			{
				_iLogger.LogError("Could not find the TestRunId. Exception message is: ", ex.Message);
				return Result<PagedResponsePersonModel<List<TestRunTestCasesModel>>>.Error(ReturnMessage.TestRunIdDoesnotExists);
			}
		}
		public async Task<Result<PagedResponseModel<List<TestRunTestCaseModel>>>> GetTestRunTestPlanByTestRunIdAsync(PaginationFilterModel filter, int testRunId)
		{
			try
			{
				var testRun = await _context.TestRun.Where(x => x.TestRunId == testRunId).AnyAsync();
				if (testRun)
				{

					var getTestRunTestCaseDetails = (from t in _context.TestRunPlanDetail
													 join trp in _context.TestRunPlan on t.TestRunPlanId equals trp.TestRunPlanId
													 where trp.TestRunId == testRunId
													 select new TestRunTestCaseListModel
													 {
														 TestRunId = trp.TestRunId,
														 TestPlanId = trp.TestPlanId,
														 TestRunPlanDetailId = t.TestRunPlanDetailId,
														 TestPlanDetailJson = t.TestPlanDetailJson,
													 });
					List<TestPlanJson> testPlanDetailsList = new List<TestPlanJson>();
					foreach (var itemInGetTestRunTestCaseDetails in getTestRunTestCaseDetails)
					{
						itemInGetTestRunTestCaseDetails.TestPlanDetailJsons = JsonSerializer.Deserialize<List<TestPlanJson>>(itemInGetTestRunTestCaseDetails.TestPlanDetailJson);
						testPlanDetailsList.AddRange(itemInGetTestRunTestCaseDetails.TestPlanDetailJsons);
					}

					var testPlanIds = testPlanDetailsList.Select(x => x.TestPlanId).ToList();
					var testRunTestCaseHistory = (from trtch in _context.TestRunTestCaseHistory
												  join trh in _context.TestRunHistory on trtch.TestRunHistoryId equals trh.TestRunHistoryId
												  join tr in _context.TestRun on trh.TestRunId equals tr.TestRunId
												  where tr.TestRunId == testRunId && testPlanIds.Contains(trtch.TestPlanId)
												  select new TestRunTestCaseHistoryStatusModel()
												  {
													  TestPlanId = trtch.TestPlanId,
													  TestRunId = tr.TestRunId,
													  Status = trtch.TestRunStatusListItemId,
													  ProjectModuleId = trtch.ProjectModuleId,
													  UpdateDate = trtch.UpdateDate,
													  StatusListItemSystemName = trtch.TestRunStatusListItem.ListItemSystemName
												  });


					var testPlanJsons = testPlanDetailsList.Select(x => new TestRunTestCaseModel
					{
						TestRunId = x.TestRunId,
						TestPlanId = x.TestPlanId,
						TestPlanName = x.TestPlanName,
						PassedCount = StatusCountByTestRunId(x.TestRunId, x.TestPlanId, ListItem.Passed.ToString(), testRunTestCaseHistory),
						FailedCount = StatusCountByTestRunId(x.TestRunId, x.TestPlanId, ListItem.Failed.ToString(), testRunTestCaseHistory),
						BlockedCount = StatusCountByTestRunId(x.TestRunId, x.TestPlanId, ListItem.Blocked.ToString(), testRunTestCaseHistory),
						PendingCount = StatusCountByTestRunId(x.TestRunId, x.TestPlanId, ListItem.Pending.ToString(), testRunTestCaseHistory),
					}).OrderBy(x=>x.TestPlanName).AsQueryable();

					if (!string.IsNullOrEmpty(filter.SearchValue))
					{
						testPlanJsons = testPlanJsons.Where(
							x => x.TestPlanName.ToLower().Contains(filter.SearchValue.ToLower())

							);
					}

					var records = testPlanJsons;
					var totalRecords = records.Count();
					var filteredData = testPlanJsons
						.Skip((filter.PageNumber - 1) * filter.PageSize)
						.Take(filter.PageSize)
						.ToList();

					if (filter.PageSize > totalRecords && totalRecords > 0)
					{
						filter.PageSize = totalRecords;
					}
					var totalPages = (totalRecords / filter.PageSize);


					var data = new PagedResponseModel<List<TestRunTestCaseModel>>(filteredData, filter.PageNumber, filter.PageSize, totalRecords, totalPages);

					if (filteredData.Count > 0)
					{
						return Result<PagedResponseModel<List<TestRunTestCaseModel>>>.Success(data);
					}
					else
					{
						return Result<PagedResponseModel<List<TestRunTestCaseModel>>>.Success(null);
					}


				}
				else
				{
					throw new TestRunIdNotFoundException();
				}

			}
			catch (Exception ex)
			{
				_iLogger.LogError("Could not find the TestRunId. Exception message is: ", ex.Message);
				return Result<PagedResponseModel<List<TestRunTestCaseModel>>>.Error(ReturnMessage.TestRunIdDoesnotExists);
			}
		}

		public async Task<Result<PagedResponseModel<List<TestRunTestCaseFilterModel>>>> GetTestRunTestCaseDetailsAsync(PaginationFilterModel filter, int testRunId, int testPlanId, int? projectMemeberId)
		{
			try
			{
				var testRun = await _context.TestRun.Where(x => x.TestRunId == testRunId).AnyAsync();
				if (testRun)
				{
					var getTestRunTestCaseDetails = (from t in _context.TestRunPlanDetail
													 join trp in _context.TestRunPlan on t.TestRunPlanId equals trp.TestRunPlanId
													 where trp.TestRunId == testRunId && trp.TestPlanId == testPlanId
													 orderby t.UpdateDate descending
													 select new TestRunTestCaseDetailListModel
													 {
														 TestRunId = trp.TestRunId,
														 TestPlanId = trp.TestPlanId,
														 TestRunPlanDetailId = t.TestRunPlanDetailId,
														 TestCaseDetailJson = t.TestCaseDetailJson,

													 });

					List<TestPlanTestCaseJson> testPlanTestCaseDetailsList = new List<TestPlanTestCaseJson>();
					List<TestRunTestCaseFilterModel> testRunTestCaseResult = new List<TestRunTestCaseFilterModel>();

					foreach (var itemInGetTestRunTestCaseDetails in getTestRunTestCaseDetails)
					{
						itemInGetTestRunTestCaseDetails.TestCaseDetailJsons = JsonSerializer.Deserialize<List<TestPlanTestCaseJson>>(itemInGetTestRunTestCaseDetails.TestCaseDetailJson);
						testPlanTestCaseDetailsList.AddRange(itemInGetTestRunTestCaseDetails.TestCaseDetailJsons);
					}

					var testRunTestCase = (from trtc in _context.TestRunTestCaseHistory
										   join trh in _context.TestRunHistory on trtc.TestRunHistoryId equals trh.TestRunHistoryId
										   join tr in _context.TestRun on trh.TestRunId equals tr.TestRunId
										   where tr.TestRunId == testRunId && trtc.TestPlanId == testPlanId

										   select new TestRunTestCaseFilterModel
										   {
											   TestRunTestCaseHistoryId = trtc.TestRunTestCaseHistoryId,
											   TestPlanId = trtc.TestPlanId,
											   TestRunId = tr.TestRunId,
											   ProjectModuleId = trtc.ProjectModuleId,
											   TotalTimeSpent = trtc.TotalTimeSpent,
											   TestRunTestCaseStatusListItemId = trtc.TestRunStatusListItemId,
											   TestRunTestCaseStatusListItemSystemName = trtc.TestRunStatusListItem.ListItemName,
											   AssigneeListItemId = trtc.AssigneeProjectMemberId,
											   Assignee = trtc.ProjectMember.Person.Email,
											   InsertDate = trtc.UpdateDate

										   }).AsQueryable();



					foreach (var itemInTestRunTestCaseResult in testRunTestCase)
					{
						itemInTestRunTestCaseResult.TestCaseName = (from tptcd in testPlanTestCaseDetailsList
																	orderby tptcd.InsertDate descending
																	where tptcd.ProjectModuleId == itemInTestRunTestCaseResult.ProjectModuleId
																	select tptcd.ProjectModuleName).FirstOrDefault();

						testRunTestCaseResult.Add(itemInTestRunTestCaseResult);

					}

					var result = testRunTestCaseResult.OrderByDescending(x => x.InsertDate).GroupBy(x => x.ProjectModuleId).Select(x => new TestRunTestCaseFilterModel
					{
						TestRunTestCaseHistoryId = x.OrderByDescending(x => x.InsertDate).Select(x => x.TestRunTestCaseHistoryId).FirstOrDefault(),
						TestRunId = x.OrderByDescending(x => x.InsertDate).Select(x => x.TestRunId).FirstOrDefault(),
						TestPlanId = x.OrderByDescending(x => x.InsertDate).Select(x => x.TestPlanId).FirstOrDefault(),
						TestCaseName = x.OrderByDescending(x => x.InsertDate).Select(x => x.TestCaseName).FirstOrDefault(),
						TotalTimeSpent = x.OrderByDescending(x => x.InsertDate).Select(x => x.TotalTimeSpent).FirstOrDefault(),
						ProjectModuleId = x.OrderByDescending(x => x.InsertDate).Select(x => x.ProjectModuleId).FirstOrDefault(),
						TestRunTestCaseStatusListItemId = x.OrderByDescending(x => x.InsertDate).Select(x => x.TestRunTestCaseStatusListItemId).FirstOrDefault(),
						TestRunTestCaseStatusListItemSystemName = x.OrderByDescending(x => x.InsertDate).Select(x => x.TestRunTestCaseStatusListItemSystemName).FirstOrDefault(),
						AssigneeListItemId = x.OrderByDescending(x => x.InsertDate).Select(x => x.AssigneeListItemId).FirstOrDefault(),
						Assignee = x.OrderByDescending(x => x.InsertDate).Select(x => x.Assignee).FirstOrDefault(),
						InsertDate = x.OrderByDescending(x => x.InsertDate).Select(x => x.InsertDate).FirstOrDefault()

					}).OrderBy(x => x.TestCaseName).AsQueryable();


					if (projectMemeberId > 0)
					{
						result = from res in result
								 where res.AssigneeListItemId == projectMemeberId
								 select res;
					}

					if (!string.IsNullOrEmpty(filter.SearchValue))
					{
						result = result.Where(x => (x.TestCaseName.ToLower().Contains(filter.SearchValue.ToLower())));

					}


					var records = result;
					var totalRecords = records.Count();
					var filteredData = result
							.Skip((filter.PageNumber - 1) * filter.PageSize)
							.Take(filter.PageSize)
							.ToList();

					if (filter.PageSize > totalRecords && totalRecords > 0)
					{
						filter.PageSize = totalRecords;
					}
					var totalPages = (totalRecords / filter.PageSize);
					var data = new PagedResponseModel<List<TestRunTestCaseFilterModel>>(filteredData, filter.PageNumber, filter.PageSize, totalRecords, totalPages);

					if (filteredData.Count > 0)
					{
						return Result<PagedResponseModel<List<TestRunTestCaseFilterModel>>>.Success(data);
					}
					else
					{
						return Result<PagedResponseModel<List<TestRunTestCaseFilterModel>>>.Success(null);
					}

				}
				else
				{
					throw new TestRunIdNotFoundException();
				}


			}
			catch (Exception ex)
			{
				if (ex is TestRunIdNotFoundException)
				{
					_iLogger.LogError("Could not find the TestRunId. Exception message is: ", ex.Message);
					return Result<PagedResponseModel<List<TestRunTestCaseFilterModel>>>.Error(ReturnMessage.TestRunIdDoesnotExists);
				}
				_iLogger.LogError("Could not find the test case with given testrunid and testplanid. Exception message is: ", ex.Message);
				return Result<PagedResponseModel<List<TestRunTestCaseFilterModel>>>.Error(ReturnMessage.FailedToGetTestCaseWithTestRunIdAndTestPlanId);
			}
		}

		//GetTestRunTeamStats
		public async Task<Result<List<TestRunTeamStatsModel>>> GetTestRunTeamStatsModelAsync(int testRunId)
		{
			try
			{
				var testRunDetail = await _context.TestRun.Where(x => x.TestRunId == testRunId).FirstOrDefaultAsync();
				if (testRunDetail != null)
				{
					var getTestRunTeamStats = await
							(from tr in _context.TestRun
							 join p in _context.Project on tr.ProjectId equals p.ProjectId
							 join pm in _context.ProjectMember on p.ProjectId equals pm.ProjectId
							 join per in _context.Person on pm.PersonId equals per.PersonId
							 join trh in _context.TestRunHistory on tr.TestRunId equals trh.TestRunId
							 join trtch in _context.TestRunTestCaseHistory on trh.TestRunHistoryId equals trtch.TestRunHistoryId
							 join li in _context.ListItem on trtch.TestRunStatusListItemId equals li.ListItemId
							 join lic in _context.ListItemCategory on li.ListItemCategoryId equals lic.ListItemCategoryId
							 where tr.TestRunId == testRunId && lic.ListItemCategoryName == ListItemCategory.TestRunStatus.ToString()
							 select new
							 {
								 TestRunId = tr.TestRunId,
								 TimeSpent = trtch.TotalTimeSpent,
								 UserId = per.PersonId == null ? 0 : per.PersonId,
								 Username = per.UserName == null ? string.Empty : per.UserName,
								 Role = per.UserRoleListItemId == null ? 0 : per.UserRoleListItemId,
								 RoleName = per.UserRoleListItem.ListItemSystemName == null ? string.Empty : per.UserRoleListItem.ListItemSystemName,
								 Email = per.Email == null ? string.Empty : per.Email,
								 StatusId = li.ListItemId,
								 StatusName = li.ListItemSystemName,
								 ProjectMemberId = trtch.AssigneeProjectMemberId == null ? 0 : trtch.AssigneeProjectMemberId,
								 Person = trtch.ProjectMember.Person.Name == null ? string.Empty : trtch.ProjectMember.Person.Name,
								 PersonId = trtch.ProjectMember.Person.PersonId == null ? 0 : trtch.ProjectMember.Person.PersonId,
								 TestPlanId = trtch.TestPlanId,
								 ProjectModuleId = trtch.ProjectModuleId,
								 InsertDate = trtch.InsertDate,
								 TestRunTestCaseHistoryId = trtch.TestRunTestCaseHistoryId
							 }).ToListAsync();



					var status = getTestRunTeamStats.OrderByDescending(x => x.InsertDate).GroupBy(x => new { x.TestPlanId, x.ProjectModuleId }).Select(x => new
					{
						TestPlanId = x.Key.TestPlanId,
						ProjectModuleId = x.Key.ProjectModuleId,
						TestRunTestCaseHsitoryId = x.Select(x => x.TestRunTestCaseHistoryId).FirstOrDefault(),
						AsssigneePerson = x.Select(x => x.Person).FirstOrDefault(),
						PersonId = x.Select(x => x.PersonId).FirstOrDefault(),
						UserId = x.Select(x => x.UserId).FirstOrDefault(),
						StatusName = x.Select(x => x.StatusName).FirstOrDefault(),
						TimeSpent = x.Select(x => x.TimeSpent).FirstOrDefault(),

					}).ToList();

					var timeSpent = status.GroupBy(x => x.PersonId).Select(x => new
					{
						PersonId = x.Key,
						TimeSpent = x.Select(x => x.TimeSpent).ToList().Sum(),
					});

					var getTestRunTeamStatsAsync = getTestRunTeamStats.GroupBy(a => a.UserId).Select(x => new TestRunTeamStatsModel
					{
						Id = testRunId,
						TimeSpent = timeSpent.Where(y => y.PersonId == x.Key).Select(y => y.TimeSpent).FirstOrDefault(),
						User = new UserModel
						{
							UserId = x.Key,
							Name = x.Select(y => y.Username).FirstOrDefault(),
							Email = x.Select(y => y.Email).FirstOrDefault(),
							Image = null,
							Role = x.Select(y => y.Role).FirstOrDefault(),
							RoleName = x.Select(y => y.RoleName).FirstOrDefault(),
							DefaultAssignee = x.Select(x => x.ProjectMemberId).FirstOrDefault(),
							DefaultAssigneeName = x.Select(x => x.Person).FirstOrDefault(),
						},
						Status = GetStatus(status.Where(y => y.PersonId == x.Key).Select(y => y.StatusName).ToList())
					}).ToList();

					if (getTestRunTeamStatsAsync != null)
					{
						return Result<List<TestRunTeamStatsModel>>.Success(getTestRunTeamStatsAsync);
					}
					else
					{
						return Result<List<TestRunTeamStatsModel>>.Success(null);
					}
				}
				else
				{
					throw new TestRunIdNotFoundException();
				}
			}
			catch (Exception ex)
			{
				_iLogger.LogError("Could not find the TestRunId. Exception message is: ", ex.Message);
				return Result<List<TestRunTeamStatsModel>>.Error(ReturnMessage.TestRunIdDoesnotExists);

			}
		}

		//GetTestCaseResultsData
		public async Task<Result<TestCaseResultsDataModel>> GetTestCaseResultsDataModelAsync(int testRunId, int testCaseId, int testPlanId)
		{
			try
			{
				var testPlanTestCaseIdStatusData = await (from t in _context.TestRunPlanDetail
														  join trp in _context.TestRunPlan on t.TestRunPlanId equals trp.TestRunPlanId
														  join tr in _context.TestRun on trp.TestRunId equals tr.TestRunId
														  where tr.TestRunId == testRunId && trp.TestPlanId == testPlanId
														  select new
														  {
															  TestRunId = trp.TestRunId,
															  TestPlanId = trp.TestPlanId,
															  TestPlanDetailJson = t.TestPlanDetailJson,
															  TestCaseDetailJson = t.TestCaseDetailJson,
															  TestCaseStepDetailJson = t.TestCaseStepDetailJson,
															  TestRunPlanId = trp.TestRunPlanId,
															  TestRunPlanDetailId = t.TestRunPlanDetailId
														  }).ToListAsync();


				var testPlanTestCaseIdStatusDataResult = testPlanTestCaseIdStatusData.Where(x => x.TestPlanId == testPlanId).Select(x => new
				{
					TestRunId = x.TestRunId,
					TestPlanId = x.TestPlanId,
					TestRunPlanId = x.TestRunPlanId,
					TestRunPlanDetailId = x.TestRunPlanDetailId,
					TestPlanDetailJsons = JsonSerializer.Deserialize<List<TestPlanJson>>(x.TestPlanDetailJson),
					TestCaseDetailJsons = JsonSerializer.Deserialize<List<TestPlanTestCaseJson>>(x.TestCaseDetailJson),
					TestCaseStepDetailJsons = JsonSerializer.Deserialize<List<TestCaseStepDetailsJson>>(x.TestCaseStepDetailJson),

				}).ToList();

				var testCaseQuery = (from trtch in _context.TestRunTestCaseHistory
									 join trh in _context.TestRunHistory on trtch.TestRunHistoryId equals trh.TestRunHistoryId
									 join tr in _context.TestRun on trh.TestRunId equals tr.TestRunId
									 join trp in _context.TestRunPlan on tr.TestRunId equals trp.TestRunId
									 join trpd in _context.TestRunPlanDetail on trp.TestRunPlanId equals trpd.TestRunPlanId
									 join trtchd in _context.TestRunTestCaseHistoryDocument on trtch.TestRunTestCaseHistoryId equals trtchd.TestRunTestCaseHistoryId into env
									 from trtchd in env.DefaultIfEmpty()
									 where trtch.TestRunHistory.TestRun.TestRunId == testRunId && trtch.TestPlanId == testPlanId && trtch.ProjectModuleId == testCaseId
									 orderby trtchd.UpdateDate descending
									 select new
									 {

										 ProjectModuleId = trtch.ProjectModuleId,
										 TestRunTestCaseHistoryId = trtch.TestRunTestCaseHistoryId,
										 StatusId = trtch.TestRunStatusListItemId,
										 Status = trtch.TestRunStatusListItem.ListItemSystemName,
										 Assignee = trtch.ProjectMember == null ? 0 : trtch.ProjectMember.Person.PersonId,
										 Assigneename = trtch.ProjectMember.Person.Email == null ? string.Empty : trtch.ProjectMember.Person.Email,
										 Timespent = trtch.TotalTimeSpent,
										 FinishTime = trtch.EndTime,
										 TestRunTestCaseHistoryDocumentId = trtchd.TestRunTestCaseHistoryDocumentId == null ? 0 : trtchd.TestRunTestCaseHistoryDocumentId,
										 DocumentId = trtchd.DocumentId == null ? 0 : trtchd.DocumentId,
										 FileName = trtchd.ProjectMember.Name == null ? string.Empty : trtchd.ProjectMember.Name,
										 Comment = trtchd.Comment == null ? string.Empty : trtchd.Comment,
										 UpdateDate = trtch.UpdateDate

									 }).ToList();



				var getTestRunTestCaseWizardDetail = await (from trtch in _context.TestRunTestCaseHistory
															join trtcsh in _context.TestRunTestCaseStepHistory on trtch.TestRunTestCaseHistoryId equals trtcsh.TestRunTestCaseHistoryId
															join trh in _context.TestRunHistory on trtch.TestRunHistoryId equals trh.TestRunHistoryId
															join tr in _context.TestRun on trh.TestRunId equals tr.TestRunId
															join tcsd in _context.TestCaseStepDetail on trtcsh.TestCaseStepDetailId equals tcsd.TestCaseStepDetailId
															join tcd in _context.TestCaseDetail on trtch.ProjectModuleId equals tcd.ProjectModuleId
															where tr.TestRunId == testRunId && trtch.ProjectModuleId == testCaseId && trtch.TestPlanId == testPlanId

															select new GetTestRunTestCaseWizardDetail()
															{
																TestRunHistoryId = trh.TestRunHistoryId,
																EnvironmentId = tr.EnvironmentId == null ? null : tr.EnvironmentId,
																Environment = tr.Environment.EnvironmentName == null ? string.Empty : tr.Environment.EnvironmentName,
																TestRunStatusListItemId = trtch.TestRunStatusListItemId,
																TestRunTestCaseStatus = trtch.TestRunStatusListItem.ListItemSystemName,
																TimeSpent = trtch.TotalTimeSpent,
																TestRunNameTitle = tr.Title,
																InsertDate = trtch.InsertDate,
																TestRunTestCaseHistoryId = trtch.TestRunTestCaseHistoryId,
																ProjectModuleId = trtch.ProjectModuleId,
																TestRunTestCaseStepHistoryId = trtcsh.TestRunTestCaseStepHistoryId,
																TestCaseStepDetailId = trtcsh.TestCaseStepDetailId,
																TestRunTestCaseStepHistoryStatusId = trtcsh.TestRunStatusListItemId,
																TestRunTestCaseStepHistoryStatus = trtcsh.TestRunStatusListItem.ListItemSystemName,
																TestRunId = tr.TestRunId,
																TestPlanId = trtch.TestPlanId,
																TestCaseId = trtch.ProjectModuleId,
																TestCaseDetailId = tcd.TestCaseDetailId,
															}).ToListAsync();


				var testRunTestCaseStepHistoryList = getTestRunTestCaseWizardDetail.Select(x => x.TestRunTestCaseStepHistoryId).ToList();
				var testRunTestCaseStepHistoryDocumentId = await _context.TestRunTestCaseStepHistoryDocument.Where(x => testRunTestCaseStepHistoryList.Contains(x.TestRunTestCaseStepHistoryId)).ToListAsync();

				var document = await _context.Document.Select(x => new DocumentFileModel
				{
					DocumentId = x.DocumentId,
					FileName = x.Name,
					Extension = x.Extension,
					UpdateDate = x.UpdateDate

				}).ToListAsync();

				TestCaseResultsDataModel testCaseDetailList = new TestCaseResultsDataModel();

				foreach (var item in testPlanTestCaseIdStatusDataResult)
				{
					testCaseDetailList = item.TestCaseDetailJsons.Where(x => x.TestRunId == testRunId && x.ProjectModuleId == testCaseId && x.TestPlanId == testPlanId)
					   .Select(a => new TestCaseResultsDataModel
					   {
						   Id = a.ProjectModuleId,
						   TestCaseTitle = a.ProjectModuleName,

						   UpdateDate = a.UpdateDate,
						   Results = testCaseQuery.Where(x => x.ProjectModuleId == a.ProjectModuleId).Select(x => new TestCaseResult
						   {
							   UserId = x.Assignee,
							   User = x.Assigneename,
							   Status = x.Status,
							   TimeSpent = x.Timespent,
							   FinishTime = x.FinishTime,
							   Comment = x.Comment,
							   TestRunTestCaseHistoryId = x.TestRunTestCaseHistoryId,
							   TestRunTestCaseHistoryDocumentId = x.TestRunTestCaseHistoryDocumentId,
							   FileName = x.FileName,
							   DocumentId = x.DocumentId,
							   UpdateDate = x.UpdateDate,

							   StepsToReproduce = item.TestCaseStepDetailJsons.Where(y => y.TestRunTestCaseHistoryId == x.TestRunTestCaseHistoryId).Select(y => new TestCaseStepsResult
							   {
								   TestRunTestCaseStepHistoryId = y.TestRunTestCaseStepHistoryId,
								   ExpectedResult = y.ExpectedResult,
								   Step = y.StepDescription,
								   TestCaseStepResultId = y.TestCaseStepDetailId,
							   }).DistinctBy(x => x.TestRunTestCaseStepHistoryId).ToList(),

						   }).OrderByDescending(x => x.UpdateDate).DistinctBy(x => x.TestRunTestCaseHistoryId).ToList(),

					   }).OrderByDescending(x=>x.UpdateDate).FirstOrDefault();

				}
				List<int> ids = new List<int>();

				foreach (var tcd in testCaseDetailList.Results)
				{
					foreach (var res in tcd.StepsToReproduce)
					{
						var testRunTestCaseDetailIdsResult = res.TestCaseStepResultId;
						ids.Add(testRunTestCaseDetailIdsResult);
					}
				}

				List<int> resultIds = ids.ToList();
				var testRunTestCaseStepHistoryIds = await (from trtcsh in _context.TestRunTestCaseStepHistory
														   join li in _context.ListItem on trtcsh.TestRunStatusListItemId equals li.ListItemId
														   where resultIds.Contains(trtcsh.TestCaseStepDetailId)
														   && trtcsh.TestRunTestCaseHistoryId == testCaseDetailList.Results.Select(x => x.TestRunTestCaseHistoryId).FirstOrDefault()
														   select new TestRunTestCaseHistoryModelWizard()
														   {
															   TestRunTestCaseStepHistoryId = trtcsh.TestRunTestCaseStepHistoryId,
															   TestCaseStepDetailId = trtcsh.TestCaseStepDetailId,
															   TestRunTestCaseHistoryId = trtcsh.TestRunTestCaseHistoryId,
															   TestRunTestCaseHistoryStatus = li.ListItemSystemName
														   }
													   ).ToListAsync();

				foreach (var item in testCaseDetailList.Results)
				{
					foreach (var items in item.StepsToReproduce)
					{

						items.DocumentId = testRunTestCaseStepHistoryDocumentId.Where(x => x.TestRunTestCaseStepHistoryId == items.TestRunTestCaseStepHistoryId).OrderByDescending(x => x.UpdateDate).Select(x => x.DocumentId).FirstOrDefault();

						items.FileName = document.Where(z => z.DocumentId == items.DocumentId).OrderByDescending(z => z.UpdateDate).Select(z => z.FileName).FirstOrDefault();

						items.Status = testRunTestCaseStepHistoryIds.Where(x => x.TestCaseStepDetailId == items.TestCaseStepResultId && x.TestRunTestCaseHistoryId == item.TestRunTestCaseHistoryId).Select(x => x.TestRunTestCaseHistoryStatus).FirstOrDefault();
						items.Comment = testRunTestCaseStepHistoryDocumentId.Where(x => x.TestRunTestCaseStepHistoryId == items.TestRunTestCaseStepHistoryId).OrderByDescending(x => x.UpdateDate).Select(x => x.Comment).FirstOrDefault();

					}

				}

				var result = testCaseDetailList;

				return Result<TestCaseResultsDataModel>.Success(result);

			}
			catch (Exception ex)
			{
				_iLogger.LogError("Could not find the TestPlanTestCaseId. Exception message is: ", ex.Message);
				return Result<TestCaseResultsDataModel>.Error(ReturnMessage.TestPlanTestCaseIdDoesnotExists);
			}
		}


		//GetEditTestRunById
		public async Task<Result<GetTestRunEdit>> GetEditTestRunByIdAsync(int testRunId)
		{
			try
			{
				var getTestRunDetail = _context.TestRun.Where(x => x.TestRunId == testRunId).FirstOrDefault();

				var test = _context.TestRunPlan.Where(x => x.TestRunId == testRunId).ToList();
				List<int> testPlanIds = test.Select(x => x.TestPlanId).ToList();

				if (getTestRunDetail != null)
				{
					var testRunDetail = await (from tr in _context.TestRun
											   join trp in _context.TestRunPlan on tr.TestRunId equals trp.TestRunId
											   join en in _context.Environment on tr.EnvironmentId equals en.EnvironmentId into environment
											   from en in environment.DefaultIfEmpty()
											   join pm in _context.ProjectMember on tr.DefaultAssigneeProjectMemberId equals pm.ProjectMemberId into assignee
											   from pm in assignee.DefaultIfEmpty()
											   where tr.TestRunId == testRunId
											   select new GetTestRunEdit
											   {
												   TestRunId = tr.TestRunId,
												   Title = tr.Title,
												   Description = tr.Description,
												   AssigneeId = tr.DefaultAssigneeProjectMemberId ?? null,
												   EnvironmentId = tr.EnvironmentId == null ? null : en.EnvironmentId,
												   Environment = en.EnvironmentName ?? String.Empty,
												   TestPlanId = testPlanIds,
											   }).FirstOrDefaultAsync();

					if (testRunDetail != null)
					{
						return Result<GetTestRunEdit>.Success(testRunDetail);
					}
					else
					{
						return Result<GetTestRunEdit>.Success(null);
					}
				}
				else
				{
					throw new TestRunIdNotFoundException();
				}

			}
			catch (Exception ex)
			{

				_iLogger.LogError("Could not find the TestRunId. Exception message is: ", ex.Message);
				return Result<GetTestRunEdit>.Error(ReturnMessage.TestRunIdDoesnotExists);
			}
		}

		public async Task<Result<TestRunTestCaseWizardModel>> GetTestRunTestCaseWizardAsync(int testRunId, int testCaseId, int testPlanId)
		{
			try
			{
				var getTestRunTestCaseDetailJson = await (from t in _context.TestRunPlanDetail
														  join trp in _context.TestRunPlan on t.TestRunPlanId equals trp.TestRunPlanId
														  join tr in _context.TestRun on trp.TestRunId equals tr.TestRunId
														  where trp.TestRunId == testRunId && trp.TestPlanId == testPlanId
														  select new GetTestRunTestCaseDetailJsonModel()
														  {
															  TestRunId = trp.TestRunId,
															  TestPlanId = trp.TestPlanId,
															  TestRunPlanDetailId = t.TestRunPlanDetailId,
															  TestPlanDetailJson = t.TestPlanDetailJson,
															  TestCaseDetailJson = t.TestCaseDetailJson,
															  TestCaseStepDetailJson = t.TestCaseStepDetailJson,

														  }).FirstOrDefaultAsync();

				getTestRunTestCaseDetailJson.TestPlanDetailJsons = JsonSerializer.Deserialize<List<TestPlanJson>>(getTestRunTestCaseDetailJson.TestPlanDetailJson);
				getTestRunTestCaseDetailJson.TestCaseDetailJsons = JsonSerializer.Deserialize<List<TestPlanTestCaseJson>>(getTestRunTestCaseDetailJson.TestCaseDetailJson);
				getTestRunTestCaseDetailJson.TestCaseStepDetailJsons = JsonSerializer.Deserialize<List<TestCaseStepDetailsJson>>(getTestRunTestCaseDetailJson.TestCaseStepDetailJson);


				var getTestRunTestCaseWizardDetail = await (from trtch in _context.TestRunTestCaseHistory
															join trtcsh in _context.TestRunTestCaseStepHistory on trtch.TestRunTestCaseHistoryId equals trtcsh.TestRunTestCaseHistoryId
															join trh in _context.TestRunHistory on trtch.TestRunHistoryId equals trh.TestRunHistoryId
															join tr in _context.TestRun on trh.TestRunId equals tr.TestRunId
															join tcsd in _context.TestCaseStepDetail on trtcsh.TestCaseStepDetailId equals tcsd.TestCaseStepDetailId
															join tcd in _context.TestCaseDetail on trtch.ProjectModuleId equals tcd.ProjectModuleId
															where trtch.TestRunHistory.TestRunId == testRunId && trtch.ProjectModuleId == testCaseId && trtch.TestPlanId == testPlanId

															select new GetTestRunTestCaseWizardDetail()
															{
																TestRunHistoryId = trh.TestRunHistoryId,
																EnvironmentId = tr.EnvironmentId == null ? null : tr.EnvironmentId,
																Environment = tr.Environment.EnvironmentName == null ? string.Empty : tr.Environment.EnvironmentName,
																TestRunStatusListItemId = trtch.TestRunStatusListItemId,
																TestRunTestCaseStatus = trtch.TestRunStatusListItem.ListItemSystemName,
																TimeSpent = trtch.TotalTimeSpent,
																TestRunNameTitle = tr.Title,
																InsertDate = trtch.InsertDate,
																TestRunTestCaseHistoryId = trtch.TestRunTestCaseHistoryId,
																ProjectModuleId = trtch.ProjectModuleId,
																TestRunTestCaseStepHistoryId = trtcsh.TestRunTestCaseStepHistoryId,
																TestCaseStepDetailId = trtcsh.TestCaseStepDetailId,
																TestRunTestCaseStepHistoryStatusId = trtcsh.TestRunStatusListItemId,
																TestRunTestCaseStepHistoryStatus = trtcsh.TestRunStatusListItem.ListItemSystemName,
																TestRunId = tr.TestRunId,
																TestPlanId = trtch.TestPlanId,
																TestCaseId = trtch.ProjectModuleId,
																TestCaseDetailId = tcd.TestCaseDetailId,
																UpdateDate = trtch.UpdateDate
															}).ToListAsync();
				var testRunTestCaseHistoryList = getTestRunTestCaseWizardDetail.OrderByDescending(x=>x.UpdateDate).Select(x => x.TestRunTestCaseHistoryId).FirstOrDefault();
				var testRunTestCaseStepHistoryList = getTestRunTestCaseWizardDetail.Select(x => x.TestRunTestCaseStepHistoryId).ToList();

				var testRunTestCaseHistoryDocumentId = await _context.TestRunTestCaseHistoryDocument.Where(x => x.TestRunTestCaseHistoryId == testRunTestCaseHistoryList).ToListAsync();
				var testRunTestCaseStepHistoryDocumentId = await _context.TestRunTestCaseStepHistoryDocument.Where(x => testRunTestCaseStepHistoryList.Contains(x.TestRunTestCaseStepHistoryId)).ToListAsync();

				var getTestRunTestCaseWizardDetailResult = getTestRunTestCaseWizardDetail.GroupBy(x => new { x.TestCaseId }).Select(x => new GetTestRunTestCaseWizardDetailResult
				{
					TestPlanId = x.Select(x => x.TestPlanId).FirstOrDefault(),
					TestRunId = x.Select(x => x.TestRunId).FirstOrDefault(),
					TestCaseId = x.Select(x => x.TestCaseId).FirstOrDefault(),
					TestCaseDetailId = x.Select(x => x.TestCaseDetailId).FirstOrDefault(),
					TimeSpent = x.Select(x => x.TimeSpent).FirstOrDefault(),
					TestRunTestCaseHistoryId = x.OrderByDescending(x => x.InsertDate).Select(x => x.TestRunTestCaseHistoryId).FirstOrDefault(),
					TestRunStatusListItemId = x.OrderByDescending(x => x.InsertDate).Select(x => x.TestRunStatusListItemId).FirstOrDefault(),
					TestRunTestCaseStatus = x.OrderByDescending(x => x.InsertDate).Select(x => x.TestRunTestCaseStatus).FirstOrDefault(),
					Environment = x.Select(x => x.Environment).FirstOrDefault(),
					EnvironmentId = x.Select(x => x.EnvironmentId).FirstOrDefault(),
					TestCaseStepDetailId = x.Select(x => x.TestCaseStepDetailId).Distinct().ToList(),
					TestRunTestCaseStepHistoryId = x.Select(x => x.TestRunTestCaseStepHistoryId).Distinct().ToList(),
					TestRunTestCaseStepHistoryStatus = x.Select(x => x.TestRunTestCaseStepHistoryStatus).FirstOrDefault(),
					ProjectModuleId = x.Select(x => x.ProjectModuleId).FirstOrDefault(),

				}).FirstOrDefault();

				var testRunTestCaseWizardList = getTestRunTestCaseDetailJson.TestCaseDetailJsons.OrderByDescending(x => x.InsertDate).GroupBy(x => new { getTestRunTestCaseWizardDetailResult.TestRunTestCaseHistoryId }).Where(x => x.Key.TestRunTestCaseHistoryId == getTestRunTestCaseWizardDetailResult.TestRunTestCaseHistoryId).Select(a => new TestRunTestCaseWizardModel
				{
					TestPlanId = getTestRunTestCaseWizardDetailResult.TestPlanId,
					TestRunTestCaseHistoryDocumentId = testRunTestCaseHistoryDocumentId.OrderByDescending(x => x.UpdateDate).Select(x => x.TestRunTestCaseHistoryDocumentId).FirstOrDefault(),
					HistoryDocumentId = testRunTestCaseHistoryDocumentId.OrderByDescending(x => x.UpdateDate).Select(x => x.DocumentId).FirstOrDefault(),
					TestRunTestCaseHistoryId = getTestRunTestCaseWizardDetailResult.TestRunTestCaseHistoryId,
					TestCaseName = getTestRunTestCaseDetailJson.TestCaseDetailJsons.OrderByDescending(x => x.InsertDate).Where(x => x.TestPlanId == testPlanId && x.ProjectModuleId == testCaseId && x.TestRunId == testRunId).Select(x => x.ProjectModuleName).FirstOrDefault(),
					TestCaseScenario = getTestRunTestCaseDetailJson.TestCaseDetailJsons.OrderByDescending(x => x.InsertDate).Where(x => x.TestPlanId == testPlanId && x.ProjectModuleId == testCaseId && x.TestRunId == testRunId).Select(x => x.ProjectModuleDescription).FirstOrDefault(),
					PreConditions = getTestRunTestCaseDetailJson.TestCaseDetailJsons.OrderByDescending(x => x.InsertDate).Where(x => x.TestPlanId == testPlanId && x.ProjectModuleId == testCaseId && x.TestRunId == testRunId).Select(x => x.PreCondition).FirstOrDefault(),
					ExpectedResult = getTestRunTestCaseDetailJson.TestCaseDetailJsons.OrderByDescending(x => x.InsertDate).Where(x => x.TestPlanId == testPlanId && x.ProjectModuleId == testCaseId && x.TestRunId == testRunId).Select(x => x.ExpectedResult).FirstOrDefault(),
					TestCaseDetail = a.Select(a => a.TestCaseDetailId).FirstOrDefault(),
					TestRunStatusListItemId = getTestRunTestCaseWizardDetailResult.TestRunStatusListItemId,
					Status = getTestRunTestCaseWizardDetailResult.TestRunTestCaseStatus,
					Environment = getTestRunTestCaseWizardDetailResult.Environment,
					EnvironmentId = getTestRunTestCaseWizardDetailResult.EnvironmentId,
					Comment = testRunTestCaseHistoryDocumentId.Where(x=>x.TestRunTestCaseHistoryId== getTestRunTestCaseWizardDetailResult.TestRunTestCaseHistoryId).OrderByDescending(x => x.UpdateDate).Select(x => x.Comment).FirstOrDefault(),
					StepsToReproduce = getTestRunTestCaseDetailJson.TestCaseStepDetailJsons.OrderByDescending(x => x.UpdateDate).Where(x => getTestRunTestCaseWizardDetailResult.TestCaseStepDetailId.Contains(x.TestCaseStepDetailId) && x.ProjectModuleId == testCaseId).GroupBy(x => x.TestCaseStepDetailId).Select(b => new TestCaseStepResult
					{
						TestCaseStepDetailId = b.Select(b => b.TestCaseStepDetailId).FirstOrDefault(),
						ExpectedResult = b.Select(b => b.ExpectedResult).FirstOrDefault(),
						Step = b.Select(b => b.StepNumber).FirstOrDefault(),
						StepDescription = b.Select(b => b.StepDescription).FirstOrDefault(),

					}).OrderBy(x => x.Step).ToList(),
				}).FirstOrDefault();

				var document = await _context.Document.Select(x => new DocumentFileModel
				{
					DocumentId = x.DocumentId,
					FileName = x.Name,
					Extension = x.Extension,
					UpdateDate = x.UpdateDate

				}).ToListAsync();
				testRunTestCaseWizardList.FileName = document.Where(x => x.DocumentId == testRunTestCaseWizardList.HistoryDocumentId).Select(x => x.FileName).FirstOrDefault();
				testRunTestCaseWizardList.Extension = document.Where(x => x.DocumentId == testRunTestCaseWizardList.HistoryDocumentId).Select(x => x.Extension).FirstOrDefault();
				var testRunTestCaseDetailIdsResult = testRunTestCaseWizardList.StepsToReproduce.Select(x => x.TestCaseStepDetailId).ToList();

				var testRunTestCaseStepHistoryIds = await (from trtcsh in _context.TestRunTestCaseStepHistory
														   join li in _context.ListItem on trtcsh.TestRunStatusListItemId equals li.ListItemId
														   where testRunTestCaseDetailIdsResult.Contains(trtcsh.TestCaseStepDetailId)
														   && trtcsh.TestRunTestCaseHistoryId == testRunTestCaseWizardList.TestRunTestCaseHistoryId
														   select new TestRunTestCaseHistoryModelWizard()
														   {
															   TestRunTestCaseStepHistoryId = trtcsh.TestRunTestCaseStepHistoryId,
															   TestCaseStepDetailId = trtcsh.TestCaseStepDetailId,
															   TestRunTestCaseHistoryId = trtcsh.TestRunTestCaseHistoryId,
															   TestRunTestCaseHistoryStatus = li.ListItemSystemName
														   }
														   ).ToListAsync();



				foreach (var itemInStepsToReproduce in testRunTestCaseWizardList.StepsToReproduce)
				{
					itemInStepsToReproduce.TestRunTestCaseStepHistoryId = testRunTestCaseStepHistoryIds.Where(x => x.TestCaseStepDetailId == itemInStepsToReproduce.TestCaseStepDetailId && x.TestRunTestCaseHistoryId == testRunTestCaseWizardList.TestRunTestCaseHistoryId).Select(x => x.TestRunTestCaseStepHistoryId).FirstOrDefault();

					itemInStepsToReproduce.Status = testRunTestCaseStepHistoryIds.Where(x => x.TestCaseStepDetailId == itemInStepsToReproduce.TestCaseStepDetailId && x.TestRunTestCaseHistoryId == testRunTestCaseWizardList.TestRunTestCaseHistoryId).Select(x => x.TestRunTestCaseHistoryStatus).FirstOrDefault();

					itemInStepsToReproduce.TestRunTestCaseHistoryDocumentId = testRunTestCaseStepHistoryDocumentId.Where(x => x.TestRunTestCaseStepHistoryId == itemInStepsToReproduce.TestRunTestCaseStepHistoryId).OrderByDescending(x => x.UpdateDate).Select(x => x.TestRunTestCaseStepHistoryDocumentId).FirstOrDefault();

					itemInStepsToReproduce.StepHistoryDocumentId = testRunTestCaseStepHistoryDocumentId.Where(x => x.TestRunTestCaseStepHistoryId == itemInStepsToReproduce.TestRunTestCaseStepHistoryId).OrderByDescending(x => x.UpdateDate).Select(x => x.DocumentId).FirstOrDefault();

					itemInStepsToReproduce.FileName = document.Where(x => x.DocumentId == itemInStepsToReproduce.StepHistoryDocumentId).OrderByDescending(x => x.UpdateDate).Select(x => x.FileName).FirstOrDefault();

					itemInStepsToReproduce.Extension = document.Where(x => x.DocumentId == itemInStepsToReproduce.StepHistoryDocumentId).OrderByDescending(x => x.UpdateDate).Select(x => x.Extension).FirstOrDefault();

					itemInStepsToReproduce.Comment = testRunTestCaseStepHistoryDocumentId.Where(x => x.TestRunTestCaseStepHistoryId == itemInStepsToReproduce.TestRunTestCaseStepHistoryId).OrderByDescending(x => x.UpdateDate).Select(x => x.Comment).FirstOrDefault();


				}


				if (testRunTestCaseWizardList != null)
				{
					return Result<TestRunTestCaseWizardModel>.Success(testRunTestCaseWizardList);
				}
				else
				{
					return Result<TestRunTestCaseWizardModel>.Success(null);
				}
			}
			catch (Exception ex)
			{

				_iLogger.LogError("Could not find the GetTestRunTestCaseWizard. Exception message is: ", ex.Message);
				return Result<TestRunTestCaseWizardModel>.Error(ReturnMessage.TestRunIdOrTestCaseIdDoesnotExists);

			}


		}

		public async Task<Result<string>> UpdateTestRunTestCaseHistoryWizardAsync(UpdateTestRunTestCaseHistoryWizard model)
		{
			await _context.Database.BeginTransactionAsync();
			try
			{
				int? documentId = null;
				var listItemName = await _context.ListItem.Where(x => x.ListItemId == model.TestRunStatusListItemId).FirstOrDefaultAsync();
				if (listItemName.ListItemSystemName == ListItem.Failed.ToString() && model.files == null)
				{
					throw new FileNotUploadException();

				}

				var getPersonId = await _context.Person.FindAsync(_iPersonAccessor.GetPersonId());
				var updateTestRunTestCaseHistory = await _context.TestRunTestCaseHistory.Include(x => x.TestRunHistory).ThenInclude(x => x.TestRun).Where(x
					   => x.TestRunTestCaseHistoryId == model.TestRunTestCaseHistoryId).FirstOrDefaultAsync();
				var projectId = await _context.Project.Where(x
					=> x.ProjectId == updateTestRunTestCaseHistory.TestRunHistory.TestRun.ProjectId).Select
					(x => x.ProjectId).FirstOrDefaultAsync();

				var projectMemberPersonId = await _context.ProjectMember.Include(x => x.Person).Where(x => x.PersonId == getPersonId.PersonId && x.ProjectId == projectId).Select(x => x.ProjectMemberId).FirstOrDefaultAsync();

				if (projectMemberPersonId > 0)
				{

					if (updateTestRunTestCaseHistory != null)
					{

						updateTestRunTestCaseHistory.TotalTimeSpent = model.TimeSpent;
						updateTestRunTestCaseHistory.TestRunStatusListItemId = model.TestRunStatusListItemId;
						updateTestRunTestCaseHistory.AssigneeProjectMemberId = projectMemberPersonId;

						_context.TestRunTestCaseHistory.Update(updateTestRunTestCaseHistory);
						await _context.SaveChangesAsync();

						if (model.files != null)
						{
							var filesUploadDocumentId = await FileUploadTestRunTestCaseWizard(model.files);
							documentId = filesUploadDocumentId;
						}
						await TestRunTestCaseHistoryDocument(model.TestRunTestCaseHistoryId, model.Comment, documentId);


						var updateTestRunTestCaseStep = await _context.TestRunTestCaseStepHistory.Where(x =>
					  x.TestRunTestCaseHistoryId == updateTestRunTestCaseHistory.TestRunTestCaseHistoryId).ToListAsync();
						foreach (var updateTestStep in updateTestRunTestCaseStep)
						{
							updateTestStep.TotalTimeSpent = model.TimeSpent;
							updateTestStep.TestRunStatusListItemId = model.TestRunStatusListItemId;
							_context.TestRunTestCaseStepHistory.UpdateRange(updateTestRunTestCaseStep);
						}
						await _context.SaveChangesAsync();

						var testRunTestCaseStepHistoryIds = updateTestRunTestCaseStep.Select(x => x.TestRunTestCaseStepHistoryId).ToList();
						await TestRunTestCaseStepHistoryListDocument(testRunTestCaseStepHistoryIds, model.Comment, documentId);
						await _context.Database.CommitTransactionAsync();
						return Result<string>.Success(ReturnMessage.UpdatedSuccessfully);

					}
					else
					{
						throw new TestRunTestCaseHistoryIdNotFoundException();
					}
				}
				else
				{
					throw new PersonIdIsNotAssignToThisProjectException();
				}

			}
			catch (Exception ex)
			{
				await _context.Database.RollbackTransactionAsync();
				if (ex is TestRunTestCaseHistoryIdNotFoundException)
				{
					_iLogger.LogError("Could not find the TestRunTestCaseHistoryId. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestRunIdDoesnotExists);
				}
				else if (ex is FileNotUploadException)
				{
					_iLogger.LogError("Could not find the File. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.FileNotUploadException);
				}
				else if (ex is PersonIdIsNotAssignToThisProjectException)
				{
					_iLogger.LogError("Could not find the memeber id to this project. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.PersonIdIsNotAssignToThisProjectException);
				}
				_iLogger.LogError("Could not update the TestRunTestCaseHistory. Exception message is: ", ex.Message);
				return Result<string>.Error(ReturnMessage.FailedToUpdateTestRunTestCaseHistory);
			}
		}

		public async Task<Result<string>> AddUpdateTestRunTestCaseStepHistoryWizardAsync(AddUpdateTestRunTestCaseStepHistoryWizard model)
		{
			await _context.Database.BeginTransactionAsync();
			try
			{
				int? documentId = null;
				var listItemName = await _context.ListItem.Where(x => x.ListItemId == model.TestRunStatusListItemId).FirstOrDefaultAsync();
				if (listItemName.ListItemSystemName == ListItem.Failed.ToString() && model.files == null)
				{
					throw new FileNotUploadException();
				}

				var addupdateTestRunTestCaseHistory = await _context.TestRunTestCaseStepHistory.Include(x => x.TestRunTestCaseHistory).ThenInclude(x => x.TestRunHistory).ThenInclude(x => x.TestRun).Where(x
							   => x.TestRunTestCaseStepHistoryId == model.TestRunTestCaseStepHistoryId).FirstOrDefaultAsync();
				if (addupdateTestRunTestCaseHistory != null)
				{
					addupdateTestRunTestCaseHistory.TotalTimeSpent = model.TimeSpent;
					addupdateTestRunTestCaseHistory.TestRunStatusListItemId = model.TestRunStatusListItemId;
					_context.TestRunTestCaseStepHistory.Update(addupdateTestRunTestCaseHistory);
					await _context.SaveChangesAsync();

					if (model.files != null)
					{
						var filesUploadDocumentId = await FileUploadTestRunTestCaseWizard(model.files);
						documentId = filesUploadDocumentId;
					}
					await TestRunTestCaseStepHistoryDocument(addupdateTestRunTestCaseHistory.TestRunTestCaseStepHistoryId, model.Comment, documentId);


					var testRunStatusListItemIdForNewHistory = await TestRunTestCaseStepStatusCount(addupdateTestRunTestCaseHistory.TestRunTestCaseHistoryId, model.TestRunStatusListItemId);
					var getPersonId = await _context.Person.FindAsync(_iPersonAccessor.GetPersonId());
					var projectId = await _context.Project.Where(x
						=> x.ProjectId == addupdateTestRunTestCaseHistory.TestRunTestCaseHistory.TestRunHistory.TestRun.ProjectId).Select
						(x => x.ProjectId).FirstOrDefaultAsync();

					var projectMemberPersonId = await _context.ProjectMember.Include(x => x.Person).Where(x => x.PersonId == getPersonId.PersonId && x.ProjectId == projectId).Select(x => x.ProjectMemberId).FirstOrDefaultAsync();
					if (projectMemberPersonId > 0)
					{
						var updateTestRunTestCaseHistory = await _context.TestRunTestCaseHistory.Include(x => x.TestRunHistory).ThenInclude(x => x.TestRun).Include(x => x.TestRunStatusListItem).Where(x
			=> x.TestRunTestCaseHistoryId == addupdateTestRunTestCaseHistory.TestRunTestCaseHistoryId).FirstOrDefaultAsync();
						//if (updateTestRunTestCaseHistory.TestRunStatusListItemId < testRunStatusListItemIdForNewHistory)
						//{
						//	await TestRunTestCaseHistoryDocumentStep(updateTestRunTestCaseHistory.TestRunTestCaseHistoryId, model.Comment, documentId);
						//}

						if (updateTestRunTestCaseHistory != null)
						{
							updateTestRunTestCaseHistory.TestRunStatusListItemId = testRunStatusListItemIdForNewHistory;
							updateTestRunTestCaseHistory.AssigneeProjectMemberId = projectMemberPersonId;

							_context.TestRunTestCaseHistory.Update(updateTestRunTestCaseHistory);
							await _context.SaveChangesAsync();

						}
						await _context.Database.CommitTransactionAsync();
						return Result<string>.Success(ReturnMessage.UpdatedSuccessfully);
					}
					else
					{
						throw new PersonIdIsNotAssignToThisProjectException();
					}
				}
				else
				{
					throw new TestRunTestCaseHistoryIdNotFoundException();
				}
			}
			catch (Exception ex)
			{
				await _context.Database.RollbackTransactionAsync();
				if (ex is TestRunTestCaseHistoryIdNotFoundException)
				{
					_iLogger.LogError("Could not find the TestRunTestCaseHistoryId. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestRunIdDoesnotExists);
				}
				else if (ex is FileNotUploadException)
				{
					_iLogger.LogError("Could not find the File. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.FileNotUploadException);
				}
				else if (ex is PersonIdIsNotAssignToThisProjectException)
				{
					_iLogger.LogError("Could not find the memeber id to this project. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.PersonIdIsNotAssignToThisProjectException);
				}
				_iLogger.LogError("Could not update the TestRunTestCaseHistory. Exception message is: ", ex.Message);
				return Result<string>.Error(ReturnMessage.FailedToUpdateTestRunTestCaseHistory);
			}
		}

		public async Task<Result<string>> AddEnvironmentAsync(EnvironmentModel model)
		{
			await _context.Database.BeginTransactionAsync();
			try
			{
				if (string.IsNullOrEmpty(model.EnvironmentName) || string.IsNullOrWhiteSpace(model.EnvironmentName))
				{
					throw new EnvironmentNameShoulNotBeNullOrOnlySpaceException();
				}
				else
				{
					var projectId = _context.Project.Where(x => x.ProjectSlug == model.ProjectSlug).Select(x => x.ProjectId).FirstOrDefault();
					if (projectId > 0)
					{
						bool environmentNameValid = await _context.Environment.Include(x => x.Project).Where(x => x.EnvironmentName == model.EnvironmentName && x.Project.ProjectSlug == model.ProjectSlug).AnyAsync();

						if (environmentNameValid == true)
						{
							throw new EnvironmentNameAlreadyExistException();
						}
						else
						{

							Entities.Environment environment = new()
							{
								EnvironmentId = model.EnvironmentId,
								EnvironmentName = model.EnvironmentName,
								URL = model?.URL == null ? null : model.URL,
								ProjectId = projectId
							};
							await _context.AddAsync(environment);
							await _context.SaveChangesAsync();
							await _context.Database.CommitTransactionAsync();
							return Result<string>.Success(ReturnMessage.EnvironmentAddedSuccessfully);
						}
					}
					else
					{
						throw new ProjectSlugNotFoundException();
					}
				}



			}
			catch (Exception ex)
			{

				await _context.Database.RollbackTransactionAsync();
				if (ex is EnvironmentNameShoulNotBeNullOrOnlySpaceException)
				{
					_iLogger.LogError("Could not add the Environment as environmentname is empty or space. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.EnvironmentNameShoulNotBeNullOrOnlySpace);
				}
				else if (ex is ProjectSlugNotFoundException)
				{
					_iLogger.LogError("Could not find the ProjectSlug. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ProjectSlugDoesNotExists);
				}
				else if (ex is EnvironmentNameAlreadyExistException)
				{
					_iLogger.LogError("Environment Name Already Exist. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.EnvironmentNameAlreadyExist);
				}


				_iLogger.LogError("Could not add the Environment. Exception message is: ", ex.Message);
				return Result<string>.Error(ReturnMessage.FailedToAddEnvironment);
			}
		}

		public async Task<Result<string>> UpdateEnvironmentAsync(EnvironmentModel model)
		{
			await _context.Database.BeginTransactionAsync();
			try
			{
				var updateEnvironment = await _context.Environment.Where(x =>
				x.EnvironmentId == model.EnvironmentId)
					.FirstOrDefaultAsync();

				if (updateEnvironment != null)
				{
					var projectId = _context.Project.Where(x => x.ProjectSlug == model.ProjectSlug).Select(x => x.ProjectId).FirstOrDefault();
					if (projectId > 0)
					{
						if (string.IsNullOrEmpty(model.EnvironmentName) || string.IsNullOrWhiteSpace(model.EnvironmentName))
						{
							throw new EnvironmentNameShoulNotBeNullOrOnlySpaceException();
						}
						else
						{

							bool environmentNameValid = await _context.Environment.Include(x => x.Project).Where(x => x.EnvironmentName == model.EnvironmentName && x.Project.ProjectSlug == model.ProjectSlug && x.EnvironmentId != model.EnvironmentId).AnyAsync();

							if (environmentNameValid)
							{
								throw new EnvironmentNameAlreadyExistException();
							}
							else
							{
								updateEnvironment.EnvironmentName = model.EnvironmentName;
								updateEnvironment.URL = model.URL;
								updateEnvironment.ProjectId = projectId;
								_context.Environment.Update(updateEnvironment);
								await _context.SaveChangesAsync();
								await _context.Database.CommitTransactionAsync();
								return Result<string>.Success(ReturnMessage.EnvironmentUpdatedSuccessfully);
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
					throw new EnvironmentIdNotFoundException();
				}

			}
			catch (Exception ex)
			{

				await _context.Database.RollbackTransactionAsync();
				if (ex is EnvironmentIdNotFoundException)
				{
					_iLogger.LogError("Could not find the EnvironmentId. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.EnvironmentIdDoesnotExists);
				}
				else if (ex is EnvironmentNameShoulNotBeNullOrOnlySpaceException)
				{
					_iLogger.LogError("Could not add the Environment as environmentname is empty or space. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.EnvironmentNameShoulNotBeNullOrOnlySpace);
				}
				else if (ex is ProjectSlugNotFoundException)
				{
					_iLogger.LogError("Could not find the ProjectSlug. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.ProjectSlugDoesNotExists);
				}
				else if (ex is EnvironmentNameAlreadyExistException)
				{
					_iLogger.LogError("Environment Name Already Exist. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.EnvironmentNameAlreadyExist);
				}
				_iLogger.LogError("Could not update the Environment. Exception message is: ", ex.Message);
				return Result<string>.Error(ReturnMessage.FailedToUpdateEnvironment);
			}
		}

		public async Task<Result<string>> DeleteEnvironmentAsync(int environmentId)
		{
			await _context.Database.BeginTransactionAsync();
			try
			{
				var deleteEnvironment = await _context.Environment.Where(x => x.EnvironmentId == environmentId).FirstOrDefaultAsync();
				if (deleteEnvironment != null)
				{
					var isValidEnvironment = await _context.TestRun.Where(x => x.EnvironmentId == (int)environmentId).ToListAsync();

					if (isValidEnvironment.Count > 0)
					{
						throw new EnvironmentIdUsedInTestRunException();
					}
					_context.Environment.Remove(deleteEnvironment);
					await _context.SaveChangesAsync();

					await _context.Database.CommitTransactionAsync();
					return Result<string>.Success(ReturnMessage.EnvironmentDeletedSuccessfully);
				}
				else
				{
					throw new EnvironmentIdNotFoundException();
				}

			}

			catch (Exception ex)
			{
				await _context.Database.RollbackTransactionAsync();

				if (ex is EnvironmentIdNotFoundException)
				{
					_iLogger.LogError("Could not find the EnvironmentId. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.EnvironmentIdDoesnotExists);
				}
				if (ex is EnvironmentIdUsedInTestRunException)
				{
					_iLogger.LogError("Environment is used in the test run. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.EnvironmentIdUsedInTestRun);
				}
				_iLogger.LogError("Failed to delete the Environment. Exception message is: ", ex.Message);
				return Result<string>.Error(ReturnMessage.FailedToDeleteEnvironment);

			}
		}

		public async Task<Result<GetAllEnvironmentListModel>> GetEnvironmentByIdAsync(int environmentId)
		{
			try
			{
				var environmentDetail = await _context.Environment.Where(x => x.EnvironmentId == environmentId).FirstOrDefaultAsync();

				if (environmentDetail != null)
				{
					var getTestRutnDetailByIdAsync = await (from e in _context.Environment
															where e.EnvironmentId == environmentId
															select new GetAllEnvironmentListModel
															{
																EnvironmentId = e.EnvironmentId,
																EnvironmentName = e.EnvironmentName,
																URL = e.URL == null ? null : e.URL,

															}).FirstOrDefaultAsync();



					if (getTestRutnDetailByIdAsync != null)
					{
						return Result<GetAllEnvironmentListModel>.Success(getTestRutnDetailByIdAsync);
					}
					else
					{
						return Result<GetAllEnvironmentListModel>.Success(null);
					}
				}
				else
				{
					throw new EnvironmentIdNotFoundException();
				}
			}
			catch (Exception ex)
			{

				_iLogger.LogError("Could not find the EnvironmentId. Exception message is: ", ex.Message);
				return Result<GetAllEnvironmentListModel>.Error(ReturnMessage.EnvironmentIdDoesnotExists);

			}
		}

		public async Task<Result<PagedResponseModel<List<GetAllEnvironmentListModel>>>> GetEnvironmentListAsync(PaginationFilterModel filter, string projectSlug)
		{

			try
			{
				IQueryable<GetAllEnvironmentListModel> getEnvironmentList;
				var projectId = _context.Project.Where(x => x.ProjectSlug == projectSlug).Select(x => x.ProjectId).FirstOrDefault();
				if (projectId > 0)
				{
					var getEnvironmentListAsync = (from e in _context.Environment
												   join p in _context.Project on e.ProjectId equals p.ProjectId
												   where e.ProjectId == projectId
												   select new
												   {
													   EnvironmentId = e.EnvironmentId,
													   EnvironmentName = e.EnvironmentName,
													   URL = e.URL == null ? null : e.URL,
													   UpdateDate = e.UpdateDate,
													   ProjectId = e.ProjectId
												   });
					getEnvironmentList = getEnvironmentListAsync.GroupBy(x => x.EnvironmentId).Select(x => new GetAllEnvironmentListModel
					{
						EnvironmentId = x.Select(x => x.EnvironmentId).FirstOrDefault(),
						EnvironmentName = x.Select(x => x.EnvironmentName).FirstOrDefault(),
						URL = x.Select(x => x.URL).FirstOrDefault(),
						UpdateDate = x.Select(x => x.UpdateDate).FirstOrDefault()

					}).OrderByDescending(x => x.UpdateDate);

					if (!string.IsNullOrEmpty(filter.SearchValue))
					{
						getEnvironmentList = getEnvironmentList.Where
							(
							  x => x.EnvironmentName.ToLower().Contains(filter.SearchValue.ToLower())

							);
					}

					var records = getEnvironmentList;
					var totalRecords = records.Count();
					var filteredData = await getEnvironmentList
						.Skip((filter.PageNumber - 1) * filter.PageSize)
						.Take(filter.PageSize)
						.ToListAsync();

					if (filter.PageSize > totalRecords && totalRecords > 0)
					{
						filter.PageSize = totalRecords;
					}
					var totalPages = (totalRecords / filter.PageSize);


					var data = new PagedResponseModel<List<GetAllEnvironmentListModel>>(filteredData, filter.PageNumber, filter.PageSize, totalRecords, totalPages);

					if (filteredData.Count > 0)
					{
						return Result<PagedResponseModel<List<GetAllEnvironmentListModel>>>.Success(data);
					}
					else
					{
						return Result<PagedResponseModel<List<GetAllEnvironmentListModel>>>.Success(null);
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
				return Result<PagedResponseModel<List<GetAllEnvironmentListModel>>>.Error(ReturnMessage.ProjectSlugDoesNotExists);

			}



		}

		public async Task<byte[]> ExportTestRunTestCaseByTestRunIdAsync(int testRunId)
		{
			try
			{
				var testRun = await _context.TestRun.Where(x => x.TestRunId == testRunId).FirstOrDefaultAsync();
				if (testRun != null)
				{
					var excelReportResult = await PdfExcelTestRunTestCaseReport(testRunId);
					var getExcelDataReport = TestRunTestCaseExportHelper.TestRunTestCaseByTestRunIdToExcel(excelReportResult.TestRunTestCaseExportModelForExcelReport, excelReportResult.BarChart, excelReportResult.RedarDiagram, excelReportResult.PieChart, excelReportResult.TestRunExcelModelTitle, excelReportResult.FunctionTestCase);
					return getExcelDataReport;
				}
				else
				{
					throw new TestRunIdNotFoundException();
				}
			}
			catch (Exception ex)
			{
				if (ex is PdfOrExcelNameIsNotValidException)
				{
					_iLogger.LogError("Could not find the pdf or excel to print. Exception message is: ", ex.Message);
					throw new PdfOrExcelNameIsNotValidException();
				}
				_iLogger.LogError("Could not find the TestRunId. Exception message is: ", ex.Message);
				throw new TestRunIdNotFoundException();
			}
		}

		public async Task<Result<string>> DeleteTestRunTestPlanTestCaseIdAsync(int projectModuleId, int testPlanId, int testRunId)
		{

			await _context.Database.BeginTransactionAsync();
			try
			{
				var getTestRunTestCaseDetails = await (from t in _context.TestRunPlanDetail
													   join trp in _context.TestRunPlan on t.TestRunPlanId equals trp.TestRunPlanId
													   join tr in _context.TestRun on trp.TestRunId equals tr.TestRunId
													   join trh in _context.TestRunHistory on tr.TestRunId equals trh.TestRunId
													   where trp.TestRunId == testRunId && trp.TestPlanId == testPlanId
													   select new
													   {
														   TestRunId = tr.TestRunId,
														   TestPlanId = trp.TestPlanId,
														   TestRunPlanId = trp.TestRunPlanId,
														   TestPlanDetailJson = t.TestPlanDetailJson,
														   TestCaseDetailJson = t.TestCaseDetailJson,
														   TestCaseStepDetailJson = t.TestCaseStepDetailJson,
														   TestRunPlanDetailId = t.TestRunPlanDetailId,
													   }).ToListAsync();

				var getTestRunTestCaseDetailsResult = getTestRunTestCaseDetails.Select(x => new
				{
					TestRunId = x.TestRunId,
					TestPlanId = x.TestPlanId,
					TestRunPlanId = x.TestRunPlanId,
					TestRunPlanDetailId = x.TestRunPlanDetailId,
					TestPlanDetailJsons = JsonSerializer.Deserialize<List<TestPlanJson>>(x.TestPlanDetailJson),
					TestCaseDetailJsons = JsonSerializer.Deserialize<List<TestPlanTestCaseJson>>(x.TestCaseDetailJson),
					TestCaseStepDetailJsons = JsonSerializer.Deserialize<List<TestCaseStepDetailsJson>>(x.TestCaseStepDetailJson),
				}).FirstOrDefault();


				var testCaseId = getTestRunTestCaseDetailsResult.TestCaseDetailJsons.Where(x => x.TestRunId == testRunId && x.TestPlanId == testPlanId && x.ProjectModuleId == projectModuleId).FirstOrDefault();

				getTestRunTestCaseDetailsResult.TestCaseDetailJsons.Remove(testCaseId);

				getTestRunTestCaseDetailsResult.TestCaseStepDetailJsons.RemoveAll(x => x.ProjectModuleId == projectModuleId);


				var testCaseJsonDetails = JsonSerializer.Serialize<List<TestPlanTestCaseJson>>(getTestRunTestCaseDetailsResult.TestCaseDetailJsons);
				var testCaseStepDetailJson = JsonSerializer.Serialize<List<TestCaseStepDetailsJson>>(getTestRunTestCaseDetailsResult.TestCaseStepDetailJsons);

				var testRunPlanDetailId = _context.TestRunPlanDetail.Include(x => x.TestRunPlan).Where(x => x.TestRunPlan.TestRunId == testRunId && x.TestRunPlan.TestPlanId == testPlanId).FirstOrDefault();



				if (testRunPlanDetailId != null)
				{
					testRunPlanDetailId.TestCaseDetailJson = testCaseJsonDetails;
					testRunPlanDetailId.TestCaseStepDetailJson = testCaseStepDetailJson;
					_context.TestRunPlanDetail.Update(testRunPlanDetailId);
					await _context.SaveChangesAsync();


					var testRunTestCaseHistoryId = _context.TestRunTestCaseHistory.Include(x => x.TestRunHistory).ThenInclude(x => x.TestRun).Include(x => x.TestPlan).Include(x => x.TestRunStatusListItem).Include(x => x.ProjectModule).Where(x => x.TestRunHistory.TestRun.TestRunId == testRunId && x.TestPlanId == testPlanId && x.ProjectModuleId == projectModuleId).FirstOrDefault();

					if (testRunTestCaseHistoryId != null)
					{
						_context.TestRunTestCaseHistory.Remove(testRunTestCaseHistoryId);
						await _context.SaveChangesAsync();
					}


					await _context.Database.CommitTransactionAsync();
					return Result<string>.Success(ReturnMessage.DeletedSuccessfully);
				}
				else
				{
					return Result<string>.Error(ReturnMessage.TestCaseIdNotFound);
				}

			}
			catch (Exception ex)
			{
				await _context.Database.RollbackTransactionAsync();
				if (ex is TestCaseIdNotFoundException)
				{
					_iLogger.LogError("Could not find the testcase. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestCaseIdNotFound);
				}

				_iLogger.LogError("Failed to delete the testCase. Exception message is: ", ex.Message);
				return Result<string>.Error(ReturnMessage.FailedToDeleteTestCase);

			}
		}

		public async Task<Result<List<GetEnvironmentListModel>>> GetAllEnvironmentListAsync(string projectSlug)
		{
			try
			{
				var projectId = _context.Project.Where(x => x.ProjectSlug == projectSlug).Select(x => x.ProjectId).FirstOrDefault();
				if (projectId > 0)
				{
					var getEnvironmentListAsync = await (from e in _context.Environment
														 join p in _context.Project on e.ProjectId equals p.ProjectId
														 where e.ProjectId == projectId
														 select new GetEnvironmentListModel
														 {
															 EnvironmentId = e.EnvironmentId,
															 EnvironmentName = e.EnvironmentName,

														 }).ToListAsync();
					if (getEnvironmentListAsync.Any())
					{
						return Result<List<GetEnvironmentListModel>>.Success(getEnvironmentListAsync);
					}
					else
					{
						return Result<List<GetEnvironmentListModel>>.Success(null);
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
				return Result<List<GetEnvironmentListModel>>.Error(ReturnMessage.ProjectSlugDoesNotExists);

			}


		}

		public async Task<Result<string>> DeleteMultipleTestCaseIdAsync(DeleteMultipleTestCaseModel model)
		{
			await _context.Database.BeginTransactionAsync();
			try
			{
				var getTestRunTestCaseDetails = await (from t in _context.TestRunPlanDetail
													   join trp in _context.TestRunPlan on t.TestRunPlanId equals trp.TestRunPlanId
													   join tr in _context.TestRun on trp.TestRunId equals tr.TestRunId
													   join trh in _context.TestRunHistory on tr.TestRunId equals trh.TestRunId
													   where trp.TestRunId == model.TestRunId
													   select new
													   {
														   TestRunId = tr.TestRunId,
														   TestPlanId = trp.TestPlanId,
														   TestRunPlanId = trp.TestRunPlanId,
														   TestPlanDetailJson = t.TestPlanDetailJson,
														   TestCaseDetailJson = t.TestCaseDetailJson,
														   TestCaseStepDetailJson = t.TestCaseStepDetailJson,
														   TestRunPlanDetailId = t.TestRunPlanDetailId,
													   }).ToListAsync();

				var getTestRunTestCaseDetailsResult = getTestRunTestCaseDetails.Select(x => new
				{
					TestRunId = x.TestRunId,
					TestPlanId = x.TestPlanId,
					TestRunPlanId = x.TestRunPlanId,
					TestRunPlanDetailId = x.TestRunPlanDetailId,
					TestPlanDetailJsons = JsonSerializer.Deserialize<List<TestPlanJson>>(x.TestPlanDetailJson),
					TestCaseDetailJsons = JsonSerializer.Deserialize<List<TestPlanTestCaseJson>>(x.TestCaseDetailJson),
					TestCaseStepDetailJsons = JsonSerializer.Deserialize<List<TestCaseStepDetailsJson>>(x.TestCaseStepDetailJson),
				}).ToList();



				List<int> testplanIds = model.TestPlan.Select(x => x.TestPlanId).ToList();
				string testCaseJsonDetails;
				string testCaseStepDetailJson;
				List<int> testCaseIds = new List<int>();
				List<int> planList = new List<int>();
				List<int> projectModuleIds = new List<int>();
				List<TestPlanTestCaseJson> testPlanTestCaseList = new List<TestPlanTestCaseJson>();

				foreach (var item in getTestRunTestCaseDetailsResult)
				{
					foreach (var items in item.TestPlanDetailJsons)
					{
						testCaseIds = model.TestPlan.Where(x => x.TestPlanId == items.TestPlanId).SelectMany(x => x.TestCaseId).ToList();
						testPlanTestCaseList = item.TestCaseDetailJsons.Where(x => testCaseIds.Contains(x.ProjectModuleId) && x.TestPlanId == items.TestPlanId).ToList();
						item.TestCaseDetailJsons.RemoveAll(x => x.TestPlanId == items.TestPlanId && testCaseIds.Contains(x.ProjectModuleId));
						projectModuleIds = testPlanTestCaseList.Select(x => x.ProjectModuleId).ToList();
						planList = testPlanTestCaseList.Select(x => x.TestPlanId).ToList();
						List<TestCaseStepDetailsJson> testCaseStepDetailJsonList = item.TestCaseStepDetailJsons.Where(x => planList.Contains(items.TestPlanId) && projectModuleIds.Contains(x.ProjectModuleId)).ToList();
						int stepsCount = testCaseStepDetailJsonList.Count();
						item.TestCaseStepDetailJsons.RemoveAll(x => planList.Contains(items.TestPlanId) && projectModuleIds.Contains(x.ProjectModuleId));

						var testRunTestCaseHistoryId = _context.TestRunTestCaseHistory.Include(x => x.TestRunHistory).ThenInclude(x => x.TestRun).Include(x => x.TestPlan).Include(x => x.TestRunStatusListItem).Include(x => x.ProjectModule).Where(x => x.TestRunHistory.TestRun.TestRunId == model.TestRunId && planList.Contains(x.TestPlanId) && testCaseIds.Contains(x.ProjectModuleId)).ToList();

						if (testRunTestCaseHistoryId != null)
						{
							_context.TestRunTestCaseHistory.RemoveRange(testRunTestCaseHistoryId);
							await _context.SaveChangesAsync();
						}


					}
					testCaseJsonDetails = JsonSerializer.Serialize<List<TestPlanTestCaseJson>>(item.TestCaseDetailJsons);
					testCaseStepDetailJson = JsonSerializer.Serialize<List<TestCaseStepDetailsJson>>(item.TestCaseStepDetailJsons);


					var testRunPlanDetailId = _context.TestRunPlanDetail.Include(x => x.TestRunPlan).ThenInclude(x => x.TestPlan).Where(x => x.TestRunPlan.TestPlanId == item.TestPlanId && x.TestRunPlan.TestRunId == model.TestRunId).ToList();

					if (testRunPlanDetailId != null)
					{
						foreach (var itms in testRunPlanDetailId)
						{
							itms.TestRunPlan.TestRunId = model.TestRunId;
							itms.TestCaseDetailJson = testCaseJsonDetails;
							itms.TestCaseStepDetailJson = testCaseStepDetailJson;
						}
						_context.TestRunPlanDetail.UpdateRange(testRunPlanDetailId);
						await _context.SaveChangesAsync();
					}

				}


				await _context.Database.CommitTransactionAsync();
				return Result<string>.Success(ReturnMessage.DeletedSuccessfully);


			}
			catch (Exception ex)
			{
				await _context.Database.RollbackTransactionAsync();
				if (ex is TestCaseIdNotFoundException)
				{
					_iLogger.LogError("Could not find the testcase. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestCaseIdNotFound);
				}

				_iLogger.LogError("Failed to delete the testCase. Exception message is: ", ex.Message);
				return Result<string>.Error(ReturnMessage.FailedToDeleteTestCase);

			}

		}

		public async Task<Result<PdfTestRunTestCaseReportModel>> GeneratePdfReportForTestRunTestCaseByTestRunIdAsync(int testRunId)
		{
			try
			{
				var testRun = await _context.TestRun.Where(x => x.TestRunId == testRunId).FirstOrDefaultAsync();
				if (testRun != null)
				{
					var pdfReportResult = await PdfExcelTestRunTestCaseReport(testRunId);
					var pdfReport = new PdfTestRunTestCaseReportModel();
					pdfReport.TestRunExcelModelTitle = pdfReportResult.TestRunExcelModelTitle;
					pdfReport.RedarDiagram = pdfReportResult.RedarDiagram;
					pdfReport.PieChart = pdfReportResult.PieChart;
					pdfReport.BarChart = pdfReportResult.BarChart;
					pdfReport.FunctionTestCase = pdfReportResult.FunctionTestCase;
					return Result<PdfTestRunTestCaseReportModel>.Success(pdfReport);

				}
				else
				{
					throw new TestRunIdNotFoundException();
				}
			}
			catch (Exception ex)
			{
				if (ex is PdfOrExcelNameIsNotValidException)
				{
					_iLogger.LogError("Could not find the pdf or excel to print. Exception message is: ", ex.Message);
					throw new PdfOrExcelNameIsNotValidException();
				}
				_iLogger.LogError("Could not find the TestRunId. Exception message is: ", ex.Message);
				throw new TestRunIdNotFoundException();
			}
		}

		public async Task<Result<string>> RetestTestPlanTestCaseIdAsync(RetestMultipleTestCaseModel retestModel)
		{
			await _context.Database.BeginTransactionAsync();
			try
			{
				List<int> testplanIds = retestModel.TestPlan.Select(x => x.TestPlanId).ToList();

				var getTestRunTestCaseDetails = await (from t in _context.TestRunPlanDetail
													   join trp in _context.TestRunPlan on t.TestRunPlanId equals trp.TestRunPlanId
													   join tr in _context.TestRun on trp.TestRunId equals tr.TestRunId
													   where trp.TestRunId == retestModel.TestRunId
													   select new
													   {
														   TestRunId = tr.TestRunId,
														   TestPlanId = trp.TestPlanId,
														   TestRunPlanId = trp.TestRunPlanId,
														   TestPlanDetailJson = t.TestPlanDetailJson,
														   TestCaseDetailJson = t.TestCaseDetailJson,
														   TestCaseStepDetailJson = t.TestCaseStepDetailJson,
														   TestRunPlanDetailId = t.TestRunPlanDetailId,
													   }).ToListAsync();

				var getTestRunTestCaseDetailsResult = getTestRunTestCaseDetails.Where(x => testplanIds.Contains(x.TestPlanId)).Select(x => new
				{
					TestRunId = x.TestRunId,
					TestPlanId = x.TestPlanId,
					TestRunPlanId = x.TestRunPlanId,
					TestRunPlanDetailId = x.TestRunPlanDetailId,
					TestPlanDetailJsons = JsonSerializer.Deserialize<List<TestPlanJson>>(x.TestPlanDetailJson),
					TestCaseDetailJsons = JsonSerializer.Deserialize<List<TestPlanTestCaseJson>>(x.TestCaseDetailJson),
					TestCaseStepDetailJsons = JsonSerializer.Deserialize<List<TestCaseStepDetailsJson>>(x.TestCaseStepDetailJson),
				}).ToList();


				string testCaseJsonDetails;
				string testCaseStepDetailJson;
				List<int> testCaseIds = new List<int>();
				List<int> planList = new List<int>();
				List<int> projectModuleIds = new List<int>();
				List<TestPlanTestCaseJson> testPlanTestCaseList = new List<TestPlanTestCaseJson>();
				List<TestPlanTestCaseJson> testCaseDetail = new List<TestPlanTestCaseJson>();
				List<TestPlanTestCaseJson> updateTestCaseJsonData = new List<TestPlanTestCaseJson>();




				foreach (var item in getTestRunTestCaseDetailsResult)
				{
					foreach (var items in item.TestPlanDetailJsons)
					{
						testCaseIds = retestModel.TestPlan.Where(x => x.TestPlanId == items.TestPlanId).SelectMany(x => x.TestCaseId).ToList();
						testPlanTestCaseList = item.TestCaseDetailJsons.Where(x => testCaseIds.Contains(x.ProjectModuleId) && x.TestPlanId == items.TestPlanId).ToList();
						projectModuleIds = testPlanTestCaseList.Select(x => x.ProjectModuleId).ToList();
						var projectModule = _context.ProjectModule.Where(x => projectModuleIds.Contains(x.ProjectModuleId)).ToList();
						if (projectModule.Count > 0)
						{
							foreach (var itm in projectModule)
							{
								var projectModuleId = _context.ProjectModule.Where(x => x.ProjectModuleId == itm.ProjectModuleId).FirstOrDefault();
								var testRunName = _context.TestRun.Where(x => x.TestRunId == retestModel.TestRunId).Select(x => x.Title).FirstOrDefault();


								var projectModuleListId = await (from tptc in _context.TestPlanTestCase
																 join tp in _context.TestPlan on tptc.TestPlanId equals tp.TestPlanId
																 join li in _context.ListItem on tp.TestPlanTypeListItemId equals li.ListItemId
																 join pm in _context.ProjectModule on tptc.ProjectModuleId equals pm.ProjectModuleId
																 join li2 in _context.ListItem on pm.ProjectModuleListItemId equals li2.ListItemId
																 where pm.ProjectModuleId == projectModuleId.ProjectModuleId
																 && tptc.TestPlanId == items.TestPlanId

																 select new TestPlanTestCaseModel
																 {
																	 ProjectModuleId = pm.ProjectModuleId,
																	 ProjectModuleListItemId = li2.ListItemId,
																	 ProjectModuleListItemName = li2.ListItemSystemName,
																	 TestPlanTypeListItemId = li.ListItemId,
																	 TestPlanTypeListItemName = li.ListItemSystemName,
																	 TestPlanId = tp.TestPlanId,
																 }).ToListAsync();


								//Adding TestCaseDetailJson	
								updateTestCaseJsonData = await (from tptc in _context.TestPlanTestCase
																join pm in _context.ProjectModule on tptc.ProjectModuleId equals pm.ProjectModuleId
																join li in _context.ListItem on pm.ProjectModuleListItemId equals li.ListItemId
																join p in _context.Project on pm.ProjectId equals p.ProjectId
																join tp in _context.TestPlan on tptc.TestPlanId equals tp.TestPlanId
																join li2 in _context.ListItem on tp.TestPlanTypeListItemId equals li2.ListItemId
																where pm.ProjectModuleId == projectModuleId.ProjectModuleId && tp.TestPlanId == items.TestPlanId
																select new TestPlanTestCaseJson()
																{
																	TestPlanTestCaseId = tptc.TestPlanTestCaseId,
																	ProjectModuleId = pm.ProjectModuleId,
																	ParentProjectModuleId = pm.ParentProjectModuleId,
																	ProjectModuleName = pm.ModuleName,
																	ProjectModuleListItemId = pm.ProjectModuleListItemId,
																	OrderDate = pm.OrderDate,
																	ProjectModuleListItemName = li.ListItemSystemName,
																	ProjectId = p.ProjectId,
																	ProjectName = p.ProjectName,
																	ProjectSlug = p.ProjectSlug,
																	ProjectModuleDescription = pm.Description == null ? string.Empty : pm.Description,
																	ProjectDescription = p.ProjectDescription == null ? string.Empty : p.ProjectDescription,
																	TestPlanId = tp.TestPlanId,
																	TestPlanName = tp.TestPlanName,
																	TestPlanTypeListItemName = li2.ListItemSystemName,
																	TestPlanTitle = tp.Title,
																	ParentTestPlanId = tp.ParentTestPlanId,
																	TestPlanDescription = tp.Description == null ? string.Empty : tp.Description,
																	TestPlanTypeListItemId = li2.ListItemId,
																	TestRunId = retestModel.TestRunId,
																	TestRunName = testRunName,
																	InsertDate = DateTimeOffset.UtcNow,
																	InsertPersonId = _iPersonAccessor.GetPersonId(),
																	UpdateDate = DateTimeOffset.UtcNow,
																	UpdatedPersonId = _iPersonAccessor.GetPersonId()
																}).ToListAsync();


								var projectModuleIdList = testPlanTestCaseList.Select(x => x.ProjectModuleId).ToList();

								var test = await (from tcd in _context.TestCaseDetail
												  join pm in _context.ProjectModule on tcd.ProjectModuleId equals pm.ProjectModuleId
												  join li in _context.ListItem on tcd.TestCaseListItemId equals li.ListItemId
												  where projectModuleIdList.Contains(pm.ProjectModuleId) && pm.IsDeleted == false
												  select new TestCaseDetail
												  {
													  TestCaseDetailId = tcd.TestCaseDetailId,
													  ProjectModuleId = pm.ProjectModuleId,
													  PreCondition = tcd.PreCondition,
													  ExpectedResult = tcd.ExpectedResult,
													  TestCaseListItemId = li.ListItemId,
													  TestCaseListItemName = li.ListItemSystemName,

												  }).ToListAsync();



								foreach (var itemss in updateTestCaseJsonData)
								{
									itemss.FunctionName = _context.ProjectModule.Where(x => x.ProjectModuleId == itemss.ParentProjectModuleId).Select(x => x.ModuleName).FirstOrDefault();
									itemss.TestCaseDetailId = test.Where(z => z.ProjectModuleId == itemss.ProjectModuleId).Select(z => z.TestCaseDetailId).FirstOrDefault();
									itemss.PreCondition = test.Where(z => z.ProjectModuleId == itemss.ProjectModuleId).Select(z => z.PreCondition).FirstOrDefault();
									itemss.ExpectedResult = test.Where(z => z.ProjectModuleId == itemss.ProjectModuleId).Select(z => z.ExpectedResult).FirstOrDefault();
									itemss.TestCaseListItemId = test.Where(z => z.ProjectModuleId == itemss.ProjectModuleId).Select(z => z.TestCaseListItemId).FirstOrDefault();
									itemss.TestCaseListItemName = test.Where(z => z.ProjectModuleId == itemss.ProjectModuleId).Select(z => z.TestCaseListItemName).FirstOrDefault();

								}
								item.TestCaseDetailJsons.AddRange(updateTestCaseJsonData);



								//Adding TestRunTestCaseHistory 
								var testRunStatus = await (_iCommonService.GetListItemDetailByListItemSystemName(ListItem.Pending.ToString()));
								var testRunDetail = _context.TestRun.Where(x => x.TestRunId == retestModel.TestRunId).FirstOrDefault();
								var testRunHistoryId = _context.TestRunHistory.Include(x => x.TestRun).Where(x => x.TestRun.TestRunId == retestModel.TestRunId).FirstOrDefault();


								var testRunTestCaseHistory = projectModuleListId.Select(x => new Entities.TestRunTestCaseHistory
								{
									StartTime = null,
									EndTime = null,
									TotalTimeSpent = null,
									TestRunStatusListItemId = testRunStatus.ListItemId,
									AssigneeProjectMemberId = testRunDetail.DefaultAssigneeProjectMemberId,
									ProjectModuleId = x.ProjectModuleId,
									TestRunHistoryId = testRunHistoryId.TestRunHistoryId,
									TestPlanId = x.TestPlanId,
									InsertDate = DateTimeOffset.UtcNow,
									UpdateDate = DateTimeOffset.UtcNow,

								}).ToList();

								await _context.TestRunTestCaseHistory.AddRangeAsync(testRunTestCaseHistory);
								await _context.SaveChangesAsync();


								//Adding TestRunTestCaseHistoryDocument
								var testRunTestCaseHistoryDocumentList = testRunTestCaseHistory.Select(x => new Entities.TestRunTestCaseHistoryDocument
								{
									TestRunTestCaseHistoryId = x.TestRunTestCaseHistoryId,
									DocumentId = null,
									Comment = null,
									InsertDate = DateTimeOffset.UtcNow,
									UpdateDate = DateTimeOffset.UtcNow,

								}).ToList();

								await _context.TestRunTestCaseHistoryDocument.AddRangeAsync(testRunTestCaseHistoryDocumentList);
								await _context.SaveChangesAsync();


								List<Entities.TestRunTestCaseStepHistory> addTestRunTestCaseStepHistories = new();
								//Add TestRunTestCaseStepHistory
								foreach (var TestRunTestCaseHistory in testRunTestCaseHistory)
								{
									var testCaseStepDetail = _context.TestCaseStepDetail.Where(x => x.ProjectModuleId == TestRunTestCaseHistory.ProjectModuleId).Select(x => x.TestCaseStepDetailId).ToList();

									foreach (var TestCaseStepDetailId in testCaseStepDetail)
									{
										Entities.TestRunTestCaseStepHistory testRunTestCaseStepHistory = new Entities.TestRunTestCaseStepHistory();

										testRunTestCaseStepHistory.StartTime = null;
										testRunTestCaseStepHistory.EndTime = null;
										testRunTestCaseStepHistory.TotalTimeSpent = null;
										testRunTestCaseStepHistory.TestRunStatusListItemId = testRunStatus.ListItemId;
										testRunTestCaseStepHistory.TestRunTestCaseHistoryId = TestRunTestCaseHistory.TestRunTestCaseHistoryId;
										testRunTestCaseStepHistory.TestCaseStepDetailId = TestCaseStepDetailId;
										addTestRunTestCaseStepHistories.Add(testRunTestCaseStepHistory);
									}
								}
								await _context.TestRunTestCaseStepHistory.AddRangeAsync(addTestRunTestCaseStepHistories);
								await _context.SaveChangesAsync();


								//Adding TestRunTestCaseStepHistoryDocument
								var testRunTestCaseStepHistoryDocumentList = addTestRunTestCaseStepHistories.Select(x => new Entities.TestRunTestCaseStepHistoryDocument
								{

									TestRunTestCaseStepHistoryId = x.TestRunTestCaseStepHistoryId,
									DocumentId = null,
									Comment = null,
									InsertDate = DateTimeOffset.UtcNow,
									UpdateDate = DateTimeOffset.UtcNow,

								}).ToList();

								await _context.TestRunTestCaseStepHistoryDocument.AddRangeAsync(testRunTestCaseStepHistoryDocumentList);
								await _context.SaveChangesAsync();


								List<TestCaseStepsUpdateModel> testCaseStepUpdateJsonResult = new List<TestCaseStepsUpdateModel>();

								foreach (var itemss in testRunTestCaseHistory)
								{
									var testCaseStepUpdateJson = addTestRunTestCaseStepHistories.Where(x => x.TestRunTestCaseHistoryId == itemss.TestRunTestCaseHistoryId).Select(x => new TestCaseStepsUpdateModel
									{
										TestRunTestCaseStepHistoryId = x.TestRunTestCaseStepHistoryId,
										TestRunTestCaseHistoryId = x.TestRunTestCaseHistoryId,
										TestPlanId = itemss.TestPlanId,
										ProjectModuleId = itemss.ProjectModuleId,
										StartTime = null,
										EndTime = null,
										TotalTimeSpent = null,
										TestRunStatusListItemId = testRunStatus.ListItemId,

										TestCaseStepDetailId = x.TestCaseStepDetailId,
									}).ToList();

									testCaseStepUpdateJsonResult.AddRange(testCaseStepUpdateJson);
								}
								//TestCaseStepDetailJson
								var projectModuleIdTestCase = updateTestCaseJsonData.Select(x => x.ProjectModuleId).ToList();
								var testCaseHistoryIds = testCaseStepUpdateJsonResult.Select(x => x.TestRunTestCaseHistoryId).ToList();



								var testCaseStepDetails = await (
									from trtch in _context.TestRunTestCaseHistory
									join trtcsh in _context.TestRunTestCaseStepHistory on trtch.TestRunTestCaseHistoryId equals trtcsh.TestRunTestCaseHistoryId
									join tcsd in _context.TestCaseStepDetail on trtcsh.TestCaseStepDetailId equals tcsd.TestCaseStepDetailId
									join pm in _context.ProjectModule on trtch.ProjectModuleId equals pm.ProjectModuleId
									join li in _context.ListItem on pm.ProjectModuleListItemId equals li.ListItemId
									join li2 in _context.ListItem on tcsd.TestCaseListItemId equals li2.ListItemId
									join tp in _context.TestPlan on trtch.TestPlanId equals tp.TestPlanId
									where projectModuleIdTestCase.Contains(pm.ProjectModuleId) && pm.IsDeleted == false && testCaseHistoryIds.Contains(trtch.TestRunTestCaseHistoryId)

									select new TestCaseStepDetailsJson
									{
										TestCaseStepDetailId = tcsd.TestCaseStepDetailId,
										StepNumber = tcsd.StepNumber,
										StepDescription = tcsd.StepDescription,
										ExpectedResult = tcsd.ExpectedResult,
										TestCaseListItemId = li2.ListItemId,
										TestCaseListItemName = li2.ListItemName,
										ProjectId = pm.ProjectId,
										ProjectModuleId = pm.ProjectModuleId,
										ProjectModuleName = pm.ModuleName,
										ParentProjectModuleId = pm.ParentProjectModuleId,
										ProjectModuleDescription = pm.Description,
										ProjectModuleListItemId = li.ListItemId,
										ProjectModuleListItemName = li.ListItemSystemName,
										OrderDate = DateTimeOffset.UtcNow,
										InsertDate = DateTimeOffset.UtcNow,
										InsertPersonId = _iPersonAccessor.GetPersonId(),
										UpdateDate = DateTimeOffset.UtcNow,
										UpdatePersonId = _iPersonAccessor.GetPersonId(),
										TestRunTestCaseHistoryId = trtch.TestRunTestCaseHistoryId,
										TestRunTestCaseStepHistoryId = trtcsh.TestRunTestCaseStepHistoryId,
										TestPlanId = tp.TestPlanId,
										TestCaseStepStatusName = trtcsh.TestRunStatusListItem.ListItemSystemName
									}).ToListAsync();

								item.TestCaseStepDetailJsons.AddRange(testCaseStepDetails);
							}
						}


						//TestRunPlanDetail Table Updated 
						testCaseJsonDetails = JsonSerializer.Serialize<List<TestPlanTestCaseJson>>(item.TestCaseDetailJsons);
						testCaseStepDetailJson = JsonSerializer.Serialize<List<TestCaseStepDetailsJson>>(item.TestCaseStepDetailJsons);

						var testRunPlanDetailId = _context.TestRunPlanDetail.Include(x => x.TestRunPlan).ThenInclude(x => x.TestPlan).Where(x => x.TestRunPlan.TestPlanId == item.TestPlanId && x.TestRunPlan.TestRunId == retestModel.TestRunId).ToList();

						if (testRunPlanDetailId != null)
						{
							foreach (var itms in testRunPlanDetailId)
							{
								itms.TestRunPlan.TestRunId = retestModel.TestRunId;
								itms.TestCaseDetailJson = testCaseJsonDetails;
								itms.TestCaseStepDetailJson = testCaseStepDetailJson;
							}
							_context.TestRunPlanDetail.UpdateRange(testRunPlanDetailId);
							await _context.SaveChangesAsync();
						}

					}
				}
				await _context.Database.CommitTransactionAsync();
				return Result<string>.Success(ReturnMessage.RetestSuccessfully);


			}
			catch (Exception ex)
			{
				await _context.Database.RollbackTransactionAsync();
				if (ex is TestCaseIdNotFoundException)
				{
					_iLogger.LogError("Could not find the testcase. Exception message is: ", ex.Message);
					return Result<string>.Error(ReturnMessage.TestCaseIdNotFound);
				}

				_iLogger.LogError("Failed to delete the testCase. Exception message is: ", ex.Message);
				return Result<string>.Error(ReturnMessage.FailedToRetestTestCase);

			}

		}

		public async Task<Result<string>> RefreshTestPlanAsync(int testRunId, int testPlanId)
		{
			await _context.Database.BeginTransactionAsync();
			try
			{
				var testRunTestCaseHistoryAsync = await _context.TestRunTestCaseHistory.Where(x => x.TestRunHistory.TestRunId == testRunId && x.TestPlanId == testPlanId).AnyAsync();

				if (testRunTestCaseHistoryAsync)
				{
					var getTestRunPlanDetailByTestRunIdTestPlanIdResult = await (from t in _context.TestRunPlanDetail
																				 join trp in _context.TestRunPlan on t.TestRunPlanId equals trp.TestRunPlanId
																				 join tr in _context.TestRun on trp.TestRunId equals tr.TestRunId
																				 join trh in _context.TestRunHistory on trp.TestRunId equals trh.TestRunId
																				 where trp.TestRunId == testRunId && trp.TestPlanId == testPlanId
																				 select new TestRunPlanDetailByTestRunIdTestPlanIdModel()
																				 {
																					 TestRunId = tr.TestRunId,
																					 TestRunName = tr.Title,
																					 TestRunHistoryId = trh.TestRunHistoryId,
																					 TestPlanId = trp.TestPlanId,
																					 TestRunPlanId = trp.TestRunPlanId,
																					 TestRunPlanDetailId = t.TestRunPlanDetailId,
																					 TestPlanDetailJson = t.TestPlanDetailJson,
																					 TestCaseDetailJson = t.TestCaseDetailJson,
																					 TestCaseStepDetailJson = t.TestCaseStepDetailJson

																				 }).FirstOrDefaultAsync();

					getTestRunPlanDetailByTestRunIdTestPlanIdResult.TestPlanDetailJsons = JsonSerializer.Deserialize<List<TestPlanJson>>(getTestRunPlanDetailByTestRunIdTestPlanIdResult.TestPlanDetailJson);
					getTestRunPlanDetailByTestRunIdTestPlanIdResult.TestCaseDetailJsons = JsonSerializer.Deserialize<List<TestPlanTestCaseJson>>(getTestRunPlanDetailByTestRunIdTestPlanIdResult.TestCaseDetailJson);
					getTestRunPlanDetailByTestRunIdTestPlanIdResult.TestCaseStepDetailJsons = JsonSerializer.Deserialize<List<TestCaseStepDetailsJson>>(getTestRunPlanDetailByTestRunIdTestPlanIdResult.TestCaseStepDetailJson);


					string testCaseDetailJson = string.Empty;
					string testCaseStepDetailJson = string.Empty;

					List<TestPlanTestCaseJson> refreshUpdateTestCaseJson = new List<TestPlanTestCaseJson>();
					List<TestPlanTestCaseJson> testPlanTestCaseIdListJsonAsync = new List<TestPlanTestCaseJson>();
					var testCaseStepDetailList = new List<TestCaseStepDetailsJson>();
					var testPlanTestCaseList = new List<TestPlanTestCaseJson>();
					var testPlanTestCaseIdList = new List<int>();
					var testPlanNewTestCaseIdList = new List<int>();
					bool isInTestPlanTestCaseJsonListId = false;



					testPlanTestCaseIdListJsonAsync = getTestRunPlanDetailByTestRunIdTestPlanIdResult.TestCaseDetailJsons;
					testPlanTestCaseIdList = await (_context.TestPlanTestCase.Where(x => x.IsDeleted == false && x.TestPlanId == testPlanId)).Select(x => x.ProjectModuleId).ToListAsync();
					if (testPlanTestCaseIdList.Count > 0)
					{
						foreach (var itemInTestPlanTestCaseIdList in testPlanTestCaseIdList)
						{
							isInTestPlanTestCaseJsonListId = false;

							isInTestPlanTestCaseJsonListId = (from tptc in testPlanTestCaseIdList
															  join tptc2 in testPlanTestCaseIdListJsonAsync on itemInTestPlanTestCaseIdList equals tptc2.ProjectModuleId
															  select tptc).Any();

							if (isInTestPlanTestCaseJsonListId == false)
							{
								testPlanNewTestCaseIdList.Add(itemInTestPlanTestCaseIdList);
							}
						}
					}

					if (testPlanNewTestCaseIdList.Count > 0)
					{
						testPlanTestCaseList = await (from tptc in _context.TestPlanTestCase
													  join pm in _context.ProjectModule on tptc.ProjectModuleId equals pm.ProjectModuleId
													  join li in _context.ListItem on pm.ProjectModuleListItemId equals li.ListItemId
													  join p in _context.Project on pm.ProjectId equals p.ProjectId
													  join tp in _context.TestPlan on tptc.TestPlanId equals tp.TestPlanId
													  join li2 in _context.ListItem on tp.TestPlanTypeListItemId equals li2.ListItemId
													  where testPlanNewTestCaseIdList.Contains(pm.ProjectModuleId) && pm.IsDeleted == false && tptc.TestPlanId == testPlanId
													  select new TestPlanTestCaseJson
													  {
														  TestPlanTestCaseId = tptc.TestPlanTestCaseId,
														  ProjectModuleId = pm.ProjectModuleId,
														  ParentProjectModuleId = pm.ParentProjectModuleId,
														  ProjectModuleName = pm.ModuleName,
														  ProjectModuleListItemId = pm.ProjectModuleListItemId,
														  OrderDate = pm.OrderDate,
														  ProjectModuleListItemName = li.ListItemSystemName,
														  ProjectId = p.ProjectId,
														  ProjectName = p.ProjectName,
														  ProjectSlug = p.ProjectSlug,
														  ProjectModuleDescription = pm.Description == null ? string.Empty : pm.Description,
														  ProjectDescription = p.ProjectDescription == null ? string.Empty : p.ProjectDescription,
														  TestPlanId = tp.TestPlanId,
														  TestPlanName = tp.TestPlanName,
														  TestPlanTypeListItemName = li2.ListItemSystemName,
														  TestPlanTitle = tp.Title,
														  ParentTestPlanId = tp.ParentTestPlanId,
														  TestPlanDescription = tp.Description == null ? string.Empty : tp.Description,
														  TestPlanTypeListItemId = li2.ListItemId,
														  TestRunId = getTestRunPlanDetailByTestRunIdTestPlanIdResult.TestRunId,
														  TestRunName = getTestRunPlanDetailByTestRunIdTestPlanIdResult.TestRunName,
														  InsertDate = DateTimeOffset.UtcNow,
														  InsertPersonId = _iPersonAccessor.GetPersonId(),
														  UpdateDate = DateTimeOffset.UtcNow,
														  UpdatedPersonId = _iPersonAccessor.GetPersonId()
													  }).ToListAsync();


						var testRunStatusPending = await (_iCommonService.GetListItemDetailByListItemSystemName(ListItem.Pending.ToString()));
						var projectModuleId = testPlanTestCaseList.Select(x => x.ProjectModuleId).ToList();

						//TestCaseHistory

						var testRunTestCaseHistory = testPlanTestCaseList.Select(x => new Entities.TestRunTestCaseHistory
						{
							StartTime = null,
							EndTime = null,
							TotalTimeSpent = null,
							TestRunStatusListItemId = testRunStatusPending.ListItemId,
							AssigneeProjectMemberId = null,
							ProjectModuleId = x.ProjectModuleId,
							TestRunHistoryId = getTestRunPlanDetailByTestRunIdTestPlanIdResult.TestRunHistoryId,
							TestPlanId = testPlanId,
							InsertDate = DateTimeOffset.UtcNow,
							UpdateDate = DateTimeOffset.UtcNow,

						}).ToList();

						await _context.TestRunTestCaseHistory.AddRangeAsync(testRunTestCaseHistory);
						await _context.SaveChangesAsync();

						var testCaseStepDetailListAsync = await _context.TestCaseStepDetail.Where(x => projectModuleId.Contains(x.ProjectModuleId)).ToListAsync();

						List<Entities.TestRunTestCaseStepHistory> testRunTestCaseStepHistoriesToAdd = new();
						//Add TestRunTestCaseStepHistory
						foreach (var itemInTestRunTestCaseHistory in testRunTestCaseHistory)
						{
							var testCaseStepDetailIdList = testCaseStepDetailListAsync.Where(x => x.ProjectModuleId == itemInTestRunTestCaseHistory.ProjectModuleId).Select(x => x.TestCaseStepDetailId).ToList();

							foreach (var itemInTestCaseStepDetailIdList in testCaseStepDetailIdList)
							{
								var addTestRunTestCaseStepHistory = new Entities.TestRunTestCaseStepHistory()
								{
									StartTime = null,
									EndTime = null,
									TotalTimeSpent = null,
									TestRunStatusListItemId = testRunStatusPending.ListItemId,
									TestRunTestCaseHistoryId = itemInTestRunTestCaseHistory.TestRunTestCaseHistoryId,
									TestCaseStepDetailId = itemInTestCaseStepDetailIdList,
								};
								testRunTestCaseStepHistoriesToAdd.Add(addTestRunTestCaseStepHistory);
							}
						}
						await _context.TestRunTestCaseStepHistory.AddRangeAsync(testRunTestCaseStepHistoriesToAdd);
						await _context.SaveChangesAsync();

						//TestCaseStepHistory
						//testCaseStepDetailList = await _context.TestCaseStepDetail.Include(x => x.ProjectModule).ThenInclude(x => x.ProjectModuleListItem).Include(x => x.TestCaseListItem).Where(x => projectModuleId.Contains(x.ProjectModuleId)).ToListAsync();
						var testCaseHistoryIds = testRunTestCaseHistory.Select(x => x.TestRunTestCaseHistoryId).ToList();


						testCaseStepDetailList = await (
							from trtch in _context.TestRunTestCaseHistory
							join trtcsh in _context.TestRunTestCaseStepHistory on trtch.TestRunTestCaseHistoryId equals trtcsh.TestRunTestCaseHistoryId
							join tcsd in _context.TestCaseStepDetail on trtcsh.TestCaseStepDetailId equals tcsd.TestCaseStepDetailId
							join pm in _context.ProjectModule on trtch.ProjectModuleId equals pm.ProjectModuleId
							join li in _context.ListItem on pm.ProjectModuleListItemId equals li.ListItemId
							join li2 in _context.ListItem on tcsd.TestCaseListItemId equals li2.ListItemId
							join tp in _context.TestPlan on trtch.TestPlanId equals tp.TestPlanId
							where projectModuleId.Contains(tcsd.ProjectModuleId) && pm.IsDeleted == false && testCaseHistoryIds.Contains(trtch.TestRunTestCaseHistoryId)

							select new TestCaseStepDetailsJson
							{
								TestCaseStepDetailId = tcsd.TestCaseStepDetailId,
								StepNumber = tcsd.StepNumber,
								StepDescription = tcsd.StepDescription,
								ExpectedResult = tcsd.ExpectedResult,
								TestCaseListItemId = li2.ListItemId,
								TestCaseListItemName = li2.ListItemName,
								ProjectId = pm.ProjectId,
								ProjectModuleId = pm.ProjectModuleId,
								ProjectModuleName = pm.ModuleName,
								ParentProjectModuleId = pm.ParentProjectModuleId,
								ProjectModuleDescription = pm.Description,
								ProjectModuleListItemId = li.ListItemId,
								ProjectModuleListItemName = li.ListItemSystemName,
								OrderDate = DateTimeOffset.UtcNow,
								InsertDate = DateTimeOffset.UtcNow,
								InsertPersonId = _iPersonAccessor.GetPersonId(),
								UpdateDate = DateTimeOffset.UtcNow,
								UpdatePersonId = _iPersonAccessor.GetPersonId(),
								TestRunTestCaseHistoryId = trtch.TestRunTestCaseHistoryId,
								TestRunTestCaseStepHistoryId = trtcsh.TestRunTestCaseStepHistoryId,
								TestPlanId = tp.TestPlanId,
								TestCaseStepStatusName = trtcsh.TestRunStatusListItem.ListItemSystemName
							}).ToListAsync();


						var testCaseDetailAsync = await (from pm in _context.ProjectModule
														 join tcd in _context.TestCaseDetail on pm.ProjectModuleId equals tcd.ProjectModuleId
														 join li in _context.ListItem on tcd.TestCaseListItemId equals li.ListItemId
														 select new TestCaseDetailAsyncModel
														 {
															 ProjectModuleId = pm.ProjectModuleId,
															 TestCaseDetailId = tcd.TestCaseDetailId,
															 PreCondition = tcd.PreCondition,
															 ExpectedResult = tcd.ExpectedResult,
															 TestCaseListItemId = li.ListItemId,
															 TestCaseListItemName = li.ListItemSystemName
														 }).ToListAsync();


						foreach (var itemInTestPlanTestCaseListJson in testPlanTestCaseList)
						{
							itemInTestPlanTestCaseListJson.FunctionName = await _context.ProjectModule.Where(x => x.ProjectModuleId == itemInTestPlanTestCaseListJson.ParentProjectModuleId).Select(x => x.ModuleName).FirstOrDefaultAsync();
							itemInTestPlanTestCaseListJson.TestCaseDetailId = testCaseDetailAsync.Where(z => z.ProjectModuleId == itemInTestPlanTestCaseListJson.ProjectModuleId).Select(z => z.TestCaseDetailId).FirstOrDefault();
							itemInTestPlanTestCaseListJson.PreCondition = testCaseDetailAsync.Where(z => z.ProjectModuleId == itemInTestPlanTestCaseListJson.ProjectModuleId).Select(z => z.PreCondition).FirstOrDefault();
							itemInTestPlanTestCaseListJson.ExpectedResult = testCaseDetailAsync.Where(z => z.ProjectModuleId == itemInTestPlanTestCaseListJson.ProjectModuleId).Select(z => z.ExpectedResult).FirstOrDefault();
							itemInTestPlanTestCaseListJson.TestCaseListItemId = testCaseDetailAsync.Where(z => z.ProjectModuleId == itemInTestPlanTestCaseListJson.ProjectModuleId).Select(z => z.TestCaseListItemId).FirstOrDefault();
							itemInTestPlanTestCaseListJson.TestCaseListItemName = testCaseDetailAsync.Where(z => z.ProjectModuleId == itemInTestPlanTestCaseListJson.ProjectModuleId).Select(z => z.TestCaseListItemName).FirstOrDefault();

						}

						getTestRunPlanDetailByTestRunIdTestPlanIdResult.TestCaseDetailJsons.AddRange(testPlanTestCaseList);
						getTestRunPlanDetailByTestRunIdTestPlanIdResult.TestCaseStepDetailJsons.AddRange(testCaseStepDetailList);
						testCaseDetailJson = JsonSerializer.Serialize<List<TestPlanTestCaseJson>>(getTestRunPlanDetailByTestRunIdTestPlanIdResult.TestCaseDetailJsons);
						testCaseStepDetailJson = JsonSerializer.Serialize<List<TestCaseStepDetailsJson>>(getTestRunPlanDetailByTestRunIdTestPlanIdResult.TestCaseStepDetailJsons);
						var testRunPlanDetailId = await _context.TestRunPlanDetail.Where(x => x.TestRunPlan.TestPlanId == testPlanId && x.TestRunPlan.TestRunId == testRunId).FirstOrDefaultAsync();

						if (testRunPlanDetailId != null)
						{
							testRunPlanDetailId.TestCaseDetailJson = testCaseDetailJson;
							testRunPlanDetailId.TestCaseStepDetailJson = testCaseStepDetailJson;
							_context.TestRunPlanDetail.UpdateRange(testRunPlanDetailId);
							await _context.SaveChangesAsync();
						}

					}
					else
					{
						return Result<string>.Success(ReturnMessage.RefreshSuccessfullyButThereIsNothingToAdd);
					}
					await _context.Database.CommitTransactionAsync();
					return Result<string>.Success(ReturnMessage.RefreshSuccessfully);
				}
				else
				{
					var testRunTestRunHistory = await (from tr in _context.TestRun
													   join trh in _context.TestRunHistory on tr.TestRunId equals trh.TestRunId
													   where tr.TestRunId == testRunId
													   select new TestRunTestRunHistoryModel
													   {
														   TestRunId = tr.TestRunId,
														   TestRunHistoryId = trh.TestRunHistoryId,
														   TestRunName = tr.Title
													   }).FirstOrDefaultAsync();

					if (testRunTestRunHistory != null)
					{
						var getTestRunPlanDetailByTestRunIdTestPlanIdResult = await (from t in _context.TestRunPlanDetail
																					 join trp in _context.TestRunPlan on t.TestRunPlanId equals trp.TestRunPlanId
																					 join tr in _context.TestRun on trp.TestRunId equals tr.TestRunId
																					 join trh in _context.TestRunHistory on trp.TestRunId equals trh.TestRunId
																					 where trp.TestRunId == testRunId && trp.TestPlanId == testPlanId
																					 select new TestRunPlanDetailByTestRunIdTestPlanIdModel()
																					 {
																						 TestRunId = tr.TestRunId,
																						 TestRunName = tr.Title,
																						 TestRunHistoryId = trh.TestRunHistoryId,
																						 TestPlanId = trp.TestPlanId,
																						 TestRunPlanId = trp.TestRunPlanId,
																						 TestRunPlanDetailId = t.TestRunPlanDetailId,
																						 TestPlanDetailJson = t.TestPlanDetailJson,
																						 TestCaseDetailJson = t.TestCaseDetailJson,
																						 TestCaseStepDetailJson = t.TestCaseStepDetailJson

																					 }).FirstOrDefaultAsync();


						getTestRunPlanDetailByTestRunIdTestPlanIdResult.TestCaseDetailJsons = JsonSerializer.Deserialize<List<TestPlanTestCaseJson>>(getTestRunPlanDetailByTestRunIdTestPlanIdResult.TestCaseDetailJson);
						getTestRunPlanDetailByTestRunIdTestPlanIdResult.TestCaseStepDetailJsons = JsonSerializer.Deserialize<List<TestCaseStepDetailsJson>>(getTestRunPlanDetailByTestRunIdTestPlanIdResult.TestCaseStepDetailJson);

						string testCaseDetailJson = string.Empty;
						string testCaseStepDetailJson = string.Empty;

						var testRunStatus = await (_iCommonService.GetListItemDetailByListItemSystemName(ListItem.Pending.ToString()));
						var projectModule = await (from tptc in _context.TestPlanTestCase
												   join tp in _context.TestPlan on tptc.TestPlanId equals tp.TestPlanId
												   join li in _context.ListItem on tp.TestPlanTypeListItemId equals li.ListItemId
												   join pm in _context.ProjectModule on tptc.ProjectModuleId equals pm.ProjectModuleId
												   join li2 in _context.ListItem on pm.ProjectModuleListItemId equals li2.ListItemId
												   where tp.TestPlanId == testPlanId && tptc.IsDeleted == false && pm.IsDeleted == false
												   select new TestPlanTestCaseModel
												   {
													   ProjectModuleId = pm.ProjectModuleId,
													   ProjectModuleListItemId = li2.ListItemId,
													   ProjectModuleListItemName = li2.ListItemSystemName,
													   TestPlanTypeListItemId = li.ListItemId,
													   TestPlanTypeListItemName = li.ListItemSystemName,
													   TestPlanId = tp.TestPlanId,
												   }).ToListAsync();


						var testRunTestCaseHistory = projectModule.Select(x => new Entities.TestRunTestCaseHistory
						{
							StartTime = null,
							EndTime = null,
							TotalTimeSpent = null,
							TestRunStatusListItemId = testRunStatus.ListItemId,
							AssigneeProjectMemberId = null,
							ProjectModuleId = x.ProjectModuleId,
							TestRunHistoryId = testRunTestRunHistory.TestRunHistoryId,
							TestPlanId = x.TestPlanId,

						}).ToList();
						await _context.TestRunTestCaseHistory.AddRangeAsync(testRunTestCaseHistory);
						await _context.SaveChangesAsync();

						var projectModuleId = projectModule.Select(x => x.ProjectModuleId).ToList();

						var testCaseStepDetailListAsync = await _context.TestCaseStepDetail.Where(x => projectModuleId.Contains(x.ProjectModuleId)).ToListAsync();

						List<Entities.TestRunTestCaseStepHistory> testRunTestCaseStepHistoriesToAdd = new();
						//Add TestRunTestCaseStepHistory
						foreach (var itemInTestRunTestCaseHistory in testRunTestCaseHistory)
						{
							var testCaseStepDetailIdList = testCaseStepDetailListAsync.Where(x => x.ProjectModuleId == itemInTestRunTestCaseHistory.ProjectModuleId).Select(x => x.TestCaseStepDetailId).ToList();

							foreach (var itemInTestCaseStepDetailIdList in testCaseStepDetailIdList)
							{
								var addTestRunTestCaseStepHistory = new Entities.TestRunTestCaseStepHistory()
								{
									StartTime = null,
									EndTime = null,
									TotalTimeSpent = null,
									TestRunStatusListItemId = testRunStatus.ListItemId,
									TestRunTestCaseHistoryId = itemInTestRunTestCaseHistory.TestRunTestCaseHistoryId,
									TestCaseStepDetailId = itemInTestCaseStepDetailIdList,
								};
								testRunTestCaseStepHistoriesToAdd.Add(addTestRunTestCaseStepHistory);
							}
						}
						await _context.TestRunTestCaseStepHistory.AddRangeAsync(testRunTestCaseStepHistoriesToAdd);
						await _context.SaveChangesAsync();



						var testPlanTestCaseList = await (from tptc in _context.TestPlanTestCase
														  join pm in _context.ProjectModule on tptc.ProjectModuleId equals pm.ProjectModuleId
														  join li in _context.ListItem on pm.ProjectModuleListItemId equals li.ListItemId
														  join p in _context.Project on pm.ProjectId equals p.ProjectId
														  join tp in _context.TestPlan on tptc.TestPlanId equals tp.TestPlanId
														  join li2 in _context.ListItem on tp.TestPlanTypeListItemId equals li2.ListItemId
														  where projectModuleId.Contains(pm.ProjectModuleId) && pm.IsDeleted == false && tptc.TestPlanId == testPlanId
														  select new TestPlanTestCaseJson
														  {
															  TestPlanTestCaseId = tptc.TestPlanTestCaseId,
															  ProjectModuleId = pm.ProjectModuleId,
															  ParentProjectModuleId = pm.ParentProjectModuleId,
															  ProjectModuleName = pm.ModuleName,
															  ProjectModuleListItemId = pm.ProjectModuleListItemId,
															  OrderDate = pm.OrderDate,
															  ProjectModuleListItemName = li.ListItemSystemName,
															  ProjectId = p.ProjectId,
															  ProjectName = p.ProjectName,
															  ProjectSlug = p.ProjectSlug,
															  ProjectModuleDescription = pm.Description == null ? string.Empty : pm.Description,
															  ProjectDescription = p.ProjectDescription == null ? string.Empty : p.ProjectDescription,
															  TestPlanId = tp.TestPlanId,
															  TestPlanName = tp.TestPlanName,
															  TestPlanTypeListItemName = li2.ListItemSystemName,
															  TestPlanTitle = tp.Title,
															  ParentTestPlanId = tp.ParentTestPlanId,
															  TestPlanDescription = tp.Description == null ? string.Empty : tp.Description,
															  TestPlanTypeListItemId = li2.ListItemId,
															  TestRunId = testRunTestRunHistory.TestRunId,
															  TestRunName = testRunTestRunHistory.TestRunName,
															  InsertDate = DateTimeOffset.UtcNow,
															  InsertPersonId = _iPersonAccessor.GetPersonId(),
															  UpdateDate = DateTimeOffset.UtcNow,
															  UpdatedPersonId = _iPersonAccessor.GetPersonId()
														  }).ToListAsync();


						var testCaseHistoryIds = testRunTestCaseHistory.Select(x => x.TestRunTestCaseHistoryId).ToList();


						var testCaseStepDetailList = await (
							  from trtch in _context.TestRunTestCaseHistory
							  join trtcsh in _context.TestRunTestCaseStepHistory on trtch.TestRunTestCaseHistoryId equals trtcsh.TestRunTestCaseHistoryId
							  join tcsd in _context.TestCaseStepDetail on trtcsh.TestCaseStepDetailId equals tcsd.TestCaseStepDetailId
							  join pm in _context.ProjectModule on trtch.ProjectModuleId equals pm.ProjectModuleId
							  join li in _context.ListItem on pm.ProjectModuleListItemId equals li.ListItemId
							  join li2 in _context.ListItem on tcsd.TestCaseListItemId equals li2.ListItemId
							  join tp in _context.TestPlan on trtch.TestPlanId equals tp.TestPlanId
							  where projectModuleId.Contains(tcsd.ProjectModuleId) && pm.IsDeleted == false && testCaseHistoryIds.Contains(trtch.TestRunTestCaseHistoryId)

							  select new TestCaseStepDetailsJson
							  {
								  TestCaseStepDetailId = tcsd.TestCaseStepDetailId,
								  StepNumber = tcsd.StepNumber,
								  StepDescription = tcsd.StepDescription,
								  ExpectedResult = tcsd.ExpectedResult,
								  TestCaseListItemId = li2.ListItemId,
								  TestCaseListItemName = li2.ListItemName,
								  ProjectId = pm.ProjectId,
								  ProjectModuleId = pm.ProjectModuleId,
								  ProjectModuleName = pm.ModuleName,
								  ParentProjectModuleId = pm.ParentProjectModuleId,
								  ProjectModuleDescription = pm.Description,
								  ProjectModuleListItemId = li.ListItemId,
								  ProjectModuleListItemName = li.ListItemSystemName,
								  OrderDate = DateTimeOffset.UtcNow,
								  InsertDate = DateTimeOffset.UtcNow,
								  InsertPersonId = _iPersonAccessor.GetPersonId(),
								  UpdateDate = DateTimeOffset.UtcNow,
								  UpdatePersonId = _iPersonAccessor.GetPersonId(),
								  TestRunTestCaseHistoryId = trtch.TestRunTestCaseHistoryId,
								  TestRunTestCaseStepHistoryId = trtcsh.TestRunTestCaseStepHistoryId,
								  TestPlanId = tp.TestPlanId,
								  TestCaseStepStatusName = trtcsh.TestRunStatusListItem.ListItemSystemName
							  }).ToListAsync();


						var testCaseDetailAsync = await (from pm in _context.ProjectModule
														 join tcd in _context.TestCaseDetail on pm.ProjectModuleId equals tcd.ProjectModuleId

														 select new TestCaseDetailAsyncModel
														 {
															 ProjectModuleId = pm.ProjectModuleId,
															 TestCaseDetailId = tcd.TestCaseDetailId,
															 PreCondition = tcd.PreCondition,
															 ExpectedResult = tcd.ExpectedResult,
															 TestCaseListItemId = tcd.TestCaseListItemId,
															 TestCaseListItemName = tcd.TestCaseListItem.ListItemSystemName

														 }).ToListAsync();
						var ParentProjectModuleId = testPlanTestCaseList.Select(x => x.ParentProjectModuleId).ToList();
						var functionName = await _context.ProjectModule.Where(x => ParentProjectModuleId.Contains(x.ProjectModuleId)).ToListAsync();

						foreach (var itemInTestPlanTestCaseListJson in testPlanTestCaseList)
						{
							itemInTestPlanTestCaseListJson.FunctionName = functionName.Where(x => x.ProjectModuleId == itemInTestPlanTestCaseListJson.ParentProjectModuleId).Select(x => x.ModuleName).FirstOrDefault();
							itemInTestPlanTestCaseListJson.TestCaseDetailId = testCaseDetailAsync.Where(z => z.ProjectModuleId == itemInTestPlanTestCaseListJson.ProjectModuleId).Select(z => z.TestCaseDetailId).FirstOrDefault();
							itemInTestPlanTestCaseListJson.PreCondition = testCaseDetailAsync.Where(z => z.ProjectModuleId == itemInTestPlanTestCaseListJson.ProjectModuleId).Select(z => z.PreCondition).FirstOrDefault();
							itemInTestPlanTestCaseListJson.ExpectedResult = testCaseDetailAsync.Where(z => z.ProjectModuleId == itemInTestPlanTestCaseListJson.ProjectModuleId).Select(z => z.ExpectedResult).FirstOrDefault();
							itemInTestPlanTestCaseListJson.TestCaseListItemId = testCaseDetailAsync.Where(z => z.ProjectModuleId == itemInTestPlanTestCaseListJson.ProjectModuleId).Select(z => z.TestCaseListItemId).FirstOrDefault();
							itemInTestPlanTestCaseListJson.TestCaseListItemName = testCaseDetailAsync.Where(z => z.ProjectModuleId == itemInTestPlanTestCaseListJson.ProjectModuleId).Select(z => z.TestCaseListItemName).FirstOrDefault();

						}

						getTestRunPlanDetailByTestRunIdTestPlanIdResult.TestCaseDetailJsons.AddRange(testPlanTestCaseList);
						getTestRunPlanDetailByTestRunIdTestPlanIdResult.TestCaseStepDetailJsons.AddRange(testCaseStepDetailList);
						testCaseDetailJson = JsonSerializer.Serialize<List<TestPlanTestCaseJson>>(getTestRunPlanDetailByTestRunIdTestPlanIdResult.TestCaseDetailJsons);
						testCaseStepDetailJson = JsonSerializer.Serialize<List<TestCaseStepDetailsJson>>(getTestRunPlanDetailByTestRunIdTestPlanIdResult.TestCaseStepDetailJsons);
						var testRunPlanDetailId = await _context.TestRunPlanDetail.Where(x => x.TestRunPlan.TestPlanId == testPlanId && x.TestRunPlan.TestRunId == testRunId).FirstOrDefaultAsync();

						if (testRunPlanDetailId != null)
						{
							testRunPlanDetailId.TestCaseDetailJson = testCaseDetailJson;
							testRunPlanDetailId.TestCaseStepDetailJson = testCaseStepDetailJson;
							_context.TestRunPlanDetail.UpdateRange(testRunPlanDetailId);
							await _context.SaveChangesAsync();
						}
						await _context.Database.CommitTransactionAsync();
						return Result<string>.Success(ReturnMessage.RefreshSuccessfully);
					}
					else
					{
						throw new TestRunIdTestPlanIdNotFoundException();
					}



				}
			}
			catch (Exception ex)
			{
				await _context.Database.RollbackTransactionAsync();
				if (ex is TestRunIdTestPlanIdNotFoundException)
				{
					_iLogger.LogError("Could not find testrunid and testplanid. Exception message is: ", ex.Message);
					throw new TestRunIdTestPlanIdNotFoundException();
				}
				_iLogger.LogError("Could not refresh the testplan. Exception message is: ", ex.Message);
				return Result<string>.Error(ReturnMessage.FailedToRefreshTestplan);
			}
		}

		public async Task<DocumentFileDownloadModel> DownloadTestRunWizardStausFileAsync(int documentId)
		{
			try
			{

				var fileToRetrieve = await _context.Document.Where(x => x.DocumentId == documentId).Select(x => new DocumentFileDownloadModel
				{
					DocumentId = x.DocumentId,
					FileName = x.Name,
					File = x.File,
					Extension = x.Extension,
					ContentType = GetMimeTypes()[x.Extension]
				}).FirstOrDefaultAsync();

				if (fileToRetrieve != null)
				{
					return fileToRetrieve;
				}
				else
				{
					throw new DocumentIdNotFoundException();
				}
			}
			catch (Exception ex)
			{
				_iLogger.LogError("Could not find the DocumentId. Exception message is: ", ex.Message);
				throw new DocumentIdNotFoundException();


			}
		}

		public async Task<Result<PagedResponseModel<List<TestRunTestCaseWithTestCaseIdModel>>>> GetTestRunTestCaseHistoryWizardAsync(PaginationFilterModel filter, int testCaseId)
		{
			try
			{
				var testCaseIds = await _context.TestRunTestCaseHistory.Where(x => x.ProjectModuleId == testCaseId).AnyAsync();
				if (testCaseIds)
				{
					var testRunTestCaseList = (from trtch in _context.TestRunTestCaseHistory
											   join trh in _context.TestRunHistory on trtch.TestRunHistoryId equals trh.TestRunHistoryId
											   join tcd in _context.TestCaseDetail on trtch.ProjectModuleId equals tcd.ProjectModuleId
											   orderby trtch.UpdateDate descending
											   where trtch.ProjectModuleId == testCaseId
											   select new TestRunTestCaseWithTestCaseIdModel
											   {
												   TestRunTestCaseHistoryId = trtch.TestRunTestCaseHistoryId,
												   ProjectModuleId = tcd.ProjectModuleId,
												   TestRunName = trh.TestRun.Title,
												   TimeSpent = trtch.TotalTimeSpent,
												   UpdateDate = trtch.UpdateDate,
												   TestRunStatusListItemId = trtch.TestRunStatusListItemId,
												   Status = trtch.TestRunStatusListItem.ListItemSystemName
											   });

					if (!string.IsNullOrEmpty(filter.SearchValue))
					{
						testRunTestCaseList = testRunTestCaseList.Where
							(
								x => x.TestRunName.ToLower().Contains(filter.SearchValue.ToLower())
							);
					}

					var records = testRunTestCaseList;
					var totalRecords = records.Count();
					var filteredData = await testRunTestCaseList
						.Skip((filter.PageNumber - 1) * filter.PageSize)
						.Take(filter.PageSize)
						.ToListAsync();


					if (filter.PageSize > totalRecords && totalRecords > 0)
					{
						filter.PageSize = totalRecords;
					}

					var totalPages = (totalRecords / filter.PageSize);

					var data = new PagedResponseModel<List<TestRunTestCaseWithTestCaseIdModel>>(filteredData, filter.PageNumber, filter.PageSize, totalRecords, totalPages);
					if (filteredData.Count > 0)
					{
						return Result<PagedResponseModel<List<TestRunTestCaseWithTestCaseIdModel>>>.Success(data);
					}
					else
					{
						return Result<PagedResponseModel<List<TestRunTestCaseWithTestCaseIdModel>>>.Success(null);
					}
				}
				else
				{
					throw new TestCaseIdNotFoundException();
				}

			}
			catch (Exception ex)
			{

				_iLogger.LogError("Could not find the GetTestRunTestCaseHistoryWizardAsync. Exception message is: ", ex.Message);
				return Result<PagedResponseModel<List<TestRunTestCaseWithTestCaseIdModel>>>.Error(ReturnMessage.TestCaseIdNotFound);
			}

		}

		public async Task<Result<PagedResponseModel<List<TestRunTestPlanWithTestPlanIdModel>>>> GetTestRunTestPlanListAsync(PaginationFilterModel filterModel, int testPlanId)
		{
			try
			{
				var testPlanIds = await _context.TestPlan.Where(x => x.TestPlanId == testPlanId).AnyAsync();
				if (testPlanIds)
				{
					List<TestRunTestPlanWithTestPlanIdModel> testRunWithTestPlanId = new List<TestRunTestPlanWithTestPlanIdModel>();


					var getTestRunWithTestPlanId = (from trtch in _context.TestRunTestCaseHistory
													join trh in _context.TestRunHistory on trtch.TestRunHistoryId equals trh.TestRunHistoryId
													join tp in _context.TestPlan on trtch.TestPlanId equals tp.TestPlanId
													orderby trtch.UpdateDate descending
													where trtch.TestPlanId == testPlanId
													select new TestRunTestPlanWithTestPlanIdModel
													{
														TestPlanId = tp.TestPlanId,
														TestPlanName = tp.TestPlanName,
														TestRunId = trh.TestRun.TestRunId,
														TestRunName = trh.TestRun.Title,
														TestRunTestCaseHistoryId = trtch.TestRunTestCaseHistoryId,
														UpdateDate = trtch.UpdateDate,
														TimeSpent = trtch.TotalTimeSpent,

													});

					testRunWithTestPlanId.AddRange(getTestRunWithTestPlanId);
					var testRunIds = testRunWithTestPlanId.Select(x => x.TestRunId).ToList();
					var testPlanIdList = testRunWithTestPlanId.Select(x => x.TestPlanId).ToList();

					var testRunTestCaseHistory = (from trtch in _context.TestRunTestCaseHistory
												  join trh in _context.TestRunHistory on trtch.TestRunHistoryId equals trh.TestRunHistoryId
												  join tr in _context.TestRun on trh.TestRunId equals tr.TestRunId
												  where trtch.TestPlanId == testPlanId && testRunIds.Contains(tr.TestRunId)
												  select new TestRunTestCaseHistoryStatusModel()
												  {
													  TestPlanId = trtch.TestPlanId,
													  TestRunId = tr.TestRunId,
													  Status = trtch.TestRunStatusListItemId,
													  ProjectModuleId = trtch.ProjectModuleId,
													  UpdateDate = trtch.UpdateDate,
													  StatusListItemSystemName = trtch.TestRunStatusListItem.ListItemSystemName,
													  TimeSpent = trtch.TotalTimeSpent,
													  TestRunTestCaseHistoryId = trtch.TestRunTestCaseHistoryId,

												  });
														

					var testRunWithTestPlanIdResult = testRunWithTestPlanId.OrderByDescending(x => x.UpdateDate).GroupBy(x => x.TestRunId).Select(x => new TestRunTestPlanWithTestPlanIdModel
					{
						TestPlanId = x.OrderByDescending(x => x.UpdateDate).Select(x => x.TestPlanId).FirstOrDefault(),
						TestRunId = x.OrderByDescending(x => x.UpdateDate).Select(x => x.TestRunId).FirstOrDefault(),
						TestRunName = x.OrderByDescending(x => x.UpdateDate).Select(x => x.TestRunName).FirstOrDefault(),
						TestRunTestCaseHistoryId = x.OrderByDescending(x => x.UpdateDate).Select(x => x.TestRunTestCaseHistoryId).FirstOrDefault(),
						UpdateDate = x.OrderByDescending(x => x.UpdateDate).Select(x => x.UpdateDate).FirstOrDefault(),
						TimeSpent = TotalTimeSpent(x.Select(x => x.TestRunId).FirstOrDefault(), x.Select(x => x.TestPlanId).FirstOrDefault(), testRunTestCaseHistory),
						PassedCount = StatusCountByTestRunId(x.Select(x => x.TestRunId).FirstOrDefault(), x.Select(x => x.TestPlanId).FirstOrDefault(), ListItem.Passed.ToString(), testRunTestCaseHistory),
						FailedCount = StatusCountByTestRunId(x.Select(x => x.TestRunId).FirstOrDefault(), x.Select(x => x.TestPlanId).FirstOrDefault(), ListItem.Failed.ToString(), testRunTestCaseHistory),
						BlockedCount = StatusCountByTestRunId(x.Select(x => x.TestRunId).FirstOrDefault(), x.Select(x => x.TestPlanId).FirstOrDefault(), ListItem.Blocked.ToString(), testRunTestCaseHistory),
						PendingCount = StatusCountByTestRunId(x.Select(x => x.TestRunId).FirstOrDefault(), x.Select(x => x.TestPlanId).FirstOrDefault(), ListItem.Pending.ToString(), testRunTestCaseHistory),

					}).AsQueryable();


					if (!string.IsNullOrEmpty(filterModel.SearchValue))
					{
						testRunWithTestPlanIdResult = testRunWithTestPlanIdResult.Where
							(
								x => x.TestRunName.ToLower().Contains(filterModel.SearchValue.ToLower())
							);
					}

					var records = testRunWithTestPlanIdResult;
					var totalRecords = records.Count();
					var filteredData = testRunWithTestPlanIdResult
						.Skip((filterModel.PageNumber - 1) * filterModel.PageSize)
						.Take(filterModel.PageSize)
						.ToList();


					if (filterModel.PageSize > totalRecords && totalRecords > 0)
					{
						filterModel.PageSize = totalRecords;
					}

					var totalPages = (totalRecords / filterModel.PageSize);

					var data = new PagedResponseModel<List<TestRunTestPlanWithTestPlanIdModel>>(filteredData, filterModel.PageNumber, filterModel.PageSize, totalRecords, totalPages);
					if (filteredData.Count > 0)
					{
						return Result<PagedResponseModel<List<TestRunTestPlanWithTestPlanIdModel>>>.Success(data);
					}
					else
					{
						return Result<PagedResponseModel<List<TestRunTestPlanWithTestPlanIdModel>>>.Success(null);
					}
				}
				else
				{
					throw new TestPlanIdNotFoundException();
				}
			}

			catch (Exception ex)
			{

				if (ex is TestPlanIdNotFoundException)
				{
					_iLogger.LogError("Could not find the test plan Id. Exception message is: ", ex.Message);
					return Result<PagedResponseModel<List<TestRunTestPlanWithTestPlanIdModel>>>.Error(ReturnMessage.TestPlanIdNotFound);
				}
				_iLogger.LogError("Could not find the test plan Id. Exception message is: ", ex.Message);
				return Result<PagedResponseModel<List<TestRunTestPlanWithTestPlanIdModel>>>.Error(ReturnMessage.FailedToShowTestRun);
			}
		}

	

		#endregion
	}
}
