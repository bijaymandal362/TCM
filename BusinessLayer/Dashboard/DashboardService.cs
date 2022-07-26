using Data;
using Data.Exceptions;
using Infrastructure.Helper.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Models.Constant.Dashboard;
using Models.Constant.ReturnMessage;
using Models.Core;
using Models.Dashboard;
using Models.Enum;
using Models.TestRun;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly DataContext _context;
        private readonly ILogger<DashboardService> _iLogger;

        public DashboardService(DataContext context, ILogger<DashboardService> logger)
        {
            _context = context;
            _iLogger = logger;
        }

        #region Implementaion of API

        public async Task<Result<DashboardModel>> GetTestCaseTestPlanTestRunCountAsync(string projectSlug)
        {
            try
            {
                var projectId = await _context.Project.Where(x => x.ProjectSlug == projectSlug).Select(x => x.ProjectId).FirstOrDefaultAsync();
                if (projectId > 0)
                {
                    var testCaseCount = await (from pm in _context.ProjectModule
                                               join li in _context.ListItem on pm.ProjectModuleListItemId equals li.ListItemId
                                               join p in _context.Project on pm.ProjectId equals p.ProjectId into testcase
                                               from p in testcase.DefaultIfEmpty()
                                               where pm.ProjectId == projectId && pm.ProjectModuleListItem.ListItemSystemName == ListItem.TestCase.ToString() && pm.IsDeleted == false
                                               group new { pm, p } by new { pm.ProjectId, li.ListItemId } into g
                                               select new
                                               {
                                                   TestCaseCount = g.Count(x => x.pm != null)
                                               }).FirstOrDefaultAsync();

                    var testRunCount = await (from tr in _context.TestRun
                                              join p in _context.Project on tr.ProjectId equals p.ProjectId into testRun
                                              from p in testRun.DefaultIfEmpty()
                                              where tr.ProjectId == projectId
                                              group new { tr, p } by new { tr.ProjectId } into g
                                              select new
                                              {
                                                  TestRunCount = g.Count(x => x.tr != null)
                                              }).FirstOrDefaultAsync();

                    var testPlanCount = await (from tp in _context.TestPlan
                                               join li in _context.ListItem on tp.TestPlanTypeListItemId equals li.ListItemId
                                               join p in _context.Project on tp.ProjectId equals p.ProjectId into testPlan
                                               from p in testPlan.DefaultIfEmpty()
                                               where tp.ProjectId == projectId && tp.TestPlanTypeListItem.ListItemSystemName == ListItem.TestPlan.ToString() && tp.IsDeleted == false
                                               group new { tp, p } by new { tp.ProjectId } into g
                                               select new
                                               {
                                                   TestPlanCount = g.Count(x => x.tp != null)
                                               }).FirstOrDefaultAsync();
                    int tcCount = testCaseCount == null ? 0 : testCaseCount.TestCaseCount;
                    int tpCount = testPlanCount == null ? 0 : testPlanCount.TestPlanCount;
                    int trCount = testRunCount == null ? 0 : testRunCount.TestRunCount;

                    var result = new DashboardModel
                    {
                        TestCase = DashboardHelper.TestCase,
                        TestCaseCount = tcCount,
                        TestPlan = DashboardHelper.TestPlan,
                        TestPlanCount = tpCount,
                        TestRun = DashboardHelper.TestRun,
                        TestRunCount = trCount
                    };

                    return Result<DashboardModel>.Success(result);
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
                    _iLogger.LogError("Could not find the ProjectSlug. Exception message is: ", ex.Message);
                    return Result<DashboardModel>.Error(ReturnMessage.ProjectSlugDoesNotExists);
                }
                return Result<DashboardModel>.Error(ReturnMessage.UnauthorizedUser);
            }
        }

        public async Task<Result<List<TestCaseRepositoryListModel>>> GetTestCaseRepositoryListAsync(string projectSlug)
        {
            try
            {
                var project = await _context.Project.Where(x => x.ProjectSlug == projectSlug).FirstOrDefaultAsync();
                if (project != null)
                {
                    var getProjectModuleListAsync = await (from pm in _context.ProjectModule
                                                           join li in _context.ListItem on pm.ProjectModuleListItemId equals li.ListItemId
                                                           where pm.ProjectId == project.ProjectId && pm.IsDeleted == false
                                                           orderby pm.OrderDate
                                                           select new TestCaseRepositoryListModel
                                                           {
                                                               ProjectModuleId = pm.ProjectModuleId,
                                                               ModuleName = pm.ModuleName,
                                                               OrderDate = pm.OrderDate,
                                                               ParentProjectModuleId = pm.ParentProjectModuleId,
                                                               ProjectId = pm.ProjectId,
                                                               Description = pm.Description,
                                                               ProjectModuleListItemId = pm.ProjectModuleListItemId,
                                                               ProjectModuleType = li.ListItemSystemName,
                                                               IsDeleted = pm.IsDeleted
                                                           }).ToListAsync();

                    getProjectModuleListAsync = getProjectModuleListAsync.OrderBy(x => x.OrderDate)
                                    .Where(c => c.ParentProjectModuleId == null && c.IsDeleted == false)
                                    .Select(c => new TestCaseRepositoryListModel()
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
                                        ChildModule = ChildModule(getProjectModuleListAsync.Where(x => x.ParentProjectModuleId != null && x.ProjectModuleType == ListItem.Module.ToString()).ToList(), c.ProjectModuleId)
                                    })
                                    .ToList();

                    if (getProjectModuleListAsync.Any())
                    {
                        return Result<List<TestCaseRepositoryListModel>>.Success(getProjectModuleListAsync);
                    }
                    else
                    {
                        return Result<List<TestCaseRepositoryListModel>>.Success(null);
                    }
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
                    _iLogger.LogError("Could not find the ProjectSlug. Exception message is: ", ex.Message);
                    return Result<List<TestCaseRepositoryListModel>>.Error(ReturnMessage.ProjectSlugDoesNotExists);
                }
                return Result<List<TestCaseRepositoryListModel>>.Error(ReturnMessage.UnauthorizedUser);
            }
        }

        public async Task<Result<List<FunctionTestCaseListCountModel>>> GetFunctionTestCaseListCountAsync(string projectSlug, int projectModuleId)
        {
            try
            {
                var projectModule = await (from p in _context.Project
                                           join pm in _context.ProjectModule on p.ProjectId equals pm.ProjectId
                                           where pm.ProjectModuleId == projectModuleId
                                           && pm.Project.ProjectSlug == projectSlug
                                           && pm.IsDeleted == false
                                           select pm).ToListAsync();
                if (projectModule != null)
                {
                    var dict = new Dictionary<int, List<FunctionTestCaseListCountModel>>();
                    var moduleFunction = new List<FunctionTestCaseListCountModel>();
                    var moduleFunctionResult = new List<FunctionTestCaseListCountModel>();
                    var moduleFunctionFinalResult = new List<FunctionTestCaseListCountModel>();
                    var projectModuleIds = projectModule.Select(x => x.ProjectModuleId).ToList();
                    var functionItem = await ProjectModule(projectModuleIds);

                    foreach (var itemInfunctionItem in functionItem)
                    {
                        if (itemInfunctionItem.ProjectModuleListItem.ListItemSystemName == ListItem.Module.ToString())
                        {
                            var moduleFunctionResult1 = await ModuleFunction(itemInfunctionItem.ProjectModuleId, dict, moduleFunctionFinalResult);
                        }
                        else
                        {
                            var moduleFunctionResult2 = await FunctionProjectModule(functionItem.Where(x => x.ProjectModuleListItem.ListItemSystemName == ListItem.Function.ToString() && x.ProjectModuleId == itemInfunctionItem.ProjectModuleId).ToList(), dict, moduleFunctionFinalResult);
                        }
                    }

                    if (moduleFunctionFinalResult.Any())
                    {
                        return Result<List<FunctionTestCaseListCountModel>>.Success(moduleFunctionFinalResult);
                    }
                    else
                    {
                        return Result<List<FunctionTestCaseListCountModel>>.Success(null);
                    }
                }
                else
                {
                    throw new ProjectSlugProjectModuleIdNotFoundException();
                }
            }
            catch (Exception ex)
            {
                if (ex is ProjectSlugProjectModuleIdNotFoundException)
                {
                    _iLogger.LogError("Could not find the projectmoduleid or projectslug. Exception message is: ", ex.Message);
                    return Result<List<FunctionTestCaseListCountModel>>.Error(ReturnMessage.ProjectModuleProjectSlugIdNotFound);
                }
                return Result<List<FunctionTestCaseListCountModel>>.Error(ReturnMessage.FailedToFetchData);
            }
        }

        public async Task<Result<List<FunctionTestCaseListCountModel>>> GetDefaultFunctionTestCaseListCountByProjectSlugAsync(string projectSlug)
        {
            try
            {
                var projectModule = await (from p in _context.Project
                                           join pm in _context.ProjectModule on p.ProjectId equals pm.ProjectId
                                           join li in _context.ListItem on pm.ProjectModuleListItemId equals li.ListItemId
                                           where pm.Project.ProjectSlug == projectSlug
                                           && pm.IsDeleted == false
                                           select new ProjectModuleListCountModel
                                           {
                                               ProjectId = p.ProjectId,
                                               ProjectModuleId = pm.ProjectModuleId,
                                               ParentProjectModuleId = pm.ParentProjectModuleId,
                                               ModuleName = pm.ModuleName,
                                               ProjectModuleTypeListItemId = pm.ProjectModuleListItemId,
                                               ProjectModuleTypeListItemName = li.ListItemSystemName,
                                               IsDelete=pm.IsDeleted
                                           }).ToListAsync();
                
                if (projectModule.Count > 0)
                {
                    var dict = new Dictionary<int, List<FunctionTestCaseListCountModel>>();
                    var function = new List<FunctionTestCaseListCountModel>();
                    foreach (var item in projectModule)
                    {

                        function = projectModule
                           .Where(x => x.ProjectModuleTypeListItemName == ListItem.Function.ToString() && x.IsDelete == false)
                           .Select(x => new FunctionTestCaseListCountModel
                           {
                               ProjectModuleId = x.ProjectModuleId,
                               ParentProjectModuleId = x.ParentProjectModuleId,
                               ModuleName = x.ModuleName,
                               ProjectId = x.ProjectId
                           }).ToList();

                        foreach (var item2 in function)
                        {
                            if (item2 != null)
                            {
                                var testcase = projectModule.Where(x => x.ParentProjectModuleId == item2.ProjectModuleId && x.ProjectModuleTypeListItemName == ListItem.TestCase.ToString() && x.IsDelete == false).Select(x => new FunctionTestCaseListCountModel
                                {
                                    ProjectModuleId = x.ProjectModuleId,
                                    ParentProjectModuleId = x.ParentProjectModuleId,
                                    ModuleName = x.ModuleName,
                                    ProjectId = x.ProjectId
                                }).ToList().Count;
                                item2.TestCaseCount = testcase;
                            }
                        }
                        dict.Add(item.ProjectModuleId, function);

                    }
                    if (function.Any())
                    {
                        return Result<List<FunctionTestCaseListCountModel>>.Success(function);
                    }
                    else
                    {
                        return Result<List<FunctionTestCaseListCountModel>>.Success(null);
                    }
                }
                else
                {
                    throw new ProjectSlugProjectModuleIdNotFoundException();
                }
               
              
            }
            catch (Exception ex)
            {
                if (ex is ProjectSlugProjectModuleIdNotFoundException)
                {
                    _iLogger.LogError("Could not find the projectmoduleid or projectslug. Exception message is: ", ex.Message);
                    return Result<List<FunctionTestCaseListCountModel>>.Error(ReturnMessage.ProjectModuleProjectSlugIdNotFound);
                }      
                return Result<List<FunctionTestCaseListCountModel>>.Error(ReturnMessage.UnauthorizedUser);
            }
        }

        public async Task<Result<List<TestRunListModels>>> GetTestRunListAsync(string projectSlug)
        {
            try
            {
                var projectId = await _context.Project.Where(x => x.ProjectSlug == projectSlug).FirstOrDefaultAsync();

                if (projectId != null)
                {
                    var res = await (from tr in _context.TestRun
                                     where tr.ProjectId == projectId.ProjectId
                                     select new TestRunListModels
                                     {
                                         TestRunId = tr.TestRunId,
                                         TestRunName = tr.Title
                                     }).OrderByDescending(x => x.TestRunId).ToListAsync();

                    if (res.Any())
                    {
                        return Result<List<TestRunListModels>>.Success(res);
                    }
                    else
                    {
                        return Result<List<TestRunListModels>>.Success(null);
                    }
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
                    _iLogger.LogError("Could not find the ProjectSlug. Exception message is: ", ex.Message);
                    return Result<List<TestRunListModels>>.Error(ReturnMessage.ProjectSlugDoesNotExists);
                }

                return Result<List<TestRunListModels>>.Error(ReturnMessage.UnauthorizedUser);
            }
        }

        public async Task<Result<TestRunStatusCountModel>> GetTestRunStatusCountByTestRunIdAsync(int testRunId)
        {
            try
            {
                var testRun = await _context.TestRun.Where(x => x.TestRunId == testRunId).FirstOrDefaultAsync();

                if (testRun != null)
                {
                    var record = await (from tr in _context.TestRun
                                        join trh in _context.TestRunHistory on tr.TestRunId equals trh.TestRunId
                                        join trtch in _context.TestRunTestCaseHistory on trh.TestRunHistoryId equals trtch.TestRunHistoryId
                                        join li in _context.ListItem on trtch.TestRunStatusListItemId equals li.ListItemId into status
                                        from li in status.DefaultIfEmpty()
                                        join lic in _context.ListItemCategory on li.ListItemCategoryId equals lic.ListItemCategoryId
                                        where tr.TestRunId == testRunId && lic.ListItemCategoryName == ListItemCategory.TestRunStatus.ToString()
                                        group new { tr, li, trtch } by new { li.ListItemId, tr.Title, tr.TestRunId, trtch.TestRunStatusListItemId, trtch.ProjectModuleId, trtch.InsertDate, trtch.TestRunStatusListItem.ListItemSystemName, trtch.TestRunTestCaseHistoryId } into g

                                        select new
                                        {
                                            TestRunId = g.Key.TestRunId,
                                            TestRunName = g.Key.Title,
                                            ProjectModuleId = g.Key.ProjectModuleId,
                                            InsertDate = g.Key.InsertDate,
                                            StatusId = g.Key.ListItemId,
                                            StatusName = g.Key.ListItemSystemName,
                                            TestRunTestCaseHistoryId = g.Key.TestRunTestCaseHistoryId
                                        }).ToListAsync();

                    var result = record.GroupBy(x => x.TestRunId).Select(x => new
                    {
                        TestRunId = x.Select(x => x.TestRunId).FirstOrDefault(),
                        TestRunName = x.Select(x => x.TestRunName).FirstOrDefault(),
                        ProjectModuleId = x.Select(x => x.ProjectModuleId).FirstOrDefault(),
                        InsertDate = x.Select(x => x.InsertDate).FirstOrDefault(),
                        Status = GetStatus(x.Select(x => x.StatusName).ToList()),
                    }).ToList();

                    int PendingCount = 0;
                    int PassedCount = 0;
                    int FailedCount = 0;
                    int BlockedCount = 0;

                    foreach (var item in result)
                    {
                        foreach (var item2 in item.Status)
                        {
                            if (item2.Status == ListItem.Pending.ToString())
                            {
                                PendingCount = item2.StatusCount;
                            }
                            else if (item2.Status == ListItem.Passed.ToString())
                            {
                                PassedCount = item2.StatusCount;
                            }
                            else if (item2.Status == ListItem.Failed.ToString())
                            {
                                FailedCount = item2.StatusCount;
                            }
                            else if (item2.Status == ListItem.Blocked.ToString())
                            {
                                BlockedCount = item2.StatusCount;
                            }
                        }
                    }

                    var res = result.Select(x => new TestRunStatusCountModel
                    {
                        TestRunId = result.Select(x => x.TestRunId).FirstOrDefault(),
                        TestRunName = result.Select(x => x.TestRunName).FirstOrDefault(),
                        Pending = ListItem.Pending.ToString(),
                        PendingCount = result.Where(x => PendingCount != 0).Select(x => PendingCount).FirstOrDefault(),
                        Passed = ListItem.Passed.ToString(),
                        PassedCount = result.Where(x => PassedCount != 0).Select(x => PassedCount).FirstOrDefault(),
                        Failed = ListItem.Failed.ToString(),
                        FailedCount = result.Where(x => FailedCount != 0).Select(x => FailedCount).FirstOrDefault(),
                        Blocked = ListItem.Blocked.ToString(),
                        BlockedCount = result.Where(x => BlockedCount != 0).Select(x => BlockedCount).FirstOrDefault(),
                    }).FirstOrDefault();

                    if (res != null)
                    {
                        return Result<TestRunStatusCountModel>.Success(res);
                    }
                    else
                    {
                        return Result<TestRunStatusCountModel>.Success(null);
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
                    return Result<TestRunStatusCountModel>.Error(ReturnMessage.TestRunIdDoesnotExists);
                }
                return Result<TestRunStatusCountModel>.Error(ReturnMessage.UnauthorizedUser);
            }
        }

        public async Task<Result<List<TestCaseListDetail>>> GetTestCaseListDetailStatusCountAsync(string projectSlug)
        {
            try
            {
                var project = await _context.Project.Where(x => x.ProjectSlug == projectSlug).FirstOrDefaultAsync();
                if (project != null)
                {
                    var query = await (from trtch in _context.TestRunTestCaseHistory
                                       join trh in _context.TestRunHistory on trtch.TestRunHistoryId equals trh.TestRunHistoryId
                                       join tr in _context.TestRun on trh.TestRunId equals tr.TestRunId
                                       join p in _context.Project on tr.ProjectId equals p.ProjectId
                                       join pm in _context.ProjectModule on trtch.ProjectModuleId equals pm.ProjectModuleId
                                       join tcd in _context.TestCaseDetail on pm.ProjectModuleId equals tcd.ProjectModuleId
                                       join li in _context.ListItem on trtch.TestRunStatusListItemId equals li.ListItemId into status
                                       from li in status.DefaultIfEmpty()
                                       where p.ProjectId == project.ProjectId
                                       select new
                                       {
                                           TestRunTestCaseHistoryId = trtch.TestRunTestCaseHistoryId,
                                           TestCaseId = tcd.TestCaseDetailId,
                                           ParentProjectModuleId = pm.ParentProjectModuleId,
                                           TestCaseScenario = pm.Description,
                                           ExpectedResult = tcd.ExpectedResult,
                                           TestCaseName = pm.ModuleName,
                                           ProjectModuleId = trtch.ProjectModuleId,
                                           StatusId = trtch.TestRunStatusListItemId,
                                           StatusName = trtch.TestRunStatusListItem.ListItemSystemName
                                       }).ToListAsync();

                    var record = query.GroupBy(x => x.ProjectModuleId).Select(x => new TestCaseListDetail
                    {
                        TestCaseId = x.Select(x => x.TestCaseId).FirstOrDefault(),
                        ParentProjectModuleId = x.Select(x => x.ParentProjectModuleId).FirstOrDefault(),
                        TestCaseScenario = x.Select(x => x.TestCaseScenario).FirstOrDefault(),
                        ExpectedResult = x.Select(x => x.ExpectedResult).FirstOrDefault(),
                        TestCaseName = x.Select(x => x.TestCaseName).FirstOrDefault(),
                        TestCaseFailedCount = x.Where(x => x.StatusName == ListItem.Failed.ToString()).Count(x => x.StatusName != null),
                    }).OrderByDescending(x => x.TestCaseFailedCount).Take(10).ToList();

                    foreach (var item in record)
                    {
                        if (item != null)
                        {
                            var functionname = _context.ProjectModule.Where(x => x.ProjectModuleId == item.ParentProjectModuleId && x.ProjectModuleListItem.ListItemSystemName == ListItem.Function.ToString())
                                  .Select(x => new
                                  {
                                      ProjectModuleId = x.ProjectModuleId,
                                      ParentProjectModuleId = x.ParentProjectModuleId,
                                      ModuleName = x.ModuleName,
                                  }).FirstOrDefault();

                            item.Function = functionname.ModuleName == null ? string.Empty : functionname.ModuleName;
                        }
                    }

                    var result = record.Where(x => x.TestCaseFailedCount != 0).DistinctBy(x => x.TestCaseId).OrderByDescending(x => x.TestCaseFailedCount).Take(10).ToList();

                    if (result.Count > 0)
                    {
                        return Result<List<TestCaseListDetail>>.Success(result);
                    }
                    else
                    {
                        return Result<List<TestCaseListDetail>>.Success(null);
                    }
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
                    _iLogger.LogError("Could not find the ProjectSlug. Exception message is: ", ex.Message);
                    return Result<List<TestCaseListDetail>>.Error(ReturnMessage.ProjectSlugDoesNotExists);
                }
                return Result<List<TestCaseListDetail>>.Error(ReturnMessage.UnauthorizedUser);
            }
        }

        public async Task<Result<DashboardModel>> GetTestCaseTestPlanTestRunCountAsync()
        {
            try
            {
                var testCaseCount = await (from pm in _context.ProjectModule
                                           join li in _context.ListItem on
                                           pm.ProjectModuleListItemId equals li.ListItemId
                                           where !pm.IsDeleted && pm.ProjectModuleListItem.ListItemSystemName == ListItem.TestCase.ToString()
                                           select pm).CountAsync();

                var testPlanCount = await (from tp in _context.TestPlan
                                           join li in _context.ListItem on
                                           tp.TestPlanTypeListItemId equals li.ListItemId
                                           where !tp.IsDeleted && tp.TestPlanTypeListItem.ListItemSystemName == ListItem.TestPlan.ToString()
                                           select tp).CountAsync();

                var testRunCount = await _context.TestRun.CountAsync();

                var result = new DashboardModel
                {
                    TestCase = DashboardHelper.TestCase,
                    TestCaseCount = testCaseCount,
                    TestPlan = DashboardHelper.TestPlan,
                    TestPlanCount = testPlanCount,
                    TestRun = DashboardHelper.TestRun,
                    TestRunCount = testRunCount
                };

                return Result<DashboardModel>.Success(result);
            }
            catch (Exception ex)
            {
                _iLogger.LogError("Error while fetching total count. Exception message: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<Result<int>> GetProjectCountAsync()
        {
            try
            {
                var projectCount = await _context.Project.CountAsync();
                return Result<int>.Success(projectCount);
            }
            catch (Exception ex)
            {
                _iLogger.LogError("Error while fetching total project count. Exception message: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<Result<int>> GetUserCountAsync()
        {
            try
            {
                var userCount = await _context.Person.CountAsync();
                return Result<int>.Success(userCount);
            }
            catch (Exception ex)
            {
                _iLogger.LogError("Error while fetching total user count. Exception message: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<Result<List<TestCaseCountModel>>> GetTestCaseCountFromLastMonthAsync()
        {
            try
            {
                var now = DateTimeOffset.UtcNow;
                var day = now.Day > 1 ? now.Day - 1 : 0;
                DateTimeOffset firstDayOfCurrentMonth = now.AddDays(-day);
                DateTimeOffset firstDayOfLastMonth = firstDayOfCurrentMonth.AddMonths(-1);

                var testCases = await (from pm in _context.ProjectModule
                                       join li in _context.ListItem on
                                       pm.ProjectModuleListItemId equals li.ListItemId
                                       where !pm.IsDeleted && pm.ProjectModuleListItem.ListItemSystemName == ListItem.TestCase.ToString()
                                       group pm.ProjectModuleId by pm.InsertDate into g
                                       select new TestCaseCountModel
                                       {
                                           DateTime = g.Key,
                                           TestCaseCountPerDay = g.Count(),
                                       }).ToListAsync();

                var testCaseListFromLastMonth = testCases
                    .OrderBy(d => d.DateTime)
                    .Where(x => x.DateTime >= firstDayOfLastMonth && x.DateTime <= firstDayOfCurrentMonth)
                    .GroupBy(y => y.DateTime.Day)
                    .Select(z =>
                    {
                        string dateInString = z.Select(x => x.DateTime.Date).FirstOrDefault().ToString();
                        string date = dateInString.Split(" ").First();
                        return new TestCaseCountModel
                        {
                            Date = date,
                            TestCaseCountPerDay = z.Count()
                        };
                    }).ToList();

                return Result<List<TestCaseCountModel>>.Success(testCaseListFromLastMonth);
            }
            catch (Exception ex)
            {
                _iLogger.LogError("Error while fetching the total count of test case from past 30 days. Exception message: {Message}", ex.Message);
                throw;
            }
        }

        #endregion Implementaion of API

        #region Functions

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

        private List<TestCaseRepositoryListModel> ChildModule(List<TestCaseRepositoryListModel> list, int parentProjectModuleId)
        {
            return list
                   .Where(c => c.ParentProjectModuleId == parentProjectModuleId && c.ProjectModuleType == ListItem.Module.ToString() && c.IsDeleted == false)
                   .Select(c => new TestCaseRepositoryListModel
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
                       ChildModule = ChildModule(list, c.ProjectModuleId)
                   }).ToList();
        }

        public async Task<List<Entities.ProjectModule>> ProjectModule(List<int> projectModuleIds)
        {
            var projectMoudelList = await _context.ProjectModule.Include(x => x.ProjectModuleListItem).Where(x => projectModuleIds.Contains((int)x.ParentProjectModuleId) && x.IsDeleted == false).ToListAsync();
            return projectMoudelList;
        }

        private async Task<List<FunctionTestCaseListCountModel>> ModuleFunction(int projectModuleId, Dictionary<int, List<FunctionTestCaseListCountModel>> result, List<FunctionTestCaseListCountModel> moduleFunctionFinalResult)
        {
            var nestedProjectModule = await _context.ProjectModule.Include(x => x.ProjectModuleListItem).Where(x => x.ParentProjectModuleId == projectModuleId && x.IsDeleted == false).ToListAsync();

            if (nestedProjectModule.Count > 0)
            {
                var nestedProjectModuleIds = nestedProjectModule.Select(x => x.ProjectModuleId).ToList();
                var nestFunctionList = await (from pm in _context.ProjectModule
                                              join li in _context.ListItem on pm.ProjectModuleListItemId equals li.ListItemId
                                              where nestedProjectModuleIds.Contains((int)pm.ParentProjectModuleId) && pm.IsDeleted == false
                                              select new { pm, li }).ToListAsync();

                foreach (var itemInfunctionItem in nestedProjectModule)
                {
                    if (itemInfunctionItem.ProjectModuleListItem.ListItemSystemName == ListItem.Module.ToString())
                    {
                        var nesteded = nestFunctionList.Where(x => x.pm.ParentProjectModuleId == itemInfunctionItem.ProjectModuleId && x.pm.IsDeleted == false).FirstOrDefault();
                        if (nesteded != null)
                        {
                            if (nesteded.pm.ProjectModuleListItem.ListItemSystemName == ListItem.Module.ToString())
                            {
                                await ModuleFunction(itemInfunctionItem.ProjectModuleId, result, moduleFunctionFinalResult);
                            }
                            else
                            {
                                var moduleFunctionAsync = nestFunctionList.Where(x => x.pm.ParentProjectModuleId == itemInfunctionItem.ProjectModuleId && x.pm.ProjectModuleListItem.ListItemSystemName == ListItem.Function.ToString() && x.pm.IsDeleted == false)
                                                    .Select(x => new FunctionTestCaseListCountModel
                                                    {
                                                        ProjectModuleId = x.pm.ProjectModuleId,
                                                        ParentProjectModuleId = x.pm.ParentProjectModuleId,
                                                        ModuleName = x.pm.ModuleName,
                                                        ProjectId = x.pm.ProjectId
                                                    }).ToList();
                                if (moduleFunctionAsync.Count > 0)
                                {
                                    await ChildFucntionTestCase(moduleFunctionAsync);
                                    moduleFunctionFinalResult.AddRange(moduleFunctionAsync);
                                    result.Add(itemInfunctionItem.ProjectModuleId, moduleFunctionAsync);
                                }
                            }
                        }
                    }
                    else if (itemInfunctionItem.ProjectModuleListItem.ListItemSystemName == ListItem.Function.ToString())
                    {
                        var moduleFunctionAsync = nestedProjectModule.Where(x => x.ProjectModuleId == itemInfunctionItem.ProjectModuleId && x.ProjectModuleListItem.ListItemSystemName == ListItem.Function.ToString() && x.IsDeleted == false)
                                              .Select(x => new FunctionTestCaseListCountModel
                                              {
                                                  ProjectModuleId = x.ProjectModuleId,
                                                  ParentProjectModuleId = x.ParentProjectModuleId,
                                                  ModuleName = x.ModuleName,
                                                  ProjectId = x.ProjectId
                                              }).ToList();

                        if (moduleFunctionAsync.Count > 0)
                        {
                            await ChildFucntionTestCase(moduleFunctionAsync);
                            moduleFunctionFinalResult.AddRange(moduleFunctionAsync);
                            result.Add(itemInfunctionItem.ProjectModuleId, moduleFunctionAsync);
                        }
                    }
                }
            }

            return moduleFunctionFinalResult;
        }

        private async Task<List<FunctionTestCaseListCountModel>> FunctionProjectModule(List<Entities.ProjectModule> projectModule, Dictionary<int, List<FunctionTestCaseListCountModel>> result, List<FunctionTestCaseListCountModel> moduleFunctionResult)
        {
            var moduleFunctionAsync = projectModule.Select(x => new FunctionTestCaseListCountModel
            {
                ProjectModuleId = x.ProjectModuleId,
                ParentProjectModuleId = x.ParentProjectModuleId,
                ModuleName = x.ModuleName,
                ProjectId = x.ProjectId
            }).ToList();

            var projectModuleId = moduleFunctionAsync.Select(x => x.ProjectModuleId).FirstOrDefault();
            if (moduleFunctionAsync.Count > 0)
            {
                await ChildFucntionTestCase(moduleFunctionAsync);
                moduleFunctionResult.AddRange(moduleFunctionAsync);
                result.Add(projectModuleId, moduleFunctionAsync);
            }
            return moduleFunctionResult;
        }

        private async Task ChildFucntionTestCase(List<FunctionTestCaseListCountModel> testCaseList)
        {
            var projectMouduleIds = testCaseList.Select(x => x.ProjectModuleId).ToList();
            var testCaseListResult = await ProjectModule(projectMouduleIds);

            foreach (var itemIntestCaseListResult in testCaseList)
            {
                var testCase = testCaseListResult.Where(x => x.ParentProjectModuleId == itemIntestCaseListResult.ProjectModuleId && x.ProjectModuleListItem.ListItemSystemName == ListItem.TestCase.ToString() && x.IsDeleted == false).Select(x => new FunctionTestCaseListCountModel
                {
                    ProjectModuleId = x.ProjectModuleId,
                    ParentProjectModuleId = x.ParentProjectModuleId,
                    ModuleName = x.ModuleName,
                    ProjectId = x.ProjectId
                }).ToList().Count;
                itemIntestCaseListResult.TestCaseCount = testCase;
            }
        }

        #endregion Functions
    }
}