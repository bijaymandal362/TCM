using Models.Dashboard;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.SymbolStore;

namespace Models.Project
{
    public class ProjectModel
	{

		public int ProjectId { get; set; }
		public string ProjectName { get; set; }   
        public DateTimeOffset StartDate { get; set; }		
		public int ProjectMarketListItemId { get; set; }
		public string ProjectDescription { get; set; }
	}
	
	public class ListProjectModel
	{
		public int ProjectId { get; set; }
		public string ProjectName { get; set; }
		public string ProjectSlug { get; set; }
		public int ProjectMarketId { get; set; }
		public string ProjectMarketName { get; set; }
		

	}
	public class ProjectListModel
	{
		public int ProjectId { get; set; }
		public string ProjectName { get; set; }
		public string ProjectSlug { get; set; }
		public int ProjectMarketId { get; set; }
		public string ProjectMarketName { get; set; }

		public bool IsStarredProject { get; set; }



	}

	public class GetAllListProjectModel
	{
		public int ProjectId { get; set; }
		public string ProjectName { get; set; }
		public string ProjectMarket { get; set; }
		public string ProjectSlug { get; set; }
        public int ProjectRoleId { get; set; }
        public string ProjectRole { get; set; }
        public DateTimeOffset Date { get; set; }
		public int ProjectMarketListItemId { get; set; }
		public string ProjectDescription { get; set; }

		public int TestCaseCount { get; set; }
		public int TestRunCount { get; set; }
		public int TestPlanCount { get; set; }

	}


	public class ProjectListViewModel
    {
		public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectMarketName { get; set; }
        public string ProjectStartDate { get; set; }
		public int TestCaseCount { get; set; }
		public int TestPlanCount { get; set; }
		public int TestRunCount { get; set; }
		 

    }
}
