using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ProjectStarred
{
	public class ProjectStarredModel
	{
		public int ProjectStarredId { get; set; }
		public int ProjectId { get; set; }
		public int PersonId { get; set; }

		public string ProjectName { get; set; }

		public string ProjectSlug { get; set; }
		public string PersonName { get; set; }
		public DateTimeOffset Date { get; set; }


		public int TestCaseCount { get; set; }
		public int TestRunCount { get; set; }
		public int TestPlanCount { get; set; }

	}
	public class AssignProjectToProjectStarredModel
	{


		public int ProjectStarredId { get; set; }
		public int ProjectId { get; set; }
		public int PersonId { get; set; }
	}
	public class UnAssignProjectToProjectStarredModel
	{


		public int ProjectStarredId { get; set; }
		public int ProjectId { get; set; }
		public int PersonId { get; set; }



	}

}
