using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
	public class TestRunTestCaseHistory : BaseEntity
	{
		[Key]
		public int TestRunTestCaseHistoryId { get; set; }

		public DateTimeOffset? StartTime { get; set; }

		public DateTimeOffset? EndTime { get; set; }

		public int? TotalTimeSpent { get; set; }

		public int TestRunStatusListItemId { get; set; }
		[ForeignKey(nameof(TestRunStatusListItemId))]
		public ListItem TestRunStatusListItem { get; set; }

		public int? AssigneeProjectMemberId { get; set; }
		[ForeignKey(nameof(AssigneeProjectMemberId))]
		public ProjectMember ProjectMember { get; set; }

		public int TestRunHistoryId { get; set; }
		[ForeignKey(nameof(TestRunHistoryId))]
		public TestRunHistory TestRunHistory { get; set; }

		public int ProjectModuleId { get; set; }
		[ForeignKey(nameof(ProjectModuleId))]
		public ProjectModule ProjectModule { get; set; }

		public int TestPlanId { get; set; }
		[ForeignKey(nameof(TestPlanId))]
		public TestPlan TestPlan { get; set; }
	}
}
