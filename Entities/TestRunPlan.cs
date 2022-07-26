using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
	public class TestRunPlan : BaseEntity
	{
		[Key]
		public int TestRunPlanId { get; set; }

		public int TestRunId { get; set; }
		[ForeignKey(nameof(TestRunId))]
		public TestRun TestRun { get; set; }

		public int TestPlanId { get; set; }
		[ForeignKey(nameof(TestPlanId))]
		public TestPlan TestPlan { get; set; }
	}
}
