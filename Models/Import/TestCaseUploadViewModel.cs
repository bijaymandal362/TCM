using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Models.Import
{
    public class TestCaseUploadViewModel
    {
        public IFormFile File { get; set; }
    }

    public class ImportProjectModuleModel
    {
        public int ProjectModuleId { get; set; }
        public string ProjectSlug { get; set; }
        public IFormFile File { get; set; }
        public string ProjectModuleType { get; set; }
    }

    public class StepCountModel
    {
        public List<int> Steps { get; set; }
        public string TestCaseName { get; set; }

    }

    public class ImportProjectModel
    {
        public int? ParentProjectModuleId { get; set; }
        public int ProjectModuleId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectSlug { get; set; }
        public string ProjectModuleType { get; set; }
    }


    public class TestCaseStepListModel
    {
        public int TestCaseStepDetailId { get; set; }
        public int? ParentProjectModuleId { get; set; }
        public int ProjectModuleId { get; set; }
        public int StepNumber { get; set; }
        public string StepDescription { get; set; }
        public string ExpectedResult { get; set; }
        public string ModuleName { get; set; }
    }
    public class UpdateProjectModuleModel
    {
        public int ProjectModuleId { get; set; }
        public int? ParentProjectModuleId { get; set; }
        public int ProjectId { get; set; }
        public string ModuleName { get; set; }
        public int ProjectModuleListItemId { get; set; }
        public string Description { get; set; }
        public DateTimeOffset OrderDate { get; set; }

    }

    public class ListItemModel
    {
        public int ListItemId { get; set; }
        public string ListItemSystemName { get; set; }

    }

    public class ProjectModuleListModel
    {
        public int ProjectModuleId { get; set; }
        public int? ParentProjectModuleId { get; set; }
        public int ProjectId { get; set; }
        public string ModuleName { get; set; }
        public int ProjectModuleListItemId { get; set; }
        public string Description { get; set; }
        public DateTimeOffset OrderDate { get; set; }
    }


    public class TestCaseListModel
    {
        public int TestCaseDetailId { get; set; }
        public int ProjectModuleId { get; set; }
        public int? ParentProjectModuleId { get; set; }
        public int ProjectId { get; set; }
        public string ModuleName { get; set; }
        public int TestCaseListItemId { get; set; }
        public string PreCondition { get; set; }
        public string ExpectedResult { get; set; }

    }
}
