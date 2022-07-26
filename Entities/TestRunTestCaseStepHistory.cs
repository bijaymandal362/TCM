using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Entities
{
    public class TestRunTestCaseStepHistory : BaseEntity
	{
		[Key]
		public int TestRunTestCaseStepHistoryId { get; set; }

		public DateTimeOffset? StartTime { get; set; }
	
		public DateTimeOffset? EndTime { get; set; }

		public int? TotalTimeSpent { get; set; }

		public int TestRunStatusListItemId { get; set; }
		[ForeignKey(nameof(TestRunStatusListItemId))]
		public ListItem TestRunStatusListItem { get; set; }

		public int TestCaseStepDetailId { get; set; }
		[ForeignKey(nameof(TestCaseStepDetailId))]
		public TestCaseStepDetail TestCaseStepDetail { get; set; }

		public int TestRunTestCaseHistoryId { get; set; }
		[ForeignKey(nameof(TestRunTestCaseHistoryId))]
		public TestRunTestCaseHistory TestRunTestCaseHistory { get; set; }


	}
}
