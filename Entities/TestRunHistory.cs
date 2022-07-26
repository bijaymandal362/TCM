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
	public class TestRunHistory : BaseEntity
	{
		[Key]
		public int TestRunHistoryId { get; set; }

		public DateTimeOffset? StartTime { get; set; }

		public DateTimeOffset? EndTime { get; set; }

		public int? TotalTimeSpent { get; set; }
	
		public int TestRunId { get; set; }
		[ForeignKey(nameof(TestRunId))]
		public TestRun TestRun { get; set; }
	}
}
